using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;

namespace Redis.Extension.Tests
{
    [TestClass]
    public class ManagerTest
    {
        private static string _connectionString = "127.0.0.1:6379";
        readonly RedisManager _redisManager = new RedisManager(_connectionString);

        [TestMethod]
        public void TestAllKeys()
        {
            var allKeys = _redisManager.AllKeys().Count();
            var searchKeys = _redisManager.Search(RedisValue.Null).Count();
            var keys = _redisManager.AllKeys().ToList();    // 速度实际与上面2种基本一致
            var keyCount = _redisManager.CountKeys();   // 速度最快
            Assert.IsTrue(allKeys == searchKeys);
        }


        [TestMethod]
        public void TestAddOrUpdate()
        {
            Assert.IsTrue(_redisManager.AddOrUpdate("hello1", "world", TimeSpan.FromSeconds(30)));
            Assert.IsTrue(_redisManager.AddOrUpdate("hello2", new { Name = "hello", Age = "world" }, TimeSpan.FromSeconds(30)));
        }


        [TestMethod]
        public void TestGetOrAdd()
        {
            Assert.AreEqual(_redisManager.GetOrAdd("hello3", () => "hello world", TimeSpan.FromSeconds(30)), "hello world");
            Assert.IsNotNull(_redisManager.GetOrAdd<object>("hello4", () => new { Name = "hello", Age = "world" }, TimeSpan.FromSeconds(30)));
        }

        [TestMethod]
        public void TestDelete()
        {
            Assert.IsTrue(_redisManager.Delete("hello1"));
            _redisManager.DeleteByPattern("hello*");
        }


        [TestMethod]
        public void TestTrans()
        {
            var rst = _redisManager.TransExcute(trans =>
           {
               trans.StringSetAsync("hello5", Guid.NewGuid().ToString());
               trans.StringSetAsync("hello6", Guid.NewGuid().ToString());
               //throw new Exception("hss");
           });

            Assert.IsTrue(rst);
        }
    }
}
