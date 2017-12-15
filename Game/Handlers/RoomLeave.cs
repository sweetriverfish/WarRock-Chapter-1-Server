using System;
using System.Linq;
using System.Collections;

namespace Game.Handlers {
    class RoomLeave : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            if (u.Authorized) {
                if (u.Room != null) {
                    Entities.Room r = u.Room;
                    r.Remove(u);

                    if (r != null && r.Players.Count > 0)
                    {
                        // SEND THE ROOM UPDATE TO THE LOBBY //
                        byte roomPage = (byte)Math.Floor((decimal)(r.ID / 8));
                        var targetList = Managers.ChannelManager.Instance.Get(r.Channel).Users.Select(n => n.Value).Where(n => n.RoomListPage == roomPage && n.Room == null);
                        if (targetList.Count() > 0) {
                            byte[] outBuffer = new Packets.RoomUpdate(r, false).BuildEncrypted();
                            foreach (Entities.User usr in targetList)
                                usr.Send(outBuffer);                    
                        }
                    }
                    //this player needs an update of the userlist
                    ArrayList UserList = new ArrayList();
                    foreach (Entities.User User in Managers.UserManager.Instance.Sessions.Values)
                        UserList.Add(User);

                    u.UserListPage = 0;
                    u.Send(new Packets.UserList(0, UserList));
                }
            } else {
                u.Disconnect();
            }
        }
    }
}
