using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class Hacker2Factory : ProgramFactory
    {
        public Hacker2Factory()
        {
            Moves = 3;
            Size = 5;
            SpawnWeight = 30;
            ProgramCost = 1250;
            TypeName = "Hacker 2.0";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Crack 2.0", "Deletes 3 cells from target.", 3, 1));
            Commands = commands;
        }
    }
}
