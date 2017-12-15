using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Authorization.Handlers {
    class Nickname : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            if (u.Authorized) {
                string nickname = GetString(0);
                if (nickname.Length >= 3 && Core.Constants.isAlphaNumeric(nickname)) {
                    if (nickname.Length <= 16) {
                        try {
                            MySqlDataReader reader = Databases.Auth.Select(
                                new string[] { "id" },
                                "users",
                                new Dictionary<string, object>() {
                                    { "displayname", nickname }
                                });

                            if (!reader.HasRows) { // TODO: is the nickname allowed?
                                reader.Close();
                                Databases.Auth.Query(string.Concat("UPDATE users SET `displayname` ='", nickname ,"' WHERE id=", u.ID ,";"));
                                u.UpdateDisplayname(nickname);
                                u.Send(new Packets.ServerList(u));
                                u.Disconnect();
                            } else {
                                reader.Close();
                                u.Send(new Packets.ServerList(Packets.ServerList.ErrorCodes.NicknameTaken));
                            }

                        } catch { u.Disconnect(); }
                    } else {
                        u.Send(new Packets.ServerList(Packets.ServerList.ErrorCodes.NewNickname));
                    }
                } else {
                    u.Send(new Packets.ServerList(Packets.ServerList.ErrorCodes.NewNickname));
                }
            } else {
                u.Disconnect(); // Not authorized, cheating!
            }
        }
    }
}
