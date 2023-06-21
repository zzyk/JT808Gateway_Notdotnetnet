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
        /// ���ٵĳ��ƺ�
        /// </summary>
        private static string TrackVehNo { get; set; } = string.Empty;
        /// <summary>
        /// ÿ�Մh��logʱ��
        /// </summary>
        private string deleteTime { get; set; } = DateTime.Now.ToString();
        /// <summary>
        /// ��־��ʾ����
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
            //�����쳣
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            //Э���׼
            string AlarmAnalysisStr = string.Empty;
            switch (_jT809Configuration.AlarmAnalysis)
            {
                case 0:
                    AlarmAnalysisStr = "����";
                    break;
                case 1:
                    AlarmAnalysisStr = "�ձ�";
                    break;
                case 2:
                    AlarmAnalysisStr = "���ձ�";
                    break;
                case 3:
                    AlarmAnalysisStr = "�Ĵ���";
                    break;
            }
            this.Text = $"{Text}-������׼({AlarmAnalysisStr})��Э��汾(����2011��2019):{System.Windows.Forms.Application.ProductVersion} ����ʱ��:{DateTime.Now}";
            WinFromDisplay.ShowSysMsg += this.ShowSysMsg;
            WinFromDisplay.ShowOperatorLineMsg += this.ShowOperatorLineMsg;
            WinFromDisplay.ShowOperatorTrackMsg += this.ShowOperatorTrackMsg;
            WinFromDisplay.ClientLoginRemove += this.ClientLoginRemove;
            //��ʱ�h��log
            TimeToDeleteLog();
            //��ʱGCǿ�ƻ���
            GCExecutionCheck();
            //��ʼ��ѩ��Id�㷨
            YitIdHelper.SetIdGenerator(new IdGeneratorOptions(1));
            //�״ζ�ȡ�������ӷ������Ӫ������
            //var status = _mainFormService.UpdateOperatorInfo();
            //if (!status)
            //{
            //    MessageBox.Show("��ȡ���ݿ���Ӫ�����ϣ�ʧ�ܣ�");
            //    //������׵��˳���ʽ������ʲô�̶߳���ǿ���˳����ѳ�������ĺܸɾ���
            //    Process.GetCurrentProcess().Kill();// ɱ������
            //    return;
            //}
            await _jt808TcpServer.StartAsync(_cancellationToken);
            await _jt808UdpServer.StartAsync(_cancellationToken);
            await _CallHttpClientJob.StartAsync(_cancellationToken);
            await _UpJob.StartAsync(_cancellationToken);
            ////����809TCP
            //await _jT809_2011MainServerHost.StartAsync();
            //await _jT809_2019MainServerHost.StartAsync();
            ////����������·��Ϣ����
            //await _jT809MainServerMsgDispatchHostHandler.StartAsync();
            //await _jT809SubordinateClientMsgDispatchHostHandler.StartAsync();
            ////����ƽ̨��Ϣ����
            //_mainFormService.UpdatePlatformData();
            ////���°��공��������Ϣ
            //_mainFormService.UpdateVehInfo();
            ////���³���������Ϣ����
            //_mainFormService.UpdateVehicleData();
            ////��ȡ�����ļ������������
            //ReadLocalStorageHandle();
            ////����GPS��Ϣ��Kafka����,������������д��Oracle���ݿ�
            //_mainFormService.SaveGPSToKafka();
            ////�����ն˱�����Ϣ��Kafka����,������������д��Oracle���ݿ�
            //_mainFormService.ProcessNewAlarmToKafka();
            ////�����ʻԱ�����Ϣ��Kafka����,������������д��Oracle���ݿ�
            //_mainFormService.ProcessDriverICToKafka();
            ////����ADAS������Ϣ��Kafka����,������������д��Oracle���ݿ�
            //_mainFormService.ProcessADASAlarmToKafka();
            ////����DSM������Ϣ��Kafka����,������������д��Oracle���ݿ�
            //_mainFormService.ProcessDSMAlarmToKafka();
            ////��Ӫ�������б���kafka������
            //_mainFormService.VideoCommandCenterTransferProducerOnlineOperationListToKafka();
            ////�·�������ϢKafka������,�����߲��������Ŀ
            //_mainFormService.DownMsgVehicleConsumerTask();
            ////�����·����ݵ����ӷ������Ӫ�̵����ݶ���,����·������Ϣ
            //_mainFormService.TranspondOperator();
            ////(0x9404������ȫ��������Ŀ¼������Ϣ)����·��Ϣ���г���
            //_mainFormService.DownFile();
            ////(0x9404������ȫ��������Ŀ¼������Ϣ)����·��Ϣ����
            //_mainFormService.ReSendDownFile();
            ////������Ӫ�̴������ݼ�¼
            //_mainFormService.UpdateOPERATORCONNSTATELOG();
        }
        /// <summary>
        /// ��ʱ�h��log
        /// </summary>
        private void TimeToDeleteLog()
        {
            //ÿ4Сʱִ��һ�μ���ʱ��ִ�Єh��log
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000 * 60 * 4;//ÿ4Сʱִ��һ��
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
                                _logger.LogDebug($"ɾ����־·��:{file.FullName}");
                            }
                        }
                        deleteTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm");
                    }
                    _logger.LogDebug("ÿ4Сʱִ��һ�μ���ʱ��ִ��");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            });
            timer.AutoReset = true;//�Զ�ִ��
            timer.Enabled = true;
        }
        /// <summary>
        /// ��ʱGCǿ�ƻ���
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void GCExecutionCheck()
        {
            if (_systemConfiguration.GCExecutionTime > 0)
            {
                System.Timers.Timer timerGC = new System.Timers.Timer();
                timerGC.Interval = 1000 * 60 * _systemConfiguration.GCExecutionTime;//?����GCǿ�ƻ���һ��
                timerGC.Elapsed += new System.Timers.ElapsedEventHandler((source, e) =>
                {
                    try
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        _logger.LogDebug($"ÿ{_systemConfiguration.GCExecutionTime}����GCǿ�ƻ���һ�μ�ʱ��ִ��");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                });
                timerGC.AutoReset = true;//�Զ�ִ��
                timerGC.Enabled = true;
            }
        }
        /// <summary>
        /// ҳ����ʾ��Ϣ
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
        /// �Ƴ���¼��ʾ
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
                            li.SubItems[2].Text = "δ����";
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
        /// ��ʾ������־
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
        private void ���ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            lvwOperatorLineMsg.Items.Clear();
        }
        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
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
                var directoryPath = $"{Directory.GetCurrentDirectory()}/���ٳ�����¼/{DateTime.Now.ToString("yyyy-MM-dd")}/{vehicleNo}_{vehicleColor}";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string filePath = $"{directoryPath}/{vehicleNo}_{vehicleColor}_{DateTime.Now.ToString("yyyy_MM_dd_HH")}.log";
                //File.AppendAllText(filePath, $"��ҵ�����룺{gnid},���ƺţ�{vehicleNo},������ɫ��{(JT808VehicleColorType)vehicleColor},ʱ��:{time},��Ϣ��{isUp}<<< {note}\r\n");
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
                case 1://״̬�б�
                    break;
            }
        }
        private void ���ToolStripMenuItem_Click(object sender, EventArgs e)
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
        private void ȡ��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (ClientLogin.SelectedItems.Count > 0)
            //{
            //    JT808Session tcpPro = (JT808Session)ClientLogin.SelectedItems[0].Tag;
            //    tcpPro.ShowLog = false;
            //    ClientLogin.SelectedItems[0].BackColor = Color.White;
            //}
        }
        private void ��������·�쳣ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (ClientLogin.SelectedItems.Count > 0)
            //{
            //    JT808Session tcpPro = (JT808Session)ClientLogin.SelectedItems[0].Tag;
            //    tcpPro.TestError = true;
            //    ShowOperatorLineMsg("��������·�쳣����", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "��������·�쳣����", "�����Ҫ�ȴ�����60��", "");
            //    ClientLogin.SelectedItems[0].BackColor = Color.DarkOrange;
            //}
        }
        private void ȡ������·�쳣ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (ClientLogin.SelectedItems.Count > 0)
            //{
            //    JT808Session tcpPro = (JT808Session)ClientLogin.SelectedItems[0].Tag;
            //    tcpPro.TestError = false;
            //    ShowOperatorLineMsg("ȡ������·�쳣����", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "ȡ������·�쳣����", "", "");
            //    ClientLogin.SelectedItems[0].BackColor = Color.White;
            //}
        }
        private void ��¼����GPS���ݳ����б�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (ClientLogin.SelectedItems.Count > 0)
            //{
            //    JT808Session tcpPro = (JT808Session)ClientLogin.SelectedItems[0].Tag;
            //    if (tcpPro.HaveVeh.Keys.Count > 0)
            //    {
            //        var text = string.Join("\r\n", tcpPro.HaveVeh.Keys);
            //        var directoryPath = $"{Directory.GetCurrentDirectory()}/����GPS���ݳ����б�";
            //        if (!Directory.Exists(directoryPath))
            //        {
            //            Directory.CreateDirectory(directoryPath);
            //        }
            //        //GlobalCollection.OperatorInfoHash.TryGetValue(tcpPro.MsgGNSSCENTERID.ToString(), out var oper);
            //        string filePath = $"{directoryPath}/{StringHelper.GetIPAddress(tcpPro.MainChannel.RemoteAddress).Replace(":", "_")}_{oper?.PlatformName}_{tcpPro.JT809Version}_�����б�.txt";
            //        File.WriteAllText(filePath, text);
            //        MessageBox.Show($"����GPS���ݳ����б�д���ļ��ɹ���");
            //    }
            //    else
            //    {
            //        MessageBox.Show($"�����ݣ�");
            //    }
            //}
        }
        private void UserInfo_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {

        }
        private void button_2db_Click(object sender, EventArgs e)
        {

        }
        private void ���ToolStripMenuItem1_Click(object sender, EventArgs e)
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
                        tmp = tmp + "���ƣ�" + lv.SubItems[0].Text + " ��ҵ�룺" + lv.SubItems[1].Text + " �����У�" + lv.SubItems[2].Text + " ʱ�䣺" + lv.SubItems[3].Text + "���ݣ�" + lv.SubItems[4].Text + "\r\n";
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
                MessageBox.Show("�����복�ƺ�", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                sysRev.Text = $"808���ݽ��ջ��棺{_JT808ReceiveAtomicCounterService.MsgSuccessCount}";
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
                //        li2.SubItems.Add(tcpPro.MainConnStatus ? "������ ���ѵ�¼" : "δ����");
                //        li2.SubItems.Add(tcpPro.StartTime.ToString());
                //        li2.SubItems.Add(tcpPro.SlaveIPAddress);
                //        li2.SubItems.Add(tcpPro.SlaveConnStatus ? "������ ���ѵ�¼" : "δ����");
                //        li2.SubItems.Add("�ܰ���" + tcpPro.MainRecvAll + "/λ�ð���" + tcpPro.LocationRecv + "/��������" + tcpPro.HaveVeh?.Count);
                //        li2.SubItems.Add(tcpPro.MainSend.ToString());
                //        li2.SubItems.Add(tcpPro.SlaveRecv.ToString());
                //        li2.SubItems.Add(tcpPro.SlaveSend.ToString());
                //        li2.SubItems.Add(tcpPro.ENCRYPT_FLAG ? "��" : "��");
                //    }
                //    else
                //    {
                //        ListViewItem li = tcpPro.ListViewItem;
                //        li.SubItems[0].Text = oper?.PlatformName;
                //        li.SubItems[1].Text = tcpPro.MsgGNSSCENTERID.ToString();
                //        li.SubItems[2].Text = tcpPro.JT809Version.ToString();
                //        li.SubItems[3].Text = tcpPro.MainIPAddress;
                //        li.SubItems[4].Text = tcpPro.MainConnStatus ? "������ ���ѵ�¼" : "δ����";
                //        li.SubItems[5].Text = tcpPro.StartTime.ToString();
                //        li.SubItems[6].Text = tcpPro.SlaveIPAddress;
                //        li.SubItems[7].Text = tcpPro.SlaveConnStatus ? "������ ���ѵ�¼" : "δ����";
                //        li.SubItems[8].Text = "�ܰ���" + tcpPro.MainRecvAll + "/λ�ð���" + tcpPro.LocationRecv + "/��������" + tcpPro.HaveVeh?.Count;
                //        li.SubItems[9].Text = tcpPro.MainSend.ToString();
                //        li.SubItems[10].Text = tcpPro.SlaveRecv.ToString();
                //        li.SubItems[11].Text = tcpPro.SlaveSend.ToString();
                //        li.SubItems[12].Text = tcpPro.ENCRYPT_FLAG ? "��" : "��";
                //        //������δ���ӵ������Ƴ���ʾ
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
        /// ��ȡ�����ļ������������
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
                    //    _logger.LogDebug($"��ȡ����GPSInfoQueue��ӳɹ���������{queueGPS.Count},����Json{JsonConvert.SerializeObject(queueGPS)}");
                    //}
                    ////NewAlarm
                    //var queueNewAlarm = GpsLocalStorage.DeserializeStorage<GPSInfoDto>("NewAlarm.db");
                    //if (queueNewAlarm != null && queueNewAlarm.Count > 0)
                    //{
                    //    queueNewAlarm.ForEach(item => { GlobalCollection.GPSInfoQueueForNewAlarm.Enqueue(item); });
                    //    GpsLocalStorage.ClearStorage("NewAlarm.db");
                    //    _logger.LogDebug($"��ȡ����GPSInfoQueueForNewAlarm��ӳɹ���������{queueNewAlarm.Count},����Json{JsonConvert.SerializeObject(queueNewAlarm)}");
                    //}
                    ////DriverIC
                    //var queueDriverIC = GpsLocalStorage.DeserializeStorage<DriverICDto>("DriverIC.db");
                    //if (queueDriverIC != null && queueDriverIC.Count > 0)
                    //{
                    //    queueDriverIC.ForEach(item => { GlobalCollection.DriverICMsgQueue.Enqueue(item); });
                    //    GpsLocalStorage.ClearStorage("DriverIC.db");
                    //    _logger.LogDebug($"��ȡ����DriverICMsgQueue��ӳɹ���������{queueDriverIC.Count},����Json{JsonConvert.SerializeObject(queueDriverIC)}");
                    //}
                    ////ADASAlarm
                    //var queueADASAlarm = GpsLocalStorage.DeserializeStorage<ADASAlarmDto>("ADASAlarm.db");
                    //if (queueADASAlarm != null && queueADASAlarm.Count > 0)
                    //{
                    //    queueADASAlarm.ForEach(item => { GlobalCollection.ADASAlarmMsgQueue.Enqueue(item); });
                    //    GpsLocalStorage.ClearStorage("ADASAlarm.db");
                    //    _logger.LogDebug($"��ȡ����ADASAlarmMsgQueue��ӳɹ���������{queueADASAlarm.Count},����Json{JsonConvert.SerializeObject(queueADASAlarm)}");
                    //}
                    ////DSMAlarm
                    //var queueDSMAlarm = GpsLocalStorage.DeserializeStorage<DSMAlarmDto>("DSMAlarm.db");
                    //if (queueDSMAlarm != null && queueDSMAlarm.Count > 0)
                    //{
                    //    queueDSMAlarm.ForEach(item => { GlobalCollection.DSMAlarmMsgQueue.Enqueue(item); });
                    //    GpsLocalStorage.ClearStorage("DSMAlarm.db");
                    //    _logger.LogDebug($"��ȡ����DSMAlarmMsgQueue��ӳɹ���������{queueDSMAlarm.Count},����Json{JsonConvert.SerializeObject(queueDSMAlarm)}");
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
            DialogResult result = MessageBox.Show("�Ƿ�ȷ���˳�Ӧ�ã�", "ѯ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                GlobalCollection.RecvBufferStop = true;
                //DialogInfoTips dialogInfoTips = new DialogInfoTips();
                //Task.Run(() =>
                //{
                //    try
                //    {
                //        //�ȴ��������ݿ��߳�ִ�����������Ϣ�ڴ洦�����
                //        while (!GlobalCollection.ExitSource.Token.IsCancellationRequested
                //        && (_jT809MainServerMsgChannel.MsgChannel.Reader.Count > 0 || _jT809SubordinateClientMsgChannel.MsgChannel.Reader.Count > 0))
                //        {
                //            Thread.Sleep(1000);
                //        }
                //        //�������ִ��,�ȴ��������ݿ��߳�ִ�в�����ɺ��ڼ���ʣ������
                //        GpsLocalStorage.SerializeStorage(GlobalCollection.GPSInfoQueue.ToList(), "gps.db");
                //        GpsLocalStorage.SerializeStorage(GlobalCollection.GPSInfoQueueForNewAlarm.ToList(), "NewAlarm.db");
                //        GpsLocalStorage.SerializeStorage(GlobalCollection.DriverICMsgQueue.ToList(), "DriverIC.db");
                //        GpsLocalStorage.SerializeStorage(GlobalCollection.ADASAlarmMsgQueue.ToList(), "ADASAlarm.db");
                //        GpsLocalStorage.SerializeStorage(GlobalCollection.DSMAlarmMsgQueue.ToList(), "DSMAlarm.db");

                //        _logger.LogDebug(@$"��ǰ�ڴ����������ʣ�ࣺGPSInfoQueue>>>{GlobalCollection.GPSInfoQueue.Count}��,
                //                                                GPSInfoQueueForNewAlarm>>{GlobalCollection.GPSInfoQueueForNewAlarm.Count}����
                //                                                DriverICMsgQueue>>>{GlobalCollection.DriverICMsgQueue.Count}����
                //                                                ADASAlarmMsgQueue>>>{GlobalCollection.ADASAlarmMsgQueue.Count}����
                //                                                DSMAlarmMsgQueue>>>{GlobalCollection.DSMAlarmMsgQueue.Count}������{DateTime.Now} �رճ���...");
                //        //�رյ���
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
                _logger.LogDebug("��ʼ�˳�Ӧ��.....");
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
            //������׵��˳���ʽ������ʲô�̶߳���ǿ���˳����ѳ�������ĺܸɾ���
            Process.GetCurrentProcess().Kill();// ɱ������
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