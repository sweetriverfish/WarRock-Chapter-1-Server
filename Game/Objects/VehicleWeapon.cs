using System;
using System.Collections.Generic;


namespace Game.Objects
{
    public sealed class VehicleWeapon
    {
        public string Code { get; private set; }
        public uint Damage { get; private set; }
        public byte[] HitBox { get; private set; }

        public VehicleWeapon(string _Code, uint _damage, byte[] _hitbox)
        {
            Code = _Code;
            Damage = _damage;
            HitBox = _hitbox;
        }

    }
}
