namespace JT808Server.WinForm
{
    public partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        private TabControl tabControl1;

        private TabPage tabPage_State;

        private TabPage tabPage_Log;

        private ListView ClientLogin;

        private ContextMenuStrip contextMenuStrip_log_sys;

        private ToolStripMenuItem 清空ToolStripMenuItem;

        private ColumnHeader columnHeader_StateUSERID;

        private ColumnHeader columnHeader_StateMainConnectionAddress;

        private ColumnHeader columnHeader_StateMain;

        private ColumnHeader columnHeader_StateSalve;

        private ColumnHeader columnHeader_StateMainDate;

        private ColumnHeader columnHeader_StateMainReceive;

        private ColumnHeader columnHeader_StateMainSend;

        private ColumnHeader columnHeader_StateSalveReceive;

        private ColumnHeader columnHeader_StateSalveSend;

        private ContextMenuStrip contextMenuStrip_LinkState;

        private ToolStripMenuItem toolStripMenuItem1;

        private ColumnHeader columnHeader_StateEncry;

        public StatusStrip statusStrip_log_sys;

        public ToolStripStatusLabel toolStripStatusLabel1;

        private System.Windows.Forms.Timer tmUpdateStatus;

        private ToolStripMenuItem 取消ToolStripMenuItem;

        private TabPage tabPage_Link;

        private ListView lvwOperatorLineMsg;

        private ColumnHeader columnHeader_InteractionType;

        private ColumnHeader columnHeader_InteractionDate;

        private ColumnHeader columnHeader_InteractionInfo;

        private ColumnHeader columnHeader_InteractionNote1;

        private ColumnHeader columnHeader_InteractionNote2;

        private TabPage tabPage_GenZong;

        private ListView lvwOperatorTrackMsg;

        private ColumnHeader columnHeader_TrackTarget;

        private ColumnHeader columnHeader_TrackDate;

        private ColumnHeader columnHeader_TrackInfo;

        private Panel panel2;

        private Button btnStartTrackVeh;

        private TextBox txtTrackVeh;

        private Label label3;

        private Button btnStopTrackVeh;

        private ColumnHeader columnHeader_TrackType;

        private Button button2;

        private ContextMenuStrip contextMenuStrip_lvwOperatorTrackMsg;

        private ToolStripMenuItem 清空ToolStripMenuItem1;

        private ToolStripStatusLabel sysRev;

        private ToolStripStatusLabel sysSend;

        private ToolStripStatusLabel sysCenter;

        private ToolStripMenuItem tsmi_traceMsg_copy;

        private ColumnHeader columnHeader_TrackAccessCode;

        private ListView lv_Log_sys;

        private ColumnHeader columnHeader_LogTime;

        private ColumnHeader columnHeader_LogNotes;

        private ToolStripMenuItem rmi_log_sys_copy;

        private ToolStripStatusLabel syskafka;

