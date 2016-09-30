using OrientDB.BinaryDriver.Prototype.Command;

namespace OrientDB.BinaryDriver.Prototype
{
    internal class CommandPayloadConstructorFactory : ICommandPayloadConstructorFactory
    {
        public ICommandPayload CreatePayload(string query, string fetchPlan)
        {
            if (query.ToLower().StartsWith("select"))
                return new SelectCommandPayload(query, fetchPlan);

            return null;
        }
    }
}
