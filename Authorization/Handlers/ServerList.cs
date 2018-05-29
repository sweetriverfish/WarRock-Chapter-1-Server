using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Authorization.Handlers {
    class ServerList : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            string username = GetString(2);
            string password = GetString(3);
            bool forceDisconnect = true;

            if (username.Length >= 3 && Core.Constants.isAlphaNumeric(username)) {
                if (password.Length > 1) {
                    MySqlDataReader reader = Databases.Auth.Select(
                       new string[] { "id", "name", "password", "salt", "displayname", "accesslevel" },
                        "users",
                       new Dictionary<string, object>() {
                        { "name", username }
                    });

                    if (reader.HasRows && reader.Read()) {
                        uint id = reader.GetUInt32(0);
                        username = reader.GetString(1);
                        string dbPassword = reader.GetString(2);
                        string dbSalt = reader.GetString(3);
                        string displayname = reader.GetString(4);
                        byte _accessLevel = reader.GetByte(5);
                        string hashedPassword = Core.Constants.MD5(password);
                        string doubleHashedPassword = Core.Constants.MD5(hashedPassword);
                        string hashedSalt = Core.Constants.MD5(dbSalt);
                        string finalHash = Core.Constants.MD5(hashedPassword + hashedSalt + doubleHashedPassword);

                        if (password == dbPassword)
                        {
                            var IsOnline = Managers.SessionManager.Instance.Sessions.Select(n => n.Value).Where(n => n.ID == id && n.IsActivated && !n.IsEnded).Count();
                            if (IsOnline == 0) {

                                if (_accessLevel > 0)
                                {
                                    u.OnAuthorize(id, username, displayname, _accessLevel);

                                    if (displayname.Length > 0)
                                        u.Send(new Packets.ServerList(u));
                                    else
                                    {
                                        forceDisconnect = false;
                                        u.Send(new Packets.ServerList(Packets.ServerList.ErrorCodes.NewNickname));
                                    }
                                    ServerLogger.Instance.Append(ServerLogger.AlertLevel.Information, String.Concat("Player ", u.Displayname, " logged on"));
                                }
                                else
                                    u.Send(new Packets.ServerList(Packets.ServerList.ErrorCodes.Banned));
                                } else {
                                u.Send(new Packets.ServerList(Packets.ServerList.ErrorCodes.AlreadyLoggedIn));
                            }
                            // Made a valid authorization //
                        } else {
                            u.Send(new Packets.ServerList(Packets.ServerList.ErrorCodes.WrongPW));
                        }
                    } else {
                        u.Send(new Packets.ServerList(Packets.ServerList.ErrorCodes.WrongUser));
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                } else {
                    u.Send(new Packets.ServerList(Packets.ServerList.ErrorCodes.WrongPW));
                }
            } else {
                u.Send(new Packets.ServerList(Packets.ServerList.ErrorCodes.WrongUser));
            }

            if (forceDisconnect)
                u.Disconnect();
        }
    }
}
