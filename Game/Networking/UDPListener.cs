/*
CMD_UDP_PEER_ADDR = 0x2100,
CMD_UDP_KEEPALIVE = 0x3000,
CMD_UDP_CHAR_MOVE = 0x3100,
CMD_UDP_CHAR_MOVE_A = 0x3104,
CMD_UDP_CHAR_MOVE_B = 0x3105,
CMD_UDP_CHAR_ROLL = 0x3101,
CMD_UDP_CHAR_ZOOMIN = 0x3102,
CMD_UDP_CHAR_EMOTION = 0x3103,
CMD_UDP_OBJ_MOVE = 0x3200,
CMD_UDP_VEHICLE_MOVE = 0x3201,
CMD_UDP_VEHICLE_MOVE_A = 0x3202,
CMD_UDP_VEHICLE_MOVE_B = 0x3203,
CMD_UDP_BULLET = 0x3400,
CMD_UDP_EXPLOSION = 0x3403,
CMD_UDP_DUMMY_INFO = 0x3401,
CMD_UDP_HACK_INFO = 0x3402,
CMD_UDP_TEXT_CHAT = 0x3600,

I've found my old list with packets, this is a list from a newer dump then chapter 1 tough but the ids should be the same.

These are the REAL UDP packet ids, you can get them by parsing an ushort inside the packet. I'm not sure where they are located tough. If I find more information about it then I'll let you know.

Please don't share this with the public because this might mean the end of the real game :'D

*/
using System;
using System.Net;
using System.Net.Sockets;

namespace Game.Networking
{
    class UDPListener
    {

        private int port;
        private Socket socket;
        private EndPoint endPoint;
        private byte[] buffer = new byte[1024];

        public UDPListener(int port)
        {
            this.port = port;
        }

