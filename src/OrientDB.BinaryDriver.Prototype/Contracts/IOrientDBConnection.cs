namespace OrientDB.BinaryDriver.Prototype.Contracts
{
    internal interface IOrientDBConnection
    {        
        IOrientDBCommand CreateCommand();
    }
}