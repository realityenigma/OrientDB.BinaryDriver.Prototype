namespace OrientDB.BinaryDriver.Prototype.Contracts
{
    public interface IOrientDBConnection
    {        
        IOrientDBCommand CreateCommand();
    }
}