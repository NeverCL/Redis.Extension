# Redis.Extension
.NET 基于StackExchange.Redis的扩展

## 目录
- 简介

- 主从复制

- 备份与恢复

- API
    - AddOrUpdate
    - GetOrAdd
    - DeleteByPattern
    - SearchKeys
    - TransExcute

- Todo
    - .NET Core
    - Configuration
    - Log
    - Async

## 简介
- [Redis.Extension GitHub](https://github.com/NeverCL/Redis.Extension)
- [Redis.Extension nuget](https://www.nuget.org/packages/Redis.Extension/)

- Redis 开源的分布式NoSQL,c语言开发
- Redis 存储结构:key-value
- 持久化(容灾)
    - snapshot(快照)
        - Redis默认持久化方式，每次保存RDB的时候，fork()出1个子进程进行持久化
        - dump.rdb
        - 配置n秒超过m个key 开始快照
        - 性能高，丢失数据比较多
    - Append-only file(aof)
        - 新命令到达则fsync一次，文件足够大的时候，rewrite一次。
        - appendfilename "appendonly.aof" 设置存储文件
        - appendfsync 设置频率
        - auto-aof-rewrite 设置自动重写
        - 性能稍慢，丢失数据非常少

- 5种数据类型:string,list(链表),hash(哈希),set(集合),zset(排序集合)

## 安装
- Windows
    - 管理多个服务实例
        - redis-server.exe --service-install redis.windows-service --service-name Redis --port 6369
        - sc delete Redis
    - 直接启动
        - redis-server.exe redis.windows.conf

## 主从复制
- 过程
    - 当设置好slave服务器后，slave会建立和master的连接，然后发送sync命令。
    - 无论是第一次同步建立的连接还是连接断开后的重新连接，master都会启动一个后台进程，将数据库快照保存到文件中，同时master主进程会开始收集新的写命令并缓存起来。
    - 后台进程完成写文件后，master就发送文件给slave，slave将文件保存到磁盘上，然后加载到内存恢复数据库快照到slave上。
    - 接着master就会把缓存的命令转发给slave。
    - 而且后续master收到的写命令都会通过开始建立的连接发送给slave。
    - 从master到slave的同步数据的命令和从client发送的命令使用相同的协议格式。
    - 当master和slave的连接断开时slave可以自动重新建立连接。
    - 如果master同时收到多个slave发来的同步连接命令，只会使用启动一个进程来写数据库镜像，然后发送给所有slave。

- 配置
    - slaveof 127.0.0.1 6379

## 备份与恢复
- appendfilename "appendonly.aof"

- snapshot "dump.rdb"

## 使用
- string:   set | get
- hash:     hmset | hgetall
- list:     lpush | lrange
- set:      sadd | smembers
- zset:     zadd | zrange
- save bgsave
- subscribe publish
- keys *
- info