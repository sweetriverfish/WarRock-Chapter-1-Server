namespace Game.Handlers.Internal {
    class Connection : Networking.PacketHandler {
        protected override void Process(Networking.ServerClient s) {
            s.Send(new Packets.Internal.Authorization());
        }
    }
}
