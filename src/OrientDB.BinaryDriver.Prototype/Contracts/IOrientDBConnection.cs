namespace OrientDB.BinaryDriver.Prototype.Contracts
{
    public interface IOrientDBConnection
    {
        IOrientDBQuery CreateQuery();
    }
}