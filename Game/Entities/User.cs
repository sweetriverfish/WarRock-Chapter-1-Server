using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

// DEBUG //
using System.Diagnostics;

using System.Linq;
using System.Collections;

using Core.Networking;
using Game.Objects;
using Game.Objects.Inventory;
using Game.Managers;
using Game.Enums;

namespace Game.Entities {
    public class User : Core.Entities.Entity {

        #region Connection Variables
        private Socket socket;
        private byte[] buffer = new byte[1024];
        private byte[] cacheBuffer = new byte[0];
        private bool isDisconnect = false;
        private uint packetCount = 0;
        #endregion

        #region Lobby Variables

        public Entities.Room Room { get; private set; }
        public Premium Premium { get; private set; }
        public ulong PremiumExpireDate { get; private set; }
        public long PremiumTimeInSeconds { get; private set; }
        public uint Kills { get; set; }
        public uint Headshots { get; set; }
        public uint Deaths { get; set; }
        public ulong XP { get; private set; }
        public uint Money { get; set; }
        public Inventory Inventory { get; private set; }
        public int LastRoomId { get; set; }
        public byte RoomSlot { get; private set; }
        public byte RoomListPage { get; set; }
        public int SpectatorId { get; private set; }
        public byte UserListPage { get; set; } //the userlist page the player is viewing
        public uint Ping { get; private set; }
        public int ClanId { get; private set; }
        public int ClanRank { get; private set; }

        private object pingLock = new object();
        private bool pingOk = true;
        private DateTime lastPingTime = DateTime.Now;

        public IPEndPoint RemoteEndPoint;
        public IPEndPoint LocalEndPoint;

        public ushort RemotePort;
        public ushort LocalPort;

        #endregion

        #region Information Variables

        public uint BombsPlanted = 0;
        public uint BombsDefused = 0;
        public uint RoundsPlayed = 0;
        public uint FlagsTaken = 0;
        public uint Wins = 0;
        public uint Losses = 0;
        public uint VehiclesDestroyed = 0;

        #endregion

