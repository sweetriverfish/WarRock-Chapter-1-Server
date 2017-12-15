namespace Game.Handlers.Game.Ingame {
    class ConfirmSpawn : Networking.GameDataHandler {
        protected override void Handle() {
            if (Room.State == Enums.RoomState.Playing) {
                Player.RoundReady();
            }
        }
    }
}
