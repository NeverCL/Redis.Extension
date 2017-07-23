﻿using System;

namespace Redis.Extension.Core
{
    /// <summary>
    /// IRedisManager 接口
    /// </summary>
    public interface IRedisManager
    {
        /// <summary>
        /// 数据链接选择器
        /// </summary>
        ConnectionMultiplexer ConnectionMultiplexer { get; }

        #region Keys
        /// <summary>
        /// 获取所有Keys
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        IEnumerable<RedisKey> AllKeys(int database = 0);

        /// <summary>
        /// 模糊查询Keys
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        IEnumerable<RedisKey> Search(RedisValue pattern, int database = 0);

        /// <summary>
        /// 获取Keys数量
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        long CountKeys(int database = 0);
        #endregion

        #region Object Set Get
        /// <summary>
        /// 添加到Redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        bool AddOrUpdate(RedisKey key, RedisValue value, TimeSpan? expiry = null);

        /// <summary>
        /// 使用Json序列化对象，添加到Redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        bool AddOrUpdate(RedisKey key, object obj, TimeSpan? expiry = null);

        /// <summary>
        /// 从Redis获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getValueFunc"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        RedisValue GetOrAdd(RedisKey key, Func<RedisValue> getValueFunc = null, TimeSpan? expiry = null);

        /// <summary>
        /// 从Redis获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="getValueFunc"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        T GetOrAdd<T>(RedisKey key, Func<object> getValueFunc = null, TimeSpan? expiry = null) where T : class;

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Delete(RedisKey key);

        /// <summary>
        /// 模糊删除
        /// </summary>
        /// <param name="pattern"></param>
        void DeleteByPattern(RedisValue pattern);
        #endregion

        #region Transaction

        /// <summary>
        /// 事务执行
        /// </summary>
        /// <param name="transAction"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        bool TransExcute(Action<ITransaction> transAction, int database = 0);

        #endregion
    }
}