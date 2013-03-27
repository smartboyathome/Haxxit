using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit
{
    public class Movement
    {
        private ushort _speed;
        private ushort _moves_left;

        public Movement(ushort Speed)
        {
            _speed = _moves_left = Speed;
        }

        public ushort Speed
        {
            get
            {
                return _speed;
            }
        }

        public ushort MovesLeft
        {
            get
            {
                return _moves_left;
            }
        }

        public bool CanMove()
        {
            return _moves_left > 0;
        }

        public void Moved()
        {
            if(!CanMove())
                return;
            _moves_left--;
        }

        public void UndoMove()
        {
            _moves_left++;
        }

        public void Reset()
        {
            _moves_left = _speed;
        }

        public void IncreaseSpeed(ushort amount)
        {
            _speed += amount;
            IncreaseMovesLeft(amount);
        }

        public void DecreaseSpeed(ushort amount)
        {
            if (_speed < amount)
            {
                _speed = 0;
            }
            else
            {
                _speed -= amount;
            }
            DecreaseMovesLeft(amount);
        }

        public void IncreaseMovesLeft(ushort amount)
        {
            if (_moves_left + amount > _speed)
            {
                _moves_left = _speed;
            }
            else
            {
                _moves_left += amount;
            }
        }

        public void DecreaseMovesLeft(ushort amount)
        {
            if (_moves_left < amount)
            {
                _moves_left = 0;
            }
            else
            {
                _moves_left -= amount;
            }
        }
    }

    public class ProgramSize
    {
        private ushort _max_size;
        private ushort _current_size;

        public ProgramSize(ushort _size)
        {
            _max_size = _size;
            _current_size = 1;
        }

        public ushort MaxSize
        {
            get
            {
                return _max_size;
            }
        }

        public ushort CurrentSize
        {
            get
            {
                return _current_size;
            }
        }

        public bool IsMaxSize()
        {
            return _max_size == _current_size;
        }

        public void IncreaseMaxSize(ushort amount)
        {
            _max_size += amount;
            IncreaseCurrentSize(amount);
        }

        public void DecreaseMaxSize(ushort amount)
        {
            if (_max_size < amount)
            {
                _max_size = 0;
            }
            else
            {
                _max_size -= amount;
            }
            DecreaseCurrentSize(amount);
        }

        public void IncreaseCurrentSize(ushort amount)
        {
            if (IsMaxSize())
                return;
            if (_current_size + amount > _max_size)
            {
                _current_size = _max_size;
            }
            else
            {
                _current_size += amount;
            }
        }

        public void DecreaseCurrentSize(ushort amount)
        {
            if (_current_size < amount)
            {
                _current_size = 0;
            }
            else
            {
                _current_size -= amount;
            }
        }
    }

    public abstract class Program
    {
        protected Movement _moves;
        public Movement Moves
        {
            get
            {
                return _moves;
            }
        }
        
        protected ProgramSize _size;
        public ProgramSize Size
        {
            get
            {
                return _size;
            }
        }

        protected Dictionary<string, Command> _commands = new Dictionary<string,Command>();
        protected bool has_run_command = false;

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
        
        public UndoCommand RunCommand(Map map, Point attacked_point, string command_name)
        {
            if (has_run_command || !HasCommand(command_name))
                return null;
            UndoCommand retval = GetCommand(command_name).Run(map, attacked_point);
            if (retval != null)
            {
                retval.MovesLeft = _moves.MovesLeft;
                _moves.DecreaseMovesLeft(_moves.MovesLeft);
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
                _moves.DecreaseMovesLeft(_moves.MovesLeft); // to make sure moves left is zero
                _moves.IncreaseMovesLeft(command.MovesLeft);
                has_run_command = false;
            }
            return retval;
        }

        public void Reset()
        {
            has_run_command = false;
            _moves.Reset();
        }
    }
}
