using System;
using System.Collections;


namespace Game.Handlers
{
    class UserList : Networking.PacketHandler
    {
        protected override void Process(Entities.User u)
        {
            if (!u.Authorized)
                return;

            ArrayList List = new ArrayList();
            foreach (Entities.User user in Managers.UserManager.Instance.Sessions.Values)
                List.Add(user);

            u.UserListPage = GetByte(0);
            u.Send(new Packets.UserList(u.UserListPage, List));
        }
    }
}
