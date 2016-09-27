using OrientDB.BinaryDriver.Prototype.Constants;
using System;
using System.IO;
using System.Net.Sockets;

namespace OrientDB.BinaryDriver.Prototype
{
    internal class DatabaseCloseOperation
    {
        private readonly ConnectionMetaData _metaData;
        private readonly byte[] _connectionToken;
        private OrientDBBinaryConnectionStream _stream;
        private int _sessionId;

        public DatabaseCloseOperation(byte[] token, int sessionId, ConnectionMetaData metaData, OrientDBBinaryConnectionStream stream)
        {
            _connectionToken = token;
            _metaData = metaData;
            _stream = stream;
            _sessionId = sessionId;
        }

        private Request CreateRequest()
        {
            Request request = new Prototype.Request(OperationMode.Asynchronous);
            
            request.AddDataItem((byte)OperationType.DB_CLOSE);
            request.AddDataItem(request.SessionId);

            if (DriverConstants.ProtocolVersion > 26 && _metaData.UseTokenBasedSession)
            {
                request.AddDataItem(_connectionToken);
            }

            return request;
        }

        internal byte[] ReadToken(BinaryReader reader)
        {
            var size = reader.ReadInt32EndianAware();
            var token = reader.ReadBytesRequired(size);            

            return token;
        }

        internal void Close()
        {
            Request request = CreateRequest();

            byte[] buffer = _stream.CreateBytes(request);
            _stream.Send(buffer);            
        }
    }
}