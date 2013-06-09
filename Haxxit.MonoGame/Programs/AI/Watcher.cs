using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class WatcherFactory : MonoGameProgramFactory
    {
        public WatcherFactory()
        {
            Moves = 0;
            Size = 1;
            SpawnWeight = 15;
            ProgramCost = 700;
            TypeName = "Watcher";
            HeadColor = new Color(81, 149, 72); // A Dream in Green
            TailColor = new Color(27, 103, 107); // A Dream In Color
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Spot", "Deletes 1 cell from target.", 1, 3));
            Commands = commands;
        }
    }
}
