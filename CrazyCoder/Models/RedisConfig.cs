using System;

namespace CrazyCoder.Models
{
    /// <summary>Redis 服务器节点配置</summary>
    public class RedisConfig
    {
        /// <summary>节点名称</summary>
        public String Name { get; set; } = "";

        /// <summary>服务器地址</summary>
        public String Server { get; set; } = "127.0.0.1";

        /// <summary>端口</summary>
        public Int32 Port { get; set; } = 6379;

        /// <summary>用户名</summary>
        public String Username { get; set; } = "";

        /// <summary>密码</summary>
        public String Password { get; set; } = "";
    }
}
