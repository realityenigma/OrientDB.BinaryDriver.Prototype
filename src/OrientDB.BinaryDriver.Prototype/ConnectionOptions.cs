using System.Net;

namespace OrientDB.BinaryDriver.Prototype
{
    public class ConnectionOptions
    {
        public IPAddress HostName { get; internal set; }
        public int Port { get; internal set; }
    }
}