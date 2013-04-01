using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit
{
    public class Player : IEquatable<Player>, IDeepCloneable<Player>
    {
        private Dictionary<ProgramFactory, ushort> owned_programs;
        
        public ushort TotalSilicoins
        {
            get;
            private set;
        }

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

        public string Name
        {
            get;
            protected set;
        }

        private Guid _guid;

        public Player(string name="")
        {
            owned_programs = new Dictionary<ProgramFactory, ushort>();
            TotalSilicoins = 0;
            _notifiable_manager = new NotifiableManager();
            Name = name;
            _guid = Guid.NewGuid();
        }

        public Player DeepClone()
        {
            Player duplicate = new Player(string.Copy(Name));
            foreach (KeyValuePair<ProgramFactory, ushort> pair in owned_programs)
            {
                duplicate.owned_programs.Add(pair.Key, pair.Value);
            }
            duplicate.TotalSilicoins = TotalSilicoins;
            duplicate.Notifiable = Notifiable;
            duplicate._guid = _guid;
            return duplicate;
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
            if (TotalSilicoins + count > TotalSilicoins)
            {
                TotalSilicoins += count;
                return true;
            }
            return false;
        }

        public bool RemoveSilicoins(ushort count)
        {
            if (TotalSilicoins - count < TotalSilicoins)
            {
                TotalSilicoins -= count;
                return true;
            }
            return false;
        }

        public bool Equals(Player other)
        {
            return Name == other.Name && _guid == other._guid;
        }
    }
}
