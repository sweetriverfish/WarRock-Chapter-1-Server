namespace Game.Handlers
{
    public class Ping : Networking.PacketHandler
    {
        protected override void Process(Entities.User u)
        {
            if (u.Authorized)
                u.PingReceived();
            
            else
                u.Disconnect(); // Player not authorized - cheating?     
        }
    }
}