        public bool Start()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                Log.Instance.WriteLine("Setting up the udp socket on port: " + port.ToString());
                socket.Bind(new IPEndPoint(IPAddress.Any, port));
                this.endPoint = new IPEndPoint(IPAddress.Any, 0);
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref this.endPoint, new AsyncCallback(OnReceive), null);
                Log.Instance.WriteLine("Binded the UDP socket to port: " + port.ToString());
                return true;
            }
            catch { }
            return false;
        }

        public void OnReceive(IAsyncResult iAr)
        {
            try
            {
                EndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
                int length = socket.EndReceiveFrom(iAr, ref receiveEndPoint);

                if (length > 0)
                {
                    byte[] packetBuffer = new byte[length];
                    Array.Copy(buffer, 0, packetBuffer, 0, length);
                    Handle(packetBuffer, receiveEndPoint as IPEndPoint);
                }

                endPoint = new IPEndPoint(IPAddress.Any, 0);
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref this.endPoint, new AsyncCallback(OnReceive), null);
            }
            catch
            {
                socket.Close();
                Start();
            }

        }

        public ushort ReversePort(IPEndPoint ipEndp)
        {
            byte[] reversedPort = BitConverter.GetBytes((ushort)ipEndp.Port);
            Array.Reverse(reversedPort);
            return BitConverter.ToUInt16(reversedPort, 0);
        }

        public void Handle(byte[] packet, IPEndPoint endPoint)
        {
            uint type = packet.ToUShort(0);
            ushort sessionID = packet.ToUShort(4);
            Entities.User user = Managers.UserManager.Instance.Get(sessionID);
            if (user == null)
                return;

            switch (type)
            {
                case 0x1001: //Initial packet
                    packet.WriteUShort((ushort)(this.port + 1), 4);
                    socket.SendTo(packet, endPoint);
                    break;
                case 0x1010: //UDP Ping packet
                    if (packet[14] == 0x21)
                    {
                        user.LocalEndPoint = packet.ToIPEndPoint(32);
                        user.RemoteEndPoint = endPoint;
                        user.RemotePort = ReversePort(endPoint);
                        user.LocalPort = ReversePort(user.LocalEndPoint);

                        byte[] response = packet.Extend(65);

                        #region old response bytes
                        /*
                            byte[] response = new byte[65]
                                {
                                    0x10, 0x10, //0
                                    0x0, 0x0, //2
                                    0x0, 0x0, //4
                                    0xFF, 0xFF, 0xFF, 0xFF, //6
                                    0x0, 0x0, 0x0, 0x0, //10
                                    0x21, 0x0, 0x0, 0x41, //14
                                    0x0, 0x0, 0x0, 0x0, //18
                                    0x0, 0x0, 0x0, 0x0, //22
                                    0x0, 0x0, //26
                                    0x1, 0x11, 0x13, 0x11, //28
                                    0x0, 0x0, 0x0, 0x0, 0x0, 0x0, //32 remoteip
                                    0x11, 0x11, 0x11, 0x11, //38
                                    0x11, 0x11, 0x11, 0x11, //42
                                    0x01, 0x11, 0x13, 0x11, //48
                                    0x0, 0x0, 0x0, 0x0, 0x0, 0x0, //50 localip
                                    0x19, 0x19, 0x19, 0x19, //56
                                    0x19, 0x19, 0x19, 0x19, //60
                                    0x11 //64
                                };
                             */
                        #endregion

                        response[17] = 0x41;
                        response[response.Length - 1] = 0x11;
                        response.WriteUShort((ushort)user.SessionID, 4); //Not really necessary
                        response.WriteIPEndPoint(user.RemoteEndPoint, 32);
                        response.WriteIPEndPoint(user.LocalEndPoint, 50);
                        socket.SendTo(response, endPoint);
                    }
                        //On Thursday 20/8/2015 Tira triggered an Unhandled UDP subpacket 16... Added on 21/8 for testing
                else if (packet[14] == 0x10 || packet[14] == 0x30 || packet[14] == 0x31 || packet[14] == 0x32 || packet[14] == 0x34)
                    {
                        if (user.Room == null)
                            return;

                        byte[] SessionIDBytes = new byte[2] { buffer[5], buffer[4] };
                        ushort targetID = packet.ToUShort(22);
                        uint room = packet.ToUShort(6);    
                        ushort SessionID = BitConverter.ToUInt16(SessionIDBytes, 0);
                        
                        Entities.User U1 = Managers.UserManager.Instance.Get(SessionID);
                        Entities.User U2 = Managers.UserManager.Instance.Get(targetID);

                    //TODO CHECK THIS
                       if(U1 != null && U2 != null && U1.Room.ID == room && U2.Room.ID == room && U1.Room.State == Enums.RoomState.Playing)
                           socket.SendTo(packet, U2.RemoteEndPoint);               
                        else
                            Log.Instance.WriteError("UDP TUNNEL PACKET FAULTY - ROOM DID NOT MATCH SENDER" + U1.Displayname + '/' + U2.Displayname
                                + '/' + U1.Room.ID.ToString() + '/' + U2.Room.ID.ToString() + '/' + room.ToString());         
                        
                    }
                    else
                        Log.Instance.WriteError("UNHANDLED UDP SUB PACKET " + packet[14].ToString());
                    break;
                        
                default:
                    Log.Instance.WriteError("Unhandled UDP Packet " + type);
                    break;
                        
            }
        }
    }

    public static class UdpReader
    {
        private const byte xOrSendKey = 0xc3;
        private const byte xOrReceiveKey = 0x96;

        public static ushort ToUShort(this byte[] packet, int offset)
        {
            byte[] value = new byte[2];
            Array.Copy(packet, offset, value, 0, 2);
            Array.Reverse(value);
            return BitConverter.ToUInt16(value, 0);
        }

        public static uint ToUInt(this byte[] packet, int offset)
        {
            byte[] value = new byte[4];
            Array.Copy(packet, offset, value, 0, 4);
            Array.Reverse(value);
            return BitConverter.ToUInt32(value, 0);
        }

        public static IPEndPoint ToIPEndPoint(this byte[] packet, int offset)
        {
            for (int i = offset; i < offset + 6; i++)
                packet[i] ^= xOrSendKey;
            ushort port = BitConverter.ToUInt16(packet, offset);
            uint ip = BitConverter.ToUInt32(packet, offset + 2);
            return new IPEndPoint(ip, port);
        }

        public static void WriteUShort(this byte[] packet, ushort value, int offset)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            Array.Copy(bytes, 0, packet, offset, 2);
        }

        public static void WriteUInt(this byte[] packet, uint value, int offset)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            Array.Copy(bytes, 0, packet, offset, 4);
        }

        public static void WriteIPEndPoint(this byte[] packet, IPEndPoint endpoint, int offset)
        {
            byte[] value = new byte[6];
            Array.Copy(BitConverter.GetBytes(endpoint.Port), 0, value, 0, 2);
            Array.Copy(endpoint.Address.GetAddressBytes(), 0, value, 2, 4);
            Array.Reverse(value);
            for (int i = offset; i < offset + 6; i++)
                packet[i] = (byte)(value[i - offset] ^ xOrReceiveKey);
        }

        public static byte[] Extend(this byte[] packet, int length)
        {
            byte[] newPacket = new byte[length];
            Array.Copy(packet, newPacket, packet.Length);
            return newPacket;
        }
    }
}
