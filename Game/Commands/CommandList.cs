using System;
using Game.Entities;
using Game.Enums;

namespace Game.Commands
{
   public sealed class CommandList : Objects.Command
    {
        public CommandList() 
            : base (3, 1)
        {

        }

        public override void Process(User Caster)
        {
           if(Caster.Authorized && Caster.AccessLevel >= AccessLevel)
            {
                foreach(Objects.Command Command in Managers.CommandManager.Instance.GetAllCommands().Values)
                {
                    if (Command.AccessLevel <= Caster.AccessLevel)
                    {
                        string _commandName = Enum.GetName(typeof(Command), Command.ID);
                        Cristina.Core.Cristina.Chat.SayToPlayer("/" + _commandName, Caster);
                    }
                }
            }
        }
    }
}
