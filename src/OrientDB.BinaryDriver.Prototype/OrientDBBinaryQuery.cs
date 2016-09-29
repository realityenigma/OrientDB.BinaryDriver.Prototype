using OrientDB.BinaryDriver.Prototype.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }
    }
}
