using JT808.Gateway;
using JT808.Gateway.Abstractions;
using JT808.Gateway.Client.Services;
using JT808.Gateway.Session;
using JT808.Protocol.Enums;
using JT808Server.Application.Contracts.Constant;
using JT808Server.Application.Jobs;
using JT808Server.Application.Services;
using JT808Server.Utility.Common;
using JT808Server.Utility.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using Yitter.IdGenerator;

namespace JT808Server.WinForm
{
    public partial class MainForm : Form
    {
        CancellationToken _cancellationToken = new CancellationToken();
        /// <summary>
        /// 跟踪的车牌号
        /// </summary>
        private static string TrackVehNo { get; set; } = string.Empty;
        /// <summary>
        /// 每日刪除log时间
        /// </summary>
        private string deleteTime { get; set; } = DateTime.Now.ToString();
        /// <summary>
        /// 日志显示个数
        /// </summary>
        private static int ShowCount { get; set; } = 5000;

        private Boolean IsStoping = false;

        private readonly ILogger<MainForm> _logger;
        private readonly JT808SessionManager _jT809SessionManager;
        private readonly JT809Configuration _jT809Configuration;
        private readonly SystemConfiguration _systemConfiguration;
        private readonly MainFormService _mainFormService;
        private readonly CallHttpClientJob _CallHttpClientJob;
        private readonly UpJob _UpJob;
        private readonly TCPServer _jt808TcpServer;
        private readonly JT808UdpServer _jt808UdpServer;
        private readonly JT808ReceiveAtomicCounterService _JT808ReceiveAtomicCounterService;
        public MainForm(ILogger<MainForm> logger,
            JT808SessionManager jT809SessionManager,
            IOptions<JT809Configuration> jT809ConfigurationOptions,
            IOptions<SystemConfiguration> systemConfigurationOptions,
            MainFormService mainFormService,
            CallHttpClientJob callHttpClientJob,
            UpJob upJob,
            TCPServer jt808TcpServer,
            JT808UdpServer jt808UdpServer,
            JT808ReceiveAtomicCounterService JT808ReceiveAtomicCounterService
            //JT809MainServerMsgDispatchHostHandler jT809MainServerMsgDispatchHostHandler,
            //JT809SubordinateClientMsgDispatchHostHandler jT809SubordinateClientMsgDispatchHostHandler,
            //JT809MainServerMsgChannel jT809MainServerMsgChannel,
            //JT809SubordinateClientMsgChannel jT809SubordinateClientMsgChannel
            )
        {
            InitializeComponent();
            _logger = logger;
            _jT809SessionManager = jT809SessionManager;
            _jT809Configuration = jT809ConfigurationOptions.Value;
            _systemConfiguration = systemConfigurationOptions.Value;
            _mainFormService = mainFormService;
            _CallHttpClientJob = callHttpClientJob;
            _UpJob = upJob;
            _jt808TcpServer = jt808TcpServer;
            _jt808UdpServer = jt808UdpServer;
            _JT808ReceiveAtomicCounterService = JT808ReceiveAtomicCounterService;
            //_jT809MainServerMsgDispatchHostHandler = jT809MainServerMsgDispatchHostHandler;
            //_jT809SubordinateClientMsgDispatchHostHandler = jT809SubordinateClientMsgDispatchHostHandler;
            //_jT809MainServerMsgChannel = jT809MainServerMsgChannel;
            //_jT809SubordinateClientMsgChannel = jT809SubordinateClientMsgChannel;
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            //捕获异常
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            //协议标准
            string AlarmAnalysisStr = string.Empty;
            switch (_jT809Configuration.AlarmAnalysis)
            {
                case 0:
                    AlarmAnalysisStr = "国标";
                    break;
                case 1:
                    AlarmAnalysisStr = "苏标";
                    break;
                case 2:
                    AlarmAnalysisStr = "新苏标";
                    break;
                case 3:
                    AlarmAnalysisStr = "四川标";
                    break;
            }
            this.Text = $"{Text}-所属标准({AlarmAnalysisStr})，协议版本(兼容2011、2019):{System.Windows.Forms.Application.ProductVersion} 启动时间:{DateTime.Now}";
            WinFromDisplay.ShowSysMsg += this.ShowSysMsg;
            WinFromDisplay.ShowOperatorLineMsg += this.ShowOperatorLineMsg;
            WinFromDisplay.ShowOperatorTrackMsg += this.ShowOperatorTrackMsg;
            WinFromDisplay.ClientLoginRemove += this.ClientLoginRemove;
            //定时刪除log
            TimeToDeleteLog();
            //定时GC强制回收
            GCExecutionCheck();
            //初始化雪花Id算法
            YitIdHelper.SetIdGenerator(new IdGeneratorOptions(1));
            //首次读取可以连接服务的运营商资料
            //var status = _mainFormService.UpdateOperatorInfo();
            //if (!status)
            //{
            //    MessageBox.Show("读取数据库运营商资料：失败！");
            //    //这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。
            //    Process.GetCurrentProcess().Kill();// 杀掉进程
            //    return;
            //}
            await _jt808TcpServer.StartAsync(_cancellationToken);
            await _jt808UdpServer.StartAsync(_cancellationToken);
            await _CallHttpClientJob.StartAsync(_cancellationToken);
            await _UpJob.StartAsync(_cancellationToken);
            ////运行809TCP
            //await _jT809_2011MainServerHost.StartAsync();
            //await _jT809_2019MainServerHost.StartAsync();
            ////运行主从链路消息消费
            //await _jT809MainServerMsgDispatchHostHandler.StartAsync();
            //await _jT809SubordinateClientMsgDispatchHostHandler.StartAsync();
            ////更新平台信息数据
            //_mainFormService.UpdatePlatformData();
            ////更新安标车辆数据信息
            //_mainFormService.UpdateVehInfo();
            ////更新车辆公共信息数据
            //_mainFormService.UpdateVehicleData();
            ////读取本地文件队列数据入队
            //ReadLocalStorageHandle();
            ////保存GPS信息到Kafka队列,其他服务消费写入Oracle数据库
            //_mainFormService.SaveGPSToKafka();
            ////保存终端报警信息到Kafka队列,其他服务消费写入Oracle数据库
            //_mainFormService.ProcessNewAlarmToKafka();
            ////保存驾驶员身份信息到Kafka队列,其他服务消费写入Oracle数据库
            //_mainFormService.ProcessDriverICToKafka();
            ////保存ADAS报警信息到Kafka队列,其他服务消费写入Oracle数据库
            //_mainFormService.ProcessADASAlarmToKafka();
            ////保存DSM报警信息到Kafka队列,其他服务消费写入Oracle数据库
            //_mainFormService.ProcessDSMAlarmToKafka();
            ////运营商在线列表入kafka生产者
            //_mainFormService.VideoCommandCenterTransferProducerOnlineOperationListToKafka();
            ////下发车辆消息Kafka消费者,生产者不在这个项目
            //_mainFormService.DownMsgVehicleConsumerTask();
            ////处理下发数据到连接服务的运营商的数据队列,从链路发送消息
            //_mainFormService.TranspondOperator();
            ////(0x9404主动安全报警附件目录请求消息)从链路消息队列出队
            //_mainFormService.DownFile();
            ////(0x9404主动安全报警附件目录请求消息)从链路消息重送
            //_mainFormService.ReSendDownFile();
            ////更新运营商传输数据记录
            //_mainFormService.UpdateOPERATORCONNSTATELOG();
        }
        /// <summary>
        /// 定时刪除log
        /// </summary>
        private void TimeToDeleteLog()
        {
            //每4小时执行一次检查计时器执行刪除log
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000 * 60 * 4;//每4小时执行一次
            timer.Elapsed += new System.Timers.ElapsedEventHandler((source, e) =>
            {
                try
                {
                    if (DateTime.Now >= Convert.ToDateTime(deleteTime))
                    {
                        DirectoryInfo fileInfo = new DirectoryInfo(Directory.GetCurrentDirectory() + "/Logs");
                        foreach (DirectoryInfo file in fileInfo.GetDirectories())
                        {
                            DateTime fileDate = DateTime.Parse(file.Name);
                            if (fileDate < DateTime.Now.AddDays(-_systemConfiguration.LogSaveDay))
                            {
                                file.Delete(true);
                                _logger.LogDebug($"删除日志路径:{file.FullName}");
                            }
                        }
                        deleteTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm");
                    }
                    _logger.LogDebug("每4小时执行一次检查计时器执行");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            });
            timer.AutoReset = true;//自动执行
            timer.Enabled = true;
        }
        /// <summary>
        /// 定时GC强制回收
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void GCExecutionCheck()
        {
            if (_systemConfiguration.GCExecutionTime > 0)
            {
                System.Timers.Timer timerGC = new System.Timers.Timer();
                timerGC.Interval = 1000 * 60 * _systemConfiguration.GCExecutionTime;//?分钟GC强制回收一次
                timerGC.Elapsed += new System.Timers.ElapsedEventHandler((source, e) =>
                {
                    try
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        _logger.LogDebug($"每{_systemConfiguration.GCExecutionTime}分钟GC强制回收一次计时器执行");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                });
                timerGC.AutoReset = true;//自动执行
                timerGC.Enabled = true;
            }
        }
        /// <summary>
        /// 页面显示信息
        /// </summary>
        /// <param name = "sNote" ></ param >
        private void ShowSysMsg(string SimNo, string sNote)
        {
            try
            {
                if (this.IsHandleCreated)
                {
                    BeginInvoke(new Action(() =>
                    {
                        if (lv_Log_sys.Items.Count >= ShowCount)
                        {
                            lv_Log_sys.Items.Clear();
                        }
                        ListViewItem item = new ListViewItem
                        {
                            Text = DateTime.Now.ToString(),
                        };
                        item.SubItems.Add(SimNo);
                        item.SubItems.Add(sNote);
                        lv_Log_sys.Items.Insert(0, item);
                    }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        /// <summary>
        /// 移除登录显示
        /// </summary>
        /// <param name = "List" ></ param >
        private void ClientLoginRemove(dynamic List)
        {
            try
            {
                if (List is ListViewItem)
                {
                    Invoke((EventHandler)delegate
                    {
                        ClientLogin.Items.Remove(List);
                    });
                }
                else
                {
                    Invoke((EventHandler)delegate
                    {
                        ListViewItem li = List.ListViewItem;
                        if (li != null)
                        {
                            li.SubItems[2].Text = "未连接";
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        /// <summary>
        /// 显示交互日志
        /// </summary>
        /// <param name = "type" ></ param >
        /// < param name="time"></param>
        /// <param name = "note" ></ param >
        /// < param name="note1"></param>
        /// <param name = "note2" ></ param >
        private void ShowOperatorLineMsg(string type, string time, string note, string note1, string note2)
        {
            try
            {
                Invoke((EventHandler)delegate
                {
                    if (lvwOperatorLineMsg.Items.Count > ShowCount)
                    {
                        lvwOperatorLineMsg.Items.Clear();
                    }
                    ListViewItem listViewItem = lvwOperatorLineMsg.Items.Add(type);
                    listViewItem.SubItems.Add(time);
                    listViewItem.SubItems.Add(note);
                    listViewItem.SubItems.Add(note1);
                    listViewItem.SubItems.Add(note2);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        private void 清空ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            lvwOperatorLineMsg.Items.Clear();
        }
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvwOperatorLineMsg.Items.Count > 0 && lvwOperatorLineMsg.SelectedItems.Count > 0)
                {
                    string cptxt = "";
                    foreach (ListViewItem lv in lvwOperatorLineMsg.SelectedItems)
                    {
                        foreach (ListViewItem.ListViewSubItem item in lv.SubItems)
                        {
                            cptxt = cptxt + item.Text + "|";
                        }
                    }
                    Clipboard.SetText(cptxt);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        private void ShowOperatorTrackMsg(string gnid, string vehicleNo, int vehicleColor, string isUp, string time, string note)
        {
            try
            {
                if (!isShowTrackInfo.Checked)
                {
                    Invoke((EventHandler)delegate
                    {
                        if (lvwOperatorTrackMsg.Items.Count > ShowCount)
                        {
                            lvwOperatorTrackMsg.Items.Clear();
                        }
                        ListViewItem listViewItem = lvwOperatorTrackMsg.Items.Add(vehicleNo);
                        listViewItem.SubItems.Add(gnid);
                        listViewItem.SubItems.Add(isUp);
                        listViewItem.SubItems.Add(time);
                        listViewItem.SubItems.Add(note);
                    });
                }
                var directoryPath = $"{Directory.GetCurrentDirectory()}/跟踪车辆记录/{DateTime.Now.ToString("yyyy-MM-dd")}/{vehicleNo}_{vehicleColor}";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string filePath = $"{directoryPath}/{vehicleNo}_{vehicleColor}_{DateTime.Now.ToString("yyyy_MM_dd_HH")}.log";
                //File.AppendAllText(filePath, $"企业接入码：{gnid},车牌号：{vehicleNo},车牌颜色：{(JT808VehicleColorType)vehicleColor},时间:{time},信息：{isUp}<<< {note}\r\n");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 1://状态列表
                    break;
            }
        }
        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lv_Log_sys.Items.Clear();
        }
        private void rmi_log_sys_copy_Click(object sender, EventArgs e)
        {
            try
            {
                if (lv_Log_sys.Items.Count > 0 && lv_Log_sys.SelectedItems.Count > 0)
                {
                    string cptxt = "";
                    foreach (ListViewItem lv in lv_Log_sys.SelectedItems)
                    {
                        cptxt = cptxt + lv.SubItems[1].Text + "\r\n";
                    }
                    Clipboard.SetText(cptxt);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        private void ClientLogin_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
        }
        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {

        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //if (ClientLogin.SelectedItems.Count > 0)
            //{
            //    JT808Session tcpPro = (JT808Session)ClientLogin.SelectedItems[0].Tag;
            //    tcpPro.ShowLog = true;
            //    ClientLogin.SelectedItems[0].BackColor = Color.Red;
            //    tabControl1.SelectedIndex = 0;
            //}
        }
        private void 取消ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (ClientLogin.SelectedItems.Count > 0)
            //{
            //    JT808Session tcpPro = (JT808Session)ClientLogin.SelectedItems[0].Tag;
            //    tcpPro.ShowLog = false;
            //    ClientLogin.SelectedItems[0].BackColor = Color.White;
            //}
        }
        private void 触发主链路异常ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (ClientLogin.SelectedItems.Count > 0)
            //{
            //    JT808Session tcpPro = (JT808Session)ClientLogin.SelectedItems[0].Tag;
            //    tcpPro.TestError = true;
            //    ShowOperatorLineMsg("触发主链路异常测试", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "触发主链路异常测试", "最多需要等待心跳60秒", "");
            //    ClientLogin.SelectedItems[0].BackColor = Color.DarkOrange;
            //}
        }
        private void 取消主链路异常ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (ClientLogin.SelectedItems.Count > 0)
            //{
            //    JT808Session tcpPro = (JT808Session)ClientLogin.SelectedItems[0].Tag;
            //    tcpPro.TestError = false;
            //    ShowOperatorLineMsg("取消主链路异常测试", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "取消主链路异常测试", "", "");
            //    ClientLogin.SelectedItems[0].BackColor = Color.White;
            //}
        }
        private void 记录传输GPS数据车辆列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (ClientLogin.SelectedItems.Count > 0)
            //{
            //    JT808Session tcpPro = (JT808Session)ClientLogin.SelectedItems[0].Tag;
            //    if (tcpPro.HaveVeh.Keys.Count > 0)
            //    {
            //        var text = string.Join("\r\n", tcpPro.HaveVeh.Keys);
            //        var directoryPath = $"{Directory.GetCurrentDirectory()}/传输GPS数据车辆列表";
            //        if (!Directory.Exists(directoryPath))
            //        {
            //            Directory.CreateDirectory(directoryPath);
            //        }
            //        //GlobalCollection.OperatorInfoHash.TryGetValue(tcpPro.MsgGNSSCENTERID.ToString(), out var oper);
            //        string filePath = $"{directoryPath}/{StringHelper.GetIPAddress(tcpPro.MainChannel.RemoteAddress).Replace(":", "_")}_{oper?.PlatformName}_{tcpPro.JT809Version}_车辆列表.txt";
            //        File.WriteAllText(filePath, text);
            //        MessageBox.Show($"传输GPS数据车辆列表写入文件成功！");
            //    }
            //    else
            //    {
            //        MessageBox.Show($"无数据！");
            //    }
            //}
        }
        private void UserInfo_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {

        }
        private void button_2db_Click(object sender, EventArgs e)
        {

        }
        private void 清空ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            lvwOperatorTrackMsg.Items.Clear();
        }
        private void tsmi_traceMsg_copy_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvwOperatorTrackMsg.Items.Count > 0 && lvwOperatorTrackMsg.SelectedItems.Count > 0)
                {
                    string tmp = "";
                    foreach (ListViewItem lv in lvwOperatorTrackMsg.SelectedItems)
                    {
                        tmp = tmp + "车牌：" + lv.SubItems[0].Text + " 企业码：" + lv.SubItems[1].Text + " 上下行：" + lv.SubItems[2].Text + " 时间：" + lv.SubItems[3].Text + "内容：" + lv.SubItems[4].Text + "\r\n";
                    }
                    Clipboard.SetText(tmp);
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            lvwOperatorTrackMsg.Items.Clear();
        }
        private void btnStartTrackVeh_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTrackVeh.Text.Trim()))
            {
                MessageBox.Show("请输入车牌号", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            btnStartTrackVeh.Enabled = false;
            btnStopTrackVeh.Enabled = true;
            TrackVehNo = txtTrackVeh.Text.Trim();
            WinFromDisplay.TrackVehNo = TrackVehNo;
            txtTrackVeh.Enabled = false;
            isShowTrackInfo.Enabled = false;
        }
        private void btnStopTrackVeh_Click(object sender, EventArgs e)
        {
            btnStartTrackVeh.Enabled = true;
            btnStopTrackVeh.Enabled = false;
            txtTrackVeh.Enabled = true;
            isShowTrackInfo.Enabled = true;
            TrackVehNo = "";
            WinFromDisplay.TrackVehNo = TrackVehNo;
        }
        private void tmUpdateStatus_Tick(object sender, EventArgs e)
        {
            try
            {
                sysRev.Text = $"808数据接收缓存：{_JT808ReceiveAtomicCounterService.MsgSuccessCount}";
                //List<JT809Session> jT809Session = _jT808SessionManager.GetAll();
                //for (int i = 0; i < jT809Session.Count; i++)
                //{
                //    JT809Session tcpPro = jT809Session[i];
                //    GlobalCollection.OperatorInfoHash.TryGetValue(tcpPro.MsgGNSSCENTERID.ToString(), out var oper);
                //    if (tcpPro.ListViewItem == null)
                //    {
                //        ListViewItem li2 = tcpPro.ListViewItem = ClientLogin.Items.Add(oper?.PlatformName);
                //        li2.Tag = tcpPro;
                //        li2.SubItems.Add(tcpPro.MsgGNSSCENTERID.ToString());
                //        li2.SubItems.Add(tcpPro.JT809Version.ToString());
                //        li2.SubItems.Add(tcpPro.MainIPAddress);
                //        li2.SubItems.Add(tcpPro.MainConnStatus ? "已连接 ：已登录" : "未连接");
                //        li2.SubItems.Add(tcpPro.StartTime.ToString());
                //        li2.SubItems.Add(tcpPro.SlaveIPAddress);
                //        li2.SubItems.Add(tcpPro.SlaveConnStatus ? "已连接 ：已登录" : "未连接");
                //        li2.SubItems.Add("总包：" + tcpPro.MainRecvAll + "/位置包：" + tcpPro.LocationRecv + "/车辆数：" + tcpPro.HaveVeh?.Count);
                //        li2.SubItems.Add(tcpPro.MainSend.ToString());
                //        li2.SubItems.Add(tcpPro.SlaveRecv.ToString());
                //        li2.SubItems.Add(tcpPro.SlaveSend.ToString());
                //        li2.SubItems.Add(tcpPro.ENCRYPT_FLAG ? "是" : "否");
                //    }
                //    else
                //    {
                //        ListViewItem li = tcpPro.ListViewItem;
                //        li.SubItems[0].Text = oper?.PlatformName;
                //        li.SubItems[1].Text = tcpPro.MsgGNSSCENTERID.ToString();
                //        li.SubItems[2].Text = tcpPro.JT809Version.ToString();
                //        li.SubItems[3].Text = tcpPro.MainIPAddress;
                //        li.SubItems[4].Text = tcpPro.MainConnStatus ? "已连接 ：已登录" : "未连接";
                //        li.SubItems[5].Text = tcpPro.StartTime.ToString();
                //        li.SubItems[6].Text = tcpPro.SlaveIPAddress;
                //        li.SubItems[7].Text = tcpPro.SlaveConnStatus ? "已连接 ：已登录" : "未连接";
                //        li.SubItems[8].Text = "总包：" + tcpPro.MainRecvAll + "/位置包：" + tcpPro.LocationRecv + "/车辆数：" + tcpPro.HaveVeh?.Count;
                //        li.SubItems[9].Text = tcpPro.MainSend.ToString();
                //        li.SubItems[10].Text = tcpPro.SlaveRecv.ToString();
                //        li.SubItems[11].Text = tcpPro.SlaveSend.ToString();
                //        li.SubItems[12].Text = tcpPro.ENCRYPT_FLAG ? "是" : "否";
                //        //两个都未连接的数据移除显示
                //        if (!tcpPro.MainConnStatus && !tcpPro.SlaveConnStatus)
                //        {
                //            this.ClientLoginRemove(li);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        /// <summary>
        /// 读取本地文件队列数据入队
        /// </summary>
        public void ReadLocalStorageHandle()
        {
            Task.Run(() =>
            {
                try
                {
                    ////GPS
                    //var queueGPS = GpsLocalStorage.DeserializeStorage<GPSInfoDto>("gps.db");
                    //if (queueGPS != null && queueGPS.Count > 0)
                    //{
                    //    queueGPS.ForEach(item => { GlobalCollection.GPSInfoQueue.Enqueue(item); });
                    //    GpsLocalStorage.ClearStorage("gps.db");
                    //    _logger.LogDebug($"读取本地GPSInfoQueue入队成功数据数量{queueGPS.Count},数据Json{JsonConvert.SerializeObject(queueGPS)}");
                    //}
                    ////NewAlarm
                    //var queueNewAlarm = GpsLocalStorage.DeserializeStorage<GPSInfoDto>("NewAlarm.db");
                    //if (queueNewAlarm != null && queueNewAlarm.Count > 0)
                    //{
                    //    queueNewAlarm.ForEach(item => { GlobalCollection.GPSInfoQueueForNewAlarm.Enqueue(item); });
                    //    GpsLocalStorage.ClearStorage("NewAlarm.db");
                    //    _logger.LogDebug($"读取本地GPSInfoQueueForNewAlarm入队成功数据数量{queueNewAlarm.Count},数据Json{JsonConvert.SerializeObject(queueNewAlarm)}");
                    //}
                    ////DriverIC
                    //var queueDriverIC = GpsLocalStorage.DeserializeStorage<DriverICDto>("DriverIC.db");
                    //if (queueDriverIC != null && queueDriverIC.Count > 0)
                    //{
                    //    queueDriverIC.ForEach(item => { GlobalCollection.DriverICMsgQueue.Enqueue(item); });
                    //    GpsLocalStorage.ClearStorage("DriverIC.db");
                    //    _logger.LogDebug($"读取本地DriverICMsgQueue入队成功数据数量{queueDriverIC.Count},数据Json{JsonConvert.SerializeObject(queueDriverIC)}");
                    //}
                    ////ADASAlarm
                    //var queueADASAlarm = GpsLocalStorage.DeserializeStorage<ADASAlarmDto>("ADASAlarm.db");
                    //if (queueADASAlarm != null && queueADASAlarm.Count > 0)
                    //{
                    //    queueADASAlarm.ForEach(item => { GlobalCollection.ADASAlarmMsgQueue.Enqueue(item); });
                    //    GpsLocalStorage.ClearStorage("ADASAlarm.db");
                    //    _logger.LogDebug($"读取本地ADASAlarmMsgQueue入队成功数据数量{queueADASAlarm.Count},数据Json{JsonConvert.SerializeObject(queueADASAlarm)}");
                    //}
                    ////DSMAlarm
                    //var queueDSMAlarm = GpsLocalStorage.DeserializeStorage<DSMAlarmDto>("DSMAlarm.db");
                    //if (queueDSMAlarm != null && queueDSMAlarm.Count > 0)
                    //{
                    //    queueDSMAlarm.ForEach(item => { GlobalCollection.DSMAlarmMsgQueue.Enqueue(item); });
                    //    GpsLocalStorage.ClearStorage("DSMAlarm.db");
                    //    _logger.LogDebug($"读取本地DSMAlarmMsgQueue入队成功数据数量{queueDSMAlarm.Count},数据Json{JsonConvert.SerializeObject(queueDSMAlarm)}");
                    //}
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            });
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("是否确认退出应用？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                GlobalCollection.RecvBufferStop = true;
                //DialogInfoTips dialogInfoTips = new DialogInfoTips();
                //Task.Run(() =>
                //{
                //    try
                //    {
                //        //等待插入数据库线程执行完操作和消息内存处理完毕
                //        while (!GlobalCollection.ExitSource.Token.IsCancellationRequested
                //        && (_jT809MainServerMsgChannel.MsgChannel.Reader.Count > 0 || _jT809SubordinateClientMsgChannel.MsgChannel.Reader.Count > 0))
                //        {
                //            Thread.Sleep(1000);
                //        }
                //        //放在最后执行,等待插入数据库线程执行操作完成后在加入剩余数据
                //        GpsLocalStorage.SerializeStorage(GlobalCollection.GPSInfoQueue.ToList(), "gps.db");
                //        GpsLocalStorage.SerializeStorage(GlobalCollection.GPSInfoQueueForNewAlarm.ToList(), "NewAlarm.db");
                //        GpsLocalStorage.SerializeStorage(GlobalCollection.DriverICMsgQueue.ToList(), "DriverIC.db");
                //        GpsLocalStorage.SerializeStorage(GlobalCollection.ADASAlarmMsgQueue.ToList(), "ADASAlarm.db");
                //        GpsLocalStorage.SerializeStorage(GlobalCollection.DSMAlarmMsgQueue.ToList(), "DSMAlarm.db");

                //        _logger.LogDebug(@$"当前内存队列中数据剩余：GPSInfoQueue>>>{GlobalCollection.GPSInfoQueue.Count}条,
                //                                                GPSInfoQueueForNewAlarm>>{GlobalCollection.GPSInfoQueueForNewAlarm.Count}条，
                //                                                DriverICMsgQueue>>>{GlobalCollection.DriverICMsgQueue.Count}条，
                //                                                ADASAlarmMsgQueue>>>{GlobalCollection.ADASAlarmMsgQueue.Count}条，
                //                                                DSMAlarmMsgQueue>>>{GlobalCollection.DSMAlarmMsgQueue.Count}条，于{DateTime.Now} 关闭程序...");
                //        //关闭弹窗
                //        if (dialogInfoTips.InvokeRequired)
                //        {
                //            dialogInfoTips.Invoke(new Action(() =>
                //            {
                //                dialogInfoTips.Close();
                //            }));
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        _logger.LogError(ex, ex.Message);
                //    }
                //    finally
                //    {
                //        MainForm_FormClosed(null, null);
                //    }
                //});
                _logger.LogDebug("开始退出应用.....");
                //dialogInfoTips.ShowDialog();
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。
            Process.GetCurrentProcess().Kill();// 杀掉进程
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {

        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = e.ExceptionObject as Exception;
                _logger.LogError(ex, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}