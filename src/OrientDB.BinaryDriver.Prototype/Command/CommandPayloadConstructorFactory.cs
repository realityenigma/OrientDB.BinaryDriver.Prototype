using OrientDB.BinaryDriver.Prototype.Command;

namespace OrientDB.BinaryDriver.Prototype
{
    internal class CommandPayloadConstructorFactory : ICommandPayloadConstructorFactory
    {
        public ICommandPayload CreatePayload(string query, string fetchPlan, ConnectionMetaData metaData)
        {
            if (query.ToLower().StartsWith("select"))
                return new SelectCommandPayload(query, fetchPlan, metaData);
            if (query.ToLower().StartsWith("insert"))
                return new InsertCommandPayload(query, fetchPlan, metaData);

            return null;
        }
    }
}
