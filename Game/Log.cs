using System;
using System.IO;
//using System.Diagnostics;


namespace Game
{
    public class Log
    {
        private object logLock = null;
        public Log()
        {
            logLock = new object();   
        }

        public void WriteLog(ConsoleColor colorClass, ConsoleColor colorMessage, string message)
        {
            DateTime logTime = DateTime.Now;
            lock (logLock) // Blocks all other threads till this code is executed
            {
                Console.Write(logTime.ToString("[dd/MM/yy HH:mm:ss:fff]") + " [");
                Console.ForegroundColor = colorClass;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("] » ");
                Console.ForegroundColor = colorMessage;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public void WriteLine(string message)
        {
            this.WriteLog(ConsoleColor.Green, ConsoleColor.DarkGreen, message);
        }

        public void WriteError(string message)
        {
            this.WriteLog(ConsoleColor.Red, ConsoleColor.DarkRed, message);
            ServerLogger.Instance.Append(ServerLogger.AlertLevel.ServerError, message);
        }

        public void WriteDebug(string message)
        {
            this.WriteLog(ConsoleColor.Yellow, ConsoleColor.DarkYellow, message);
        }

        public void WriteDev(string message)
        {
            this.WriteLog(ConsoleColor.Cyan, ConsoleColor.DarkCyan, message);
        }

        /// <summary>
        /// Writes the message both to the console and to the log
        /// </summary>
        /// <param name="message"></param>
        public void WriteBoth(string message)
        {
            this.WriteLog(ConsoleColor.Magenta, ConsoleColor.DarkMagenta, message);
            ServerLogger.Instance.Append(ServerLogger.AlertLevel.Unknown, message);
        }

        private static Log instance;
        public static Log Instance { get { if (instance == null) { instance = new Log(); } return instance; } }
    }
}