using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Core.Networking;
using Core.Enums;
using Game.Networking;

namespace Game {
    class Program {
        private static bool isRunning = false;
        private static DateTime startTime;
        public static ServerClient AuthServer;
        private static uint serverLoops = 0;

        static void Main(string[] args) {
            startTime = DateTime.Now;

         

            Console.Title = "[Starting] Game Server";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@" _______        _______ _______  ______ _______ _______ _     _");
            Console.WriteLine(@" |_____| |         |    |______ |_____/ |______ |  |  | |     |");
            Console.WriteLine(@" |     | |_____    |    |______ |    \_ |______ |  |  | |_____|");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(new string('_', Console.WindowWidth));
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Developed by CodeDragon, DarkRaptor");
            Console.ForegroundColor = ConsoleColor.Gray;

            if (!Config.Read()) {
                Log.Instance.WriteError("Failed to load the configuration file.");
                Console.ReadKey();
                return;
            }

            ServerLogger.Instance.Append(ServerLogger.AlertLevel.Information, "-----------------------Server booted-------------------------");

            if (!Databases.Init()) {
                Log.Instance.WriteError("Failed to initilize all database connections.");
                Console.ReadKey();
                return;
            }

            if (!GameConfig.Read())
            {
                Log.Instance.WriteError("Failed to load the game configuration");
                Console.ReadKey();
                return;
            }

            if (!Managers.ItemManager.Instance.Load()) {
                Log.Instance.WriteError("Failed to initilize the item manager.");
                Console.ReadKey();
                return;
            }

            if (!Managers.VehicleManager.Instance.Load())
            {
                Log.Instance.WriteError("Failed to initialize the vehicle Manager.");
                Console.ReadKey();
                return;
            }

            if (!Managers.MapManager.Instance.Load()) {
                Log.Instance.WriteError("Failed to initilize the map manager.");
                Console.ReadKey();
                return;
            }

            if (!Managers.CouponManager.Instance.Load())
            {
                Log.Instance.WriteError("Failed to initialize the coupon manager.");
                Console.ReadKey();
                return;
            }

            if (!Managers.ClanManager.Instance.Load())
            {
                Log.Instance.WriteError("Failed to initialize the clan manager.");
                Console.ReadKey();
                return;
            }

            // CONNECT TO THE AUTHORIZATION SERVER //
            AuthServer = new ServerClient(Config.AUTH_SERVER_IP, (int)Ports.Internal);
            if (!AuthServer.Connect()) {
                return;
            }

            if (!new UDPListener((int)Ports.UDP1).Start()) {
                return;
            }

            if (!new UDPListener((int)Ports.UDP2).Start()) {
                return;
            }

            //SHOW THE CONFIGURATION RATES

             if(Config.SERVER_DEBUG == 1)
             {
                 Console.ForegroundColor = ConsoleColor.Red;
                 Console.WriteLine("SERVER IS RUNNING IN DEBUG MODE!!!");
             }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Dinar rate is: " + GameConfig.DinarRate.ToString());
            Console.WriteLine("EXP rate is: " + GameConfig.ExpRate.ToString());
            Console.WriteLine("Bomb time is " + GameConfig.BombTime.ToString());
            Console.WriteLine("Maximum team difference is " + GameConfig.MaxTeamDifference.ToString());
            Console.WriteLine("Maximum room count is " + GameConfig.MaxRoomCount.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;

            ServerLogger.Instance.Append("Server initialiced correctly");

            // Start up the listener :)
            isRunning = (new ServerListener((int)Ports.Game)).Start();

            if (isRunning)
            {
                TimeSpan loadTime = DateTime.Now - startTime;
                Log.Instance.WriteLine(string.Format("Emulator loaded in {0} milliseconds!", loadTime.TotalMilliseconds));
                ServerLogger.Instance.Append(ServerLogger.AlertLevel.Information, string.Format("Emulator loaded in {0} milliseconds!", loadTime.TotalMilliseconds));
            }

            startTime = DateTime.Now;
            while (isRunning) {

                TimeSpan runTime = DateTime.Now - startTime;
                Console.Title = string.Format("「Game Server」Uptime {0} | Players: {1} | Peak: {2} | Rooms: {3}", runTime.ToString(@"dd\:hh\:mm\:ss"), Managers.UserManager.Instance.Sessions.Values.Count, Managers.UserManager.Instance.Peak, Managers.ChannelManager.Instance.RoomCount);

                
                if(serverLoops % 5 == 0) {
                    Parallel.ForEach(Managers.UserManager.Instance.Sessions.Values, user => {
                        if (user.Authorized)
                            user.SendPing();
                    });
                   
                }

                //ping to auth  server
                AuthServer.Send(new Packets.Internal.Ping());

                serverLoops++;

                Thread.Sleep(1000);
            }
        }



        
    }
}
