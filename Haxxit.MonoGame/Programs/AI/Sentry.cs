using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class SentryFactory : MonoGameProgramFactory
    {
        public SentryFactory()
        {
            Moves = 1;
            Size = 3;
            SpawnWeight = 5;
            ProgramCost = 50;
            TypeName = "Sentry";
            HeadColor = new Color(168, 219, 168); // sea foaming
            TailColor = new Color(121, 189, 154); // sea showing green
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Revoke", "Deletes 1 cell from target.", 1, 1));
            Commands = commands;
        }
    }
}
