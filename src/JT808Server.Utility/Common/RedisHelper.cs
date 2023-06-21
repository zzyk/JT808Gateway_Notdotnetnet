using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JT808Server.Utility.Common
{
    public class RedisHelper
    {
        private readonly string ConnectionString;

        private IConnectionMultiplexer _connMultiplexer;

        private string DefaultKey;

        private readonly object Locker = new object();

        private readonly IDatabase _db;

        public IConnectionMultiplexer GetConnectionRedisMultiplexer()
        {
            if (_connMultiplexer == null || !_connMultiplexer.IsConnected)
            {
                lock (Locker)
                {
                    if (_connMultiplexer == null || !_connMultiplexer.IsConnected)
                    {
                        _connMultiplexer = ConnectionMultiplexer.Connect(ConnectionString);
                    }
                }
            }
            return _connMultiplexer;
        }

        public ITransaction GetTransaction()
        {
            return _db.CreateTransaction();
        }

        public RedisHelper(string ConnectionString, int db = 0, string _defaultKey = null)
        {
            if (ConnectionString == null)
            {
                ConnectionString = GlobalContext.Configuration.GetSection("SystemConfiguration:RedisConfig").Get<string>();
            }
            if (ConnectionString != null)
            {
                _connMultiplexer = ConnectionMultiplexer.Connect(ConnectionString);
                DefaultKey = _defaultKey;
                _db = _connMultiplexer.GetDatabase(db);
                AddRegisterEvent();
            }
        }

        #region String
        public bool StringSet(string redisKey, string redisValue, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.StringSet(redisKey, redisValue, expiry);
        }
        public string StringGet(string redisKey, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.StringGet(redisKey);
        }
        public bool StringSet<T>(string redisKey, T redisValue, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            string json = Serialize(redisValue);
            return _db.StringSet(redisKey, json, expiry);
        }
        public T StringGet<T>(string redisKey, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            var jsonData = _db.StringGet(redisKey);
            if (!string.IsNullOrEmpty(jsonData))
            {
                return Deserialize<T>(jsonData);
            }
            return default(T);
        }
        public async Task<bool> StringSetAsync(string redisKey, string redisValue, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.StringSetAsync(redisKey, redisValue, expiry);
        }
        public async Task<string> StringGetAsync(string redisKey, string redisValue, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.StringGetAsync(redisKey);
        }
        public async Task<bool> StringSetAsync<T>(string redisKey, T redisValue, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            string json = Serialize(redisValue);
            return await _db.StringSetAsync(redisKey, json, expiry);
        }
        public async Task<T> StringGetAsync<T>(string redisKey, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            string jsonstring = await _db.StringGetAsync(redisKey);
            return Deserialize<T>(jsonstring);
        }
        #endregion

        #region Hash
        public bool HashExists(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.HashExists(redisKey, hashField);
        }
        public bool HashDelete(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.HashDelete(redisKey, hashField);
        }
        public long HashDelete(string redisKey, IEnumerable<string> hashFields)
        {
            redisKey = AddKeyPrefix(redisKey);
            IEnumerable<RedisValue> fields = hashFields.Select((Func<string, RedisValue>)((string x) => x));
            return _db.HashDelete(redisKey, fields.ToArray());
        }
        public bool HashSet(string redisKey, string hashField, string value)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.HashSet(redisKey, hashField, value);
        }
        public void HashSet(string redisKey, IEnumerable<KeyValuePair<string, string>> hashFields)
        {
            redisKey = AddKeyPrefix(redisKey);
            IEnumerable<HashEntry> entries = hashFields.Select((KeyValuePair<string, string> x) => new HashEntry(x.Key, x.Value));
            _db.HashSet(redisKey, entries.ToArray());
        }
        public string HashGet(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.HashGet(redisKey, hashField);
        }
        public IEnumerable<string> HashGet(string redisKey, IEnumerable<string> hashFields)
        {
            redisKey = AddKeyPrefix(redisKey);
            IEnumerable<RedisValue> fields = hashFields.Select((Func<string, RedisValue>)((string x) => x));
            return ConvertStrings(_db.HashGet(redisKey, fields.ToArray()));
        }
        public IEnumerable<string> HashKeys(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(_db.HashKeys(redisKey));
        }
        public IEnumerable<string> HashValues(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(_db.HashValues(redisKey));
        }
        public bool HashSet<T>(string redisKey, string hashField, T redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            string json = Serialize(redisValue);
            return _db.HashSet(redisKey, hashField, json);
        }
        public T HashGet<T>(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(_db.HashGet(redisKey, hashField));
        }
        public async Task<bool> HashExistsAsync(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.HashExistsAsync(redisKey, hashField);
        }
        public async Task<bool> HashDeleteAsync(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.HashDeleteAsync(redisKey, hashField);
        }
        public async Task<long> HashDeleteAsync(string redisKey, IEnumerable<string> hashFields)
        {
            redisKey = AddKeyPrefix(redisKey);
            IEnumerable<RedisValue> fields = hashFields.Select((Func<string, RedisValue>)((string x) => x));
            return await _db.HashDeleteAsync(redisKey, fields.ToArray());
        }
        public async Task<bool> HashSetAsync(string redisKey, string hashField, string value)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.HashSetAsync(redisKey, hashField, value);
        }
        public async Task HashSetAsync(string redisKey, IEnumerable<KeyValuePair<string, string>> hashFields)
        {
            redisKey = AddKeyPrefix(redisKey);
            IEnumerable<HashEntry> entries = hashFields.Select((KeyValuePair<string, string> x) => new HashEntry(AddKeyPrefix(x.Key), x.Value));
            await _db.HashSetAsync(redisKey, entries.ToArray());
        }
        public async Task<string> HashGetAsync(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.HashGetAsync(redisKey, hashField);
        }
        public async Task<IEnumerable<string>> HashGetAsync(string redisKey, IEnumerable<string> hashFields, string value)
        {
            redisKey = AddKeyPrefix(redisKey);
            IEnumerable<RedisValue> fields = hashFields.Select((Func<string, RedisValue>)((string x) => x));
            return ConvertStrings(await _db.HashGetAsync(redisKey, fields.ToArray()));
        }
        public async Task<IEnumerable<string>> HashKeysAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(await _db.HashKeysAsync(redisKey));
        }
        public async Task<IEnumerable<string>> HashValuesAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(await _db.HashValuesAsync(redisKey));
        }
        public async Task<bool> HashSetAsync<T>(string redisKey, string hashField, T value)
        {
            redisKey = AddKeyPrefix(redisKey);
            string json = Serialize(value);
            return await _db.HashSetAsync(redisKey, hashField, json);
        }
        public async Task<T> HashGetAsync<T>(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            string jsonstring = await _db.HashGetAsync(redisKey, hashField);
            return Deserialize<T>(jsonstring);
        }
        #endregion

        #region List
        public string ListLeftPop(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListLeftPop(redisKey);
        }
        public string ListRightPop(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListRightPop(redisKey);
        }
        public long ListRemove(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListRemove(redisKey, redisValue, 0L);
        }
        public long ListRightPush(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListRightPush(redisKey, redisValue);
        }
        public long ListLeftPush(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListLeftPush(redisKey, redisValue);
        }
        public long ListLength(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListLength(redisKey);
        }
        public IEnumerable<string> ListRange(string redisKey, long start = 0L, long stop = -1L)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(_db.ListRange(redisKey, start, stop));
        }
        public T ListLeftPop<T>(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(_db.ListLeftPop(redisKey));
        }
        public T ListRightPop<T>(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(_db.ListRightPop(redisKey));
        }
        public long ListRightPush<T>(string redisKey, T redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListRightPush(redisKey, Serialize(redisValue));
        }
        public long ListLeftPush<T>(string redisKey, T redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListLeftPush(redisKey, Serialize(redisValue));
        }
        public async Task<string> ListLeftPopAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListLeftPopAsync(redisKey);
        }
        public async Task<string> ListRightPopAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListRightPopAsync(redisKey);
        }
        public async Task<long> ListRemoveAsync(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListRemoveAsync(redisKey, redisValue, 0L);
        }
        public async Task<long> ListRightPushAsync(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListRightPushAsync(redisKey, redisValue);
        }
        public async Task<long> ListLeftPushAsync(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListLeftPushAsync(redisKey, redisValue);
        }
        public async Task<long> ListLengthAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListLengthAsync(redisKey);
        }
        public async Task<IEnumerable<string>> ListRangeAsync(string redisKey, long start = 0L, long stop = -1L)
        {
            redisKey = AddKeyPrefix(redisKey);
            return (await _db.ListRangeAsync(redisKey, start, stop)).Select((RedisValue x) => x.ToString());
        }
        public async Task<T> ListLeftPopAsync<T>(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            string jsonstring = await _db.ListLeftPopAsync(redisKey);
            return Deserialize<T>(jsonstring);
        }
        public async Task<T> ListRightPopAsync<T>(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            string jsonstring = await _db.ListRightPopAsync(redisKey);
            return Deserialize<T>(jsonstring);
        }
        public async Task<long> ListRightPushAsync<T>(string redisKey, T redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListRightPushAsync(redisKey, Serialize(redisValue));
        }
        public async Task<long> ListLeftPushAsync<T>(string redisKey, T redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListLeftPushAsync(redisKey, Serialize(redisValue));
        }
        #endregion

        #region SortedSet 有序集合
        public bool SortedSetAdd(string redisKey, string member, double score)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.SortedSetAdd(redisKey, member, score);
        }
        public IEnumerable<string> SortedSetRangeByRank(string redisKey, long start = 0L, long stop = -1L, int order = 0)
        {
            redisKey = AddKeyPrefix(redisKey);
            return from x in _db.SortedSetRangeByRank(redisKey, start, stop, (Order)order)
                   select x.ToString();
        }
        public long SortedSetLength(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.SortedSetLength(redisKey);
        }
        public bool SortedSetLength(string redisKey, string memebr)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.SortedSetRemove(redisKey, memebr);
        }
        public bool SortedSetAdd<T>(string redisKey, T member, double score)
        {
            redisKey = AddKeyPrefix(redisKey);
            string json = Serialize(member);
            return _db.SortedSetAdd(redisKey, json, score);
        }
        public double SortedSetIncrement(string redisKey, string member, double value = 1.0)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.SortedSetIncrement(redisKey, member, value);
        }
        public async Task<bool> SortedSetAddAsync(string redisKey, string member, double score)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.SortedSetAddAsync(redisKey, member, score);
        }
        public async Task<IEnumerable<string>> SortedSetRangeByRankAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(await _db.SortedSetRangeByRankAsync(redisKey, 0L, -1L));
        }
        public async Task<long> SortedSetLengthAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.SortedSetLengthAsync(redisKey);
        }
        public async Task<bool> SortedSetRemoveAsync(string redisKey, string memebr)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.SortedSetRemoveAsync(redisKey, memebr);
        }
        public async Task<bool> SortedSetAddAsync<T>(string redisKey, T member, double score)
        {
            redisKey = AddKeyPrefix(redisKey);
            string json = Serialize(member);
            return await _db.SortedSetAddAsync(redisKey, json, score);
        }
        public Task<double> SortedSetIncrementAsync(string redisKey, string member, double value = 1.0)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.SortedSetIncrementAsync(redisKey, member, value);
        }
        #endregion

        #region key操作
        public bool KeyDelete(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.KeyDelete(redisKey);
        }
        public bool KeyExists(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.KeyExists(redisKey);
        }
        public bool KeyRename(string redisKey, string redisNewKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.KeyRename(redisKey, redisNewKey);
        }
        public bool KeyExpire(string redisKey, TimeSpan? expiry)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.KeyExpire(redisKey, expiry);
        }
        public async Task<bool> KeyDeleteAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.KeyDeleteAsync(redisKey);
        }
        public async Task<bool> KeyExistsAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.KeyExistsAsync(redisKey);
        }
        public async Task<bool> KeyRenameAsync(string redisKey, string redisNewKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.KeyRenameAsync(redisKey, redisNewKey);
        }
        public async Task<bool> KeyExpireAsync(string redisKey, TimeSpan? expiry)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.KeyExpireAsync(redisKey, expiry);
        }
        #endregion

        #region 发布订阅
        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel">订阅频道</param>
        /// <param name="handler">处理过程</param>
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            });
        }
        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T">消息对象</typeparam>
        /// <param name="channel">发布频道</param>
        /// <param name="msg">消息</param>
        /// <returns>消息的数量</returns>
        public long Publish<T>(string channel, T msg)
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            return sub.Publish(channel, Serialize(msg));
        }
        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel">频道</param>
        public void Unsubscribe(string channel)
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            sub.Unsubscribe(channel);
        }
        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            sub.UnsubscribeAll();
        }
        #endregion 发布订阅

        #region 辅助方法
        private string AddKeyPrefix(string key)
        {
            return DefaultKey + ":" + key;
        }
        private IEnumerable<string> ConvertStrings<T>(IEnumerable<T> list) where T : struct
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            return list.Select((T x) => x.ToString());
        }
        private string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        private T Deserialize<T>(string jsonstring)
        {
            if (!string.IsNullOrEmpty(jsonstring))
            {
                return JsonConvert.DeserializeObject<T>(jsonstring);
            }
            return default(T);
        } 
        #endregion

        #region Redis事件
        private void AddRegisterEvent()
        {
            _connMultiplexer.ConnectionRestored += ConnMultiplexer_ConnectionRestored;
            _connMultiplexer.ConnectionFailed += ConnMultiplexer_ConnectionFailed;
            _connMultiplexer.ErrorMessage += ConnMultiplexer_ErrorMessage;
            _connMultiplexer.ConfigurationChanged += ConnMultiplexer_ConfigurationChanged;
            _connMultiplexer.HashSlotMoved += ConnMultiplexer_HashSlotMoved;
            _connMultiplexer.InternalError += ConnMultiplexer_InternalError;
            _connMultiplexer.ConfigurationChangedBroadcast += ConnMultiplexer_ConfigurationChangedBroadcast;
        }
        private void ConnMultiplexer_ConfigurationChangedBroadcast(object sender, EndPointEventArgs e)
        {
            Console.WriteLine(string.Format("{0}: {1}", "ConnMultiplexer_ConfigurationChangedBroadcast", e.EndPoint));
        }
        private void ConnMultiplexer_InternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine(string.Format("{0}: {1}", "ConnMultiplexer_InternalError", e.Exception));
        }
        private void ConnMultiplexer_HashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.WriteLine(string.Format("{0}: {1}-{2} To {3}-{4}, ", "ConnMultiplexer_HashSlotMoved", "OldEndPoint", e.OldEndPoint, "NewEndPoint", e.NewEndPoint));
        }
        private void ConnMultiplexer_ConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Console.WriteLine(string.Format("{0}: {1}", "ConnMultiplexer_ConfigurationChanged", e.EndPoint));
        }
        private void ConnMultiplexer_ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine(string.Format("{0}: {1}", "ConnMultiplexer_ErrorMessage", e.Message));
        }
        private void ConnMultiplexer_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine(string.Format("{0}: {1}", "ConnMultiplexer_ConnectionFailed", e.Exception));
        }
        private void ConnMultiplexer_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine(string.Format("{0}: {1}", "ConnMultiplexer_ConnectionRestored", e.Exception));
        }
        #endregion

    }
}
