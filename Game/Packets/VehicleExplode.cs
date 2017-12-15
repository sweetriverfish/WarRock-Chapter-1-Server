using System;
using Game.Enums;


  namespace Game.Packets {

      class VehicleExplode : Core.Networking.OutPacket
      {
          public VehicleExplode(Entities.Room r, Entities.Vehicle v)
              : base(30000)
          {
              Append(1);
              Append(-1); // Sender
              Append(r.ID); // Room id
              Append(2);
              Append(153);
              Append(0);
              Append(1);
              Append(0);
              Append(v.MapID); // Target
              Append(0);
              Append(2);
              Append(0);
              Append(7);
              Append(2);
              Append(0);
              Append(1);
              Append(100);
              Append(0);
              Append(0);
              // Coords //
              Append(0);
              Append(0);
              Append(0);
              // End Coords //
              Append(0);
              Append(0);
              Append(0);
              Append(0);
              Append(0);
              Append("FFFF");
          }

      }
}