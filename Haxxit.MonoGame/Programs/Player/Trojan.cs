using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class TrojanFactory : MonoGameProgramFactory
    {
        public TrojanFactory()
        {
            Moves = 5;
            Size = 1;
            SpawnWeight = 20;
            ProgramCost = 750;
            TypeName = "Trojan";
            HeadColor = new Color(206, 10, 49); // Hot
            TailColor = new Color(141, 32, 54); // Bothered
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Breach", "Deletes 2 cells from target.", 2, 1));
            Commands = commands;
        }
    }
}
