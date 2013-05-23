using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class Sniper2Factory : ProgramFactory
    {
        public Sniper2Factory()
        {
            Moves = 3;
            Size = 3;
            SpawnWeight = 30;
            ProgramCost = 1000;
            TypeName = "Sniper2";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Snipe 2.0", "Deletes 2 cells from target.", 2, 3));
            Commands = commands;
        }
    }
}
