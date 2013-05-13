﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class HackFactory : ProgramFactory
    {
        public HackFactory()
        {
            Moves = 2;
            Size = 4;
            TypeName = "Hack";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Slice", "Deletes 2 cells from target.", 2, 1));
            Commands = commands;
        }
    }
}
