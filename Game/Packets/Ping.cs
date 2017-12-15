using Game.Enums;

namespace Game.Packets
{
    class Ping : Core.Networking.OutPacket
    {

        public Ping(Entities.User u)
            : base((ushort)Enums.Packets.Ping)
        {
            Append(5000); // Ping frequency
            Append(u.Ping); // Ping
            Append(0);  // -1 = no evento  175 = evento de navidad
            Append(-1); // Duración del evento
            Append(4); // 3 exp weekend, 4 exp event, 0 = none
            Append(Game.GameConfig.ExpRate); // EXP Rate
            Append(Game.GameConfig.DinarRate); // Dinar Rate
            Append((u.PremiumTimeInSeconds > 0) ? u.PremiumTimeInSeconds : -1); // Premium Time
        }
    }
}
