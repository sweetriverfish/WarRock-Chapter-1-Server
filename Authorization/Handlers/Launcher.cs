namespace Authorization.Handlers {
    class Launcher : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            u.Send(new Packets.Launcher());
        }
    }
}
