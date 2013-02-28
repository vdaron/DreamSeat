using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security;
using System.Text;

namespace DreamSeat.Support
{
    public class ConnectionSettings
    {
        public const string HOST = "host";
        public const string PORT = "port";
        public const string DATABASE = "database";
        public const string USERNAME = "username";
        public const string PASSWORD = "password";

        public ConnectionSettings(string connectionString) {
            ParseConnectionString(connectionString);
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public static string Database { get; set; }
        public string UserName { get; set; }
        public SecureString Password { get; set; }

        private void ParseConnectionString(string connectionString)
        {
            DbConnectionStringBuilder csBuilder = new DbConnectionStringBuilder();
            csBuilder.ConnectionString = connectionString.ToLower();
            
            Host = (string)TryGet(ref csBuilder, HOST);
            Port = (int)TryGet(ref csBuilder, PORT);
            Database = (string)TryGet(ref csBuilder, DATABASE);
            UserName =  (string)TryGet(ref csBuilder, USERNAME);
            Password = (SecureString)TryGet(ref csBuilder, PASSWORD);       
        }

        private object TryGet(ref DbConnectionStringBuilder csBuilder, string part)
        {
            object outVal = null;
            bool hasPart = csBuilder.TryGetValue(part, out outVal);

            if (hasPart && part == PASSWORD)
            {
                var ss = new SecureString();
                foreach (var c in ((string)outVal).ToCharArray())
                {
                    ss.AppendChar(c);
                }
                outVal = null;
                ss.MakeReadOnly();
                return ss;
            }

            switch (part)
            {
                case HOST:
                    return outVal ?? "localhost";
                case PORT:
                    return Convert.ToInt32(outVal ?? 5984);
                default:
                    return outVal;
            }
        }
    }
}
