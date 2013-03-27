using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.Tests
{
    class BasicDamageCommand : DamageCommand
    {
        public BasicDamageCommand()
        {
            _strength = 2;
            Range = 3;
            Name = "Damage";
            Description = "Removes 2 nodes from target.";
        }
    }
}
