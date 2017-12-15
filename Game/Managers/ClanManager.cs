using System;
using System.Collections.Concurrent;
using MySql.Data.MySqlClient;

namespace Game.Managers
{
    class ClanManager
    {

        public ConcurrentDictionary<uint, Objects.Clan> ClanList = new ConcurrentDictionary<uint, Objects.Clan>();

        public ClanManager()
        {

        }

        public bool Load()
        {
            bool _result = false;
            ConcurrentDictionary<uint, Objects.Clan> TempList = new ConcurrentDictionary<uint, Objects.Clan>();

            try
            {

                MySqlCommand CMD = new MySqlCommand("SELECT id, name,  members, tag, wins, losses, draws, master FROM clan WHERE disbanded = 0 AND validated = 1",  Databases.Game.connection);
                MySqlDataReader Reader = CMD.ExecuteReader();

                if (Reader.HasRows)
                {
                    while(Reader.Read())
                    {
                        uint _id         = Reader.GetUInt32("id");
                        string _name     = Reader.GetString("name");
                        string _tag      = Reader.GetString("tag");
                        uint _members    = Reader.GetUInt32("members");
                        uint _wins       = Reader.GetUInt32("wins");
                        uint _losses     = Reader.GetUInt32("losses");
                        uint _draws      = Reader.GetUInt32("draws");
                        uint _master     = Reader.GetUInt32("master");

                        Objects.Clan Clan = new Objects.Clan(_name, _tag, _id, _members, _wins, _losses, _draws, _master);
                        TempList.TryAdd(_id, Clan);
                    }
                }

                if (!Reader.IsClosed)
                    Reader.Close();

                ClanList = TempList;

                _result = true;
           }
      catch
           {
               ServerLogger.Instance.Append(ServerLogger.AlertLevel.ServerError, "Could not load clan table from DB");
           }
         
            return _result;
        }

        public Objects.Clan GetClan(int _clanId)
        {
            Objects.Clan UserClan = null;

            if(_clanId != -1) //-1 means NO clan
            {
                if (!ClanList.TryGetValue(Convert.ToUInt32(_clanId), out UserClan))
                    UserClan = QueueForClan(Convert.ToUInt32(_clanId));
                    
            }         
            return UserClan;
        }

        //Called when the server could not find a clan... maybe the list is outdated
       private Objects.Clan QueueForClan(uint _clanId)
        {
            Objects.Clan NewClan = null;
            ServerLogger.Instance.Append(ServerLogger.AlertLevel.Information, "Attempting to perform  a DB search for a missing clan");
            try
            {
                MySqlCommand CMD = new MySqlCommand("SELECT name, members, tag, wins, losses, draws, master FROM clan WHERE disbanded = 0 AND validated = 1 AND id = " 
                    + _clanId.ToString(), Databases.Game.connection);

                MySqlDataReader Reader = CMD.ExecuteReader();

                Objects.Clan TempClan = null;

                 if(Reader.HasRows)
                {
                    while(Reader.Read())
                    {
                        string _name = Reader.GetString("name");
                        string _tag = Reader.GetString("tag");
                        uint _members = Reader.GetUInt32("members");
                        uint _wins = Reader.GetUInt32("wins");
                        uint _losses = Reader.GetUInt32("losses");
                        uint _draws = Reader.GetUInt32("draws");
                        uint _master = Reader.GetUInt32("master");
                       TempClan = new Objects.Clan(_name, _tag, _clanId, _members, _wins, _losses, _draws, _master);
                    }
                    NewClan = TempClan;
                }
                 else
                    ServerLogger.Instance.Append(ServerLogger.AlertLevel.ServerError, "Could not find clan with id after performing a queue to DB" + _clanId.ToString());

                if (!Reader.IsClosed)
                    Reader.Close();
            }
            catch
            {
                ServerLogger.Instance.Append(ServerLogger.AlertLevel.ServerError, "Failed to perform a queue to update ClanList");
            }

            if (NewClan != null)
                ClanList.TryAdd(NewClan.Id, NewClan);

            ServerLogger.Instance.Append(ServerLogger.AlertLevel.Information, "A new clan has been found. Updating the Concurrent Dictionary...");

            return NewClan;
        }
        
        private static ClanManager instance;
        public static ClanManager Instance { get { if (instance == null) { instance = new ClanManager(); } return instance; } }
    }
}
