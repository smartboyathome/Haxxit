﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class SantaFactory : ProgramFactory
    {
        public SantaFactory()
        {
            Moves = 4;
            Size = 25;
            SpawnWeight = 100;
            ProgramCost = 9000;
            TypeName = "Santa";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Slay", "Deletes 4 cells from target.", 4, 1));
            Commands = commands;
        }
    }
}
