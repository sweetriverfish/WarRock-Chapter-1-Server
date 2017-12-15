namespace Core.Enums.Internal {
    public enum PlayerAuthorizationErrorCodes : ushort {
        Login = 1,
        Update = 2,
        Logout = 3,
        InvalidSession = 0x100, // Invalid session id
        IvalidMatch = 0x110, // Session Doesn't match
        SessionAlreadyActivated = 0x120 // The session is already activated
    }
}
