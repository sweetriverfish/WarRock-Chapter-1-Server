using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

/*
    CONFIGURATION DATA NOT STORED IN AN INI FILE BUT IN THE DB
*/
namespace Authorization
{
    class AuthConfig
    {
        public static byte Format = 0;
        public static byte Launcher = 0;
        public static byte Updater = 0;
        public static byte Client = 0;
        public static byte Sub = 0;
        public static byte Option = 0;
        public static string URL = "http://";

        public static bool Read()
        {

            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM updater", Databases.Auth.connection);
            MySqlDataReader Reader = Cmd.ExecuteReader();

            try
            {
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        Format = Reader.GetByte("format");
                        Launcher = Reader.GetByte("launcher");
                        Updater = Reader.GetByte("updater");
                        Client = Reader.GetByte("client");
                        Sub = Reader.GetByte("sub");
                        Option = Reader.GetByte("option");
                        URL = Reader.GetString("download_url");
                    }

                }
                Reader.Close();

                return true;
            }
            catch { return false; }
        }
    }
}
