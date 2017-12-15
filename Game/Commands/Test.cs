using System;
using Game.Entities;

namespace Game.Commands
{
    public sealed class Test : Objects.Command
    {

        public Test()  
            :base(0, 1)
        {
         
        }

        public override void Process(User Caster)
        {
            if (Caster.AccessLevel >= ID)
                Cristina.Core.Cristina.Chat.SayToPlayer("Hola juapo", Caster);
        }
    }
}
