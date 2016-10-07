namespace OrientDB.BinaryDriver.Prototype.Command
{
    internal interface ICommandPayloadConstructorFactory
    {
        ICommandPayload CreatePayload(string query, string fetchPlan, ConnectionMetaData metaData);
    }
}
