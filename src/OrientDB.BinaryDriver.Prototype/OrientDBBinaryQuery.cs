using OrientDB.BinaryDriver.Prototype.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrientDB.BinaryDriver.Prototype
{
    public class OrientDBBinaryQuery : IOrientDBQuery
    {
        private readonly OrientDBBinaryConnectionStream _stream;
        private readonly IOrientDBRecordSerializer _serializer;

        public OrientDBBinaryQuery(OrientDBBinaryConnectionStream stream, IOrientDBRecordSerializer serializer)
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
