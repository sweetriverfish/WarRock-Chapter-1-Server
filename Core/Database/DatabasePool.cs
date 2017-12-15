using System;
using MySql.Data.MySqlClient;

namespace Core.Database
{
    public class DatabasePool
    {
        public ConnectionDetails ConnectionDetails { get; private set; }
        public ushort MinimumPoolSize { get; private set; }
        public ushort MaximumPoolSize { get; private set; }
        public MySqlConnectionStringBuilder ConnectionString { get; private set; }

        public DatabasePool(ConnectionDetails details, ushort minPoolSize, ushort maxPoolSize)
        {
            ConnectionDetails = details;
            this.MinimumPoolSize = minPoolSize;
            this.MaximumPoolSize = maxPoolSize;

            if (MinimumPoolSize == 0)
                throw new Exception("The minimum pool size of the database needs to atleast 1.");

            if (MinimumPoolSize > MaximumPoolSize)
                throw new Exception("The maximum pool size needs to be equal or bigger than the minimum pool size.");

            ConnectionString = new MySqlConnectionStringBuilder()
            {
                Server = ConnectionDetails.Host,
                Port = ConnectionDetails.Port,
                UserID = ConnectionDetails.Username,
                Password = ConnectionDetails.Password,
                Database = ConnectionDetails.Database,
                MinimumPoolSize = MinimumPoolSize,
                MaximumPoolSize = MaximumPoolSize
            };
        }

        //public bool Open()
        //{

        //    MySqlConnection t = new MySqlConnection(ConnectionString.GetConnectionString(true));
        //}

        public MySqlConnection GetConnection()
        {
            MySqlConnection t = new MySqlConnection(ConnectionString.GetConnectionString(true));
            try { t.Open(); } catch { t = null; }
            return t;
        }
    }
}
