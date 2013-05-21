using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class MemManFactory : ProgramFactory
    {
        public MemManFactory()
        {
            Moves = 3;
            Size = 3;
            SpawnWeight = 10;
            ProgramCost = 500;
            TypeName = "MemMan";
            List<Command> commands = new List<Command>();
            commands.Add(new RemoveNodeCommand("Corrupt", "Deletes one cell from the map.", 1));
            commands.Add(new AddNodeCommand("Fix", "Adds one cell to the map.", 1));
            Commands = commands;
        }
    }
}
