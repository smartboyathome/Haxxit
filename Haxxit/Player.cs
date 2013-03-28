using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit
{
    public abstract class Player
    {
        private Dictionary<ProgramFactory, ushort> owned_programs;
        private ushort total_silicoins;
        private NotifiableManager _notifiable_manager;
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
            owned_programs = new Dictionary<ProgramFactory, ushort>();
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

        public bool AddProgramCopies(ProgramFactory factory, ushort count)
        {
            if (owned_programs.ContainsKey(factory) && owned_programs[factory] + count > owned_programs[factory])
            {
                owned_programs[factory] += count;
                return true;
            }
            else if (!owned_programs.ContainsKey(factory))
            {
                owned_programs.Add(factory, count);
                return true;
            }
            return false;
        }

        public bool RemoveProgramCopies(ProgramFactory factory, ushort count)
        {
            if (owned_programs.ContainsKey(factory) && owned_programs[factory] - count < owned_programs[factory])
            {
                owned_programs[factory] += count;
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
