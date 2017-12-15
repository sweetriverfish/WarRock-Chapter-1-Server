using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

/*
    CONFIGURATION DATA NOT STORED IN AN INI FILE BUT IN THE DB
*/
namespace Game
{
    class GameConfig
    {

        public static double DinarRate = 1.0;
        public static double ExpRate = 1.0;
        public static uint MaxRoomCount = 10;
        public static uint MaxTeamDifference = 1;
        public static uint LevelUpMoneyReward = 25000u;
        public static long BombTime = 45000;
        public static long ExplosiveRoundTime = 180000;


        //Damage multipliers
        public static double HeadMultiplier = 1.0;
        public static double TorsoMultiplier = 1.0;
        public static double LowerLimbsMultiplier = 1.0;
        public static double SniperBoneMultiplier = 1.0;

        public static bool Read()
        {

                MySqlCommand Cmd = new MySqlCommand("SELECT gameconfig.*, damage_multipliers.* FROM gameconfig, damage_multipliers", Databases.Game.connection);
                MySqlDataReader Reader = Cmd.ExecuteReader();

                try
                {
                    if (Reader.HasRows)
                    {
                        while (Reader.Read())
                        {
                            DinarRate = Reader.GetDouble("dinarrate");
                            ExpRate = Reader.GetDouble("exprate");
                            MaxRoomCount = Reader.GetUInt16("max_room_count");
                            MaxTeamDifference = Reader.GetUInt16("max_team_difference");
                            LevelUpMoneyReward = Reader.GetUInt16("levelup_base_reward");
                            BombTime = Reader.GetInt64("bombtime");
                            ExplosiveRoundTime = Reader.GetInt64("explosivetime");


                            HeadMultiplier = Reader.GetDouble("head");
                            TorsoMultiplier = Reader.GetDouble("torso");
                            LowerLimbsMultiplier = Reader.GetDouble("lower_limbs");
                            SniperBoneMultiplier = Reader.GetDouble("sniperbone");
                        }
                        
                    }
                    Reader.Close();

                  return true;
                }
                catch { return false; }
        }
    }
}
