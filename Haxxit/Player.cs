using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit
{
    /// <summary>
    /// Information about a player.
    /// </summary>
    public class Player : IEquatable<Player>, IDeepCloneable<Player>
    {
        /// <summary>
        /// The dictionary containing the number of copies of each program that is owned.
        /// </summary>
        private List<ProgramFactory> owned_programs;

        
        /// <summary>
        /// The total number of silicoins the player has earned so far.
        /// </summary>
        public ushort TotalSilicoins
        {
            get;
            private set;
        }

        /// <summary>
        /// The NotifiableManager to be used for mediating notifications between parts of the game.
        /// </summary>
        protected NotifiableManager _notifiable_manager;

        /// <summary>
        /// The underlying Notifiable object that the NotifiableManager is mediating.
        /// </summary>
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

        /// <summary>
        /// The name for the player.
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// The Guid for the player, which is different for each instance of the player. This allows for handling name collisions.
        /// </summary>
        //private Guid _guid;

        /// <summary>
        /// Creates a new player, with an optional name.
        /// </summary>
        /// <param name="name">The name of the player, optional.</param>
        public Player(string name="")
        {
            owned_programs = new List<ProgramFactory>();
            _notifiable_manager = new NotifiableManager();
            Name = name;
            //_guid = Guid.NewGuid();
        }

        /// <summary>
        /// Do a deep clone of the player. The player with have the same information, but they will all be separate instances.
        /// </summary>
        /// <returns>The newly cloned player.</returns>
        public Player DeepClone()
        {
            Player duplicate = new Player(string.Copy(Name));
            foreach (ProgramFactory program in owned_programs)
            {
                duplicate.owned_programs.Add(program);
            }
            duplicate.TotalSilicoins = TotalSilicoins;
            duplicate.Notifiable = Notifiable;
            //duplicate._guid = _guid;
            return duplicate;
        }

        /// <summary>
        /// Adds a program specified by the passed-in ProgramFactory to the dictionary.
        /// </summary>
        /// <param name="factory">The factory representing the program which is being added.</param>
        /// <param name="count">The number of copies of the program being added.</param>
        /// <returns>Whether adding the copies are successful.</returns>
        public bool AddProgram(ProgramFactory factory)
        {
            if (!owned_programs.Contains(factory))
            {
                owned_programs.Add(factory);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a program specified by the passed-in ProgramFactory from the dictionary.
        /// </summary>
        /// <param name="factory">The factory representing the program which is being removed.</param>
        /// <param name="count">The number of copies of the program being removed.</param>
        /// <returns>Whether removing the copies are successful.</returns>
        public bool RemoveProgram(ProgramFactory factory)
        {
            if (owned_programs.Contains(factory))
            {
                owned_programs.Remove(factory);
                return true;
            }
            return false;
        }

        public bool OwnsProgrm(ProgramFactory factory)
        {
            return owned_programs.Contains(factory);
        }

        public IEnumerable<ProgramFactory> GetPrograms()
        {
            return owned_programs.AsReadOnly();
        }

        /// <summary>
        /// Adds count number of silicoins to the player's silicoin total.
        /// </summary>
        /// <param name="count">The number of silicoins to add to the player's total.</param>
        /// <returns>Whether adding the silicoins was successful (in other words, no overflow).</returns>
        public bool AddSilicoins(ushort count)
        {
            if (TotalSilicoins + count > TotalSilicoins)
            {
                TotalSilicoins += count;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes count number of silicoins from the player's silicoin total.
        /// </summary>
        /// <param name="count">The number of silicoins to remove from the player's total.</param>
        /// <returns>Whether removing the silicoins was successful (in other words, no underflow).</returns>
        public bool RemoveSilicoins(ushort count)
        {
            if (TotalSilicoins - count < TotalSilicoins)
            {
                TotalSilicoins -= count;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if two player objects are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Player other)
        {
            bool retval = Name == other.Name;
            return Name == other.Name;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Player p = obj as Player;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Equals(p);
        }

        public static bool operator ==(Player a, Player b)
        {
            return ((System.Object)a == null && (System.Object)b == null) || ((System.Object)a != null && a.Equals(b));
        }

        public static bool operator !=(Player a, Player b)
        {
            return ((System.Object)a == null && (System.Object)b != null) ||
                ((System.Object)a != null && (System.Object)b == null) ||
                ((System.Object)a != null && !a.Equals(b));
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
