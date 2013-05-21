using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class HackerFactory : ProgramFactory
    {
        public HackerFactory()
        {
            Moves = 2;
            Size = 4;
            SpawnWeight = 20;
            ProgramCost = 500;
            TypeName = "Hacker";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Crack", "Deletes 2 cells from target.", 2, 1));
            Commands = commands;
        }
    }
}
