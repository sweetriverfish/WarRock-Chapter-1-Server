/* SHARED BY CODEDRAGON. DO NOT SHARE THIS */

using Game.Enums;
using System.Collections;

namespace Game.Packets
{
    class UserList : Core.Networking.OutPacket
    {

        public UserList(int _intPage, ArrayList _userList)
            : base(28928)
        {

            Append(_userList.Count);
            Append(_intPage);

            for (int i = 0; i < _userList.Count; i++)
            {
                Entities.User u = (Entities.User)_userList[i];
                Append(i + _intPage * 10); // List Index
                Append(u.ID); // UID
                Append(u.SessionID); // Session ID
                Append(u.Displayname); // Nickname

                Objects.Clan Clan = Managers.ClanManager.Instance.GetClan(u.ClanId);

                if (Clan == null)
                    Fill(4, -1);
                else
                {
                    Append(u.ClanId);
                    Append(Clan.Name);
                    Append(u.ClanRank);
                    Append(((u.ClanId > 0) ? 0 : -1)); // Unknown?
                }

                Append(0); // Unknown
                Append(16); // Unknown
                Append(u.XP);
                Append((byte)u.Premium);
                Append(0);

            }

        }
    }
}