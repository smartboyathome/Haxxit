using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class WatcherFactory : ProgramFactory
    {
        public WatcherFactory()
        {
            Moves = 0;
            Size = 1;
            SpawnWeight = 15;
            ProgramCost = 700;
            TypeName = "Watcher";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Spot", "Deletes 1 cell from target.", 1, 3));
            Commands = commands;
        }
    }
}
