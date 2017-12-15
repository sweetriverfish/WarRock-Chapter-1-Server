namespace Game.Packets
{
    class RoomKick : Core.Networking.OutPacket
    {
        public RoomKick(int _playerSeat)
            : base((ushort)Enums.Packets.RoomKick)
        {
            Append(1);
            Append(_playerSeat);
        }
    }
}
