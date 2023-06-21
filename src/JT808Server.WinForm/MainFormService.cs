using Confluent.Kafka;
using JT808.Gateway.Abstractions;
using JT808.Gateway.Abstractions.Configurations;
using JT808.Gateway.Session;
using JT808Server.Application.Contracts.Constant;
using JT808Server.Application.Services;
using JT808Server.Kafka;
using JT808Server.Utility.Common;
using JT808Server.Utility.Options;
using JT808Server.Utility.SerializeExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Kafka;

namespace JT808Server.WinForm
{
    public partial class MainFormService : ISingletonDependency
    {
        private RedisHelper redisHelperforAlarm_ADAS_DSM_Partition = new RedisHelper(null, 0, "alarm_adas_dsm_partition");
        private RedisHelper redisHelperforGPS_Partition = new RedisHelper(null, 0, "gps_partition");
        /// <summary>
        ///刷新安标车辆数据默认时间
        /// </summary>
        private DateTime _prvRefreshVehicletime { get; set; } = DateTime.MinValue;
        /// <summary>
        /// 消费间隔时间
        /// </summary>
        private int batchInsertInterval { get; set; } = 10;
        /// <summary>
        /// 刷新数据时间
        /// </summary>
        public int RefreshTime { get; set; } = 1000 * 60 * 5;
        /// <summary>
        /// 计时器集合
        /// </summary>
        private System.Threading.Timer[] timerArr { get; set; } = new System.Threading.Timer[4];

        private readonly ILogger<MainFormService> _logger;
        private readonly SystemConfiguration _systemConfiguration;
        private readonly JT808SessionManager _jT808SessionManager;
        private readonly IProducerPool _producerPool_ByteValue;
        private readonly KafkaProducerConfig _kafkaProducerConfig;
        private readonly KafkaConsumerConfig _kafkaConsumerConfig;
        private readonly IKafkaMessageConsumerFactory _kafkaMessageConsumerFactory_ByteValue;
        private readonly JT808Configuration _jT808Configuration;
        public readonly JT808MsgReplyDataService _JT808MsgReplyDataService;
        public readonly JT808SessionService _JT808SessionService;