        public User(Socket socket)
            : base(0, "Unknown", "Unknown") {
            this.socket = socket;
            this.Room = null;
            this.Ping = 0;
            this.LastRoomId = -1;

            isDisconnect = false;

            this.socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnDataReceived), null);
            Send(new Core.Packets.Connection(Core.Constants.xOrKeySend));
        }

        public static void WriteLine(string logText)
        {
            DateTime _DTN = DateTime.Now;
            StackFrame _SF = new StackTrace().GetFrame(2);
            Console.Write("[" + _DTN.ToLongTimeString() + ":" + _DTN.Millisecond.ToString() + "] [");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(_SF.GetMethod().ReflectedType.Name + "." + _SF.GetMethod().Name);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] » ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Log.Instance.WriteLine(logText);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void OnAuthorize(uint id, string name, string displayname, byte _accessLevel) {
            this.ID = id;
            this.Name = name;
            this.Displayname = displayname;
            this.AccessLevel = _accessLevel;

            // LOAD PLAYER INFORMATION //
            if (!Load()) {
                Send(new Packets.Authorization(Packets.Authorization.ErrorCodes.BadSynchronization));
                return;
            }

            // LOAD INVENTORY //
            Inventory = new Inventory(this);
            Inventory.Load();

            this.Authorized = true;
            this.RoomListPage = 0;

            // Send the packet back //
            Send(new Packets.Authorization(this));
            SendPing();

            if (Inventory.ExpiredItems.Count > 0) {
                this.Send(new Packets.UpdateInventory(this));
                Inventory.ExpiredItems.Clear();
            }

            // Creating session //
            Databases.Game.Query(string.Concat("INSERT INTO sessions (`userid`, `sessionid`, `expired`, `session_start`, `server`) VALUES ('", ID, "', '", SessionID, "', '0', '", System.DateTime.Now.ToString("yyyyMMddHHmmss"),"', '", Game.Config.SERVER_NAME, "');"));

            //Updating the userlist
             this.UserListPage = 0;
           
            ArrayList UserList = new ArrayList();

            foreach (Entities.User User in Managers.UserManager.Instance.Sessions.Values)
                UserList.Add(User);

         
            foreach(Entities.User InLobby in UserList)
            {
                if (InLobby.Room == null)
                    InLobby.Send(new Packets.UserList(InLobby.UserListPage, UserList));
            }

         //   string _welcomeMessage1 = Cristina.Core.Cristina.Localization.GetLocMessageFrom("PLAYER_WELCOME");
           // string _welcomeMessage2 = Cristina.Core.Cristina.Localization.GetLocMessageFrom("CRISTINA_VERSION");

         //   if (_welcomeMessage1.Contains("%/nickname/%"))
           //    _welcomeMessage1 = _welcomeMessage1.Replace("%/nickname/%", Displayname);

         //   if (_welcomeMessage2.Contains("%/version/%"))
           //    _welcomeMessage2 = _welcomeMessage2.Replace("%/version/%", Cristina.Core.Cristina.Version.ToString());

            Cristina.Core.Cristina.Chat.SayToPlayer("Hola " + Displayname + ", soy el asistente personal del servidor", this);
            Cristina.Core.Cristina.Chat.SayToPlayer("Mi version es la 0.4", this);

            //Logging
            ServerLogger.Instance.Append(ServerLogger.AlertLevel.Information, String.Format("The player {0} logged on", Displayname));

        }

        private bool Load() {
            MySqlDataReader result = Databases.Game.Select(
                 new string[] { "kills", "deaths", "headshots", "xp", "money", "premium", "premium_expiredate", "play_time", "rounds_played", "bombs_planted", "bombs_defused", "clanid", "clanrank", "flags_taken", "wins", "losses", "vehicles_destroyed" },
                 "user_details",
                 new Dictionary<string, object>()
                            {
                                { "id", this.ID },
                            }
                 );

            if (result == null) return false;
            if (result.HasRows && result.Read()) {
                try {
                    this.Kills = result.GetUInt32(0);
                    this.Deaths = result.GetUInt32(1);
                    this.Headshots = result.GetUInt32(2);
                    this.XP = result.GetUInt64(3);
                    this.Money = result.GetUInt32(4);
                    this.Premium = (Premium)result.GetByte(5);
                    this.PremiumExpireDate = result.GetUInt64(6);

                    this.RoundsPlayed = result.GetUInt32("rounds_played");
                    this.BombsPlanted = result.GetUInt32("bombs_planted");
                    this.BombsDefused = result.GetUInt32("bombs_defused");
                    this.ClanId = result.GetInt32("clanid");
                    this.ClanRank = result.GetInt32("clanrank");
                    this.FlagsTaken = result.GetUInt32("flags_taken");
                    this.Wins = result.GetUInt32("wins");
                    this.Losses = result.GetUInt32("losses");
                    this.VehiclesDestroyed = result.GetUInt32("vehicles_destroyed");

                    result.Close();
                    return true;
                } catch { result.Close(); return false; }
            } else {
                result.Close();
                string query = string.Concat("INSERT INTO user_details (`id`, `kills`, `deaths`, `headshots`, `xp`, `money`, `premium`, `premium_expiredate`, `play_time`, `rounds_played`, `bombs_planted`, `bombs_defused`, `clanid`, `clanrank`, `flags_taken`, `wins`, `losses`, `vehicles_destroyed`) VALUES ('", this.ID, "', '0', '0', '0', '0', '50000', '0', '0', '0', '0','0','0','0','0','0','0','0','0');");
                Databases.Game.Query(query);
                return Load();
            }
        }

        public void SetRoom(Room room, byte slot) {
            this.Room = room;
            this.RoomSlot = slot;
            this.RoomListPage = 0;

            if (room != null)
                LastRoomId = (int)room.ID;
        }

        public void SetRoomSpectator(Room room, int _id)
        {
            this.Room = room;
            this.SpectatorId = _id;

            if (room != null)
                LastRoomId = (int)room.ID;
        }

        public void StopSpectate()
        {
            this.SpectatorId = -1;
            this.Room = null;

        }

        public void SetChannel(ChannelType type) {
            if (Channel != type) {
                ChannelManager.Instance.Remove(Channel, this); // Remove from old
                Channel = type; // change
                RoomListPage = 0;

                if (!ChannelManager.Instance.Add(Channel, this))
                    this.Disconnect(); // Failed to join :'(
            }
        }

        public void EndGame(long moneyEarned, long xpEarned) {
            // Store old XP.
            ulong oldXP = this.XP;
            if (((long)oldXP + xpEarned) > 0)
                if (xpEarned < 0)
                    XP -= (ulong)Math.Abs(xpEarned);
                else
                    XP += (ulong)xpEarned;
            else
                XP = 0;

            if (((long)Money + moneyEarned) > 0)
                if (moneyEarned < 0)
                    Money -= (uint)Math.Abs(moneyEarned);
                else
                    Money += (uint)moneyEarned;
            else
                Money = 0;

            // Detect level changes.
            byte oldLevel = Core.LevelCalculator.GetLevelforExp(oldXP);
            byte currentLevel = Core.LevelCalculator.GetLevelforExp(XP);
            if (currentLevel > oldLevel) { // Gained a level or more, send level up packet.
                // Calculate the diffrence.
                byte levelsGained = (byte)(currentLevel - oldLevel);
                uint lvlMoneyEarned = levelsGained * GameConfig.LevelUpMoneyReward;

                // Apply the Money & Send packet.
                Money += lvlMoneyEarned;
                uint nowTimeStamp = (uint)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                Send(new Packets.LevelUp(this, lvlMoneyEarned));

                // Log query
                Databases.Game.Insert("game_levels_gained", new Dictionary<string, object>() {
                    { "user_id", this.ID },
                    { "game_id", this.Displayname },
                    { "current_level", currentLevel},
                    { "levels_gained", levelsGained },
                    { "timestamp", nowTimeStamp }
                });
            }

            // Money update
            Databases.Game.AsyncQuery("UPDATE user_details SET money=" + this.Money + " WHERE id=" + this.ID);

        }

        private void UpdatePremiumState()
        {
            if (PremiumExpireDate > 0 || Premium != Enums.Premium.Free2Play)
            {
                uint currentTimestamp = (uint)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                PremiumTimeInSeconds = (long)(PremiumExpireDate - currentTimestamp);
                if (PremiumTimeInSeconds <= 0) { // The Premium expired.
                    PremiumTimeInSeconds = 0;
                    PremiumExpireDate = 0;
                    Premium = Enums.Premium.Free2Play;
                    // Execute a database query to make sure it's updated.
                    Databases.Game.AsyncQuery("UPDATE user_details SET premium=" + (byte)Premium + ", premium_expiredate=" + PremiumExpireDate + " WHERE id=" + this.ID);
                }
            } else  {
                PremiumTimeInSeconds = 0;
                Premium = Enums.Premium.Free2Play;
            }
        }

        public void SendPing()
        {
            lock (pingLock)
            {
                if (!pingOk)
                {
                    //Disconnect();
                    ServerLogger.Instance.Append(ServerLogger.AlertLevel.ServerError, "Could not send ping to player" +  Displayname);
                    return;
                }

                UpdatePremiumState();

                pingOk = false;
                Send(new Packets.Ping(this));
            }
        }

        public void PingReceived()
        {
            lock (pingLock)
            {
                this.pingOk = true;
                TimeSpan pingDiff = DateTime.Now - this.lastPingTime;
                this.Ping = (uint)pingDiff.TotalMilliseconds;
            }
        }

        public void SetSession(uint sessionId) {
            if (sessionId > 0) {
                SessionID = sessionId;
            } else {
                this.Disconnect();
            }
        }

        private void OnDataReceived(IAsyncResult iAr) {
            try {

                int bytesReceived = socket.EndReceive(iAr);

                if (bytesReceived > 0)
                {
                    byte[] packetBuffer = new byte[bytesReceived];

                    // Decrypt the bytes with the xOrKey.
                    for (int i = 0; i < bytesReceived; i++) {
                        packetBuffer[i] = (byte)(this.buffer[i] ^ Core.Constants.xOrKeyReceive);
                    }

                    int oldLength = cacheBuffer.Length;
                    Array.Resize(ref cacheBuffer, oldLength + bytesReceived);
                    Array.Copy(packetBuffer, 0, cacheBuffer, oldLength, packetBuffer.Length);

                    int startIndex = 0; // Determs where the bytes should split
                    for (int i = 0; i < cacheBuffer.Length; i++) { // loop trough our cached buffer.
                        if (cacheBuffer[i] == 0x0A) { // Found a complete packet
                            byte[] newPacket = new byte[i - startIndex]; // determ the new packet size.
                            for (int j = 0; j < (i - startIndex); j++) {
                                newPacket[j] = cacheBuffer[startIndex + j]; // copy the buffer to the buffer of the new packet.
                            }
                            packetCount++;
                            // Instant handeling
                            InPacket inPacket = new InPacket(newPacket, this);
                            ServerLogger.Instance.AppendPacket(newPacket);
                            if (inPacket != null)
                            {
                                if (inPacket.Id > 0)
                                {
                                    Networking.PacketHandler pHandler = Managers.PacketManager.Instance.FindExternal(inPacket);
                                    if (pHandler != null)
                                    {
                                        try
                                        {
                                            pHandler.Handle(inPacket);
                                        }
                                        catch (Exception e) { Log.Instance.WriteError(e.ToString()); }
                                    }
                                }
                            }
                            // Increase start index.
                            startIndex = i + 1;
                        }
                    }

                    if (startIndex > 0) {
                        byte[] fullCopy = cacheBuffer;
                        Array.Resize(ref cacheBuffer, (cacheBuffer.Length - startIndex));
                        for (int i = 0; i < (cacheBuffer.Length - startIndex); i++) {
                            cacheBuffer[i] = fullCopy[startIndex + i];
                        }
                        fullCopy = null;
                    }
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnDataReceived), null);
                } else {
                    Disconnect();
                }
            } catch {
                Disconnect();
            }
        }

        public void Send(byte[] sendBuffer) {
            try {
                socket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            } catch {
                Disconnect();
            }
        }

        public void Send(Core.Networking.OutPacket outPacket) {
            try {
                byte[] sendBuffer = outPacket.BuildEncrypted();
                socket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            } catch {
                Disconnect();
            }
        }

        private void SendCallback(IAsyncResult iAr) {
            try {
                socket.EndSend(iAr);
            } catch {
                Disconnect();
            }
        }

        public void Disconnect() {
            if (isDisconnect) return;
            isDisconnect = true;


            if (Channel > ChannelType.None)
                ChannelManager.Instance.Remove(Channel, this);

            if (SessionID > 0)
                UserManager.Instance.Remove(SessionID);



            try { socket.Close(); } catch { }

             //Updating the userlist     
            ArrayList UserList = new ArrayList();

            foreach (Entities.User User in Managers.UserManager.Instance.Sessions.Values)
                UserList.Add(User);
      
            foreach(Entities.User InLobby in UserList)
            {
                if (InLobby.Room == null)
                    InLobby.Send(new Packets.UserList(InLobby.UserListPage, UserList));
            }
        
        }

        public void changeAccessLevel(byte _newLevel)
        {
            AccessLevel = _newLevel;
        }

        public ChannelType Channel { get; private set; }
        public uint SessionID { get; private set; }
        public bool Authorized { get; private set; }
    }
}
