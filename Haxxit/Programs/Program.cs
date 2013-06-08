using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.Programs
{
    public class Program
    {
        public Movement Moves
        {
            get;
            private set;
        }
        
        public ProgramSize Size
        {
            get;
            private set;
        }

        public string TypeName
        {
            get;
            private set;
        }

        private Dictionary<string, Command> _commands;
        private bool has_run_command = false;

        public Program(ushort moves, ushort size, string TypeName, IEnumerable<Command> commands)
        {
            Moves = new Movement(moves);
            Size = new ProgramSize(size);
            this.TypeName = TypeName;
            _commands = new Dictionary<string, Command>();
            foreach (Command command in commands)
            {
                _commands.Add(command.Name, command);
            }
        }

        public bool AlreadyRanCommand()
        {
            return has_run_command;
        }

        public Command GetCommand(string command_name)
        {
            if(_commands.ContainsKey(command_name))
                return _commands[command_name];
            return null;
        }
        
        public List<string> GetAllCommands()
        {
            return new List<string>(_commands.Keys);
        }
        
        public bool HasCommand(string command_name)
        {
            return _commands.ContainsKey(command_name);
        }

        public bool CanAttack(Map map, Point attacked_point, string command_name)
        {
            if (has_run_command || !HasCommand(command_name))
                return false;
            return GetCommand(command_name).CanAttack(map, attacked_point);
        }
        
        public UndoCommand RunCommand(Map map, Point attacked_point, string command_name)
        {
            if (has_run_command || !HasCommand(command_name))
                return null;
            UndoCommand retval = GetCommand(command_name).Run(map, attacked_point);
            if (retval != null)
            {
                retval.MovesLeft = Moves.MovesLeft;
                Moves.DecreaseMovesLeft(Moves.MovesLeft);
                has_run_command = true;
            }
            return retval;
        }

        public bool RunUndoCommand(Map map, UndoCommand command)
        {
            if (command == null)
                return false;
            bool retval = command.Run(map);
            if (retval)
            {
                Moves.DecreaseMovesLeft(Moves.MovesLeft); // to make sure Moves left is zero
                Moves.IncreaseMovesLeft(command.MovesLeft);
                has_run_command = false;
            }
            return retval;
        }

        public void Reset()
        {
            has_run_command = false;
            Moves.Reset();
        }
    }
}
