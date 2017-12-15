namespace Game.Handlers {
    class Explosives : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            if (u.Authorized) {
                if (u.Room != null) {
                    if (u.Room.State == Enums.RoomState.Playing) {
                        u.Room.HandleExplosives(this.InPacket.Blocks, u);
                    } else {
                        u.Disconnect(); // Sending packets in lobby? Cheating!
                    }
                } else {
                    u.Disconnect(); // Player is not in a room, cheating?
                }
            } else {
                u.Disconnect(); // Player not authorized - cheating?
            }
        }
    }
}
