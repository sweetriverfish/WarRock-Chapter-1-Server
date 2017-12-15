using Core.IO;
using System;
namespace Authorization
{
    public class Config
    {
        public static byte MAXIMUM_SERVER_COUNT = 4; //TODO: DARK: INVESTIGATE THIS SHIT
        public static string[] AUTH_DATABASE;
        public static byte SERVER_LOGGER = 1;
        public static byte PACKET_LOGGER = 1;
       
        public static bool Read()
        {          
            bool result = false;

            try
            {
                ConfigParser XMLConfig = new ConfigParser("ServerConfig");

                Config.AUTH_DATABASE = new string[]
                {
                    XMLConfig.Read("AuthenticationServer", "Database", "Host"),
                    XMLConfig.Read("AuthenticationServer", "Database", "Port"),
                    XMLConfig.Read("AuthenticationServer", "Database", "UserName"),
                    XMLConfig.Read("AuthenticationServer", "Database", "UserPassword"),
                    XMLConfig.Read("AuthenticationServer", "Database", "DatabaseName")
                };

                SERVER_LOGGER = Convert.ToByte(XMLConfig.Read("AuthenticationServer", "Server", "LogActivity"));
                PACKET_LOGGER = Convert.ToByte(XMLConfig.Read("AuthenticationServer", "Server", "LogPackets"));

                result = true;
            }
            catch {}
  
            return result;
        }
    }
}
