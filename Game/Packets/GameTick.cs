namespace Game.Packets {
    class GameTick : Core.Networking.OutPacket {
        public GameTick(Entities.Room r)
            : 
            base((ushort)Enums.Packets.GameTick) {
            Append(r.UpTick); // Spawn Counter
            Append(r.DownTick); // Time Left
            Append(r.CurrentGameMode.CurrentRoundTeamA());
            Append(r.CurrentGameMode.CurrentRoundTeamB());
            Append(r.CurrentGameMode.ScoreboardA());
            Append(r.CurrentGameMode.ScoreboardB());
            Append(2); // ?
            Append(0); // Conquest related
            Append(30); // Conquest related
        }
    }
}
