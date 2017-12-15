namespace Game.Enums {
    public enum RoomJoinErrors : uint {
        GenericError = 94010,
        InvalidPassword = 94030,
        UsersExceeded = 94060,
        MaxSpectators = 94070,
        NotJoinDurningDataLoading = 94100,
        CalculatingResults = 94110,
        RoomIsFull = 94120,
        BadLevel = 94300,
        OnlyPremium = 94301
    }
}
