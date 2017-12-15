using System;
using System.Net;
using System.Net.Sockets;

namespace Game.Networking {
    class ServerListener {

        private readonly int bindPort;

        private Socket socket;

        public ServerListener(int port) {
            this.bindPort = port;
        }

        public bool Start() {
            try {
                Log.Instance.WriteLine(string.Concat("Binding a socket listener to port: ", this.bindPort, "."));
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Any, this.bindPort));
                socket.Listen(1);
                socket.BeginAccept(new AsyncCallback(this.OnAcceptConnection), null);
                Log.Instance.WriteLine("The socket is successfully binded to the port!");
                return true;
            }
            catch {
                Log.Instance.WriteLine("Failed to bind a network socket to the port.");
                Log.Instance.WriteLine("Is a server already running on this port?");
                return false;
            }
        }

        private void OnAcceptConnection(IAsyncResult iAr) {
            try {
                Socket s = socket.EndAccept(iAr);
                Entities.User usr = new Entities.User(s);
            }
            catch { }

            if (socket != null)
                socket.BeginAccept(new AsyncCallback(this.OnAcceptConnection), null);
        }
    }
}
