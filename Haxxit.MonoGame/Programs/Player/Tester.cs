using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class TesterFactory : ProgramFactory
    {
        public TesterFactory()
        {
            Moves = 99;
            Size = 99;
            SpawnWeight = 1;
            ProgramCost = 1;
            TypeName = "Tester";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Cheat", "Deletes target.", 99, 99));
            commands.Add(new RemoveNodeCommand("Corrupt", "Deletes one cell from the map.", 1));
            commands.Add(new AddNodeCommand("Fix", "Adds one cell to the map.", 1));
            Commands = commands;
        }
    }
}
