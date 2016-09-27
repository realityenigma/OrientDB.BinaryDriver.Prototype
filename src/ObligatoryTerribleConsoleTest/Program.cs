using OrientDB.BinaryDriver.Prototype;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObligatoryTerribleConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (OrientDBBinaryConnection conn = new OrientDBBinaryConnection(new ConnectionOptions
            {
                Database = "ConnectionTest",
                HostName = "127.0.0.1",
                Password = "root",
                Port = 2424,
                Type = DatabaseType.Graph,
                UserName = "root"
            }, null))
            {
                conn.Open();

                conn.Close();
            }
        }
    }
}
