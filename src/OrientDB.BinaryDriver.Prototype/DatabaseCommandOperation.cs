using OrientDB.BinaryDriver.Prototype.Command;
using OrientDB.BinaryDriver.Prototype.Contracts;
using System;
using System.IO;

namespace OrientDB.BinaryDriver.Prototype
{
    internal class DatabaseCommandOperation<T> : IOrientDBOperation<CommandResult<T>>
    {
        private readonly string _query;
        private readonly string _fetchPlan;
        private readonly ICommandPayloadConstructorFactory _payloadFactory;

        public DatabaseCommandOperation(ICommandPayloadConstructorFactory payloadFacctory, string query, string fetchPlan = "*:0")
        {
            _query = query;
            _fetchPlan = fetchPlan;
            _payloadFactory = payloadFacctory;
        }

        public Request CreateRequest()
        {
            return _payloadFactory.CreatePayload(_query, _fetchPlan).CreatePayloadRequest();
            
            // non-idempotent command (e.g. insert)
            //var scriptPayload = CommandPayload as CommandPayloadScript; <----- Liskov Violations We MUST fix this.
            //if (scriptPayload != null)
            //{
            //    // Write command payload length
            //    request.AddDataItem(scriptPayload.PayLoadLength);
            //    request.AddDataItem(scriptPayload.ClassName);
            //    if (scriptPayload.Language != "gremlin")
            //        request.AddDataItem(scriptPayload.Language);
            //    request.AddDataItem(scriptPayload.Text);
            //    if (scriptPayload.SimpleParams == null)
            //        request.AddDataItem((byte)0); // 0 - false, 1 - true
            //    else
            //    {
            //        request.AddDataItem((byte)1);
            //        request.AddDataItem(scriptPayload.SimpleParams);
            //    }
            //    request.AddDataItem((byte)0);

            //    return request;
            //}
            //var commandPayload = CommandPayload as CommandPayloadCommand; < -----Liskov Violations We MUST fix this.
            //if (commandPayload != null)
            //{
            //    // Write command payload length
            //    request.AddDataItem(commandPayload.PayLoadLength);
            //    request.AddDataItem(commandPayload.ClassName);
            //    // (text:string)(has-simple-parameters:boolean)(simple-paremeters:bytes[])(has-complex-parameters:boolean)(complex-parameters:bytes[])
            //    request.AddDataItem(commandPayload.Text);
            //    // has-simple-parameters boolean
            //    if (commandPayload.SimpleParams == null)
            //        request.AddDataItem((byte)0); // 0 - false, 1 - true
            //    else
            //    {
            //        request.AddDataItem((byte)1);
            //        request.AddDataItem(commandPayload.SimpleParams);
            //    }
            //    //request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(0) });
            //    // has-complex-parameters
            //    request.AddDataItem((byte)0); // 0 - false, 1 - true
            //    //request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(0) });
            //    return request;
            //}
            //throw new OException(OExceptionType.Operation, "Invalid payload");
        }

        public CommandResult<T> Execute(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
