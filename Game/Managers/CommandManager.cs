using System;
using System.Collections.Concurrent;

using Game.Enums;

namespace Game.Managers
{
    class CommandManager
    {
        private ConcurrentDictionary<Command, Objects.Command> GameCommands = new ConcurrentDictionary<Command, Objects.Command>();

        public CommandManager()
        {
            GameCommands.Clear();

            AddCommand(Command.Notice           , new Commands.Notice());
            AddCommand(Command.DisconnectUser   , new Commands.DisconnectUser());
            AddCommand(Command.List             , new Commands.CommandList());
        }


        private void AddCommand(Command CommandName, Objects.Command Command)
        {
            if (!GameCommands.ContainsKey(CommandName))
                GameCommands.TryAdd(CommandName, Command);
        }

        public Objects.Command GetCommand(string _commandName)
        {
            bool _exists = Enum.IsDefined(typeof(Command), _commandName);

            if (_exists)
            {
                Command CommandName = (Command)Enum.Parse(typeof(Command), _commandName);

                if(GameCommands.ContainsKey(CommandName))
                    return GameCommands[CommandName];             
            }
            return null;         
        }

        public ConcurrentDictionary<Command, Objects.Command> GetAllCommands()
        {
            return GameCommands;
        }

        private static CommandManager instance = null;
        public static CommandManager Instance { get { if (instance == null) instance = new CommandManager(); return instance; } set { } }

    }
   
}
