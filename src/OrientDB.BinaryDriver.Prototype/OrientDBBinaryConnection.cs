using OrientDB.BinaryDriver.Prototype.Contracts;
using System;
using System.Linq;
using System.Net.Sockets;

namespace OrientDB.BinaryDriver.Prototype
{
    public class OrientDBBinaryConnection : IOrientDBConnection, IDisposable
    {
        public ConnectionMetaData ConnectionMetaData { get; private set; }

        private readonly IOrientDBRecordSerializer _serialier;
        private readonly ConnectionOptions _connectionOptions;
        private OrientDBBinaryConnectionStream _connectionStream;
        private OpenDatabaseResult _openResult; // might not be how I model this here in the end.


        public OrientDBBinaryConnection(ConnectionOptions options, IOrientDBRecordSerializer serializer)
        {
            _connectionOptions = options;
            _serialier = serializer;
        }

        public void Open()
        {
            var readBuffer = new byte[1024];

            var socket = new TcpClient();
            socket.ReceiveTimeout = (30 * 1000);
            socket.ConnectAsync(_connectionOptions.HostName, _connectionOptions.Port).GetAwaiter().GetResult();

            var networkStream = socket.GetStream();
            networkStream.Read(readBuffer, 0, 2);

            var connectionMetaData = new ConnectionMetaData();
            connectionMetaData.ProtocolVersion = BinarySerializer.ToShort(readBuffer.Take(2).ToArray());
            if (connectionMetaData.ProtocolVersion < 27)
                connectionMetaData.UseTokenBasedSession = false;

            ConnectionMetaData = connectionMetaData;

            _openResult = new DatabaseOpenOperation(_connectionOptions, ConnectionMetaData, networkStream).Open();
        }

        public IOrientDBQuery CreateQuery()
        {
            return new OrientDBBinaryQuery(_connectionStream, _serialier);
        }

        public void Dispose()
        {
            // slkdjlksdjfdslkfjsdlkfsdjflsdfj
        }
    }
}
