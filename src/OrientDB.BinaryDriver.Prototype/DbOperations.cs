using OrientDB.BinaryDriver.Prototype.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace OrientDB.BinaryDriver.Prototype
{
    internal class DatabaseOpenOperation
    {
        private readonly ConnectionOptions _options;
        private readonly NetworkStream _stream;
        private readonly ConnectionMetaData _metaData;

        public DatabaseOpenOperation(ConnectionOptions options, ConnectionMetaData metaData, NetworkStream stream)
        {
            _options = options;
            _stream = stream;
            _metaData = metaData;
        }

        private Request CreateRequest()
        {
            Request request = new Prototype.Request();

            // standard request fields
            request.AddDataItem((byte)OperationType.DB_OPEN);
            request.AddDataItem(request.SessionId);
            // operation specific fields
            if (DriverConstants.ProtocolVersion >= 7)
            {
                request.AddDataItem(DriverConstants.DriverName);
                request.AddDataItem(DriverConstants.DriverVersion);
                request.AddDataItem(DriverConstants.ProtocolVersion);
                request.AddDataItem(DriverConstants.ClientID);
            }

            if (DriverConstants.ProtocolVersion > 21)
            {
                request.AddDataItem(DriverConstants.RecordFormat.ToString());
            }

            if (DriverConstants.ProtocolVersion > 26)
            {
                request.AddDataItem((byte)(_metaData.UseTokenBasedSession ? 1 : 0)); // Use Token Session 0 - false, 1 - true            
            }

            if (DriverConstants.ProtocolVersion >= 34)
            {
                request.AddDataItem((byte)0);// Support Push
                request.AddDataItem((byte)1);//Support collect-stats
            }

            request.AddDataItem(_options.Database);
            if (DriverConstants.ProtocolVersion >= 8 && DriverConstants.ProtocolVersion < 34)
            {
                request.AddDataItem(_options.Type.ToString().ToLower());
            }
            request.AddDataItem(_options.UserName);
            request.AddDataItem(_options.Password);

            return request;
        }

        public OpenDatabaseResult Open()
        {
            Request request = CreateRequest();

            byte[] bufferData;
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (RequestDataItem item in request.DataItems)
                {
                    switch (item.Type)
                    {
                        case "byte":
                        case "short":
                        case "int":
                        case "long":
                            stream.Write(item.Data, 0, item.Data.Length);
                            break;
                        case "record":
                            bufferData = new byte[2 + item.Data.Length];
                            Buffer.BlockCopy(BinarySerializer.ToArray(item.Data.Length), 0, bufferData, 0, 2);
                            Buffer.BlockCopy(item.Data, 0, bufferData, 2, item.Data.Length);
                            stream.Write(bufferData, 0, bufferData.Length);
                            break;
                        case "bytes":
                        case "string":
                        case "strings":
                            byte[] a = BinarySerializer.ToArray(item.Data.Length);
                            stream.Write(a, 0, a.Length);
                            stream.Write(item.Data, 0, item.Data.Length);
                            break;
                        default:
                            break;
                    }
                }

                Send(stream.ToArray());
            }

            var reader = new BinaryReader(_stream);
            var status = (ResponseStatus)reader.ReadByte();
            var sessionId = reader.ReadInt32EndianAware();

            if(status == ResponseStatus.ERROR)
            {
                string exceptionString = "";

                byte followByte = reader.ReadByte();

                while (followByte == 1)
                {
                    int exceptionClassLength = reader.ReadInt32EndianAware();
                    byte[] exceptionSringByte = reader.ReadBytes(exceptionClassLength);
                    exceptionString += System.Text.Encoding.UTF8.GetString(exceptionSringByte, 0, exceptionSringByte.Length) + ": ";

                    int exceptionMessageLength = reader.ReadInt32EndianAware();

                    // don't read exception message string if it's null
                    if (exceptionMessageLength != -1)
                    {
                        byte[] exceptionByte = reader.ReadBytes(exceptionMessageLength);
                        exceptionString += System.Text.Encoding.UTF8.GetString(exceptionByte, 0, exceptionByte.Length) + "\n";
                    }

                    followByte = reader.ReadByte();
                }
                if (_metaData.ProtocolVersion >= 19)
                {
                    int serializedVersionLength = reader.ReadInt32EndianAware();
                    var buffer = reader.ReadBytes(serializedVersionLength);
                }

                throw new Exception(exceptionString);
            }

            sessionId = reader.ReadInt32EndianAware();
            byte[] token = null;

            if (_metaData.ProtocolVersion > 26)
            {
                var size = reader.ReadInt32EndianAware();
                token = reader.ReadBytesRequired(size);
            }

            int clusterCount = -1;

            if (_metaData.ProtocolVersion >= 7)
                clusterCount = (int)reader.ReadInt16EndianAware();
            else
                clusterCount = reader.ReadInt32EndianAware();

            List<Cluster> clusters = new List<Cluster>();

            if (clusterCount > 0)
            {
                for (int i = 1; i <= clusterCount; i++)
                {
                    Cluster cluster = new Cluster();

                    int clusterNameLength = reader.ReadInt32EndianAware();

                    byte[] clusterByte = reader.ReadBytes(clusterNameLength);
                    cluster.Name = System.Text.Encoding.UTF8.GetString(clusterByte, 0, clusterByte.Length);

                    cluster.Id = reader.ReadInt16EndianAware();

                    if (_metaData.ProtocolVersion < 24)
                    {
                        int clusterTypeLength = reader.ReadInt32EndianAware();

                        byte[] clusterTypeByte = reader.ReadBytes(clusterTypeLength);
                        string clusterType = System.Text.Encoding.UTF8.GetString(clusterTypeByte, 0, clusterTypeByte.Length);
                    
                        if (_metaData.ProtocolVersion >= 12)
                            cluster.DataSegmentID = reader.ReadInt16EndianAware();
                        else
                            cluster.DataSegmentID = 0;
                    }
                    clusters.Add(cluster);
                }
            }

            int clusterConfigLength = reader.ReadInt32EndianAware();

            byte[] clusterConfig = null;

            if (clusterConfigLength > 0)
            {
                clusterConfig = reader.ReadBytes(clusterConfigLength);
            }

            string release = reader.ReadInt32PrefixedString();

            return new OpenDatabaseResult(sessionId, token, clusterCount, clusters, clusterConfig, release);
        }

        private void Send(byte[] rawData)
        {
            if ((_stream != null) && _stream.CanWrite)
            {
                try
                {
                    _stream.Write(rawData, 0, rawData.Length);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }

        }
    }
}
