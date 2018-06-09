using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Game.Handlers {
    class RequestServerTime : Networking.PacketHandler {
        protected override void Process(Entities.User u) {

            int versionId = GetInt(1);
            string MACAdress = GetString(2);

            if (versionId == 3) //valid version... TODO: Useable?
            {
                if (MACAdress.Length == 12 && Core.Constants.isAlphaNumeric(MACAdress)) //Valid MAC address
                {

                    MACAdress = String.Concat("'", MACAdress, "'");
                    string aQuery = String.Concat("SELECT macaddress FROM macaddress_blacklist WHERE macaddress = ", MACAdress, ";");

                    MySqlCommand cmd = new MySqlCommand(aQuery, Databases.Game.connection);
                    MySqlDataReader Reader = cmd.ExecuteReader();

                    if (Reader.HasRows) //banned HW 
                    {
                        u.Send(new Packets.Authorization(Packets.Authorization.ErrorCodes.NotAccessible)); //should change client string to be "Mac banned"
                    }
                    else { u.Send(new Packets.ServerTime()); }
                  
                        Reader.Close();
                }

                else
                {
                    u.Send(new Packets.Authorization(Packets.Authorization.ErrorCodes.NormalProcedure));
                }
            }
            else {
                u.Send(new Packets.ServerTime(Packets.ServerTime.ErrorCodes.DiffrentClientVersion));
            }
        }
    }
}