        public MainFormService(ILogger<MainFormService> logger,
            IOptions<SystemConfiguration> _systemConfigurationAccessor,
            IProducerPool producerPool_ByteValue,
            JT808SessionManager jT808SessionManager,
            IOptions<KafkaProducerConfig> kafkaProducerConfigOptions,
            IOptions<KafkaConsumerConfig> kafkaConsumerConfigOptions,
            IKafkaMessageConsumerFactory kafkaMessageConsumerFactory,
            IOptions<JT808Configuration> jT808ConfigurationOptions,
            JT808SessionService JT808SessionService,
            JT808MsgReplyDataService JT808MsgReplyDataService)
        {
            _logger = logger;
            _systemConfiguration = _systemConfigurationAccessor.Value;
            _jT808SessionManager = jT808SessionManager;
            _producerPool_ByteValue = producerPool_ByteValue;
            _kafkaProducerConfig = kafkaProducerConfigOptions.Value;
            _kafkaConsumerConfig = kafkaConsumerConfigOptions.Value;
            _kafkaMessageConsumerFactory_ByteValue = kafkaMessageConsumerFactory;
            _jT808Configuration = jT808ConfigurationOptions.Value;
            _JT808SessionService = JT808SessionService;
            _JT808MsgReplyDataService = JT808MsgReplyDataService;
        }
        /// <summary>
        /// 首次读取可以连接服务的运营商资料
        /// </summary>
        /// <returns></returns>
        public bool UpdateOperatorInfo(bool bFirst = true)
        {
            //var msgStr = bFirst ? "读取" : "刷新";
            //try
            //{
            //    var platformDtoList = _platformService.GetAll();
            //    if (platformDtoList != null && platformDtoList.Count > 0)
            //    {
            //        foreach (var da in platformDtoList)
            //        {
            //            GlobalCollection.OperatorInfoHash.AddOrUpdate(da.OperatorCode.ToString(string.Empty), da, (key, oldVal) => oldVal = da);
            //            GlobalCollection.PlatformEntityDic.AddOrUpdate(da.ID, da, (key, oldVal) => oldVal = da);
            //        }
            //        WinFromDisplay.ShowSysMsg($"可连接服务的运营商资料UpdateOperatorInfo{msgStr}成功! OperatorInfoHash数量{GlobalCollection.OperatorInfoHash.Count},PlatformEntityDic数量{GlobalCollection.PlatformEntityDic.Count}");
            //        return true;
            //    }
            //    WinFromDisplay.ShowSysMsg($"可连接服务的运营商资料UpdateOperatorInfo无数据!");
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    WinFromDisplay.ShowSysMsg($"可连接服务的运营商资料UpdateOperatorInfo{msgStr}异常#" + ex.Message + ex.StackTrace);
            //    _logger.LogError(ex, ex.Message);
                return false;
            //}
        }
        /// <summary>
        /// 更新平台运营商资料信息数据
        /// </summary>
        public void UpdatePlatformData()
        {
            timerArr[0] = new System.Threading.Timer((obj) => { UpdateOperatorInfo(bFirst: false); }, null, RefreshTime, RefreshTime);
        }
        /// <summary>
        /// 更新安标车辆数据信息
        /// </summary>
        public void UpdateVehInfo()
        {
            //timerArr[1] = new System.Threading.Timer(async (obj) =>
            //{
            //    try
            //    {
            //        DateTime querytime = _prvRefreshVehicletime;
            //        _prvRefreshVehicletime = DateTime.Now;
            //        var result = await _vehicleAnBiaoService.LoadVehiclesAsync(querytime);
            //        if (result.Item1)
            //        {
            //            foreach (VehicleAnBiaoDto vehicleAnBiaoDto in result.Item2)
            //            {
            //                int iColor_new = Tool.ColorToInt(vehicleAnBiaoDto.chepaiyanse);
            //                string iColorstr_new = vehicleAnBiaoDto.chepaiyanse;
            //                string Cph_new = vehicleAnBiaoDto.cheliangpaizhao;
            //                string id_new = vehicleAnBiaoDto.id;
            //                string Ipaddress_new = vehicleAnBiaoDto.zongduanid;
            //                if (GlobalCollection.IDForVehicleAnBiao.ContainsKey(id_new))
            //                {
            //                    VehicleAnBiaoDto vehicle = GlobalCollection.IDForVehicleAnBiao[id_new];
            //                    int iColor_old = Tool.ColorToInt(vehicle.chepaiyanse);
            //                    string iColorstr_old = vehicle.chepaiyanse;
            //                    string Cph_old = vehicle.cheliangpaizhao;
            //                    string id_old = vehicle.id;
            //                    string Ipaddress_old = vehicle.zongduanid;
            //                    string delValue;
            //                    if (vehicleAnBiaoDto.cheliangzhuangtai == "删除")
            //                    {
            //                        GlobalCollection.CphForIDAnBiao.TryRemove(Cph_old + iColor_old, out delValue);
            //                        GlobalCollection.CphForIp.TryRemove(Cph_old + iColor_old, out delValue);
            //                        GlobalCollection.IpForCph.TryRemove(Ipaddress_old, out delValue);
            //                        GlobalCollection.IDForVehicleAnBiao.TryRemove(id_old, out VehicleAnBiaoDto delVehicleAnBiaoDto);
            //                    }
            //                    else
            //                    {
            //                        if (vehicle.cheliangpaizhao != vehicleAnBiaoDto.cheliangpaizhao ||
            //                            vehicle.chepaiyanse != vehicleAnBiaoDto.chepaiyanse ||
            //                            vehicle.zongduanid != vehicleAnBiaoDto.zongduanid)
            //                        {
            //                            //移除旧数据,添加或更新新数据
            //                            GlobalCollection.CphForIDAnBiao.TryRemove(Cph_old + iColor_old, out delValue);
            //                            GlobalCollection.CphForIDAnBiao.AddOrUpdate(Cph_new + iColor_new, id_new, (key, oldVal) => oldVal = id_new);
            //                            //移除旧数据,添加或更新新数据
            //                            GlobalCollection.CphForIp.TryRemove(Cph_old + iColor_old, out delValue);
            //                            GlobalCollection.CphForIp.AddOrUpdate(Cph_new + iColor_new, Ipaddress_new, (key, oldVal) => oldVal = Ipaddress_new);
            //                            //移除旧数据,添加或更新新数据
            //                            GlobalCollection.IpForCph.TryRemove(Ipaddress_old, out delValue);
            //                            GlobalCollection.IpForCph.AddOrUpdate(Ipaddress_new, Cph_new + "," + iColor_new, (key, oldVal) => oldVal = Cph_new + "," + iColor_new);
            //                        }
            //                        vehicle.simnum = vehicleAnBiaoDto.simnum;
            //                        vehicle.zongduanid = vehicleAnBiaoDto.zongduanid;
            //                        vehicle.zongduanxinghao = vehicleAnBiaoDto.zongduanxinghao;
            //                        vehicle.createtime = vehicleAnBiaoDto.createtime;
            //                        vehicle.chepaiyanse = vehicleAnBiaoDto.chepaiyanse;
            //                        vehicle.cheliangzhuangtai = vehicleAnBiaoDto.cheliangzhuangtai;
            //                        vehicle.cheliangpaizhao = vehicleAnBiaoDto.cheliangpaizhao;
            //                        vehicle.caozuoshijian = vehicleAnBiaoDto.caozuoshijian;
            //                    }
            //                }
            //                else
            //                {
            //                    GlobalCollection.CphForIDAnBiao.AddOrUpdate(Cph_new + iColor_new, id_new, (key, oldVal) => oldVal = id_new);
            //                    GlobalCollection.CphForIp.AddOrUpdate(Cph_new + iColor_new, Ipaddress_new, (key, oldVal) => oldVal = Ipaddress_new);
            //                    GlobalCollection.IpForCph.AddOrUpdate(Ipaddress_new, Cph_new + "," + iColor_new, (key, oldVal) => oldVal = Cph_new + "," + iColor_new);
            //                    GlobalCollection.IDForVehicleAnBiao.TryAdd(id_new, vehicleAnBiaoDto);
            //                }
            //            }
            //            WinFromDisplay.ShowSysMsg($"安标车辆数据UpdateVehInfo刷新成功! CphForIDAnBiao数量{GlobalCollection.CphForIDAnBiao.Count},CphForIp数量{GlobalCollection.CphForIp.Count},IpForCph数量{GlobalCollection.IpForCph.Count},IDForVehicleAnBiao数量{GlobalCollection.IDForVehicleAnBiao.Count}");
            //        }
            //        else
            //        {
            //            WinFromDisplay.ShowSysMsg("安标车辆数据UpdateVehInfo刷新失败!");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        WinFromDisplay.ShowSysMsg("安标车辆数据UpdateVehInfo刷新异常#" + ex.Message + ex.StackTrace);
            //        _logger.LogError(ex, ex.Message);
            //    }
            //}, null, 0, RefreshTime);
        }
        /// <summary>
        /// 更新车辆公共信息数据
        /// </summary>
        public void UpdateVehicleData()
        {
            //timerArr[2] = new System.Threading.Timer((obj) =>
            //{
            //    try
            //    {
            //        var vehicleDtoList = _vehicleService.GetAll();
            //        if (vehicleDtoList != null && vehicleDtoList.Count > 0)
            //        {
            //            foreach (var vehicle in vehicleDtoList)
            //            {
            //                if (GlobalCollection.VehiclePublicDic.ContainsKey(vehicle.ID))
            //                {
            //                    GlobalCollection.VehiclePublicDic[vehicle.ID] = vehicle;
            //                }
            //                else
            //                {
            //                    VehicleDto publicVehicle = GlobalCollection.VehiclePublicDic.FirstOrDefault(c => c.Value.PlateNo == vehicle.PlateNo && c.Value.PlateColor == vehicle.PlateColor).Value;
            //                    if (publicVehicle != null)
            //                    {
            //                        //车牌颜色一样但Id不一样 则剔除原有字典数据 Id不一样说明系统车牌和颜色一样的车辆有一个被删除
            //                        if (publicVehicle.ID != vehicle.ID)
            //                        {
            //                            GlobalCollection.VehiclePublicDic.TryRemove(publicVehicle.ID, out _);
            //                            GlobalCollection.VehiclePublicDic.TryAdd(vehicle.ID, vehicle);
            //                        }
            //                        else
            //                        {
            //                            //车牌 颜色一样 Id一样 则更改原有数据
            //                            GlobalCollection.VehiclePublicDic.TryUpdate(vehicle.ID, vehicle, publicVehicle);
            //                        }
            //                    }
            //                    else
            //                    {
            //                        GlobalCollection.VehiclePublicDic.TryAdd(vehicle.ID, vehicle);
            //                    }
            //                }
            //            }
            //            WinFromDisplay.ShowSysMsg($"车辆信息数据UpdateVehicleData刷新成功! GlobalCollection.VehiclePublicDic数量{GlobalCollection.VehiclePublicDic.Count}");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        WinFromDisplay.ShowSysMsg("车辆信息数据UpdateVehicleData刷新异常#" + ex.Message + ex.StackTrace);
            //        _logger.LogError(ex, ex.Message);
            //    }
            //}, null, 0, RefreshTime);
        }
        /// <summary>
        /// 更新运营商传输数据记录
        /// </summary>
        public void UpdateOPERATORCONNSTATELOG()
        {
            //var RefreshTimeTemp = _systemConfiguration.ConnStateRefreshTime * 1000;
            //timerArr[3] = new System.Threading.Timer((obj) =>
            //{
            //    try
            //    {
            //        List<JT808Session> jT809Session = _jT809SessionManager.GetAll();
            //        for (int i = 0; i < jT809Session.Count; i++)
            //        {
            //            JT809Session tcpPro = jT809Session[i];
            //            GlobalCollection.OperatorInfoHash.TryGetValue(tcpPro.MsgGNSSCENTERID.ToString(), out var oper);
            //            _commonService.SaveClientConnState(tcpPro.MsgGNSSCENTERID.ToString(), tcpPro.MainIPAddress, 3, tcpPro.StartTime, true, tcpPro.HaveVeh.Count, tcpPro.MainRecvAll, DateTime.Now);
            //        }
            //        WinFromDisplay.ShowSysMsg("更新运营商传输数据记录成功");
            //    }
            //    catch (Exception ex)
            //    {
            //        WinFromDisplay.ShowSysMsg("更新运营商传输数据记录异常#" + ex.Message + ex.StackTrace);
            //        _logger.LogError(ex, ex.Message);
            //    }
            //}, null, 1000 * 120, RefreshTimeTemp);
        }
        /// <summary>
        /// 保存GPS信息到Kafka队列,其他服务消费写入Oracle数据库
        /// </summary>
        public void SaveGPSToKafka()
        {
            //int pos = 0;
            //List<string> listPartition = new List<string>();
            //for (int i = 0; i < _systemConfiguration.GPSPartitionCount; i++)
            //{
            //    listPartition.Add(i.ToString());
            //}
            //DateTime logTime = DateTime.Now;
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            //检测到关闭应用程序
            //            if (GlobalCollection.RecvBufferStop)
            //            {
            //                if (GlobalCollection.ExitSource.Token.CanBeCanceled)
            //                {
            //                    GlobalCollection.ExitSource.Cancel();
            //                }
            //                break;
            //            }
            //            while (GlobalCollection.GPSInfoQueue.TryDequeue(out GPSInfoDto? gps))
            //            {
            //                if (gps != null)
            //                {
            //                    try
            //                    {
            //                        VehicleGpsDto vehicleGpsDto = new VehicleGpsDto();
            //                        vehicleGpsDto.GpsId = gps.GPSID;
            //                        vehicleGpsDto.PlateNo = gps.plateNo;
            //                        vehicleGpsDto.VehicleId = gps.vehicleID;
            //                        vehicleGpsDto.SendTime = gps.SendTime;
            //                        vehicleGpsDto.Longitude = gps.Longitude;
            //                        vehicleGpsDto.Latitude = gps.Latitude;
            //                        vehicleGpsDto.Velocity = gps.Velocity;
            //                        vehicleGpsDto.Direction = gps.Direction.ToUShort();
            //                        vehicleGpsDto.Status = gps.Status;
            //                        vehicleGpsDto.Mileage = gps.Mileage;
            //                        vehicleGpsDto.RecordVelocity = gps.RecordVelocity;
            //                        vehicleGpsDto.Location = gps.Location;
            //                        vehicleGpsDto.CreateDate = gps.CreateDate;
            //                        vehicleGpsDto.AlarmState = gps.AlarmState.ToUInt();
            //                        vehicleGpsDto.Altitude = gps.Altitude;
            //                        vehicleGpsDto.Valid = gps.Valid ? true : false;
            //                        vehicleGpsDto.Signal = gps.Signal.ToUInt();
            //                        vehicleGpsDto.DataState = gps.DataState;
            //                        vehicleGpsDto.Platecolor = gps.plateColor;
            //                        vehicleGpsDto.IsSuppleTrans = gps.isSuppleTrans;

            //                        #region GPS信息处理入Kafka给报警程序使用 有顺序要求同步生产
            //                        try
            //                        {
            //                            StringBuilder mqmsg = new StringBuilder();
            //                            mqmsg.Append($"{vehicleGpsDto.PlateNo};");//0
            //                            mqmsg.Append($"{vehicleGpsDto.VehicleId};");//1
            //                            mqmsg.Append($"{vehicleGpsDto.SendTime};");//2
            //                            mqmsg.Append($"{vehicleGpsDto.Longitude};");//3
            //                            mqmsg.Append($"{vehicleGpsDto.Latitude};");//4
            //                            mqmsg.Append($"{vehicleGpsDto.Velocity};");//5
            //                            mqmsg.Append($"{vehicleGpsDto.Direction};");//6
            //                            mqmsg.Append($"{vehicleGpsDto.Status};");//7
            //                            mqmsg.Append($"{vehicleGpsDto.RecordVelocity};");//8
            //                            mqmsg.Append($"{vehicleGpsDto.AlarmState};");//9
            //                            mqmsg.Append($"{vehicleGpsDto.Altitude};");//10
            //                            mqmsg.Append($"{vehicleGpsDto.Valid};");//11
            //                            mqmsg.Append($"0;");//12
            //                            mqmsg.Append($"{DateTime.Now};");//13
            //                            mqmsg.Append($"{vehicleGpsDto.GpsId};");//14
            //                            mqmsg.Append($"{vehicleGpsDto.VehicleId};");//15
            //                            mqmsg.Append($"{vehicleGpsDto.Mileage.ToString("f1")};");//16
            //                            mqmsg.Append($"0;");//17
            //                            if (vehicleGpsDto.IsSuppleTrans == true)//补传信号
            //                            {
            //                                mqmsg.Append($"True;");//18
            //                                mqmsg.Append($"True");//19
            //                            }
            //                            else
            //                            {
            //                                mqmsg.Append($"False;");
            //                                mqmsg.Append($"False");
            //                            }
            //                            string index = redisHelperforGPS_Partition.StringGet($"Partition_{vehicleGpsDto.VehicleId}");
            //                            if (string.IsNullOrEmpty(index))
            //                            {
            //                                //重置索引
            //                                if (pos >= listPartition.Count)
            //                                {
            //                                    pos = 0;
            //                                }
            //                                index = listPartition[pos];
            //                                pos++;
            //                            }
            //                            _producerPool_ByteValue.Get().Produce(new TopicPartition(_kafkaProducerConfig._GPSTopic, new Partition(index.ToInt())), new Message<string, byte[]>
            //                            {
            //                                Key = vehicleGpsDto.VehicleId,
            //                                Value = Encoding.UTF8.GetBytes(mqmsg.ToString())
            //                            });
            //                            redisHelperforGPS_Partition.StringSet($"Partition_{vehicleGpsDto.VehicleId}", index, TimeSpan.FromDays(2));
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            _logger.LogError(ex, ex.Message);
            //                        }
            //                        #endregion

            //                        _ = _producerPool_ByteValue.Get().ProduceAsync(_kafkaProducerConfig._JT808_0x200Topic, new Message<string, byte[]>
            //                        {
            //                            Key = Guid.NewGuid().ToString(),
            //                            Value = ProtobufSerializerExtesions.Serialize(vehicleGpsDto)
            //                        });
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        _logger.LogError(ex, ex.Message);
            //                        //重新入队列
            //                        GlobalCollection.GPSInfoQueue.Enqueue(gps);
            //                    }
            //                }
            //                else
            //                {
            //                    break;
            //                }
            //            }
            //            //1分钟记录一下日志 证明循环在执行
            //            if ((DateTime.Now - logTime).TotalMinutes >= 1)
            //            {
            //                _logger.LogDebug($"保存GPS信息到Kafka队列循环在执行,GlobalCollection.GPSInfoQueue数量{GlobalCollection.GPSInfoQueue.Count}");
            //                logTime = DateTime.Now;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, ex.Message);
            //        }
            //        finally
            //        {
            //            await Task.Delay(batchInsertInterval);
            //        }
            //    }
            //});
        }
        /// <summary>
        /// 保存终端报警信息到Kafka队列,其他服务消费写入Oracle数据库
        /// </summary>
        public void ProcessNewAlarmToKafka()
        {
            //DateTime logTime = DateTime.Now;
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            //检测到关闭应用程序
            //            if (GlobalCollection.RecvBufferStop)
            //            {
            //                if (GlobalCollection.ExitSource.Token.CanBeCanceled)
            //                {
            //                    GlobalCollection.ExitSource.Cancel();
            //                }
            //                break;
            //            }
            //            while (GlobalCollection.GPSInfoQueueForNewAlarm.TryDequeue(out var rd))
            //            {
            //                if (rd != null)
            //                {
            //                    try
            //                    {
            //                        var BarStr = Convert.ToString((int)rd.AlarmState, 2).PadLeft(32, '0');
            //                        if (BarStr.Contains("1"))
            //                        {
            //                            char[] newChars = BarStr.ToArray().Reverse().ToArray();
            //                            var alarmId = ZKGTGPSServerHelper.GetAlarmType(newChars);
            //                            if (alarmId != 0)
            //                            {
            //                                NewAlarmDto newAlarmDto = new NewAlarmDto();
            //                                newAlarmDto.Id = Guid.NewGuid().ToString();
            //                                newAlarmDto.PlateNo = rd.plateNo;
            //                                newAlarmDto.VehicleID = rd.vehicleID;
            //                                newAlarmDto.AlarmType = alarmId;
            //                                newAlarmDto.AlarmSource = 2;
            //                                newAlarmDto.CreateDate = DateTime.Now;
            //                                newAlarmDto.AlarmTime = rd.SendTime;
            //                                newAlarmDto.Latitude = rd.Latitude;
            //                                newAlarmDto.Longitude = rd.Longitude;
            //                                newAlarmDto.Speed = rd.Velocity;
            //                                newAlarmDto.ProcessedTime = DateTime.Now;
            //                                newAlarmDto.PlateColor = rd.plateColor;
            //                                //补充字段
            //                                newAlarmDto.Direction = rd.Direction;
            //                                newAlarmDto.Status = rd.Status;
            //                                newAlarmDto.RecordVelocity = rd.RecordVelocity;
            //                                //newAlarmDto.Location = rd.Location;
            //                                newAlarmDto.AlarmState = rd.AlarmState.ToUInt();
            //                                newAlarmDto.Altitude = rd.Altitude;
            //                                newAlarmDto.Valid = rd.Valid;
            //                                newAlarmDto.GpsId = rd.GPSID;
            //                                newAlarmDto.IsSuppleTrans = rd.isSuppleTrans;
            //                                newAlarmDto.Signal = rd.Signal;
            //                                newAlarmDto.ReceiveTime = rd.CreateDate;

            //                                #region GPS信息处理终端 疲劳驾驶预警、疲劳驾驶报警、超速报警 给报警程序使用的kafka数据 有顺序要求同步生产
            //                                try
            //                                {
            //                                    //42:疲劳驾驶预警,30:疲劳驾驶报警,29:超速报警   
            //                                    if (alarmId == 42 || alarmId == 30 || alarmId == 29)
            //                                    {
            //                                        var gPSTerminalFatigueAlarmDto = new GPSTerminalFatigueAlarmDto();
            //                                        gPSTerminalFatigueAlarmDto.ID = newAlarmDto.Id;
            //                                        gPSTerminalFatigueAlarmDto.plateNo = newAlarmDto.PlateNo;
            //                                        gPSTerminalFatigueAlarmDto.vehicleID = newAlarmDto.VehicleID;
            //                                        gPSTerminalFatigueAlarmDto.SendTime = newAlarmDto.AlarmTime;
            //                                        gPSTerminalFatigueAlarmDto.Longitude = newAlarmDto.Longitude;
            //                                        gPSTerminalFatigueAlarmDto.Latitude = newAlarmDto.Latitude;
            //                                        gPSTerminalFatigueAlarmDto.Velocity = newAlarmDto.Speed;
            //                                        gPSTerminalFatigueAlarmDto.Direction = newAlarmDto.Direction;
            //                                        gPSTerminalFatigueAlarmDto.Status = newAlarmDto.Status;
            //                                        gPSTerminalFatigueAlarmDto.RecordVelocity = newAlarmDto.RecordVelocity;
            //                                        gPSTerminalFatigueAlarmDto.Location = newAlarmDto.Location;
            //                                        gPSTerminalFatigueAlarmDto.AlarmState = newAlarmDto.AlarmState;
            //                                        gPSTerminalFatigueAlarmDto.Altitude = newAlarmDto.Altitude;
            //                                        gPSTerminalFatigueAlarmDto.Valid = newAlarmDto.Valid;
            //                                        switch (alarmId)
            //                                        {
            //                                            case 42:
            //                                                gPSTerminalFatigueAlarmDto.isFatigueAlarm = 0;//疲劳驾驶预警
            //                                                break;
            //                                            case 30:
            //                                                gPSTerminalFatigueAlarmDto.isFatigueAlarm = 1;//疲劳驾驶报警
            //                                                break;
            //                                            //case 41:
            //                                            //    gPSTerminalFatigueAlarmDto.isFatigueAlarm = 2;//超速预警
            //                                            //    break;
            //                                            case 29:
            //                                                gPSTerminalFatigueAlarmDto.isFatigueAlarm = 3;//超速报警
            //                                                break;
            //                                        }
            //                                        gPSTerminalFatigueAlarmDto.GPSID = newAlarmDto.GpsId;
            //                                        gPSTerminalFatigueAlarmDto.isSuppleTrans = newAlarmDto.IsSuppleTrans;
            //                                        gPSTerminalFatigueAlarmDto.Signal = newAlarmDto.Signal;
            //                                        gPSTerminalFatigueAlarmDto.receiveTime = newAlarmDto.ReceiveTime;

            //                                        _producerPool_ByteValue.Get().Produce(_kafkaProducerConfig._TerminalFatigueTopic, new Message<string, byte[]>
            //                                        {
            //                                            Key = Guid.NewGuid().ToString(),
            //                                            Value = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(gPSTerminalFatigueAlarmDto))
            //                                        });
            //                                    }
            //                                }
            //                                catch (Exception ex)
            //                                {
            //                                    _logger.LogError(ex, ex.Message);
            //                                }
            //                                #endregion

            //                                _ = _producerPool_ByteValue.Get().ProduceAsync(_kafkaProducerConfig._JT808NewAlarmTopic, new Message<string, byte[]>
            //                                {
            //                                    Key = Guid.NewGuid().ToString(),
            //                                    Value = ProtobufSerializerExtesions.Serialize(newAlarmDto)
            //                                });
            //                            }
            //                        }
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        _logger.LogError(ex, ex.Message);
            //                        //重新入队列
            //                        GlobalCollection.GPSInfoQueueForNewAlarm.Enqueue(rd);
            //                    }
            //                }
            //                else
            //                {
            //                    break;
            //                }
            //            }
            //            //1分钟记录一下日志 证明循环在执行
            //            if ((DateTime.Now - logTime).TotalMinutes >= 1)
            //            {
            //                _logger.LogDebug($"保存终端报警信息到Kafka队列循环在执行,GlobalCollection.GPSInfoQueueForNewAlarm数量{GlobalCollection.GPSInfoQueueForNewAlarm.Count}");
            //                logTime = DateTime.Now;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, ex.Message);
            //        }
            //        finally
            //        {
            //            await Task.Delay(batchInsertInterval);
            //        }
            //    }
            //});
        }
        /// <summary>
        /// 保存驾驶员身份信息到Kafka队列,其他服务消费写入Oracle数据库
        /// </summary>
        public void ProcessDriverICToKafka()
        {
            //DateTime logTime = DateTime.Now;
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            //检测到关闭应用程序
            //            if (GlobalCollection.RecvBufferStop)
            //            {
            //                if (GlobalCollection.ExitSource.Token.CanBeCanceled)
            //                {
            //                    GlobalCollection.ExitSource.Cancel();
            //                }
            //                break;
            //            }
            //            while (GlobalCollection.DriverICMsgQueue.TryDequeue(out var rd))
            //            {
            //                if (rd != null)
            //                {
            //                    try
            //                    {
            //                        _ = _producerPool_ByteValue.Get().ProduceAsync(_kafkaProducerConfig._KafkaDriverICTopic, new Message<string, byte[]>
            //                        {
            //                            Key = Guid.NewGuid().ToString(),
            //                            Value = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rd))
            //                        });
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        _logger.LogError(ex, ex.Message);
            //                        //重新入队列
            //                        GlobalCollection.DriverICMsgQueue.Enqueue(rd);
            //                    }
            //                }
            //                else
            //                {
            //                    break;
            //                }
            //            }
            //            //1分钟记录一下日志 证明循环在执行
            //            if ((DateTime.Now - logTime).TotalMinutes >= 1)
            //            {
            //                _logger.LogDebug($"保存驾驶员身份信息到Kafka队列循环在执行,GlobalCollection.DriverICMsgQueue数量{GlobalCollection.DriverICMsgQueue.Count}");
            //                logTime = DateTime.Now;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, ex.Message);
            //        }
            //        finally
            //        {
            //            await Task.Delay(batchInsertInterval);
            //        }
            //    }
            //});
        }
        /// <summary>
        /// 保存ADAS报警信息到Kafka队列,其他服务消费写入Oracle数据库
        /// </summary>
        public void ProcessADASAlarmToKafka()
        {
            //Random random = new Random(Environment.TickCount);
            //int iPartition = _systemConfiguration.ADAS_DSM_PartitionCount;
            //DateTime logTime = DateTime.Now;
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            //检测到关闭应用程序
            //            if (GlobalCollection.RecvBufferStop)
            //            {
            //                if (GlobalCollection.ExitSource.Token.CanBeCanceled)
            //                {
            //                    GlobalCollection.ExitSource.Cancel();
            //                }
            //                break;
            //            }
            //            while (GlobalCollection.ADASAlarmMsgQueue.TryDequeue(out var rd))
            //            {
            //                if (rd != null)
            //                {
            //                    try
            //                    {
            //                        rd.TypeStr = "809Alarm";
            //                        bool isFirst = false;
            //                        string index = redisHelperforAlarm_ADAS_DSM_Partition.StringGet($"Partition_{rd.OriAlarmId}");
            //                        if (string.IsNullOrEmpty(index))
            //                        {
            //                            isFirst = true;
            //                            index = random.Next(0, iPartition).ToString();
            //                        }
            //                        _ = _producerPool_ByteValue.Get().ProduceAsync(new TopicPartition(_kafkaProducerConfig._JT808ADASTopic, new Partition(index.ToInt())), new Message<string, byte[]>
            //                        {
            //                            Key = rd.OriAlarmId,
            //                            Value = ProtobufSerializerExtesions.Serialize(rd)
            //                        });
            //                        if (isFirst)
            //                        {
            //                            redisHelperforAlarm_ADAS_DSM_Partition.StringSet($"Partition_{rd.OriAlarmId}", index, TimeSpan.FromDays(2));
            //                        }
            //                        else
            //                        {
            //                            redisHelperforAlarm_ADAS_DSM_Partition.KeyDelete($"Partition_{rd.OriAlarmId}");
            //                        }
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        _logger.LogError(ex, ex.Message);
            //                        //重新入队列
            //                        GlobalCollection.ADASAlarmMsgQueue.Enqueue(rd);
            //                    }
            //                }
            //                else
            //                {
            //                    break;
            //                }
            //            }
            //            //1分钟记录一下日志 证明循环在执行
            //            if ((DateTime.Now - logTime).TotalMinutes >= 1)
            //            {
            //                _logger.LogDebug($"保存ADAS报警信息到Kafka队列循环在执行,GlobalCollection.ADASAlarmMsgQueue数量{GlobalCollection.ADASAlarmMsgQueue.Count}");
            //                logTime = DateTime.Now;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, ex.Message);
            //        }
            //        finally
            //        {
            //            await Task.Delay(batchInsertInterval);
            //        }
            //    }
            //});
        }
        /// <summary>
        /// 保存DSM报警信息到Kafka队列,其他服务消费写入Oracle数据库
        /// </summary>
        public void ProcessDSMAlarmToKafka()
        {
            //Random random = new Random(Environment.TickCount);
            //int iPartition = _systemConfiguration.ADAS_DSM_PartitionCount;
            //DateTime logTime = DateTime.Now;
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            //检测到关闭应用程序
            //            if (GlobalCollection.RecvBufferStop)
            //            {
            //                if (GlobalCollection.ExitSource.Token.CanBeCanceled)
            //                {
            //                    GlobalCollection.ExitSource.Cancel();
            //                }
            //                break;
            //            }
            //            while (GlobalCollection.DSMAlarmMsgQueue.TryDequeue(out var rd))
            //            {
            //                if (rd != null)
            //                {
            //                    try
            //                    {
            //                        rd.TypeStr = "809Alarm";
            //                        bool isFirst = false;
            //                        string index = redisHelperforAlarm_ADAS_DSM_Partition.StringGet($"Partition_{rd.OriAlarmId}");
            //                        if (string.IsNullOrEmpty(index))
            //                        {
            //                            isFirst = true;
            //                            index = random.Next(0, iPartition).ToString();
            //                        }
            //                        _ = _producerPool_ByteValue.Get().ProduceAsync(new TopicPartition(_kafkaProducerConfig._JT808DSMTopic, new Partition(index.ToInt())), new Message<string, byte[]>
            //                        {
            //                            Key = rd.OriAlarmId,
            //                            Value = ProtobufSerializerExtesions.Serialize(rd)
            //                        });
            //                        if (isFirst)
            //                        {
            //                            redisHelperforAlarm_ADAS_DSM_Partition.StringSet($"Partition_{rd.OriAlarmId}", index, TimeSpan.FromDays(2));
            //                        }
            //                        else
            //                        {
            //                            redisHelperforAlarm_ADAS_DSM_Partition.KeyDelete($"Partition_{rd.OriAlarmId}");
            //                        }
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        _logger.LogError(ex, ex.Message);
            //                        //重新入队列
            //                        GlobalCollection.DSMAlarmMsgQueue.Enqueue(rd);
            //                    }
            //                }
            //                else
            //                {
            //                    break;
            //                }
            //            }
            //            //1分钟记录一下日志 证明循环在执行
            //            if ((DateTime.Now - logTime).TotalMinutes >= 1)
            //            {
            //                _logger.LogDebug($"保存DSM报警信息到Kafka队列循环在执行,GlobalCollection.DSMAlarmMsgQueue数量{GlobalCollection.DSMAlarmMsgQueue.Count}");
            //                logTime = DateTime.Now;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, ex.Message);
            //        }
            //        finally
            //        {
            //            await Task.Delay(batchInsertInterval);
            //        }
            //    }
            //});
        }
        /// <summary>
        /// 运营商在线列表入kafka生产者
        /// </summary>
        public void VideoCommandCenterTransferProducerOnlineOperationListToKafka()
        {
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            List<KafkaOperationInfoDto> kafkaOperationInfoList = new List<KafkaOperationInfoDto>();
            //            List<JT809Session> jT809Session = _jT809SessionManager.GetAll();
            //            for (int i = 0; i < jT809Session.Count; i++)
            //            {
            //                JT809Session tcpPro = jT809Session[i];
            //                kafkaOperationInfoList.Add(new KafkaOperationInfoDto
            //                {
            //                    Code = tcpPro.MsgGNSSCENTERID.ToString(),
            //                    Time = tcpPro.StartTime,
            //                    AvgDelay = tcpPro.GetAvgDealy()
            //                });
            //            }
            //            if (kafkaOperationInfoList.Count > 0)
            //            {
            //                _ = _producerPool_ByteValue.Get().ProduceAsync(_kafkaProducerConfig._809Server_ToKafka_OnlineOperationListTopic, new Message<string, byte[]>
            //                {
            //                    Key = Guid.NewGuid().ToString(),
            //                    Value = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(kafkaOperationInfoList))
            //                });
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, ex.Message);
            //        }
            //        finally
            //        {
            //            await Task.Delay(6 * 1000);
            //        }
            //    }
            //});
        }
        /// <summary>
        /// 下发车辆消息Kafka消费者,生产者不在这个项目
        /// </summary>
        public void DownMsgVehicleConsumerTask()
        {
            //Task.Run(() =>
            //{
            //    //初始化Kafka消费者
            //    var kafkaMessageConsumer = _kafkaMessageConsumerFactory_ByteValue.Create(_kafkaConsumerConfig._DownMsgToVehicleTopic.Topic, _kafkaConsumerConfig._DownMsgToVehicleTopic.GroupId, KafkaConnectionNameEnum.Default.ToString());
            //    kafkaMessageConsumer.OnMessageReceived(message =>
            //    {
            //        try
            //        {
            //            //下发运营商 下发车辆报文请求 JT809_0x9500_0x9503协议
            //            DownMsgDto msgEntity = JsonConvert.DeserializeObject<DownMsgDto>(Encoding.UTF8.GetString(message.Value));
            //            if (msgEntity != null)
            //            {
            //                if (_jT809Configuration.DownMsgVehicle == 1)
            //                {
            //                    VehicleDto publicVehicle = GlobalCollection.VehiclePublicDic.FirstOrDefault(c => c.Value.PlateNo == msgEntity.plateNo && c.Value.PlateColor == msgEntity.plateColor).Value;
            //                    if (publicVehicle != null)
            //                    {
            //                        //msgEntity.SimNo = publicVehicle.SimNo.PadLeft(12, '0');
            //                        SendDataDto sendData = new SendDataDto();
            //                        sendData.SubBusinessType = JT809SubBusinessType.下发车辆报文请求.ToUInt16Value();
            //                        sendData.Data = msgEntity;
            //                        //查询平台接入码
            //                        var platformVM = (from p in GlobalCollection.PlatformEntityDic
            //                                          where p.Value.ID == publicVehicle.PlatformID
            //                                          select p.Value).FirstOrDefault();
            //                        //发送给指定的OperatorCode运营商
            //                        sendData.MsgGNSSCENTERID = platformVM != null ? platformVM.OperatorCode : null;
            //                        GlobalCollection.SendQueueCente.Enqueue(sendData);
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, ex.Message);
            //        }
            //        return Task.CompletedTask;
            //    });
            //});
        }
        /// <summary>
        /// 处理下发数据到连接服务的运营商的数据队列,从链路发送消息
        /// </summary>
        public void TranspondOperator()
        {
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            int iCount = 0;
            //            //每次处理不超过200条,超过200条下次循环处理
            //            while (GlobalCollection.SendQueueCente.Count > 0 && iCount < 200)
            //            {
            //                iCount++;
            //                SendDataDto sendData = null;
            //                if (GlobalCollection.SendQueueCente.TryDequeue(out sendData))
            //                {
            //                    if (string.IsNullOrWhiteSpace(sendData.MsgGNSSCENTERID))
            //                    {//发送给全部运营商数据
            //                        List<JT809Session> jT809Session = _jT809SessionManager.GetAll();
            //                        for (int index = 0; index < jT809Session.Count; index++)
            //                        {
            //                            var session = jT809Session[index];
            //                            SlaveLinkSendData(sendData, session);
            //                        }
            //                    }
            //                    else
            //                    {//单独发送的数据
            //                        var sessionList = _jT809SessionManager.GetSessionByMsgGNSSCENTERID(sendData.MsgGNSSCENTERID.ToUInt());
            //                        foreach (var session in sessionList)
            //                        {
            //                            SlaveLinkSendData(sendData, session);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            WinFromDisplay.ShowSysMsg("从链路发送消息TranspondOperator# " + ex.Message + ex.StackTrace);
            //            _logger.LogError(ex, ex.Message);
            //        }
            //        finally
            //        {
            //            await Task.Delay(100);
            //        }
            //    }
            //});
        }
        /// <summary>
        /// (0x9404主动安全报警附件目录请求消息)从链路消息队列出队
        /// </summary>
        public void DownFile()
        {
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            List<CmdHN9404Dto> listSendData = new List<CmdHN9404Dto>();
            //            List<CmdHN9404Dto> listRetryData = new List<CmdHN9404Dto>();
            //            int iCount = 0;
            //            //每次处理不超过50条,超过50条下次循环处理
            //            while (GlobalCollection.WarnFileQueue.Count > 0 && iCount < 50)
            //            {
            //                iCount++;
            //                CmdHN9404Dto nHN9404 = null;
            //                GlobalCollection.WarnFileQueue.TryDequeue(out nHN9404);
            //                if (nHN9404 != null)
            //                {
            //                    if (nHN9404.Date < DateTime.Now)
            //                    {
            //                        listSendData.Add(nHN9404);
            //                    }
            //                    else
            //                    {
            //                        listRetryData.Add(nHN9404);
            //                    }
            //                }
            //                else
            //                {
            //                    break;
            //                }
            //            }
            //            //需要发送
            //            if (listSendData.Count > 0)
            //            {
            //                foreach (var nHN9404 in listSendData)
            //                {
            //                    var session = _jT809SessionManager.GetSessionByChannelId(nHN9404.MainChannelId);
            //                    if (session != null)
            //                    {
            //                        SendDataDto sendData = new SendDataDto();
            //                        sendData.SubBusinessType = JT809_SuBiao_SubBusinessType.主动安全报警附件目录请求消息.ToUInt16Value();
            //                        sendData.Data = JsonConvert.SerializeObject(nHN9404);
            //                        sendData.MsgGNSSCENTERID = session.MsgGNSSCENTERID.ToString();
            //                        bool sendStatus = SlaveLinkSendData(sendData, session);
            //                        if (!sendStatus)
            //                        {
            //                            GlobalCollection.WarnFileQueue.Enqueue(nHN9404);
            //                        }
            //                    }
            //                }
            //            }
            //            //继续入队重新判断
            //            if (listRetryData.Count > 0)
            //            {
            //                foreach (var nHN9404 in listRetryData)
            //                {
            //                    GlobalCollection.WarnFileQueue.Enqueue(nHN9404);
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            WinFromDisplay.ShowSysMsg("附件请求异常DownFile# " + ex.Message + ex.StackTrace);
            //            _logger.LogError(ex, ex.Message);
            //        }
            //        finally
            //        {
            //            await Task.Delay(3000);
            //        }
            //    }
            //});
        }
        /// <summary>
        /// (0x9404主动安全报警附件目录请求消息)从链路消息重送
        /// </summary>
        public void ReSendDownFile()
        {
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            List<string> delKeyList = new List<string>();
            //            foreach (KeyValuePair<string, CmdHN9404Dto> item2 in GlobalCollection.DownCmdDic)
            //            {
            //                string key = item2.Key;
            //                CmdHN9404Dto cmd = item2.Value;
            //                double diff = (DateTime.Now - cmd.Date).TotalMinutes;
            //                if (diff >= 5.0)
            //                {
            //                    //重送次数小于配置次数
            //                    if (cmd.SendCount <= _jT809Configuration.DownCount)
            //                    {
            //                        cmd.SendCount++;
            //                        cmd.Date = DateTime.Now;
            //                        GlobalCollection.WarnFileQueue.Enqueue(cmd);
            //                    }
            //                    else
            //                    {
            //                        WinFromDisplay.ShowSysMsg("附件请求多次失败,移除对象# " + key);
            //                        delKeyList.Add(key);
            //                    }
            //                }
            //            }
            //            foreach (string item in delKeyList)
            //            {
            //                GlobalCollection.DownCmdDic.TryRemove(item, out var _);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            WinFromDisplay.ShowSysMsg("重发附件请求异常ReSendDownFile# " + ex.Message + ex.StackTrace);
            //            _logger.LogError(ex, ex.Message);
            //        }
            //        finally
            //        {
            //            await Task.Delay(5 * 1000);
            //        }
            //    }
            //});
        }
        /// <summary>
        /// 下发数据到连接服务的运营商
        /// </summary>
        /// <param name="sendData">要发送的数据</param>
        /// <param name="session">当前运营商</param>
        //private bool SlaveLinkSendData(SendDataDto sendData, JT809Session session)
        //{
        //    bool sendStatus = false;
        //    try
        //    {
        //        var _jT809Serializer = new JT809Serializer(GlobalContext.ServiceProvider.GetServices<IJT809Config>().FirstOrDefault(c => c.Version == session.JT809Version));
        //        JT809Package packageResult = null;
        //        //下发运营商 主动安全报警附件目录 JT809_0x9400_0x9404 苏标请求协议
        //        if (sendData.SubBusinessType == JT809_SuBiao_SubBusinessType.主动安全报警附件目录请求消息.ToUInt16Value())
        //        {
        //            CmdHN9404Dto nHN9404 = JsonConvert.DeserializeObject<CmdHN9404Dto>(sendData.Data.ToString());
        //            var jT809_0x9400 = new JT809_0x9400();
        //            jT809_0x9400.VehicleNo = nHN9404.VEHICLE_NO;
        //            jT809_0x9400.VehicleColor = (JT809VehicleColorType)nHN9404.VEHICLE_COLOR;
        //            jT809_0x9400.SubBusinessType = JT809_SuBiao_SubBusinessType.主动安全报警附件目录请求消息.ToUInt16Value();
        //            jT809_0x9400.SubBodies = new JT809SuBiao.SubMessageBody.JT809_0x9400_0x9404()
        //            {
        //                VehicleNo = nHN9404.VEHICLE_NO,
        //                VehicleColor = (JT809VehicleColorType)nHN9404.VEHICLE_COLOR,
        //                INFO_ID = nHN9404.INFO_ID
        //            };
        //            packageResult = JT809BusinessType.从链路报警信息交互消息.Create(jT809_0x9400);
        //        }
        //        //下发运营商 实时音视频请求消息 JT809_JT1078_0x9800_0x9801协议
        //        else if (sendData.SubBusinessType == JT809_JT1078_SubBusinessType.实时音视频请求消息.ToUInt16Value())
        //        {
        //            Video809RequestDto video809Request = sendData.Data as Video809RequestDto;
        //            var jT809_JT1078_0x9800 = new JT809_JT1078_0x9800();
        //            jT809_JT1078_0x9800.VehicleNo = video809Request.vehicleNo;
        //            jT809_JT1078_0x9800.VehicleColor = EnumHelper.GetEnumValue<JT809VehicleColorType>(video809Request.vehicleColor);
        //            jT809_JT1078_0x9800.SubBusinessType = JT809_JT1078_SubBusinessType.实时音视频请求消息.ToUInt16Value();
        //            jT809_JT1078_0x9800.SubBodies = new JT809_JT1078_0x9800_0x9801()
        //            {
        //                ChannelId = video809Request.channelId.ToByte(),
        //                AVitemType = video809Request.avitemType,
        //                AuthorizeCode = session.AUTHORIZE_CODE,
        //                GnssData = null
        //            };
        //            packageResult = JT809_JT1078_BusinessType.从链路实时音视频业务类.Create(jT809_JT1078_0x9800);
        //        }
        //        //下发运营商 下发车辆报文请求 JT809_0x9500_0x9503协议
        //        else if (sendData.SubBusinessType == JT809SubBusinessType.下发车辆报文请求.ToUInt16Value())
        //        {
        //            GlobalCollection.MsgSequence++;
        //            DownMsgDto msgEntity = sendData.Data as DownMsgDto;
        //            var jT809_0x9500 = new JT809_0x9500();
        //            jT809_0x9500.VehicleNo = msgEntity.plateNo;
        //            jT809_0x9500.VehicleColor = (JT809VehicleColorType)msgEntity.plateColor;
        //            jT809_0x9500.SubBusinessType = JT809SubBusinessType.下发车辆报文请求.ToUInt16Value();
        //            jT809_0x9500.SubBodies = new JT809_0x9500_0x9503()
        //            {
        //                MsgSequence = GlobalCollection.MsgSequence,
        //                MsgPriority = (JT809_0x9503_MsgPriority)msgEntity.priority,
        //                MsgContent = msgEntity.msg
        //            };
        //            packageResult = JT809BusinessType.从链路车辆监管消息.Create(jT809_0x9500);
        //        }
        //        //转化后的数据统一处理发送
        //        if (packageResult != null)
        //        {
        //            packageResult.Header.MsgGNSSCENTERID = session.MsgGNSSCENTERID;
        //            packageResult.Header.Version = _jT809HeaderOptions.Version;
        //            packageResult.Header.EncryptKey = _jT809HeaderOptions.EncryptKey;
        //            packageResult.Header.EncryptFlag = _jT809HeaderOptions.EncryptFlag;
        //            JT809Response jT808Response = new JT809Response(packageResult, 4096);
        //            var Channel = this.GetSendChannel(session);
        //            if (Channel != null && Channel.Open && Channel.IsWritable)
        //            {
        //                Channel.WriteAndFlushAsync(jT808Response);
        //                sendStatus = true;
        //            }
        //            else
        //            {
        //                sendStatus = false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"SlaveLinkSendData数据<<<{JsonConvert.SerializeObject(sendData)} {ex.Message}");
        //    }
        //    return sendStatus;
        //}
        /// <summary>
        /// 获取当前从链路连接,从链路存在,使用从链路发送,从链路不存在,使用主链路发送
        /// </summary>
        /// <returns></returns>
        private IJT808Session GetSendChannel(IJT808Session jT808Session)
        {
            //从链路存在,使用从链路发送
            if (jT808Session.SessionID!=null)
            {
                return jT808Session;
            }
            return null;
        }

    }
}
