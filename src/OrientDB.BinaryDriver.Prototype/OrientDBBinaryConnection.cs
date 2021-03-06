﻿using OrientDB.BinaryDriver.Prototype.Command;
using OrientDB.BinaryDriver.Prototype.Contracts;
using System;

namespace OrientDB.BinaryDriver.Prototype
{
    public class OrientDBBinaryConnection : IOrientDBConnection, IDisposable
    {
        private readonly IOrientDBRecordSerializer _serialier;
        private readonly ConnectionOptions _connectionOptions;
        private OrientDBBinaryConnectionStream _connectionStream;
        private OpenDatabaseResult _openResult; // might not be how I model this here in the end.
        private ICommandPayloadConstructorFactory _payloadFactory;


        public OrientDBBinaryConnection(ConnectionOptions options, IOrientDBRecordSerializer serializer)
        {
            _connectionOptions = options;
            _serialier = serializer;
            _payloadFactory = new CommandPayloadConstructorFactory();
        }

        public void Open()
        {
            _connectionStream = new OrientDBBinaryConnectionStream(_connectionOptions);
            _openResult = _connectionStream.Send(new DatabaseOpenOperation(_connectionOptions, _connectionStream.ConnectionMetaData));
            _connectionStream.ConnectionMetaData.SessionId = _openResult.SessionId; // This is temporary.
        }

        public void Close()
        {
            _connectionStream.Send(new DatabaseCloseOperation(_openResult.Token, _connectionStream.ConnectionMetaData));
            _connectionStream.Close();            
        }

        public IOrientDBCommand CreateCommand()
        {
            return new OrientDBCommand(_connectionStream, _serialier, _payloadFactory);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
