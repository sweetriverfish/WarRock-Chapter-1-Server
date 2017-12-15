using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Game.Managers;

namespace Game.Handlers
{
    class RoomInvite : Networking.PacketHandler
    {
        protected override void Process(Entities.User u)
        {
            if (u.Authorized)
            {
                if (u.Room != null && u.Room.State == Enums.RoomState.Waiting)
                {
                    Entities.Room Room = u.Room;

                    if (Room.Players.Count < Room.MaximumPlayers)
                    {

                        List<Entities.User> ValidUsers = new List<Entities.User>();

                        foreach (Entities.User User in Managers.UserManager.Instance.Sessions.Values)
                        {
                            if (User.Room == null && User.Channel == u.Channel)
                                ValidUsers.Add(User);
                        }
                        string _target = GetString(0);
                        string _message = GetString(1);


                        if (_target == "NULL") //send invitation to random user
                        {

                            Random Random = new Random();

                            if (ValidUsers.Count < 1)
                                u.Send(new Packets.RoomInvite(Enums.RoomInviteErrors.GenericError));
                            else
                            {
                                Entities.User Target = ValidUsers[Random.Next(0, ValidUsers.Count)];
                                u.Send(new Packets.RoomInvite(Enums.RoomInviteErrors.Invited));
                                Target.Send(new Packets.RoomInvite(u, _message));
                            }
                        }
                        else
                        {
                            try
                            {
                                foreach (Entities.User pTarget in Managers.UserManager.Instance.Sessions.Values)
                                {
                                    if (pTarget.Displayname == _target)
                                    {
                                        if (pTarget.Room != null)
                                            u.Send(new Packets.RoomInvite(Enums.RoomInviteErrors.InRoom));

                                        if (pTarget.Room == null && pTarget.Channel == u.Channel)
                                            pTarget.Send(new Packets.RoomInvite(u, _message));

                                    }
                                }
                            }
                            catch
                            {
                                u.Send(new Packets.RoomInvite(Enums.RoomInviteErrors.GenericError));
                            }
                        }

                    }
                    else
                        Cristina.Core.Cristina.Chat.SayToPlayer("You cannot invite a player because the room is full", u);
                }
                else
                    u.Disconnect();
            }
            else
                u.Disconnect();
        }
    }
}
