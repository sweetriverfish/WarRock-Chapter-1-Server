using System;
using Game.Entities;

namespace Game.Handlers
{
    class CMDFindPlayer : Networking.PacketHandler
    {
        protected override void Process(User u)
        {
            if (u.Authorized)
            {
                string _targetName = GetString(0);

                Entities.User Target = Managers.UserManager.Instance.GetUser(_targetName);
                    u.Send(new Packets.CMDFindPlayer(Target));
            }
            else
                u.Disconnect();
        }
    }
}
