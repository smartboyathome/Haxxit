using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class Trojan2Factory : ProgramFactory
    {
        public Trojan2Factory()
        {
            Moves = 7;
            Size = 1;
            SpawnWeight = 20;
            ProgramCost = 1500;
            TypeName = "Trojan2";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Breach 2.0", "Deletes 3 cells from target.", 3, 1));
            Commands = commands;
        }
    }
}
