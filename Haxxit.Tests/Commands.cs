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
            _strength = strength;
            Range = range;
            Name = name;
            string type = _strength == 1 ? " node" : " nodes";
            Description = "Removes " + _strength.ToString() + type + " from target.";
        }
    }
}
