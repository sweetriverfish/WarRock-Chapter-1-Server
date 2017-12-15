using System;
using Game.Enums;

namespace Game.Packets 
{
    //287381656 30000 1 -1 18 2 151 0 1 3 813 0 1 0 2 1 5 1 750000 728653 750000 2572.4182 170.2791 3887.7246 209.0408 -142.6750 161.0769 0 0 DS05
    //287381656 30000 1 -1 18 2 151 0 1 8 813 0 1 0 2 1 5 1 750000 728653 750000 2572.4182 170.2791 3887.7246 209.0408 -142.6750 161.0769 0 0 DS05
    //287381656 30017 2 0 1000 2 0 -1 16 878 -1 -1 -1 13 0 2500 2500 NULL 1 2500 2500 NULL 2 3674 3800 NULL 3 2500 2500 NULL 4 2500 2500 NULL 5 3800 3800 NULL 6 3500 3500 NULL 7 3500 3500 NULL 8 3500 3500 NULL 9 3000 3000 NULL 10 3000 3000 NULL 11 3000 3000 NULL 12 3000 3000 NULL
    //287381671 25600 5000 26 0 -1 0 1.0000 1.0000 -1

    class VehicleSpawn : Core.Networking.OutPacket
    {
        public VehicleSpawn(Entities.Room r, Entities.Vehicle Vehicle) : base(30000)
        {
            Append(1);
            Append(-1);
            Append(r.ID);
            Append(2);
            Append(151);
            Append(0);
            Append(1);
            Append(Vehicle.MapID);
            Append(500);
            Append(0);
            Append(1);
            Append(0);
            Append(15);
            Append(1);
            Append(5);
            Append(1);
            Append(1090909);
            Append(-60401);
            Append(1090909);
            Append(4256.2275);
            Append(1100.4197);
            Append(3323.0078);
            Append(-229.4619);
            Append(-33.1609);
            Append(-190.3880);
            Append(0);
            Append(0);
            Append("DU02");
        }
    }

}
