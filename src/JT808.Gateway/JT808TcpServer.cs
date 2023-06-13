using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JT808.Gateway.Abstractions;
using JT808.Gateway.Abstractions.Configurations;
using JT808.Gateway.Services;
using JT808.Gateway.Session;
using JT808.Protocol;
using JT808.Protocol.Exceptions;
using JT808.Protocol.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JT808.Gateway
{
    /// <summary>
    /// 808 tcp服务器
    /// </summary>
    public class JT808TcpServer : IHostedService
    {
        private Socket server;

        private readonly ILogger Logger;

        private readonly JT808SessionManager SessionManager;

        private readonly JT808BlacklistManager BlacklistManager;

        private readonly JT808Serializer Serializer;
        /// <summary>
        /// 通用消息处理程序
        /// </summary>
        private readonly JT808MessageHandler MessageHandler;
        /// <summary>
        /// 数据生产接口
        /// </summary>
        private readonly IJT808MsgProducer MsgProducer;
        /// <summary>
        /// 网关应答数据日志生产接口
        /// </summary>
        private readonly IJT808MsgReplyLoggingProducer MsgReplyLoggingProducer;

        private readonly IOptionsMonitor<JT808Configuration> ConfigurationMonitor;
        /// <summary>
        /// 接收消息数
        /// </summary>
        private long MessageReceiveCounter = 0;

        /// <summary>
        /// 初始化服务注册
        /// </summary>
        /// <param name="configurationMonitor"></param>
        /// <param name="msgProducer"></param>
        /// <param name="msgReplyLoggingProducer"></param>
        /// <param name="messageHandler"></param>
        /// <param name="jT808Config"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="jT808SessionManager"></param>
        /// <param name="jT808BlacklistManager"></param>
        public JT808TcpServer(
                IOptionsMonitor<JT808Configuration> configurationMonitor,
                IJT808MsgProducer msgProducer,
                IJT808MsgReplyLoggingProducer msgReplyLoggingProducer,
                JT808MessageHandler messageHandler,
                IJT808Config jT808Config,
                ILoggerFactory loggerFactory,
                JT808SessionManager jT808SessionManager,
                JT808BlacklistManager jT808BlacklistManager)
        {
            MessageHandler = messageHandler;
            MsgProducer = msgProducer;
            MsgReplyLoggingProducer = msgReplyLoggingProducer;
            ConfigurationMonitor = configurationMonitor;
            SessionManager = jT808SessionManager;
            BlacklistManager = jT808BlacklistManager;
            Logger = loggerFactory.CreateLogger<JT808TcpServer>();
            Serializer = jT808Config.GetSerializer();
            InitServer();
        }
        /// <summary>
        /// 初始化JT808 TCP服务
        /// </summary>
        private void InitServer()
        {
            var IPEndPoint = new System.Net.IPEndPoint(IPAddress.Any, ConfigurationMonitor.CurrentValue.TcpPort);
            server = new Socket(IPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
            server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, ConfigurationMonitor.CurrentValue.MiniNumBufferSize);
            server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, ConfigurationMonitor.CurrentValue.MiniNumBufferSize);
            server.LingerState = new LingerOption(true, 0);
            server.Bind(IPEndPoint);
            server.Listen(ConfigurationMonitor.CurrentValue.SoBacklog);
        }
        /// <summary>
        /// 开始异步
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"JT808 TCP Server start at {IPAddress.Any}:{ConfigurationMonitor.CurrentValue.TcpPort}.");
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var socket = await server.AcceptAsync(cancellationToken);
                        JT808TcpSession jT808TcpSession = new JT808TcpSession(socket);
                        //添加到会话管理
                        SessionManager.TryAdd(jT808TcpSession);
                        await Task.Factory.StartNew(async (state) =>
                        {
                            var session = (JT808TcpSession)state;
                            if (Logger.IsEnabled(LogLevel.Information))
                            {
                                Logger.LogInformation($"[Connected]:{session.Client.RemoteEndPoint}");
                            }
                            var pipe = new Pipe();
                            Task writing = FillPipeAsync(session, pipe.Writer);
                            Task reading = ReadPipeAsync(session, pipe.Reader);
                            await Task.WhenAll(reading, writing);
                            SessionManager.RemoveBySessionId(session.SessionID);
                        }, jT808TcpSession);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception)
                    {
                    }
                }
            });
            return Task.CompletedTask;
        }
        /// <summary>
        /// 异步填充到管道
        /// </summary>
        /// <param name="session"></param>
        /// <param name="writer">定义提供可将数据写入到的管道的类</param>
        /// <returns></returns>
        private async Task FillPipeAsync(JT808TcpSession session, PipeWriter writer)
        {
            while (true)
            {
                try
                {
                    //GetMemory:返回要写入到的 Memory<T>，其大小按 sizeHint 参数指定至少为所请求的大小
                    Memory<byte> memory = writer.GetMemory(ConfigurationMonitor.CurrentValue.MiniNumBufferSize);
                    //设备多久没发数据就断开连接 Receive Timeout.
                    //Receives data from a connected socket
                    int bytesRead = await session.Client.ReceiveAsync(memory, SocketFlags.None, session.ReceiveTimeout.Token);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    //Advance:通知 PipeWriter：已向输出 Span<T> 或 Memory<T> 写入 bytes 字节。 必须在调用 Advance(Int32) 之后请求新的缓冲区，
                    //才能继续写入更多数据；无法写入到之前获取的缓冲区。
                    writer.Advance(bytesRead);
                    //FlushAsync:使已写入的字节可用于 PipeReader，并运行 ReadAsync(CancellationToken) 延续。
                    FlushResult result = await writer.FlushAsync(session.ReceiveTimeout.Token);
                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    Logger.LogError($"[Receive Timeout Or Operation Canceled]:{session.Client.RemoteEndPoint},{session.TerminalPhoneNo}");
                    break;
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    Logger.LogError($"[{ex.SocketErrorCode},{ex.Message}]:{session.Client.RemoteEndPoint},{session.TerminalPhoneNo}");
                    break;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"[Receive Error]:{session.Client.RemoteEndPoint},{session.TerminalPhoneNo}");
                    break;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
            //Complete:将 PipeWriter 标记为正在完成，表示不再将项写入到该通道。
            writer.Complete();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="reader">定义提供对管道读取端的访问权限的类。</param>
        /// <returns></returns>
        private async Task ReadPipeAsync(JT808TcpSession session, PipeReader reader)
        {
            while (true)
            {
                //ReadAsync:以异步方式从当前 PipeReader 读取一系列字节。
                ReadResult result = await reader.ReadAsync();
                if (result.IsCompleted)
                {
                    break;
                }
                ReadOnlySequence<byte> buffer = result.Buffer;
                SequencePosition consumed = buffer.Start;
                SequencePosition examined = buffer.End;
                try
                {
                    if (result.IsCanceled) break;
                    if (buffer.Length > 0)
                    {
                        ReaderBuffer(ref buffer, session, out consumed);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"[ReadPipe Error]:{session.Client.RemoteEndPoint},{session.TerminalPhoneNo}");
                    break;
                }
                finally
                {
                    reader.AdvanceTo(consumed, examined);
                }
            }
            reader.Complete();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="session"></param>
        /// <param name="consumed"></param>
        /// <exception cref="ArgumentException"></exception>
        private void ReaderBuffer(ref ReadOnlySequence<byte> buffer, JT808TcpSession session, out SequencePosition consumed)
        {
            SequenceReader<byte> seqReader = new SequenceReader<byte>(buffer);
            if (seqReader.TryPeek(out byte beginMark))
            {
                if (beginMark != JT808Package.BeginFlag) throw new ArgumentException("Not JT808 Packages.");
            }
            byte mark = 0;
            long totalConsumed = 0;
            while (!seqReader.End)
            {
                if (seqReader.IsNext(JT808Package.BeginFlag, advancePast: true))
                {
                    if (mark == 1)
                    {
                        byte[] data = null;
                        try
                        {
                            data = seqReader.Sequence.Slice(totalConsumed, seqReader.Consumed - totalConsumed).ToArray();
                            //过滤掉不是808标准包（14）
                            //（头）1+（消息 ID ）2+（消息体属性）2+（终端手机号）6+（消息流水号）2+（检验码 ）1+（尾）1
                            if (data != null && data.Length > 14)
                            {
                                //反序列化消息头
                                var package = Serializer.HeaderDeserialize(data);
                                if (BlacklistManager.Contains(package.Header.TerminalPhoneNo))
                                {
                                    if (Logger.IsEnabled(LogLevel.Warning))
                                        Logger.LogWarning($"[Blacklist {session.Client.RemoteEndPoint}-{session.TerminalPhoneNo}]:{package.OriginalData.ToHexString()}");
                                    session.ReceiveTimeout.Cancel();
                                    break;
                                }
# if DEBUG
                                Interlocked.Increment(ref MessageReceiveCounter);
                                if (Logger.IsEnabled(LogLevel.Trace))
                                    Logger.LogTrace($"[Accept Hex {session.Client.RemoteEndPoint}-{package.Header.TerminalPhoneNo}]:{package.OriginalData.ToHexString()},Counter:{MessageReceiveCounter}");
#else
                                if (Logger.IsEnabled(LogLevel.Trace))
                                    Logger.LogTrace($"[Accept Hex {session.Client.RemoteEndPoint}-{session.TerminalPhoneNo}]:{package.OriginalData.ToHexString()}");
#endif
                                SessionManager.TryLink(package.Header.TerminalPhoneNo, session);
                                Processor(session, package);
                            }
                        }
                        catch (NotImplementedException ex)
                        {
                            Logger.LogError(ex.Message, $"[ReaderBuffer]:{data?.ToHexString()},{session.Client.RemoteEndPoint},{session.TerminalPhoneNo}");
                        }
                        catch (JT808Exception ex)
                        {
                            Logger.LogError($"[HeaderDeserialize ErrorCode]:{ ex.ErrorCode},[ReaderBuffer]:{data?.ToHexString()},{session.Client.RemoteEndPoint},{session.TerminalPhoneNo}");
                        }
                        totalConsumed += seqReader.Consumed - totalConsumed;
                        if (seqReader.End) break;
                        seqReader.Advance(1);
                        mark = 0;
                    }
                    mark++;
                }
                else
                {
                    seqReader.Advance(1);
                }
            }
            if (seqReader.Length == totalConsumed)
            {
                consumed = buffer.End;
            }
            else
            {
                consumed = buffer.GetPosition(totalConsumed);
            }
        }
        private void Processor(in IJT808Session session, in JT808HeaderPackage package)
        {
            try
            {
                MsgProducer?.ProduceAsync(package.Header.TerminalPhoneNo, package.OriginalData);
                var downData = MessageHandler.Processor(in package);
                if (ConfigurationMonitor.CurrentValue.IgnoreMsgIdReply != null && ConfigurationMonitor.CurrentValue.IgnoreMsgIdReply.Count > 0)
                {
                    if (!ConfigurationMonitor.CurrentValue.IgnoreMsgIdReply.Contains(package.Header.MsgId))
                    {
                        session.SendAsync(downData);
                    }
                }
                else
                {
                    session.SendAsync(downData);
                }
                if (MsgReplyLoggingProducer != null)
                {
                    if (downData != null)
                        MsgReplyLoggingProducer.ProduceAsync(package.Header.TerminalPhoneNo, downData);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"[Processor]:{package.OriginalData.ToHexString()},{session.Client.RemoteEndPoint},{session.TerminalPhoneNo}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("JT808 Tcp Server Stop");
            if (server?.Connected ?? false)
                server.Shutdown(SocketShutdown.Both);
            server?.Close();
            return Task.CompletedTask;
        }
    }
}
