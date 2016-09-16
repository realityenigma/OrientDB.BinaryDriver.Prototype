using System.Collections.Generic;

namespace OrientDB.BinaryDriver.Prototype.Contracts
{
    public interface IOrientDBQuery
    {
        IEnumerable<T> Execute<T>(string query);
    }
}