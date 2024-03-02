using Authorization.Entities;
using Core.Networking;

namespace Authorization.Handlers
{
    public class Launcher : Networking.PacketHandler
    {
        protected override void Process(User sender)
        {
            Log.Instance.WriteDev(string.Format("Patch info request from {0}", sender.RemoteEndIP));
            sender.Send(new Packets.Launcher());
            sender.Disconnect();
        }
    }
}
