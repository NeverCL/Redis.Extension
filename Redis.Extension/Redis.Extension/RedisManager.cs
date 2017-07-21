using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Extension
{
    public class RedisManager : IDisposable
    {
        static ConnectionMultiplexer _redis;

        /// <summary>
        /// 默认取第一个数据库实例
        /// </summary>
        public static IDatabase RedisDatabaseInstance => ConnectionMultiplexer.GetDatabase();

        /// <summary>
        /// 默认取第一个服务实例
        /// </summary>
        public static IServer RedisServerInstance => ConnectionMultiplexer.GetServer(ConnectionMultiplexer.GetEndPoints()[0]);

        //public static IDatabase GetRedisDatabase(int db)
        //{
        //    return ConnectionMultiplexer.GetDatabase(db);
        //}

        //public static IServer GetRedisServer(int db)
        //{
        //    return ConnectionMultiplexer.GetServer(ConnectionMultiplexer.GetEndPoints()[db]);
        //}

        public static ConnectionMultiplexer ConnectionMultiplexer
        {
            get
            {
                if (_redis == null)
                {
                    lock ("RedisManager锁")
                    {
                        if (_redis != null) return _redis;
                        _redis = GetManager();
                        return _redis;
                    }
                }
                return _redis;
            }
        }

        static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConfigurationManager.AppSettings["redis"] ?? "localhost";
            }
            return ConnectionMultiplexer.Connect(connectionString);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="configurationOptions"></param>
        /// <returns></returns>
        static ConnectionMultiplexer GetManager(ConfigurationOptions configurationOptions)
        {
            return ConnectionMultiplexer.Connect(configurationOptions);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
