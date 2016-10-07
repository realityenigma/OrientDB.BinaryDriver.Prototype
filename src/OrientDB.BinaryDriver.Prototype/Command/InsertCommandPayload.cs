using OrientDB.BinaryDriver.Prototype.Constants;
using System;

namespace OrientDB.BinaryDriver.Prototype.Command
{
    internal class InsertCommandPayload : ICommandPayload
    {
        private readonly string _sqlString;
        private readonly string _fetchPlan;
        private readonly ConnectionMetaData _metaData;

        public InsertCommandPayload(string sql, string fetchPlan, ConnectionMetaData metaData)
        {
            _sqlString = sql;
            _fetchPlan = fetchPlan;
            _metaData = metaData;
        }

        public Request CreatePayloadRequest()
        {
            CommandPayloadScript payload = new CommandPayloadScript();
            payload.Text = _sqlString;
            payload.Language = "sql";

            Request request = new Request(OperationMode.Synchronous);

            request.AddDataItem((byte)OperationType.COMMAND);
            request.AddDataItem(_metaData.SessionId);

            if (DriverConstants.ProtocolVersion > 26 && _metaData.UseTokenBasedSession)
            {
                request.AddDataItem(_metaData.Token);
            }

            // operation specific fields
            request.AddDataItem((byte)request.OperationMode);

            var scriptPayload = payload;
            if (scriptPayload != null)
            {
                request.AddDataItem(scriptPayload.PayLoadLength);
                request.AddDataItem(scriptPayload.ClassName);
                if (scriptPayload.Language != "gremlin")
                    request.AddDataItem(scriptPayload.Language);
                request.AddDataItem(scriptPayload.Text);
                if (scriptPayload.SimpleParams == null)
                    request.AddDataItem((byte)0); // 0 - false, 1 - true
                else
                {
                    request.AddDataItem((byte)1);
                    request.AddDataItem(scriptPayload.SimpleParams);
                }
                request.AddDataItem((byte)0);
                return request;
            }
            // @todo Fix this to a better domain exception.
            throw new Exception("Need to fix this");
        }
    }
}
