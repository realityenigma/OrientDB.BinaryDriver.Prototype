using System.Collections.Generic;

namespace OrientDB.BinaryDriver.Prototype.Contracts
{
    public interface IOrientDBRecordSerializer
    {
        IEnumerable<T> Deserialize<T>(object data);

        void Serialize(object data);
    }
}