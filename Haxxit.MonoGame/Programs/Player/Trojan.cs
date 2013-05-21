using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class TrojanFactory : ProgramFactory
    {
        public TrojanFactory()
        {
            Moves = 5;
            Size = 1;
            SpawnWeight = 20;
            ProgramCost = 750;
            TypeName = "Trojan";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Breach", "Deletes 2 cells from target.", 2, 1));
            Commands = commands;
        }
    }
}