        private CheckBox isShowTrackInfo;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tabControl1 = new TabControl();
            tabPage_Log = new TabPage();
            lv_Log_sys = new ListView();
            columnHeader_LogTime = new ColumnHeader();
            columnHeader_SimNo = new ColumnHeader();
            columnHeader_LogNotes = new ColumnHeader();
            contextMenuStrip_log_sys = new ContextMenuStrip(components);
            清空ToolStripMenuItem = new ToolStripMenuItem();
            rmi_log_sys_copy = new ToolStripMenuItem();
            statusStrip_log_sys = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            sysRev = new ToolStripStatusLabel();
            sysSend = new ToolStripStatusLabel();
            sysCenter = new ToolStripStatusLabel();
            syskafka = new ToolStripStatusLabel();
            tabPage_State = new TabPage();
            ClientLogin = new ListView();
            columnHeader_YunYingName = new ColumnHeader();
            columnHeader_StateUSERID = new ColumnHeader();
            columnHeader_StateVersion = new ColumnHeader();
            columnHeader_StateMainConnectionAddress = new ColumnHeader();
            columnHeader_StateMain = new ColumnHeader();
            columnHeader_StateMainDate = new ColumnHeader();
            columnHeader_StateSalveConnectionAddress = new ColumnHeader();
            columnHeader_StateSalve = new ColumnHeader();
            columnHeader_StateMainReceive = new ColumnHeader();
            columnHeader_StateMainSend = new ColumnHeader();
            columnHeader_StateSalveReceive = new ColumnHeader();
            columnHeader_StateSalveSend = new ColumnHeader();
            columnHeader_StateEncry = new ColumnHeader();
            contextMenuStrip_LinkState = new ContextMenuStrip(components);
            toolStripMenuItem1 = new ToolStripMenuItem();
            取消ToolStripMenuItem = new ToolStripMenuItem();
            触发主链路异常ToolStripMenuItem = new ToolStripMenuItem();
            取消主链路异常ToolStripMenuItem = new ToolStripMenuItem();
            记录传输GPS数据车辆列表ToolStripMenuItem = new ToolStripMenuItem();
            tabPage_Link = new TabPage();
            lvwOperatorLineMsg = new ListView();
            columnHeader_InteractionType = new ColumnHeader();
            columnHeader_InteractionDate = new ColumnHeader();
            columnHeader_InteractionInfo = new ColumnHeader();
            columnHeader_InteractionNote1 = new ColumnHeader();
            columnHeader_InteractionNote2 = new ColumnHeader();
            contextMenuStrip_lvwOperatorLineMsg = new ContextMenuStrip(components);
            清空ToolStripMenuItem2 = new ToolStripMenuItem();
            复制ToolStripMenuItem = new ToolStripMenuItem();
            tabPage_GenZong = new TabPage();
            lvwOperatorTrackMsg = new ListView();
            columnHeader_TrackTarget = new ColumnHeader();
            columnHeader_TrackAccessCode = new ColumnHeader();
            columnHeader_TrackType = new ColumnHeader();
            columnHeader_TrackDate = new ColumnHeader();
            columnHeader_TrackInfo = new ColumnHeader();
            contextMenuStrip_lvwOperatorTrackMsg = new ContextMenuStrip(components);
            清空ToolStripMenuItem1 = new ToolStripMenuItem();
            tsmi_traceMsg_copy = new ToolStripMenuItem();
            panel2 = new Panel();
            isShowTrackInfo = new CheckBox();
            button2 = new Button();
            btnStopTrackVeh = new Button();
            btnStartTrackVeh = new Button();
            txtTrackVeh = new TextBox();
            label3 = new Label();
            tmUpdateStatus = new System.Windows.Forms.Timer(components);
            lvw_online = new ListView();
            columnHeader1_SN = new ColumnHeader();
            columnHeader_PlateNo = new ColumnHeader();
            columnHeader_Sim = new ColumnHeader();
            columnHeader_IP = new ColumnHeader();
            columnHeader_StartTime = new ColumnHeader();
            columnHeader_Online = new ColumnHeader();
            columnHeader_Position = new ColumnHeader();
            columnHeader_Error = new ColumnHeader();
            columnHeader_Offline = new ColumnHeader();
            columnHeader_queue = new ColumnHeader();
            columnHeader_status = new ColumnHeader();
            columnHeader_trans = new ColumnHeader();
            textBox1 = new TextBox();
            lbl_dev = new Label();
            splitContainer1 = new SplitContainer();
            tabControl1.SuspendLayout();
            tabPage_Log.SuspendLayout();
            contextMenuStrip_log_sys.SuspendLayout();
            statusStrip_log_sys.SuspendLayout();
            tabPage_State.SuspendLayout();
            contextMenuStrip_LinkState.SuspendLayout();
            tabPage_Link.SuspendLayout();
            contextMenuStrip_lvwOperatorLineMsg.SuspendLayout();
            tabPage_GenZong.SuspendLayout();
            contextMenuStrip_lvwOperatorTrackMsg.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage_Log);
            tabControl1.Controls.Add(tabPage_State);
            tabControl1.Controls.Add(tabPage_Link);
            tabControl1.Controls.Add(tabPage_GenZong);
            tabControl1.Location = new Point(3, 4);
            tabControl1.Margin = new Padding(3, 4, 3, 4);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1195, 278);
            tabControl1.TabIndex = 0;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabPage_Log
            // 
            tabPage_Log.Controls.Add(lv_Log_sys);
            tabPage_Log.Controls.Add(statusStrip_log_sys);
            tabPage_Log.Location = new Point(4, 26);
            tabPage_Log.Margin = new Padding(3, 4, 3, 4);
            tabPage_Log.Name = "tabPage_Log";
            tabPage_Log.Padding = new Padding(3, 4, 3, 4);
            tabPage_Log.Size = new Size(1187, 248);
            tabPage_Log.TabIndex = 1;
            tabPage_Log.Text = "日志信息";
            tabPage_Log.UseVisualStyleBackColor = true;
            // 
            // lv_Log_sys
            // 
            lv_Log_sys.Columns.AddRange(new ColumnHeader[] { columnHeader_LogTime, columnHeader_SimNo, columnHeader_LogNotes });
            lv_Log_sys.ContextMenuStrip = contextMenuStrip_log_sys;
            lv_Log_sys.FullRowSelect = true;
            lv_Log_sys.Location = new Point(3, 4);
            lv_Log_sys.Margin = new Padding(3, 4, 3, 4);
            lv_Log_sys.Name = "lv_Log_sys";
            lv_Log_sys.Size = new Size(1188, 210);
            lv_Log_sys.TabIndex = 127;
            lv_Log_sys.UseCompatibleStateImageBehavior = false;
            lv_Log_sys.View = View.Details;
            // 
            // columnHeader_LogTime
            // 
            columnHeader_LogTime.Text = "时间";
            columnHeader_LogTime.Width = 180;
            // 
            // columnHeader_SimNo
            // 
            columnHeader_SimNo.Text = "Sim卡号";
            columnHeader_SimNo.Width = 120;
            // 
            // columnHeader_LogNotes
            // 
            columnHeader_LogNotes.Text = "数据包";
            columnHeader_LogNotes.Width = 769;
            // 
            // contextMenuStrip_log_sys
            // 
            contextMenuStrip_log_sys.ImageScalingSize = new Size(20, 20);
            contextMenuStrip_log_sys.Items.AddRange(new ToolStripItem[] { 清空ToolStripMenuItem, rmi_log_sys_copy });
            contextMenuStrip_log_sys.Name = "contextMenuStrip1";
            contextMenuStrip_log_sys.Size = new Size(101, 48);
            // 
            // 清空ToolStripMenuItem
            // 
            清空ToolStripMenuItem.Name = "清空ToolStripMenuItem";
            清空ToolStripMenuItem.Size = new Size(100, 22);
            清空ToolStripMenuItem.Text = "清空";
            清空ToolStripMenuItem.Click += 清空ToolStripMenuItem_Click;
            // 
            // rmi_log_sys_copy
            // 
            rmi_log_sys_copy.Name = "rmi_log_sys_copy";
            rmi_log_sys_copy.Size = new Size(100, 22);
            rmi_log_sys_copy.Text = "复制";
            rmi_log_sys_copy.Click += rmi_log_sys_copy_Click;
            // 
            // statusStrip_log_sys
            // 
            statusStrip_log_sys.ImageScalingSize = new Size(20, 20);
            statusStrip_log_sys.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, sysRev, sysSend, sysCenter, syskafka });
            statusStrip_log_sys.Location = new Point(3, 218);
            statusStrip_log_sys.Name = "statusStrip_log_sys";
            statusStrip_log_sys.Padding = new Padding(1, 0, 16, 0);
            statusStrip_log_sys.RightToLeft = RightToLeft.No;
            statusStrip_log_sys.Size = new Size(1181, 26);
            statusStrip_log_sys.TabIndex = 124;
            statusStrip_log_sys.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(0, 21);
            // 
            // sysRev
            // 
            sysRev.Name = "sysRev";
            sysRev.Size = new Size(120, 21);
            sysRev.Text = "808数据接收缓存：0";
            // 
            // sysSend
            // 
            sysSend.BorderSides = ToolStripStatusLabelBorderSides.Left;
            sysSend.Name = "sysSend";
            sysSend.Size = new Size(103, 21);
            sysSend.Text = "数据发送缓存：0";
            // 
            // sysCenter
            // 
            sysCenter.Name = "sysCenter";
            sysCenter.Size = new Size(87, 21);
            sysCenter.Text = "中心连接数：0";
            // 
            // syskafka
            // 
            syskafka.Name = "syskafka";
            syskafka.Size = new Size(108, 21);
            syskafka.Text = "kafka接收/发送数:";
            // 
            // tabPage_State
            // 
            tabPage_State.Controls.Add(ClientLogin);
            tabPage_State.Location = new Point(4, 26);
            tabPage_State.Margin = new Padding(3, 4, 3, 4);
            tabPage_State.Name = "tabPage_State";
            tabPage_State.Padding = new Padding(3, 4, 3, 4);
            tabPage_State.Size = new Size(1187, 248);
            tabPage_State.TabIndex = 0;
            tabPage_State.Text = "连接状态列表";
            tabPage_State.UseVisualStyleBackColor = true;
            // 
            // ClientLogin
            // 
            ClientLogin.Columns.AddRange(new ColumnHeader[] { columnHeader_YunYingName, columnHeader_StateUSERID, columnHeader_StateVersion, columnHeader_StateMainConnectionAddress, columnHeader_StateMain, columnHeader_StateMainDate, columnHeader_StateSalveConnectionAddress, columnHeader_StateSalve, columnHeader_StateMainReceive, columnHeader_StateMainSend, columnHeader_StateSalveReceive, columnHeader_StateSalveSend, columnHeader_StateEncry });
            ClientLogin.ContextMenuStrip = contextMenuStrip_LinkState;
            ClientLogin.Dock = DockStyle.Fill;
            ClientLogin.FullRowSelect = true;
            ClientLogin.Location = new Point(3, 4);
            ClientLogin.Margin = new Padding(3, 4, 3, 4);
            ClientLogin.MultiSelect = false;
            ClientLogin.Name = "ClientLogin";
            ClientLogin.Size = new Size(1181, 240);
            ClientLogin.TabIndex = 0;
            ClientLogin.UseCompatibleStateImageBehavior = false;
            ClientLogin.View = View.Details;
            ClientLogin.ColumnWidthChanged += ClientLogin_ColumnWidthChanged;
            // 
            // columnHeader_YunYingName
            // 
            columnHeader_YunYingName.Text = "运营商名称";
            columnHeader_YunYingName.Width = 200;
            // 
            // columnHeader_StateUSERID
            // 
            columnHeader_StateUSERID.Text = "接入码";
            columnHeader_StateUSERID.Width = 100;
            // 
            // columnHeader_StateVersion
            // 
            columnHeader_StateVersion.Text = "协议版本";
            columnHeader_StateVersion.Width = 100;
            // 
            // columnHeader_StateMainConnectionAddress
            // 
            columnHeader_StateMainConnectionAddress.Text = "主链路地址";
            columnHeader_StateMainConnectionAddress.Width = 150;
            // 
            // columnHeader_StateMain
            // 
            columnHeader_StateMain.Text = "主链路";
            columnHeader_StateMain.Width = 150;
            // 
            // columnHeader_StateMainDate
            // 
            columnHeader_StateMainDate.Text = "主连接时间";
            columnHeader_StateMainDate.Width = 165;
            // 
            // columnHeader_StateSalveConnectionAddress
            // 
            columnHeader_StateSalveConnectionAddress.Text = "从链路地址";
            columnHeader_StateSalveConnectionAddress.Width = 150;
            // 
            // columnHeader_StateSalve
            // 
            columnHeader_StateSalve.Text = "从链路";
            columnHeader_StateSalve.Width = 150;
            // 
            // columnHeader_StateMainReceive
            // 
            columnHeader_StateMainReceive.Text = "主链路接收";
            columnHeader_StateMainReceive.Width = 250;
            // 
            // columnHeader_StateMainSend
            // 
            columnHeader_StateMainSend.Text = "主链路发送";
            columnHeader_StateMainSend.Width = 100;
            // 
            // columnHeader_StateSalveReceive
            // 
            columnHeader_StateSalveReceive.Text = "从链路接收";
            columnHeader_StateSalveReceive.Width = 100;
            // 
            // columnHeader_StateSalveSend
            // 
            columnHeader_StateSalveSend.Text = "从链路发送";
            columnHeader_StateSalveSend.Width = 100;
            // 
            // columnHeader_StateEncry
            // 
            columnHeader_StateEncry.Text = "加密";
            columnHeader_StateEncry.Width = 100;
            // 
            // contextMenuStrip_LinkState
            // 
            contextMenuStrip_LinkState.ImageScalingSize = new Size(20, 20);
            contextMenuStrip_LinkState.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, 取消ToolStripMenuItem, 触发主链路异常ToolStripMenuItem, 取消主链路异常ToolStripMenuItem, 记录传输GPS数据车辆列表ToolStripMenuItem });
            contextMenuStrip_LinkState.Name = "contextMenuStrip1";
            contextMenuStrip_LinkState.Size = new Size(220, 114);
            contextMenuStrip_LinkState.Text = "取消";
            contextMenuStrip_LinkState.Opening += contextMenuStrip2_Opening;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(219, 22);
            toolStripMenuItem1.Text = "跟踪";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // 取消ToolStripMenuItem
            // 
            取消ToolStripMenuItem.Name = "取消ToolStripMenuItem";
            取消ToolStripMenuItem.Size = new Size(219, 22);
            取消ToolStripMenuItem.Text = "取消";
            取消ToolStripMenuItem.Click += 取消ToolStripMenuItem_Click;
            // 
            // 触发主链路异常ToolStripMenuItem
            // 
            触发主链路异常ToolStripMenuItem.Name = "触发主链路异常ToolStripMenuItem";
            触发主链路异常ToolStripMenuItem.Size = new Size(219, 22);
            触发主链路异常ToolStripMenuItem.Text = "触发主链路异常";
            触发主链路异常ToolStripMenuItem.Click += 触发主链路异常ToolStripMenuItem_Click;
            // 
            // 取消主链路异常ToolStripMenuItem
            // 
            取消主链路异常ToolStripMenuItem.Name = "取消主链路异常ToolStripMenuItem";
            取消主链路异常ToolStripMenuItem.Size = new Size(219, 22);
            取消主链路异常ToolStripMenuItem.Text = "取消主链路异常";
            取消主链路异常ToolStripMenuItem.Click += 取消主链路异常ToolStripMenuItem_Click;
            // 
            // 记录传输GPS数据车辆列表ToolStripMenuItem
            // 
            记录传输GPS数据车辆列表ToolStripMenuItem.Name = "记录传输GPS数据车辆列表ToolStripMenuItem";
            记录传输GPS数据车辆列表ToolStripMenuItem.Size = new Size(219, 22);
            记录传输GPS数据车辆列表ToolStripMenuItem.Text = "记录传输GPS数据车辆列表";
            记录传输GPS数据车辆列表ToolStripMenuItem.Click += 记录传输GPS数据车辆列表ToolStripMenuItem_Click;
            // 
            // tabPage_Link
            // 
            tabPage_Link.Controls.Add(lvwOperatorLineMsg);
            tabPage_Link.Location = new Point(4, 26);
            tabPage_Link.Margin = new Padding(3, 4, 3, 4);
            tabPage_Link.Name = "tabPage_Link";
            tabPage_Link.Padding = new Padding(3, 4, 3, 4);
            tabPage_Link.Size = new Size(1187, 248);
            tabPage_Link.TabIndex = 3;
            tabPage_Link.Text = "链接交互日志";
            tabPage_Link.UseVisualStyleBackColor = true;
            // 
            // lvwOperatorLineMsg
            // 
            lvwOperatorLineMsg.Columns.AddRange(new ColumnHeader[] { columnHeader_InteractionType, columnHeader_InteractionDate, columnHeader_InteractionInfo, columnHeader_InteractionNote1, columnHeader_InteractionNote2 });
            lvwOperatorLineMsg.ContextMenuStrip = contextMenuStrip_lvwOperatorLineMsg;
            lvwOperatorLineMsg.Dock = DockStyle.Fill;
            lvwOperatorLineMsg.FullRowSelect = true;
            lvwOperatorLineMsg.Location = new Point(3, 4);
            lvwOperatorLineMsg.Margin = new Padding(3, 4, 3, 4);
            lvwOperatorLineMsg.MultiSelect = false;
            lvwOperatorLineMsg.Name = "lvwOperatorLineMsg";
            lvwOperatorLineMsg.Size = new Size(1181, 240);
            lvwOperatorLineMsg.TabIndex = 0;
            lvwOperatorLineMsg.UseCompatibleStateImageBehavior = false;
            lvwOperatorLineMsg.View = View.Details;
            // 
            // columnHeader_InteractionType
            // 
            columnHeader_InteractionType.Text = "事件类型";
            columnHeader_InteractionType.Width = 141;
            // 
            // columnHeader_InteractionDate
            // 
            columnHeader_InteractionDate.Text = "事件时间";
            columnHeader_InteractionDate.Width = 142;
            // 
            // columnHeader_InteractionInfo
            // 
            columnHeader_InteractionInfo.Text = "事件内容";
            columnHeader_InteractionInfo.Width = 317;
            // 
            // columnHeader_InteractionNote1
            // 
            columnHeader_InteractionNote1.Text = "备注1";
            columnHeader_InteractionNote1.Width = 83;
            // 
            // columnHeader_InteractionNote2
            // 
            columnHeader_InteractionNote2.Text = "备注2";
            columnHeader_InteractionNote2.Width = 70;
            // 
            // contextMenuStrip_lvwOperatorLineMsg
            // 
            contextMenuStrip_lvwOperatorLineMsg.ImageScalingSize = new Size(20, 20);
            contextMenuStrip_lvwOperatorLineMsg.Items.AddRange(new ToolStripItem[] { 清空ToolStripMenuItem2, 复制ToolStripMenuItem });
            contextMenuStrip_lvwOperatorLineMsg.Name = "contextMenuStrip_lvwOperatorLineMsg";
            contextMenuStrip_lvwOperatorLineMsg.Size = new Size(101, 48);
            // 
            // 清空ToolStripMenuItem2
            // 
            清空ToolStripMenuItem2.Name = "清空ToolStripMenuItem2";
            清空ToolStripMenuItem2.Size = new Size(100, 22);
            清空ToolStripMenuItem2.Text = "清空";
            清空ToolStripMenuItem2.Click += 清空ToolStripMenuItem2_Click;
            // 
            // 复制ToolStripMenuItem
            // 
            复制ToolStripMenuItem.Name = "复制ToolStripMenuItem";
            复制ToolStripMenuItem.Size = new Size(100, 22);
            复制ToolStripMenuItem.Text = "复制";
            复制ToolStripMenuItem.Click += 复制ToolStripMenuItem_Click;
            // 
            // tabPage_GenZong
            // 
            tabPage_GenZong.Controls.Add(lvwOperatorTrackMsg);
            tabPage_GenZong.Controls.Add(panel2);
            tabPage_GenZong.Location = new Point(4, 26);
            tabPage_GenZong.Margin = new Padding(3, 4, 3, 4);
            tabPage_GenZong.Name = "tabPage_GenZong";
            tabPage_GenZong.Padding = new Padding(3, 4, 3, 4);
            tabPage_GenZong.Size = new Size(1187, 248);
            tabPage_GenZong.TabIndex = 4;
            tabPage_GenZong.Text = "跟踪信息";
            tabPage_GenZong.UseVisualStyleBackColor = true;
            // 
            // lvwOperatorTrackMsg
            // 
            lvwOperatorTrackMsg.BackColor = SystemColors.Window;
            lvwOperatorTrackMsg.Columns.AddRange(new ColumnHeader[] { columnHeader_TrackTarget, columnHeader_TrackAccessCode, columnHeader_TrackType, columnHeader_TrackDate, columnHeader_TrackInfo });
            lvwOperatorTrackMsg.ContextMenuStrip = contextMenuStrip_lvwOperatorTrackMsg;
            lvwOperatorTrackMsg.Dock = DockStyle.Fill;
            lvwOperatorTrackMsg.FullRowSelect = true;
            lvwOperatorTrackMsg.Location = new Point(3, 4);
            lvwOperatorTrackMsg.Margin = new Padding(3, 4, 3, 4);
            lvwOperatorTrackMsg.Name = "lvwOperatorTrackMsg";
            lvwOperatorTrackMsg.Size = new Size(1181, 199);
            lvwOperatorTrackMsg.TabIndex = 1;
            lvwOperatorTrackMsg.UseCompatibleStateImageBehavior = false;
            lvwOperatorTrackMsg.View = View.Details;
            // 
            // columnHeader_TrackTarget
            // 
            columnHeader_TrackTarget.Text = "目标";
            columnHeader_TrackTarget.Width = 107;
            // 
            // columnHeader_TrackAccessCode
            // 
            columnHeader_TrackAccessCode.Text = "接入码";
            columnHeader_TrackAccessCode.Width = 68;
            // 
            // columnHeader_TrackType
            // 
            columnHeader_TrackType.Text = "类型";
            columnHeader_TrackType.Width = 70;
            // 
            // columnHeader_TrackDate
            // 
            columnHeader_TrackDate.Text = "时间";
            columnHeader_TrackDate.Width = 130;
            // 
            // columnHeader_TrackInfo
            // 
            columnHeader_TrackInfo.Text = "内容";
            columnHeader_TrackInfo.Width = 355;
            // 
            // contextMenuStrip_lvwOperatorTrackMsg
            // 
            contextMenuStrip_lvwOperatorTrackMsg.ImageScalingSize = new Size(20, 20);
            contextMenuStrip_lvwOperatorTrackMsg.Items.AddRange(new ToolStripItem[] { 清空ToolStripMenuItem1, tsmi_traceMsg_copy });
            contextMenuStrip_lvwOperatorTrackMsg.Name = "contextMenuStrip3";
            contextMenuStrip_lvwOperatorTrackMsg.Size = new Size(101, 48);
            // 
            // 清空ToolStripMenuItem1
            // 
            清空ToolStripMenuItem1.Name = "清空ToolStripMenuItem1";
            清空ToolStripMenuItem1.Size = new Size(100, 22);
            清空ToolStripMenuItem1.Text = "清空";
            清空ToolStripMenuItem1.Click += 清空ToolStripMenuItem1_Click;
            // 
            // tsmi_traceMsg_copy
            // 
            tsmi_traceMsg_copy.Name = "tsmi_traceMsg_copy";
            tsmi_traceMsg_copy.Size = new Size(100, 22);
            tsmi_traceMsg_copy.Text = "复制";
            tsmi_traceMsg_copy.Click += tsmi_traceMsg_copy_Click;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.Control;
            panel2.Controls.Add(isShowTrackInfo);
            panel2.Controls.Add(button2);
            panel2.Controls.Add(btnStopTrackVeh);
            panel2.Controls.Add(btnStartTrackVeh);
            panel2.Controls.Add(txtTrackVeh);
            panel2.Controls.Add(label3);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(3, 203);
            panel2.Margin = new Padding(3, 4, 3, 4);
            panel2.Name = "panel2";
            panel2.Size = new Size(1181, 41);
            panel2.TabIndex = 127;
            // 
            // isShowTrackInfo
            // 
            isShowTrackInfo.AutoSize = true;
            isShowTrackInfo.Location = new Point(299, 11);
            isShowTrackInfo.Margin = new Padding(2, 3, 2, 3);
            isShowTrackInfo.Name = "isShowTrackInfo";
            isShowTrackInfo.Size = new Size(111, 21);
            isShowTrackInfo.TabIndex = 129;
            isShowTrackInfo.Text = "不显示跟踪信息";
            isShowTrackInfo.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(633, 4);
            button2.Margin = new Padding(3, 4, 3, 4);
            button2.Name = "button2";
            button2.Size = new Size(87, 33);
            button2.TabIndex = 128;
            button2.Text = "清空";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // btnStopTrackVeh
            // 
            btnStopTrackVeh.Enabled = false;
            btnStopTrackVeh.Location = new Point(539, 4);
            btnStopTrackVeh.Margin = new Padding(3, 4, 3, 4);
            btnStopTrackVeh.Name = "btnStopTrackVeh";
            btnStopTrackVeh.Size = new Size(87, 33);
            btnStopTrackVeh.TabIndex = 3;
            btnStopTrackVeh.Text = "停止跟踪";
            btnStopTrackVeh.UseVisualStyleBackColor = true;
            btnStopTrackVeh.Click += btnStopTrackVeh_Click;
            // 
            // btnStartTrackVeh
            // 
            btnStartTrackVeh.Location = new Point(445, 4);
            btnStartTrackVeh.Margin = new Padding(3, 4, 3, 4);
            btnStartTrackVeh.Name = "btnStartTrackVeh";
            btnStartTrackVeh.Size = new Size(87, 33);
            btnStartTrackVeh.TabIndex = 2;
            btnStartTrackVeh.Text = "开始跟踪";
            btnStartTrackVeh.UseVisualStyleBackColor = true;
            btnStartTrackVeh.Click += btnStartTrackVeh_Click;
            // 
            // txtTrackVeh
            // 
            txtTrackVeh.Location = new Point(55, 6);
            txtTrackVeh.Margin = new Padding(3, 4, 3, 4);
            txtTrackVeh.Name = "txtTrackVeh";
            txtTrackVeh.Size = new Size(239, 23);
            txtTrackVeh.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 13);
            label3.Name = "label3";
            label3.Size = new Size(35, 17);
            label3.TabIndex = 0;
            label3.Text = "车牌:";
            // 
            // tmUpdateStatus
            // 
            tmUpdateStatus.Enabled = true;
            tmUpdateStatus.Interval = 2000;
            tmUpdateStatus.Tick += tmUpdateStatus_Tick;
            // 
            // lvw_online
            // 
            lvw_online.Columns.AddRange(new ColumnHeader[] { columnHeader1_SN, columnHeader_PlateNo, columnHeader_Sim, columnHeader_IP, columnHeader_StartTime, columnHeader_Online, columnHeader_Position, columnHeader_Error, columnHeader_Offline, columnHeader_queue, columnHeader_status, columnHeader_trans });
            lvw_online.Location = new Point(18, 58);
            lvw_online.Name = "lvw_online";
            lvw_online.Size = new Size(1180, 232);
            lvw_online.TabIndex = 2;
            lvw_online.UseCompatibleStateImageBehavior = false;
            lvw_online.View = View.Details;
            // 
            // columnHeader1_SN
            // 
            columnHeader1_SN.Text = "序号";
            columnHeader1_SN.Width = 80;
            // 
            // columnHeader_PlateNo
            // 
            columnHeader_PlateNo.Text = "车牌号";
            columnHeader_PlateNo.Width = 100;
            // 
            // columnHeader_Sim
            // 
            columnHeader_Sim.Text = "SimNo";
            columnHeader_Sim.Width = 120;
            // 
            // columnHeader_IP
            // 
            columnHeader_IP.Text = "终端IP";
            columnHeader_IP.Width = 80;
            // 
            // columnHeader_StartTime
            // 
            columnHeader_StartTime.Text = "上线时间";
            columnHeader_StartTime.Width = 120;
            // 
            // columnHeader_Online
            // 
            columnHeader_Online.Text = "在线时间";
            columnHeader_Online.Width = 120;
            // 
            // columnHeader_Position
            // 
            columnHeader_Position.Text = "定位时间";
            columnHeader_Position.Width = 120;
            // 
            // columnHeader_Error
            // 
            columnHeader_Error.Text = "错误包数";
            // 
            // columnHeader_Offline
            // 
            columnHeader_Offline.Text = "断线次数";
            columnHeader_Offline.Width = 80;
            // 
            // columnHeader_queue
            // 
            columnHeader_queue.Text = "队列";
            // 
            // columnHeader_status
            // 
            columnHeader_status.Text = "状态";
            // 
            // columnHeader_trans
            // 
            columnHeader_trans.Text = "转发时间";
            columnHeader_trans.Width = 120;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(106, 17);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(142, 23);
            textBox1.TabIndex = 1;
            // 
            // lbl_dev
            // 
            lbl_dev.AutoSize = true;
            lbl_dev.Location = new Point(18, 17);
            lbl_dev.Name = "lbl_dev";
            lbl_dev.Size = new Size(68, 17);
            lbl_dev.TabIndex = 0;
            lbl_dev.Text = "当前终端：";
            // 
            // splitContainer1
            // 
            splitContainer1.Location = new Point(12, 12);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(lvw_online);
            splitContainer1.Panel1.Controls.Add(textBox1);
            splitContainer1.Panel1.Controls.Add(lbl_dev);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Size = new Size(1210, 605);
            splitContainer1.SplitterDistance = 302;
            splitContainer1.TabIndex = 5;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1245, 642);
            Controls.Add(splitContainer1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "808GPS服务端 ";
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            Resize += MainForm_Resize;
            tabControl1.ResumeLayout(false);
            tabPage_Log.ResumeLayout(false);
            tabPage_Log.PerformLayout();
            contextMenuStrip_log_sys.ResumeLayout(false);
            statusStrip_log_sys.ResumeLayout(false);
            statusStrip_log_sys.PerformLayout();
            tabPage_State.ResumeLayout(false);
            contextMenuStrip_LinkState.ResumeLayout(false);
            tabPage_Link.ResumeLayout(false);
            contextMenuStrip_lvwOperatorLineMsg.ResumeLayout(false);
            tabPage_GenZong.ResumeLayout(false);
            contextMenuStrip_lvwOperatorTrackMsg.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private ContextMenuStrip contextMenuStrip_lvwOperatorLineMsg;
        private ToolStripMenuItem 清空ToolStripMenuItem2;
        private ToolStripMenuItem 复制ToolStripMenuItem;
        private ToolStripMenuItem 触发主链路异常ToolStripMenuItem;
        private ToolStripMenuItem 取消主链路异常ToolStripMenuItem;
        private ColumnHeader columnHeader_StateVersion;
        private ColumnHeader columnHeader_StateSalveConnectionAddress;
        private ColumnHeader columnHeader_YunYingName;
        private ToolStripMenuItem 记录传输GPS数据车辆列表ToolStripMenuItem;
        private ColumnHeader columnHeader_SimNo;
        private Label lbl_dev;
        private TextBox textBox1;
        private ListView listView1;
        private ColumnHeader columnHeader1_SN;
        private ColumnHeader columnHeader_PlateNo;
        private ColumnHeader columnHeader_Sim;
        private ColumnHeader columnHeader_IP;
        private ColumnHeader columnHeader_StartTime;
        private ColumnHeader columnHeader_Online;
        private ColumnHeader columnHeader_Position;
        private ColumnHeader columnHeader_Error;
        private ColumnHeader columnHeader_Offline;
        private ColumnHeader columnHeader_queue;
        private ColumnHeader columnHeader_status;
        private ColumnHeader columnHeader_trans;
        private ListView lvw_online;
        private SplitContainer splitContainer1;
    }
}