using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.Haxxit.Commands;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class SniperFactory : MonoGameProgramFactory
    {
        public SniperFactory()
        {
            Moves = 2;
            Size = 2;
            SpawnWeight = 10;
            ProgramCost = 250;
            TypeName = "Sniper";
            HeadColor = new Color(255, 223, 0); // Golden Yellow
            TailColor = new Color(255, 159, 0); // Orange Peel
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Snipe", "Deletes 1 cell from target.", 1, 2));
            Commands = commands;
        }
    }
}
