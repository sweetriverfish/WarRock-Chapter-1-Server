namespace Game.Packets
{
    class CMDFindPlayer : Core.Networking.OutPacket
    {
        public CMDFindPlayer(Entities.User Target)
            :base((ushort)Enums.Packets.CMDFindPlayer)
        {
            if (Target == null)
                Append(0);
            else
            {
                Append(1);
                Append(Target.ID);
                Append(Target.RemoteEndPoint.ToString());
                Append((byte)Target.Premium);
                Append(Target.XP);
                Append(Target.Money);
                Append(Target.Kills);
                Append(Target.Deaths);
                Append(Target.Deaths);
                if (Target.Room != null)
                {
                    Append(Target.Room.ID);
                    Append(Target.Room.Name);
                    Append(Target.Room.Password);
                }
                else
                {
                    Fill(2, -1);
                }

                Append(Target.SessionID);
            }
          
        }
    }
}
