using Core.IO;
using System;

namespace Game {
    class Config {
        public static string AUTH_SERVER_IP = "127.0.0.1";
        public static byte SERVER_ID = 0;                   
        public static string SERVER_KEY = "SERVER-KEY";
        public static string SERVER_NAME = "AlterEmu";
        public static string SERVER_IP = "";
        public static byte SERVER_DEBUG = 1;
        public static byte SERVER_LOGGER = 1;
        public static byte PACKET_LOGGER = 1;

        public static string[] GAME_DATABASE;


        public static bool Read()
        {
            try
            {
                ConfigParser Parser = new ConfigParser("ServerConfig");

                AUTH_SERVER_IP = Parser.Read("GameServer", "AuthServer", "IP");
                SERVER_KEY = Parser.Read("GameServer", "AuthServer", "Key");
                SERVER_NAME = Parser.Read("GameServer", "Server", "Name");
                SERVER_IP = Parser.Read("GameServer", "Server", "IP");

                try
                {
                    SERVER_DEBUG = Convert.ToByte(Parser.Read("GameServer", "Server", "Debug"));
                    SERVER_LOGGER = Convert.ToByte(Parser.Read("GameServer", "Server", "LogActivity"));
                    PACKET_LOGGER = Convert.ToByte(Parser.Read("GameServer", "Server", "LogPackets"));
                }
                catch { SERVER_DEBUG = 1; SERVER_LOGGER = 1; PACKET_LOGGER = 1; }
              

                GAME_DATABASE = new string[]
                {
                    Parser.Read("GameServer", "Database", "Host"),
                    Parser.Read("GameServer", "Database", "Port"),
                    Parser.Read("GameServer", "Database", "UserName"),
                    Parser.Read("GameServer", "Database", "UserPassword"),
                    Parser.Read("GameServer", "Database", "DatabaseName")
                    
                };

                return true;
            }
            catch { return false; }
        }
    }
}
