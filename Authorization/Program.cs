using System;
using System.Threading;

namespace Authorization {
    class Program {
        private static bool isRunning = false;
        public static object sessionLock = new Object();
        public static int totalPlayers = 0;
        public static int onlinePlayers = 0;
        public static int playerPeak = 0;

        static void Main(string[] args) {

            Console.Title = "「Starting」Authentication server";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@" _______        _______ _______  ______ _______ _______ _     _");
            Console.WriteLine(@" |_____| |         |    |______ |_____/ |______ |  |  | |     |");
            Console.WriteLine(@" |     | |_____    |    |______ |    \_ |______ |  |  | |_____|");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(new string('_', Console.WindowWidth));
            Console.WriteLine();

            if (!Config.Read())
            {
                Log.Instance.WriteError("Failed to load the configuration file.");
                Console.ReadKey();
                return;
            }

            if (!Databases.Init()) {
                Log.Instance.WriteError("Failed to initilize all database connections.");
                Console.ReadKey();
                return;
            }

            if (!AuthConfig.Read()) {
                Log.Instance.WriteError("Failed to read the updater table");
                Console.ReadKey();
                return;
            }

            if (!new Networking.GameServerListener((int)Core.Enums.Ports.Internal).Start()) {
                return;
            }

            isRunning = (new Networking.ServerListener((int)Core.Enums.Ports.Login)).Start();

            //SHOW THE UPDATER RATES
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Format version is: " + AuthConfig.Format.ToString());
            Console.WriteLine("Launcher version is: " + AuthConfig.Launcher.ToString());
            Console.WriteLine("Updater version is: " + AuthConfig.Updater.ToString());
            Console.WriteLine("Client version is: " + AuthConfig.Client.ToString());
            Console.WriteLine("Sub version is: " + AuthConfig.Sub.ToString());
            Console.WriteLine("Option version is: " + AuthConfig.Option.ToString());
            Console.WriteLine("Download URL is: " + AuthConfig.URL);
            Console.ForegroundColor = ConsoleColor.Gray;


            while (isRunning) {
                Console.Title = string.Format("「Authentication」Players: {0} | Peak: {1} | Total: {2}", onlinePlayers, playerPeak, totalPlayers);
                // TODO: Update the console title + basic queries.
                Thread.Sleep(1000);
            }
        }
    }
}
