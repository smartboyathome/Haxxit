using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class Trojan2Factory : MonoGameProgramFactory
    {
        public Trojan2Factory()
        {
            Moves = 7;
            Size = 1;
            SpawnWeight = 20;
            ProgramCost = 1500;
            TypeName = "Trojan2";
            HeadColor = new Color(247, 0, 49); // rose bimbo
            TailColor = new Color(141, 32, 54); // Bothered
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Breach 2.0", "Deletes 3 cells from target.", 3, 1));
            Commands = commands;
        }
    }
}
