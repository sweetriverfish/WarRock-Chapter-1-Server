namespace Game.Handlers.Game.Ingame
{
    class WeaponSwitch : Networking.GameDataHandler
    {
        protected override void Handle()
        {
            if (Room.State == Enums.RoomState.Playing)
            {
                Player.Weapon = GetUShort(2); // Get weapon
                respond = true;
            }
        }
    }
}