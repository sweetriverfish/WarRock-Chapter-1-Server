namespace Game.Enums {
    public enum ChatType : byte {
        Notice1 = 1,
        Notice2,
        Lobby_ToChannel,
        Room_ToAll,
        Room_ToTeam,
        Whisper,
        Lobby_ToAll = 8,
        Clan = 10
    }
}
