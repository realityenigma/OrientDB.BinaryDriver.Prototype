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
        private readonly ConnectionMetaData _metaData;

        public DatabaseCommandOperation(ICommandPayloadConstructorFactory payloadFacctory, ConnectionMetaData metaData, string query, string fetchPlan = "*:0")
        {
            _query = query;
            _fetchPlan = fetchPlan;
            _payloadFactory = payloadFacctory;
            _metaData = metaData;
        }

        public Request CreateRequest()
        {
            return _payloadFactory.CreatePayload(_query, _fetchPlan, _metaData).CreatePayloadRequest();
            
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

            //if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
            //    ReadToken(reader);

            //// operation specific fields
            //PayloadStatus payloadStatus = (PayloadStatus)reader.ReadByte();

            //responseDocument.SetField("PayloadStatus", payloadStatus);

            //if (OperationMode == OperationMode.Asynchronous)
            //{
            //    List<ODocument> documents = new List<ODocument>();

            //    while (payloadStatus != PayloadStatus.NoRemainingRecords)
            //    {
            //        ODocument document = ParseDocument(reader);

            //        switch (payloadStatus)
            //        {
            //            case PayloadStatus.ResultSet:
            //                documents.Add(document);
            //                break;
            //            case PayloadStatus.PreFetched:
            //                //client cache
            //                response.Connection.Database.ClientCache[document.ORID] = document;
            //                break;
            //            default:
            //                break;
            //        }

            //        payloadStatus = (PayloadStatus)reader.ReadByte();
            //    }

            //    responseDocument.SetField("Content", documents);
            //}
            //else
            //{
            //    int contentLength;

            //    switch (payloadStatus)
            //    {
            //        case PayloadStatus.NullResult: // 'n'
            //            // nothing to do
            //            break;
            //        case PayloadStatus.SingleRecord: // 'r'
            //            T document = ParseDocument(reader);
            //            responseDocument.SetField("Content", document);
            //            break;
            //        case PayloadStatus.SerializedResult: // 'a'
            //            contentLength = reader.ReadInt32EndianAware();
            //            byte[] serializedBytes = reader.ReadBytes(contentLength);
            //            string serialized = System.Text.Encoding.UTF8.GetString(serializedBytes, 0, serializedBytes.Length);
            //            responseDocument.SetField("Content", serialized);
            //            break;
            //        case PayloadStatus.RecordCollection: // 'l'
            //            List<ODocument> documents = new List<ODocument>();

            //            int recordsCount = reader.ReadInt32EndianAware();

            //            for (int i = 0; i < recordsCount; i++)
            //            {
            //                documents.Add(ParseDocument(reader));
            //            }

            //            responseDocument.SetField("Content", documents);
            //            break;
            //        case PayloadStatus.SimpleResult: //'w'
            //            ODocument sDocument = ParseDocument(reader);
            //            var isNestedCollection = sDocument["result"] is List<List<object>>;
            //            responseDocument.SetField("Content",
            //                isNestedCollection ? sDocument["result"] : sDocument["result"].ToString());
            //            break;
            //        default:
            //            break;
            //    }

            //    if (OClient.ProtocolVersion >= 17)
            //    {
            //        //Load the fetched records in cache
            //        while ((payloadStatus = (PayloadStatus)reader.ReadByte()) != PayloadStatus.NoRemainingRecords)
            //        {
            //            ODocument document = ParseDocument(reader);
            //            if (document != null && payloadStatus == PayloadStatus.PreFetched)
            //            {
            //                //Put in the client local cache
            //                response.Connection.Database.ClientCache[document.ORID] = document;
            //            }
            //        }
            //    }
            //}

            //return responseDocument;
        }

        private T ParseDocument(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
