using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.Tests
{
    public class DynamicDamageCommand : DamageCommand
    {
        public DynamicDamageCommand(ushort strength, ushort range, string name="Damage")
        {
            Strength = strength;
            Range = range;
            Name = name;
            string type = Strength == 1 ? " node" : " nodes";
            Description = "Removes " + Strength.ToString() + type + " from target.";
        }
    }
}
