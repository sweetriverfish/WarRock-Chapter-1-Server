namespace Game.Packets
{
    class Test : Core.Networking.OutPacket
    {
        public Test(Entities.Room Room)
            : base((ushort)Enums.Packets.GamePacket)
        {
            Append(1);
            Append(0);
            Append(0);
            Append(2);
            Append(51);
            Append(1);
            Append(0);
            Append(Room.Map);
            Append(0);
            Append(0);
            Append(0);
            Append(0);
            Append(0);
            Append(0);
            Append(0);

        }
    }
}
