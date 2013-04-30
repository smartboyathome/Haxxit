using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit.Programs
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
            if (!CanMove())
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
}
