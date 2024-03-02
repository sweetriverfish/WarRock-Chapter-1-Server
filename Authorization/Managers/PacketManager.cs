using System;
using System.Collections;

using Authorization.Networking;

namespace Authorization.Managers {
    class PacketManager {

        private Hashtable _externalPacketList = new Hashtable();
        private Hashtable _internalPacketList = new Hashtable();

        public PacketManager() {
            _internalPacketList.Clear();
            _externalPacketList.Clear();

            // Internal Packets//
            AddInternal(Core.Enums.InternalPackets.Authorization, new Handlers.Internal.Authorization());
            AddInternal(Core.Enums.InternalPackets.Ping, new Handlers.Internal.Ping());
            AddInternal(Core.Enums.InternalPackets.PlayerAuthorization, new Handlers.Internal.PlayerAuthorization());

            // External Packets //
            AddExternal(Enums.Packets.ServerList, new Handlers.PlayerLogin());
            AddExternal(Enums.Packets.Nickname, new Handlers.Nickname());
            AddExternal(Enums.Packets.Launcher, new Handlers.Launcher());
        }

        private void AddInternal(Core.Enums.InternalPackets packetType, PacketHandler handler) {
            if (!_internalPacketList.ContainsKey(packetType)) {
                _internalPacketList.Add((ushort)packetType, handler);
            }
        }

        public PacketHandler FindInternal(Core.Networking.InPacket inPacket) {
            if (_internalPacketList.ContainsKey(inPacket.Id)) {
                return (Networking.PacketHandler)_internalPacketList[inPacket.Id];
            } else {
                Console.WriteLine("UNKNOWN PACKET :: " + inPacket.fullPacket.Remove(inPacket.fullPacket.Length-1));
            }
            return null;
        }

        private void AddExternal(Enums.Packets packetType, PacketHandler handler) {
            if (!_externalPacketList.ContainsKey(packetType)) {
                _externalPacketList.Add((ushort)packetType, handler);
            }
        }

        public PacketHandler FindExternal(Core.Networking.InPacket inPacket) {
            if (_externalPacketList.ContainsKey(inPacket.Id)) {
                return (Networking.PacketHandler)_externalPacketList[inPacket.Id];
            } else {
                Console.WriteLine("UNKNOWN PACKET :: " + inPacket.fullPacket.Remove(inPacket.fullPacket.Length - 1));
            }
            return null;
        }

        private static PacketManager instance;
        public static PacketManager Instance { get { if (instance == null) { instance = new PacketManager(); } return instance; } }
    }
}
