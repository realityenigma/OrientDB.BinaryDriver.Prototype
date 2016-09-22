using OrientDB.BinaryDriver.Prototype.Contracts;
using System.Net.Sockets;

namespace OrientDB.BinaryDriver.Prototype
{
    public class OrientDBBinaryConnectionStream
    {
        private readonly IOrientDBConnection _connection;
        private NetworkStream Stream { get; }

        public OrientDBBinaryConnectionStream(IOrientDBConnection connection, NetworkStream stream)
        {
            _connection = connection;
            Stream = stream;
        }

        private object _syncRoot = new object();

        public byte[] Send(byte[] buffer)
        {
            lock (_syncRoot)
            {
                return null;
            }
        }
    }
}