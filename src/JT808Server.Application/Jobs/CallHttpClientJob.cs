using JT808Server.Application.Customs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace JT808Server.Application.Jobs
{
    /// <summary>
    /// 调用HttpClient作业
    /// </summary>
    public class CallHttpClientJob :IHostedService
    {

        private readonly ILogger Logger;
        private JT808HttpClientExt jT808HttpClient;
        public CallHttpClientJob(
            ILoggerFactory loggerFactory,
            JT808HttpClientExt jT808HttpClient)
        {
            Logger = loggerFactory.CreateLogger<CallHttpClientJob>();
            this.jT808HttpClient = jT808HttpClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result1 = await jT808HttpClient.GetIndex1();
                    var result2 = await jT808HttpClient.GetTcpSessionAll();
                    var result3 = await jT808HttpClient.UnificationSend(new JT808.Gateway.Abstractions.Dtos.JT808UnificationSendRequestDto 
                    { 
                        TerminalPhoneNo= "123456789012",
                        HexData= "7E02000026123456789012007D02000000010000000200BA7F0E07E4F11C0028003C00001810151010100104000000640202007D01137E"
                    });
                    Logger.LogInformation($"[GetIndex Ext]:{JsonSerializer.Serialize(result1)}");
                    Logger.LogInformation($"[GetTcpSessionAll]:{JsonSerializer.Serialize(result2)}");
                    Logger.LogInformation($"[UnificationSend]:{JsonSerializer.Serialize(result3)}");
                    Thread.Sleep(3000);
                }
            }, cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
