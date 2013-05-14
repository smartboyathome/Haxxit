using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    // This class is used to store all of the valid options which a program has for using its commands on a given turn.
    // For every node in every enemy program every valid node which is within firing range of that enemy program node
    // will be recorded in the targetOptions list.  This enables the AI to quickly look up all of its valid target
    // options when choosing a course of action for the turn.  PrioritizedCommand nodes themselves will be stored
    // in a sorted list using the comparable interface to help the program fall back to other options when some
    // command options are out of movement range for the turn.
    class PrioritizedCommand : IComparable
    {
        private int damage;
        public int Damage { get { return damage; } set { damage = value; } }

        private int range;
        public int Range { get { return range; } set { range = value; } }

        private Commands.Command command;
        public Commands.Command Command { get { return command; } set { command = value; } }

        PrioritizedCommand next;
        public PrioritizedCommand Next { get { return next; } set { next = value; } }

        private List<CommandInfo> targetOptions;
        public List<CommandInfo> TargetOptions { get { return targetOptions; } set { targetOptions = value; } }

        // Constructor
        public PrioritizedCommand(Commands.Command newCommand)
        {
            if (newCommand.GetType() == typeof(Tests.DynamicDamageCommand))
            {
                damage = ((Tests.DynamicDamageCommand)newCommand).Strength;
            }
            range = newCommand.Range;
            command = newCommand;
            next = null;
            targetOptions = new List<CommandInfo>();
        }

        // Required for IComparable inheritence
        public int CompareTo(object obj)
        {
            PrioritizedCommand otherPrioritizedCommand = obj as PrioritizedCommand;
            if (otherPrioritizedCommand.Damage > damage)
            {
                return 1;
            }
            else if (otherPrioritizedCommand.Damage == damage)
            {
                return otherPrioritizedCommand.Range.CompareTo(range);
            }
            else
            {
                return -1;
            }
        }
    }
}
