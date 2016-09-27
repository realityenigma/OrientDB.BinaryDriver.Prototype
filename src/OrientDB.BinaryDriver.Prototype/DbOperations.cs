using OrientDB.BinaryDriver.Prototype.Constants;
using System.Collections.Generic;

namespace OrientDB.BinaryDriver.Prototype
{
    internal class DatabaseOpenOperation
    {
        private readonly ConnectionOptions _options;
        private readonly OrientDBBinaryConnectionStream _stream;
        private readonly ConnectionMetaData _metaData;

        public DatabaseOpenOperation(ConnectionOptions options, ConnectionMetaData metaData, OrientDBBinaryConnectionStream stream)
        {
            _options = options;
            _stream = stream;
            _metaData = metaData;
        }

        private Request CreateRequest()
        {
            Request request = new Prototype.Request(OperationMode.Synchronous);

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

            _stream.Send(_stream.CreateBytes(request));

            var reader = _stream.GetResponseReader();

            var sessionId = reader.ReadInt32EndianAware();
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
    }
}
