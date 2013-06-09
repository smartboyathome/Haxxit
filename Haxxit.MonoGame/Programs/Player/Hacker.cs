using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.Haxxit.Commands;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class HackerFactory : MonoGameProgramFactory
    {
        public HackerFactory()
        {
            Moves = 2;
            Size = 4;
            SpawnWeight = 20;
            ProgramCost = 500;
            TypeName = "Hacker";
            HeadColor = new Color(222, 49, 99); // Cerise
            TailColor = new Color(220, 20, 60); // Crimson
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Crack", "Deletes 2 cells from target.", 2, 1));
            Commands = commands;
        }
    }
}
