using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit.Programs
{
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
}
