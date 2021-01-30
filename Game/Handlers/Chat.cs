using System;
using System.Text;
using Game.Enums;
using System.Linq;

namespace Game.Handlers {
    class Chat : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            if (u.Authorized)
            {
                byte type = GetByte(0);

                if (System.Enum.IsDefined(typeof(ChatType), type))
                {
                    Enums.ChatType chatType = (ChatType)type;
                    uint nowTimeStamp = (uint)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                    string message = GetString(3);
                    string realMessage = message.Split(new string[] { ">>" + Convert.ToChar(0x1D).ToString() }, StringSplitOptions.None)[1].Replace(Convert.ToChar(0x1D), Convert.ToChar(0x20));

                if (IsCommand(realMessage))
                    {
                        //removes /cris header
                        realMessage = realMessage.Remove(0, 6);

                        //get the command and the Arguments
                        string[] FullCommand = realMessage.Split(' ');

                        //TODO: PARSE FOR TARGET 
                        Objects.Command Command = Managers.CommandManager.Instance.GetCommand(FullCommand[0]);
                        if (Command != null)
                        {
                            //get the arguments...
                            string[] Arguments = new string[FullCommand.Length - 1];
                            Array.Copy(FullCommand, 1, Arguments, 0, (FullCommand.Length -1));

                            if(Arguments.Length < 32)
                            {
                                Command.SetArguments(Arguments);
                                Command.Process(u);
                            }
                           
                        }
                        else
                        {
                        string  _notFound = Cristina.Core.Cristina.Localization.GetLocMessageFrom("UNKNOWN_COMMAND");
                        Cristina.Core.Cristina.Chat.SayToPlayer(_notFound, u);
                        //Cristina.Core.Cristina.Chat.SayToPlayer("El comando no existe", u);
                        }
                    }
                    else
                    {
                        string targetName = GetString(2);
                        int targetId = GetInt(1);
                        if (realMessage.Length <= 60)
                        {
                            realMessage = realMessage.Trim();
                            switch (chatType)
                            {
                                case ChatType.Lobby_ToChannel:
                                    {
                                        if (u.Room == null)
                                        {
                                            Databases.Game.AsyncInsert("chat_public_lobby", new System.Collections.Generic.Dictionary<string, object>() { { "server", Config.SERVER_ID }, { "channel_id", (byte)u.Channel }, { "sender_id", u.ID }, { "target_all", 0 }, { "message", realMessage }, { "timestamp", nowTimeStamp } });
                                            Core.Networking.OutPacket p = new Packets.Chat(u, Enums.ChatType.Lobby_ToChannel, message, targetId, targetName);
                                            Managers.ChannelManager.Instance.SendLobby(u.Channel, p.BuildEncrypted());
                                        }
                                        else
                                        {
                                            u.Disconnect(); // Sending lobby messages when in a room?
                                        }
                                        break;
                                    }
                                case ChatType.Lobby_ToAll:
                                    {
                                        if (u.Room == null)
                                        {
                                            Databases.Game.AsyncInsert("chat_public_lobby", new System.Collections.Generic.Dictionary<string, object>() { { "server", Config.SERVER_ID }, { "channel_id", (byte)u.Channel }, { "sender_id", u.ID }, { "target_all", 1 }, { "message", realMessage }, { "timestamp", nowTimeStamp } });
                                            Core.Networking.OutPacket p = new Packets.Chat(u, chatType, message, targetId, targetName);
                                            Managers.ChannelManager.Instance.SendAllLobbies(p.BuildEncrypted());
                                        }
                                        else
                                        {
                                            u.Disconnect(); // Sending lobby messages when in a room?
                                        }
                                        break;
                                    }
                                case ChatType.Room_ToAll:
                                    {
                                        if (u.Room != null)
                                        {
                                            if (u.Room.State == RoomState.Waiting && u.RoomSlot == u.Room.Master)
                                            {
                                                if (u.Room.Supermaster)
                                                {
                                                    targetId = 998;
                                                }
                                            }
                                            Databases.Game.AsyncInsert("chat_public_room", new System.Collections.Generic.Dictionary<string, object>() { { "server", Config.SERVER_ID }, { "channel_id", (byte)u.Channel }, { "sender_id", u.ID }, { "room_id", u.Room.ID }, { "team_side", (byte)u.Room.Players[u.RoomSlot].Team }, { "target_all", 1 }, { "message", realMessage }, { "timestamp", nowTimeStamp } });
                                            Core.Networking.OutPacket p = new Packets.Chat(u, chatType, message, targetId, targetName);
                                            u.Room.Send(p.BuildEncrypted());
                                        }
                                        else
                                        {
                                            u.Disconnect();
                                        }
                                        break;
                                    }
                                case ChatType.Room_ToTeam:
                                    {
                                        if (u.Room != null)
                                        {
                                            if (u.Room.Mode != Mode.Free_For_All && u.Room.State == RoomState.Playing)
                                            {
                                                Databases.Game.AsyncInsert("chat_public_room", new System.Collections.Generic.Dictionary<string, object>() { { "server", Config.SERVER_ID }, { "channel_id", (byte)u.Channel }, { "sender_id", u.ID }, { "room_id", u.Room.ID }, { "team_side", (byte)u.Room.Players[u.RoomSlot].Team }, { "target_all", 0 }, { "message", realMessage }, { "timestamp", nowTimeStamp } });
                                                Core.Networking.OutPacket p = new Packets.Chat(u, chatType, message, targetId, targetName);
                                                byte[] buffer = p.BuildEncrypted();
                                                u.Room.Players.Values.Where(n => n.Team == u.Room.Players[u.RoomSlot].Team).ToList().ForEach(n => n.Send(buffer));
                                            }
                                            else
                                            {
                                                u.Disconnect(); // NO team CHAT IN FFA or in the lobby.
                                            }
                                        }
                                        break;
                                    }

                                case ChatType.Clan:
                                    {
                                        if (u.ClanId != -1)
                                        {
                                            Databases.Game.AsyncInsert("chat_private_clan", new System.Collections.Generic.Dictionary<string, object>() { { "server", Config.SERVER_ID }, { "clan_id", (byte)u.ClanId }, { "sender_id", u.ID }, { "message", realMessage }, { "timestamp", nowTimeStamp } });

                                            Core.Networking.OutPacket p = new Packets.Chat(u, chatType, message, targetId, targetName);
                                            foreach (Entities.User User in Managers.UserManager.Instance.Sessions.Values)
                                                User.Send(p.BuildEncrypted());

                                        }
                                        break;
                                    }
                                case ChatType.Whisper:
                                    {
                                        Entities.User Target = Managers.UserManager.Instance.GetUser(targetName);

                                        //fixes the lack of space between nick and "message sent" or "doesn´t exist" error code
                                        targetName = targetName + Convert.ToChar(0x1D);
                                        if (Target != null)
                                        {
                                            u.Send(new Packets.Chat(u, chatType, message, targetId, targetName)); //in this case targetId equals -1, that is "message sent"
                                            Target.Send(new Packets.Chat(u, chatType, message, (int)Target.SessionID, Target.Displayname).BuildEncrypted());
                                        }
                                        else
                                            u.Send(new Packets.Chat(95040, targetName).BuildEncrypted());
                                        break;
                                    }
                                default:
                                    {

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            u.Disconnect(); // Message is too long?
                        }
                    }
                }
            }
            else
            {
                u.Disconnect();
            }
        }

        public bool IsCommand(string _input)
        {
            if (_input.StartsWith("/cris"))
                return true;
            return false;
        }
    }
}
