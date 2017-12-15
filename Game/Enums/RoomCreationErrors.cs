namespace Game.Enums {
    public enum RoomCreationErrors : uint {
        GenericError = 92010,           // Failed to create game room. Please try again later.
        ClanOnly = 93090,               // Only clan members can host a game room for Clan Battle.
        MaxiumRoomsExceeded = 94060,    // Cannot create a game room. Number of game rooms have exceeded.
        BadLevel = 94300
    }
}
