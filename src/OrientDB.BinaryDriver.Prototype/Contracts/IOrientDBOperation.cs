using System.IO;

namespace OrientDB.BinaryDriver.Prototype.Contracts
{
    internal interface IOrientDBOperation<T>
    {
        Request CreateRequest();
        T Execute(BinaryReader reader);
    }
}
