using OrientDB.BinaryDriver.Prototype.Contracts;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace OrientDB.BinaryDriver.Prototype
{
    public class OrientDBBinaryConnectionStream
    {
        public ConnectionMetaData ConnectionMetaData { get; private set; }
        private readonly ConnectionOptions _connectionOptions;

        private ConcurrentBag<NetworkStream> _streamPool = new ConcurrentBag<NetworkStream>();

        public OrientDBBinaryConnectionStream(ConnectionOptions options)
        {
            _connectionOptions = options;

            for (var i = 0; i < options.PoolSize; i++)
            {
                _streamPool.Add(CreateNetworkStream());
            }
        }

        private NetworkStream CreateNetworkStream()
        {
            var readBuffer = new byte[1024];

            var socket = new TcpClient();
            socket.ReceiveTimeout = (30 * 1000);
            socket.ConnectAsync(_connectionOptions.HostName, _connectionOptions.Port).GetAwaiter().GetResult();

            var networkStream = socket.GetStream();
            networkStream.Read(readBuffer, 0, 2);

            ConnectionMetaData = new ConnectionMetaData();
            ConnectionMetaData.ProtocolVersion = BinarySerializer.ToShort(readBuffer.Take(2).ToArray());
            if (ConnectionMetaData.ProtocolVersion < 27)
                ConnectionMetaData.UseTokenBasedSession = false;

            return networkStream;
        }

        private NetworkStream GetNetworkStream()
        {
            NetworkStream stream;
            _streamPool.TryTake(out stream);
            if (stream == null)
                return CreateNetworkStream();
            return stream;
        }

        private BinaryReader GetResponseReader(NetworkStream stream)
        {
            var reader = new BinaryReader(stream);
            var status = (ResponseStatus)reader.ReadByte();
            var sessionId = reader.ReadInt32EndianAware();

            if (status == ResponseStatus.ERROR)
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
                if (ConnectionMetaData.ProtocolVersion >= 19)
                {
                    int serializedVersionLength = reader.ReadInt32EndianAware();
                    var buffer = reader.ReadBytes(serializedVersionLength);
                }

                throw new Exception(exceptionString);
            }

            return reader;
        }

        public void Close()
        {
            foreach (var stream in _streamPool)
            {
                stream.Dispose();
            }
        }

        internal byte[] CreateBytes(Request request)
        {
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

                return stream.ToArray();
            }
        }

        private object _syncRoot = new object();

        internal T Send<T>(IOrientDBOperation<T> operation)
        {
            var stream = GetNetworkStream();

            Request request = operation.CreateRequest();

            var reader = Send(request, stream);

            T result = operation.Execute(reader);

            ReturnStream(stream);

            return result;
        }

        // Return the Stream back to the pool.
        private void ReturnStream(NetworkStream stream)
        {
            if (_streamPool.Count >= _connectionOptions.PoolSize)
                stream.Dispose();
            else
                _streamPool.Add(stream);
        }

        private BinaryReader Send(Request request, NetworkStream stream)
        {
            Send(CreateBytes(request), stream);

            if (request.OperationMode == OperationMode.Asynchronous)
                return null;

            return GetResponseReader(stream);
        }

        private void Send(byte[] buffer, NetworkStream stream)
        {
            if ((stream != null) && stream.CanWrite)
            {
                try
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }

        }
    }
}