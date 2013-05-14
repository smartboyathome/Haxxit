using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class SentinelFactory : ProgramFactory
    {
        public SentinelFactory()
        {
            Moves = 1;
            Size = 3;
            SpawnWeight = 0;
            TypeName = "Sentinel";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Cut", "Deletes 2 cells from target.", 2, 1));
            Commands = commands;
        }
    }
}
