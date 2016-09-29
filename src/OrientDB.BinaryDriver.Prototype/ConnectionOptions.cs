using System.Net;

namespace OrientDB.BinaryDriver.Prototype
{
    public class ConnectionOptions
    {
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DatabaseType Type { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public int PoolSize { get; set; } = 10;
    }
}