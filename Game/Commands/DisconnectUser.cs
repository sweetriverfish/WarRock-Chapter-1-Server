using System;
using System.Collections.Generic;
using Game.Entities;

namespace Game.Commands
{
   public sealed class DisconnectUser : Objects.Command
    {
        public DisconnectUser()
            :base (2,  3)
        {

        }

        public override void Process(User Caster)
        {
           if(Caster.Authorized && Caster.AccessLevel >= AccessLevel)
            {
                string[] _targetUsers = Arguments;

                //search for users
                foreach(string _displayName in Arguments)
                {
                    User Target = Managers.UserManager.Instance.GetUser(_displayName);

                    if (Target != null) //disconnect the targets
                    {
                        if (Target.AccessLevel < Caster.AccessLevel)
                        {
                            Target.Disconnect();
                            Cristina.Core.Cristina.Chat.SayToPlayer("Kicked: " + Target.Displayname, Caster);
                            Cristina.Core.Cristina.Chat.SayNotice(Target.Displayname + " was kicked from the server");
                        }
                        else
                            Cristina.Core.Cristina.Chat.SayToPlayer(Target.Displayname + " cannot be kicked", Caster);
                    }
                    else
                        Cristina.Core.Cristina.Chat.SayToPlayer(_displayName + " is not online", Caster);
                }

            }
        }
    }
}
