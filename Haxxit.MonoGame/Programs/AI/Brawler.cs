using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class BrawlerFactory : ProgramFactory
    {
        public BrawlerFactory()
        {
            Moves = 4;
            Size = 5;
            SpawnWeight = 25;
            ProgramCost = 2000;
            TypeName = "Brawler";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Pummel", "Deletes 3 cells from target.", 3, 1));
            Commands = commands;
        }
    }
}
