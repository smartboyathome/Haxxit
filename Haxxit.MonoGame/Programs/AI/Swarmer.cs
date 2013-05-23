using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class SwarmerFactory : ProgramFactory
    {
        public SwarmerFactory()
        {
            Moves = 3;
            Size = 4;
            SpawnWeight = 20;
            ProgramCost = 600;
            TypeName = "Swarmer";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Infect", "Deletes 2 cells from target.", 2, 1));
            commands.Add(new DamageCommand("Spread", "Deletes 1 cell from target.", 1, 2));
            Commands = commands;
        }
    }
}
