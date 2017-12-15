using System;
using Game.Entities;

namespace Game.Commands
{
    class Notice : Objects.Command
    {
        public Notice()
            : base(1, 3)
        {

        }

        public override void Process(User Caster)
        {
               if(Caster.Authorized && Caster.AccessLevel >= AccessLevel)
                {
                   if(Arguments.Length > 0)
                     {
                         string _message = String.Join(" ", Arguments);
                         Cristina.Core.Cristina.Chat.SayNotice(_message);
                     }
               }   
        }
    }
}
