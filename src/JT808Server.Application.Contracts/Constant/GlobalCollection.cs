using System.Collections.Concurrent;

namespace JT808Server.Application.Contracts.Constant
{
    public class GlobalCollection
    {
        /// <summary>
        /// 退出取消线程标志
        /// </summary>
        public static CancellationTokenSource ExitSource { get; set; } = new CancellationTokenSource();

        /// <summary>
        /// 停止接收数据
        /// </summary>
        public static bool RecvBufferStop { get; set; } = false;

        /// <summary>
        /// 省直辖市简称集合
        /// </summary>
        public static List<string> ConstantList { get; set; } = new List<string>();

        
        /// <summary>
        /// 安标车辆牌照+安标车辆颜色为Key,安标车辆ID为Value的集合
        /// </summary>
        public static ConcurrentDictionary<string, string> CphForIDAnBiao { get; set; } = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 安标车辆牌照+安标车辆颜色为Key,安标车辆IP为Value的集合
        /// </summary>
        public static ConcurrentDictionary<string, string> CphForIp { get; set; } = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 安标车辆IP为Key,安标车辆牌照+安标车辆颜色为Value的集合
        /// </summary>
        public static ConcurrentDictionary<string, string> IpForCph { get; set; } = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 实时音视频请求消息(JT809_JT1078_0x9800_0x9801)发送此消息的车辆数据集合
        /// </summary>
        public static ConcurrentDictionary<string, string> NDictionary { get; set; } = new ConcurrentDictionary<string, string>();

        static GlobalCollection()
        {
            AddPro();//加载省直辖市简称
        }

        /// <summary>
        /// //加载省直辖市简称
        /// </summary>
        private static void AddPro()
        {
            ConstantList.Add("京");
            ConstantList.Add("津");
            ConstantList.Add("晋");
            ConstantList.Add("蒙");
            ConstantList.Add("辽");
            ConstantList.Add("吉");
            ConstantList.Add("黑");
            ConstantList.Add("沪");
            ConstantList.Add("苏");
            ConstantList.Add("浙");
            ConstantList.Add("皖");
            ConstantList.Add("闽");
            ConstantList.Add("赣");
            ConstantList.Add("鲁");
            ConstantList.Add("豫");
            ConstantList.Add("鄂");
            ConstantList.Add("湘");
            ConstantList.Add("粤");
            ConstantList.Add("桂");
            ConstantList.Add("琼");
            ConstantList.Add("川");
            ConstantList.Add("黔");
            ConstantList.Add("滇");
            ConstantList.Add("渝");
            ConstantList.Add("藏");
            ConstantList.Add("陕");
            ConstantList.Add("甘");
            ConstantList.Add("青");
            ConstantList.Add("宁");
            ConstantList.Add("新");
            ConstantList.Add("冀");
        }

        /// <summary>
        /// 下发车辆报文请求数据序号
        /// </summary>
        public static uint MsgSequence { get; set; } = 0;
    }
}
