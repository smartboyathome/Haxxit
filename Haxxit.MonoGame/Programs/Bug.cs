using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class BugFactory : ProgramFactory
    {
        public BugFactory()
        {
            Moves = 5;
            Size = 1;
            SpawnWeight = 20;
            TypeName = "Bug";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Glitch", "Deletes 2 cells from target.", 2, 1));
            Commands = commands;
        }
    }
}
