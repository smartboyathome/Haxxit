using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit
{
    public abstract class Player
    {
        Dictionary<Type, ushort> owned_programs;
        ushort total_silicoins;
        NotifiableManager _notifiable_manager;
        public INotifiable Notifiable
        {
            get
            {
                return _notifiable_manager.Notifiable;
            }
            set
            {
                _notifiable_manager.Notifiable = value;
            }
        }

        public Player()
        {
            owned_programs = new Dictionary<Type, ushort>();
            total_silicoins = 0;
            _notifiable_manager = new NotifiableManager();
        }

        public ushort TotalSilicoins
        {
            get
            {
                return total_silicoins;
            }
        }

        public bool AddProgramCopies<T>(ushort count) where T: Program
        {
            Type t = typeof(T);
            return AddProgramCopies(t, count);
        }

        public bool AddProgramCopies(Type t, ushort count)
        {
            if (!t.IsSubclassOf(typeof(Program)))
                return false;
            if (owned_programs.ContainsKey(t) && owned_programs[t] + count > owned_programs[t])
            {
                owned_programs[t] += count;
                return true;
            }
            else if (!owned_programs.ContainsKey(t))
            {
                owned_programs.Add(t, count);
                return true;
            }
            return false;
        }

        public bool RemoveProgramCopies<T>(ushort count) where T: Program
        {
            Type t = typeof(T);
            if (owned_programs.ContainsKey(t) && owned_programs[t] - count < owned_programs[t])
            {
                owned_programs[t] += count;
                return true;
            }
            return false;
        }

        public bool AddSilicoins(ushort count)
        {
            if (total_silicoins + count > total_silicoins)
            {
                total_silicoins += count;
                return true;
            }
            return false;
        }

        public bool RemoveSilicoins(ushort count)
        {
            if (total_silicoins - count < total_silicoins)
            {
                total_silicoins -= count;
                return true;
            }
            return false;
        }
    }
}
