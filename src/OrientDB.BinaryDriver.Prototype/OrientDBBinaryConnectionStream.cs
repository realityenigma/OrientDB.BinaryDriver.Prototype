using OrientDB.BinaryDriver.Prototype.Contracts;
using System;
using System.IO;
using System.Net.Sockets;

namespace OrientDB.BinaryDriver.Prototype
{
    public class OrientDBBinaryConnectionStream
    {
        private readonly ConnectionMetaData _metaData;
        private NetworkStream Stream { get; }

        public OrientDBBinaryConnectionStream(ConnectionMetaData connectionMetaData, NetworkStream stream)
        {
            _metaData = connectionMetaData;
            Stream = stream;
        }

        public BinaryReader GetResponseReader()
        {
            var reader = new BinaryReader(Stream);
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
                if ( _metaData.ProtocolVersion >= 19)
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
            Stream.Dispose();
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

        public void Send(byte[] buffer)
        {
            lock (_syncRoot)
            {
                if ((Stream != null) && Stream.CanWrite)
                {
                    try
                    {
                        Stream.Write(buffer, 0, buffer.Length);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex.InnerException);
                    }
                }
            }
        }
    }
}