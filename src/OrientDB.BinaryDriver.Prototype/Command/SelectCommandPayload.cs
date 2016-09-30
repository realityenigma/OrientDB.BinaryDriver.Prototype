using System;

namespace OrientDB.BinaryDriver.Prototype.Command
{
    internal class SelectCommandPayload : ICommandPayload
    {
        private readonly string _sqlString;
        private readonly string _fetchPlan;

        public SelectCommandPayload(string sql, string fetchPlan)
        {
            _sqlString = sql;
            _fetchPlan = fetchPlan;
        }

        public Request CreatePayloadRequest()
        {
            CommandPayloadQuery payload = new CommandPayloadQuery();
            payload.Text = _sqlString;
            payload.NonTextLimit = -1;
            payload.FetchPlan = _fetchPlan;

            Request request = new Request(OperationMode.Asynchronous);
            //base.Request(request);

            // operation specific fields
            request.AddDataItem((byte)request.OperationMode);

            // idempotent command (e.g. select)
            var queryPayload = payload;
            if (queryPayload != null)
            {
                // Write command payload length
                request.AddDataItem(queryPayload.PayLoadLength);
                request.AddDataItem(queryPayload.ClassName);
                //(text:string)(non-text-limit:int)[(fetch-plan:string)](serialized-params:bytes[])
                request.AddDataItem(queryPayload.Text);
                request.AddDataItem(queryPayload.NonTextLimit);
                request.AddDataItem(queryPayload.FetchPlan);

                if (queryPayload.SerializedParams == null || queryPayload.SerializedParams.Length == 0)
                {
                    request.AddDataItem((int)0);
                }
                else
                {
                    request.AddDataItem(queryPayload.SerializedParams);
                }
                return request;
            }
            // @todo Fix this to a better domain exception.
            throw new Exception("Need to fix this");
        }
    }
}
