using OrientDB.BinaryDriver.Prototype.Contracts;
using System.Collections.Generic;

namespace OrientDB.BinaryDriver.Prototype
{
    public class OrientDBCommand : IOrientDBCommand
    {
        private readonly OrientDBBinaryConnectionStream _stream;
        private readonly IOrientDBRecordSerializer _serializer;

        public OrientDBCommand(OrientDBBinaryConnectionStream stream, IOrientDBRecordSerializer serializer)
        {
            _stream = stream;
            _serializer = serializer;
        }

        public IEnumerable<T> Execute<T>(string query)
        {
            return _stream.Send(new DatabaseCommandOperation<T>(query)).Results;
        }
    }
}
