using System;


namespace Game.Packets
{
    class RoomInvite : Core.Networking.OutPacket
    {
        public RoomInvite(Enums.RoomInviteErrors _errorCode)
            : base((ushort)Enums.Packets.RoomInvite)
        {
            Append((uint)_errorCode);
        }
        public RoomInvite(Entities.User User, string Message)
            : base((ushort)Enums.Packets.RoomInvite)
        {
            //29520 dark Let´s play a room together, come in!!!!

            Append(1);
            Append(0);
            Append(-1);
            Append(User.SessionID);
            Append(User.SessionID); // Ping ?!
            Append(User.Displayname);

            Objects.Clan UserClan = Managers.ClanManager.Instance.GetClan(User.ClanId);

            if (UserClan == null)
                Fill(4, -1);
            else
            {
                Append(User.ClanId);
                Append(UserClan.Tag);
                Append(User.ClanRank);
                Append(User.ClanRank);
            }

            Append(1);
            Append(18);
            Append(User.XP);
            Append(3);
            Append(0);
            Append(-1);
            Append(Message);
            Append(User.Room.ID);
            Append(User.Room.Password);
        }
    }
}
