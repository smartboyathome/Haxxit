using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class SentryFactory : ProgramFactory
    {
        public SentryFactory()
        {
            Moves = 1;
            Size = 3;
            SpawnWeight = 5;
            ProgramCost = 50;
            TypeName = "Sentry";
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Revoke", "Deletes 1 cell from target.", 1, 1));
            Commands = commands;
        }
    }
}
