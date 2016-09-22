namespace OrientDB.BinaryDriver.Prototype.Constants
{
    internal class DriverConstants
    {
        public static string ClientID { get; internal set; }
        public static string DriverName { get; internal set; }
        public static string DriverVersion { get; internal set; }
        public static int ProtocolVersion { get; internal set; }
        public static RecordFormat RecordFormat { get; internal set; }

        static DriverConstants()
        {
            ProtocolVersion = 35;
            DriverName = "OrientDBBinaryConnection";
            DriverVersion = "0.0.1";
            ClientID = "null";
            RecordFormat = RecordFormat.ORecordDocument2csv;
        }
    }
}
