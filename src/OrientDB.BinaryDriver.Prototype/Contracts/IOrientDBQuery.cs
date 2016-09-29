using System.Collections.Generic;

namespace OrientDB.BinaryDriver.Prototype.Contracts
{
    public interface IOrientDBCommand
    {
        IEnumerable<T> Execute<T>(string query);
    }
}