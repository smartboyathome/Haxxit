using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class SlingshotFactory : ProgramFactory
    {
        public SlingshotFactory()
        {
            Moves = 2;
            Size = 2;
            SpawnWeight = 10;
            ProgramCost = 500;
            TypeName = "Slingshot";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Fling", "Deletes 1 cell from target.", 1, 2));
            Commands = commands;
        }
    }
}
