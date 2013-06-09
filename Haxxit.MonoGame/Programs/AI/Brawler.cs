using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class BrawlerFactory : MonoGameProgramFactory
    {
        public BrawlerFactory()
        {
            Moves = 4;
            Size = 5;
            SpawnWeight = 25;
            ProgramCost = 2000;
            TypeName = "Brawler";
            HeadColor = new Color(59, 134, 134); // there we could sail
            TailColor = new Color(11, 72, 107); // adrift in dreams
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Pummel", "Deletes 3 cells from target.", 3, 1));
            Commands = commands;
        }
    }
}
