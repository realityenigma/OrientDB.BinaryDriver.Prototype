using OrientDB.BinaryDriver.Prototype.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace OrientDB.BinaryDriver.Prototype
{
    public class OrientDBBinaryConnection : IOrientDBConnection
    {
        public ConnectionMetaData ConnectionMetaData { get; private set; }

        private readonly IOrientDBRecordSerializer _serialier;
        private readonly ConnectionOptions _connectionOptions;
        private OrientDBBinaryConnectionStream _connectionStream;


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
                connectionMetaData.UseTokenBasedSessions = false;

            Request request = new Request();
            //request.AddDataItem((byte));
            //request.AddDataItem(sessionId);

        }

        public IOrientDBQuery CreateQuery()
        {
            return new OrientDBBinaryQuery(_connectionStream, _serialier);
        }
    }
}
