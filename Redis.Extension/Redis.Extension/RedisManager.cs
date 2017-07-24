using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Redis.Extension
{
    public class RedisManager : IRedisManager, IDisposable
    {
        private ConnectionMultiplexer _redis;
        private readonly ConfigurationOptions _configurationOptions;

        public RedisManager() : this(ConfigurationManager.AppSettings["redis"] ?? "localhost")
        {

        }

        public RedisManager(string connectionString)
        {
            _configurationOptions = new ConfigurationOptions
            {
                EndPoints = { connectionString }
            };
        }

        public RedisManager(ConfigurationOptions configurationOptions)
        {
            _configurationOptions = configurationOptions;
        }

        public ConnectionMultiplexer ConnectionMultiplexer
        {
            get
            {
                if (_redis == null)
                {
                    lock ("RedisManager锁")
                    {
                        if (_redis != null) return _redis;
                        _redis = ConnectionMultiplexer.Connect(_configurationOptions);
                        return _redis;
                    }
                }
                return _redis;
            }
        }

        public IEnumerable<RedisKey> AllKeys(int database = 0)
        {
            return GetServer(database).Keys();
        }

        public IEnumerable<RedisKey> Search(RedisValue pattern, int database = 0)
        {
            return GetServer(database).Keys(pattern: pattern);
        }

        public long CountKeys(int database = 0)
        {
            return GetServer(database).DatabaseSize(database);
        }

        public virtual IServer GetServer(int database = 0)
        {
            return ConnectionMultiplexer.GetServer(ConnectionMultiplexer.GetEndPoints()[database]);
        }

        public bool AddOrUpdate(RedisKey key, RedisValue value, TimeSpan? expiry = null)
        {
            return ConnectionMultiplexer.GetDatabase().StringSet(key, value, expiry);
        }

        public bool AddOrUpdate(RedisKey key, object obj, TimeSpan? expiry = null)
        {
            var value = JsonConvert.SerializeObject(obj);
            return ConnectionMultiplexer.GetDatabase().StringSet(key, value, expiry);
        }

        public bool AddOrUpdate(RedisKey key, object obj, DateTime expireTime)
        {
            return AddOrUpdate(key, obj, expireTime - DateTime.Now);
        }

        public bool AddOrUpdate(RedisKey key, RedisValue value, DateTime expireTime)
        {
            return AddOrUpdate(key, value, expireTime - DateTime.Now);
        }

        public RedisValue GetOrAdd(RedisKey key, Func<RedisValue> getValueFunc = null, TimeSpan? expiry = null)
        {
            if (!ConnectionMultiplexer.GetDatabase().KeyExists(key) && getValueFunc != null)
            {
                var value = getValueFunc();
                ConnectionMultiplexer.GetDatabase().StringSet(key, value, expiry);
                return value;
            }
            return ConnectionMultiplexer.GetDatabase().StringGet(key);
        }

        public T GetOrAdd<T>(RedisKey key, Func<object> getValueFunc = null, TimeSpan? expiry = null) where T : class
        {
            if (!ConnectionMultiplexer.GetDatabase().KeyExists(key) && getValueFunc != null)
            {
                var obj = getValueFunc() as T;
                ConnectionMultiplexer.GetDatabase().StringSet(key, JsonConvert.SerializeObject(obj), expiry);
                return obj;
            }
            return JsonConvert.DeserializeObject<T>(ConnectionMultiplexer.GetDatabase().StringGet(key));
        }

        public bool Delete(RedisKey key)
        {
            return ConnectionMultiplexer.GetDatabase().KeyDelete(key);
        }

        public void DeleteByPattern(RedisValue pattern)
        {
            var keys = Search(pattern);
            foreach (var redisKey in keys)
            {
                Delete(redisKey);
            }
        }

        public bool TransExcute(Action<ITransaction> transAction, int database = 0)
        {
            var tran = ConnectionMultiplexer.GetDatabase(database).CreateTransaction();
            transAction(tran);
            return tran.Execute();
        }

        public virtual ISubscriber GetSubscriber()
        {
            return ConnectionMultiplexer.GetSubscriber();
        }

        public void Subscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler)
        {
            GetSubscriber().Subscribe(channel, handler);
        }

        public long Publish(RedisChannel channel, RedisValue message)
        {
            return GetSubscriber().Publish(channel, message);
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
