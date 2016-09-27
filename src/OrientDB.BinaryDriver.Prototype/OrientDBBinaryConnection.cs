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

            ConnectionMetaData = new ConnectionMetaData();
            ConnectionMetaData.ProtocolVersion = BinarySerializer.ToShort(readBuffer.Take(2).ToArray());
            if (ConnectionMetaData.ProtocolVersion < 27)
                ConnectionMetaData.UseTokenBasedSession = false;

            _connectionStream = new OrientDBBinaryConnectionStream(ConnectionMetaData, networkStream);

            _openResult = new DatabaseOpenOperation(_connectionOptions, ConnectionMetaData, _connectionStream).Open();
        }

        public void Close()
        {
            new DatabaseCloseOperation(_openResult.Token, _openResult.SessionId, ConnectionMetaData, _connectionStream).Close();
            _connectionStream.Close();
        }

        public IOrientDBQuery CreateQuery()
        {
            return new OrientDBBinaryQuery(_connectionStream, _serialier);
        }

        public void Dispose()
        {
            
        }
    }
}
