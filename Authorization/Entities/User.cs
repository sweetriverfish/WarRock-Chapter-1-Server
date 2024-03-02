﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace Authorization.Entities {
    public class User : Core.Entities.Entity {
        
        private Socket socket;
        private byte[] buffer = new byte[1024];
        private byte[] cacheBuffer = new byte[0];
        private uint packetCount = 0;

        private bool isDisconnect = false;
        private bool isAuthorized = false;

        private uint sessionId = 0;

        public User(Socket socket)
            : base(0, "Unknown", "Unknown") {
            this.socket = socket;
            isDisconnect = false;
            this.socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnDataReceived), null);
            Send(new Core.Packets.Connection(Core.Constants.xOrKeySend));
        }

        public void OnAuthorize(uint id, string name, string displayname) {
            this.ID = id;
            this.Name = name;
            this.Displayname = displayname;
            Managers.SessionManager.Instance.Add(this);
            this.isAuthorized = true;
            this.AccessLevel = 1;
        }

        public void OnAuthorize(uint id, string name, string displayname, byte _accesslevel)
        {
            this.ID = id;
            this.Name = name;
            this.Displayname = displayname;
            this.AccessLevel = _accesslevel;
            Managers.SessionManager.Instance.Add(this);
            this.isAuthorized = true;          
        }

        public void UpdateDisplayname(string displayname) {
            this.Displayname = displayname;
            Session s = Managers.SessionManager.Instance.Get(this.sessionId);
            if (s != null) {
                s.UpdateDisplayname(displayname);
            }
        }

        public void SetSession(uint sessionId) {
            this.sessionId = sessionId;
        }

        private void OnDataReceived(IAsyncResult iAr) {
            try {
                int bytesReceived = socket.EndReceive(iAr);

                if (bytesReceived > 0) {
                    byte[] packetBuffer = new byte[bytesReceived];

                    // Decrypt the bytes with the xOrKey.
                    for (int i = 0; i < bytesReceived; i++) {
                        packetBuffer[i] = (byte)(this.buffer[i] ^ Core.Constants.xOrKeyReceive);
                    }

                    int oldLength = cacheBuffer.Length;
                    Array.Resize(ref cacheBuffer, oldLength + bytesReceived);
                    Array.Copy(packetBuffer, 0, cacheBuffer, oldLength, packetBuffer.Length);

                    int startIndex = 0; // Determs whre the bytes should split
                    for (int i = 0; i < cacheBuffer.Length; i++) { // loop trough our cached buffer.
                        if (cacheBuffer[i] == 0x0A) { // Found a complete packet
                            byte[] newPacket = new byte[i - startIndex]; // determ the new packet size.
                            for (int j = 0; j < (i - startIndex); j++) {
                                newPacket[j] = cacheBuffer[startIndex + j]; // copy the buffer to the buffer of the new packet.
                            }
                            packetCount++;

                            // Handle the packet instantly.
                            Core.Networking.InPacket inPacket = new Core.Networking.InPacket(newPacket, this);
                            ServerLogger.Instance.AppendPacket(newPacket);
                            if (inPacket != null)
                            {
                                if (inPacket.Id > 0)
                                {
                                    Networking.PacketHandler pHandler = Managers.PacketManager.Instance.FindExternal(inPacket);
                                    if (pHandler != null)
                                    {
                                        try
                                        {
                                            pHandler.Handle(inPacket);
                                        }
                                        catch (Exception e) { Log.Instance.WriteError(e.ToString()); }
                                    }
                                }
                            }
                            startIndex = i + 1;
                        }
                    }

                    if (startIndex > 0) {
                        byte[] fullCopy = cacheBuffer;
                        Array.Resize(ref cacheBuffer, (cacheBuffer.Length - startIndex));
                        for (int i = 0; i < (cacheBuffer.Length - startIndex); i++) {
                            cacheBuffer[i] = fullCopy[startIndex + i];
                        }
                        fullCopy = null;
                    }

                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnDataReceived), null);
                } else {
                    Disconnect();
                }
            } catch {
                Disconnect();
            }
        }

        public void Send(Core.Networking.OutPacket outPacket) {
            try {
                byte[] sendBuffer = outPacket.BuildEncrypted();
                socket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            } catch {
                Disconnect();
            }
        }

        private void SendCallback(IAsyncResult iAr) {
            try {
                socket.EndSend(iAr);
            } catch {
                Disconnect();
            }
        }

        public void Disconnect() {
            if (isDisconnect) return;
            isDisconnect = true;

            try { socket.Close(); } catch { }
        }

        public bool Authorized { get { return this.isAuthorized; } set { } }
        public uint SessionID { get { return this.sessionId; } set { } }
        public string RemoteEndIP => socket.RemoteEndPoint.ToString().Split(':')[0];
    }
}
