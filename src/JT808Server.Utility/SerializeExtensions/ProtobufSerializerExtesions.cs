
namespace JT808Server.Utility.SerializeExtensions
{
    public static class ProtobufSerializerExtesions
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T data)
        {
            using MemoryStream memoryStream = new();
            ProtoBuf.Serializer.Serialize(memoryStream, data);
            return memoryStream.ToArray();
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(byte[] data)
        {
            try
            {
                using MemoryStream ms = new(data);
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
