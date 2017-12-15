using System;
using System.Text;
using System.IO;


//TODO: ROOM SPECIFIC LOG...
//TODO: APPEND PACKETS FOR OUT PACKETS

namespace Game
{
    public sealed class ServerLogger
    {
      //  private object ServerLogLock = null;
        private static string FileName = "console_mp.txt"; //COD4 4ever in my memory
        private static string INPacketFile = "Packets.txt";

        public enum AlertLevel : byte
        {
          Information = 0,
          BugWarning,
          LagIssue,
          Cheating,
          Unknown,
          ServerError,
          Gaming,
        }

        public ServerLogger()
        {
        }

        public void Append(string _buffer)
        {
            //If the logger is disabled then there is nothing to do here :)
            // All glory to the good-ol days of openwarfaremod.com :'(
            if (Config.SERVER_LOGGER == 0)
                return;

            string _path = Path.Combine(Environment.CurrentDirectory, FileName);
           
             //Formatting buffer to include necessary headers
            _buffer = LogBuilder(AlertLevel.Information, _buffer);

            try
            { 
                File.AppendAllText(_path, _buffer);
            }
            catch(PathTooLongException e)
            {
                Console.WriteLine("Could not write to the log file... executable path too long");
            }
           
        }
        
        public void Append(AlertLevel Level, string _buffer)
        {
        
            if (Config.SERVER_LOGGER == 0)
                return;

            string _path = Path.Combine(Environment.CurrentDirectory, FileName);
            _buffer = LogBuilder(Level, _buffer);
 
            try
            {
                File.AppendAllText(_path, _buffer);
            }
            catch (PathTooLongException e)
            {
               Console.WriteLine("Could not write to the log file... executable path too long");
            }

        }

        public void AppendPacket(byte[] _inBuffer)
        {
            if (Config.PACKET_LOGGER == 0)
                return;
            
            string _path = Path.Combine(Environment.CurrentDirectory, INPacketFile);

            string _fullPacket = Encoding.UTF8.GetString(_inBuffer);
            _fullPacket.Remove(_fullPacket.Length - 1);

            string _date = System.DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]");

            _fullPacket = String.Concat(_date, "---", _fullPacket, Environment.NewLine);


            try { File.AppendAllText(_path, _fullPacket); }

            catch (PathTooLongException e)
            {
                Console.WriteLine("Could not write to the log file... executable path too long");
            }
            
        }

        private string LogBuilder(AlertLevel Level, string _incomingData)
        {
            string _finalBuffer = String.Empty;

            //getting Date information
            string _time = System.DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]");

            //adding warning level
            string _warningLevel = String.Empty;
           
            switch(Level)
            {
                case AlertLevel.Information:
                    _warningLevel = "[Event]";
                    break;
                case AlertLevel.LagIssue:
                    _warningLevel = "[LAG]";
                    break;
                case AlertLevel.BugWarning:
                    _warningLevel = "[BUG]";
                    break;
                case AlertLevel.Cheating:
                    _warningLevel = "[CHEATING]";
                    break;
                    
                case AlertLevel.Unknown:
                    _warningLevel = "[Unknown Event]";
                    break;

                case AlertLevel.ServerError:
                    _warningLevel = "[SERVER ERROR]";
                    break;
                case AlertLevel.Gaming:
                    _warningLevel = "[Gaming]";
                    break;

                default:
                    _warningLevel = "[EVENT";
                    break;
            }

            _finalBuffer = string.Concat(_time, "-", _warningLevel, "-", _incomingData, Environment.NewLine);

            return _finalBuffer;
        }


        private static ServerLogger instance;
        public static ServerLogger Instance { get { if (instance == null) { instance = new ServerLogger(); } return instance; } }
    }
}
