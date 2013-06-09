using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.Haxxit.Commands;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class Hacker2Factory : MonoGameProgramFactory
    {
        public Hacker2Factory()
        {
            Moves = 3;
            Size = 5;
            SpawnWeight = 30;
            ProgramCost = 1250;
            TypeName = "Hacker2";
            HeadColor = new Color(227, 11, 92); // Raspberry
            TailColor = new Color(255, 0, 255); // Fuscia
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Crack 2.0", "Deletes 3 cells from target.", 3, 1));
            Commands = commands;
        }
    }
}
