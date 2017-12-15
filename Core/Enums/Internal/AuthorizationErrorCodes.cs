namespace Core.Enums.Internal {
    public enum AuthorizationErrorCodes : ushort{
        OK = 1,
        InvalidKey = 0x100,
        NameAlreadyUsed = 0x110,
        Duplicate = 0x120,
        MaxServersReached = 0x130
    }
}
