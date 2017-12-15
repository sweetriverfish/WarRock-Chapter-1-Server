using System;
using System.Collections;

namespace Game.Packets
{
     class RoomSpectators : Core.Networking.OutPacket
     {
         public RoomSpectators(ArrayList Spectators)
             : base((ushort)Enums.Packets.SpectatorInfo)    
       {
           foreach (Entities.User User in Spectators)
           {
               Append(1);
               Append(0);
               Append(User.SpectatorId);
               Append(User.ID);
               Append(User.SessionID);
               Append(User.Displayname);
               Append("0");
               Append(999);
               Append(User.RemoteEndPoint.Address.Address);
               Append(User.RemotePort);
               Append(User.LocalEndPoint.Address.Address);
               Append(User.LocalPort);
               Append(0);
           }
         }

         public RoomSpectators(Entities.User Spectator) : base((ushort)Enums.Packets.SpectatorInfo)
         {
             Append(1);
             Append(0);
             Append(Spectator.SpectatorId);
             Append(Spectator.ID);
             Append(Spectator.SessionID);
             Append(Spectator.Displayname);
             Append(999);

         }
   
    }
}