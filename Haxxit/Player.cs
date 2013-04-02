﻿using System;
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
        private Dictionary<ProgramFactory, ushort> owned_programs;
        
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
        private NotifiableManager _notifiable_manager;

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
        private Guid _guid;

        /// <summary>
        /// Creates a new player, with an optional name.
        /// </summary>
        /// <param name="name">The name of the player, optional.</param>
        public Player(string name="")
        {
            owned_programs = new Dictionary<ProgramFactory, ushort>();
            TotalSilicoins = 0;
            _notifiable_manager = new NotifiableManager();
            Name = name;
            _guid = Guid.NewGuid();
        }

        /// <summary>
        /// Do a deep clone of the player. The player with have the same information, but they will all be separate instances.
        /// </summary>
        /// <returns>The newly cloned player.</returns>
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

        /// <summary>
        /// Adds count number of copies of the program specified by the passed-in ProgramFactory to the dictionary.
        /// </summary>
        /// <param name="factory">The factory representing the program which is being added.</param>
        /// <param name="count">The number of copies of the program being added.</param>
        /// <returns>Whether adding the copies are successful.</returns>
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

        /// <summary>
        /// Removes copies of a program specified by the passed-in ProgramFactory from the dictionary.
        /// </summary>
        /// <param name="factory">The factory representing the program which is being removed.</param>
        /// <param name="count">The number of copies of the program being removed.</param>
        /// <returns>Whether removing the copies are successful.</returns>
        public bool RemoveProgramCopies(ProgramFactory factory, ushort count)
        {
            if (owned_programs.ContainsKey(factory) && owned_programs[factory] - count < owned_programs[factory])
            {
                owned_programs[factory] -= count;
                return true;
            }
            else if (owned_programs.ContainsKey(factory))
            {
                owned_programs.Remove(factory);
                return true;
            }
            return false;
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
            return Name == other.Name && _guid == other._guid;
        }
    }
}
