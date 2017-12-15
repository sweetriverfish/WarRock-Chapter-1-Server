using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Database
{
    /// <summary>
    /// This class contains all the details of a single connection.
    /// </summary>
    public class ConnectionDetails
    {
        public string Host { get; private set; }
        public uint Port { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Database { get; private set; }

        public ConnectionDetails(string host, uint port, string username, string password, string database)
        {
            this.Host = host;
            this.Port = port;
            this.Username = username;
            this.Password = password;
            this.Database = database;
        }

        public override string ToString()
        {
            return Username + "@" + Host + ":" + Port.ToString();
        }
    }
}
