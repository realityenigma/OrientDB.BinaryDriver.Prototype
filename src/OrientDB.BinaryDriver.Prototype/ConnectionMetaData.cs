using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrientDB.BinaryDriver.Prototype
{
    public class ConnectionMetaData
    {
        public int ProtocolVersion { get; internal set; }
        public int OrientRelease { get; internal set; }
        public int SessionId { get; internal set; }
        public byte[] Token { get; internal set; }
        public int ClusterCount { get; internal set; }
        public string ClusterConfig { get; internal set; }
        public bool UseTokenBasedSessions { get; internal set; }
    }
}
