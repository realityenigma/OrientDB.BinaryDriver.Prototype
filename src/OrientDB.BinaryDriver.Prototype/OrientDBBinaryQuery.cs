using OrientDB.BinaryDriver.Prototype.Command;
using OrientDB.BinaryDriver.Prototype.Contracts;
using System.Collections.Generic;

namespace OrientDB.BinaryDriver.Prototype
{
    public class OrientDBCommand : IOrientDBCommand
    {
        private readonly OrientDBBinaryConnectionStream _stream;
        private readonly IOrientDBRecordSerializer _serializer;
        private readonly ICommandPayloadConstructorFactory _payloadFactory;

        internal OrientDBCommand(OrientDBBinaryConnectionStream stream, IOrientDBRecordSerializer serializer, ICommandPayloadConstructorFactory payloadFactory)
        {
            _stream = stream;
            _serializer = serializer;
            _payloadFactory = payloadFactory;
        }

        public IEnumerable<T> Execute<T>(string query)
        {
            return _stream.Send(new DatabaseCommandOperation<T>(_payloadFactory, _stream.ConnectionMetaData, query)).Results;
        }
    }
}
