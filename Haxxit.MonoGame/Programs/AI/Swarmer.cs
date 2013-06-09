using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class SwarmerFactory : MonoGameProgramFactory
    {
        public SwarmerFactory()
        {
            Moves = 3;
            Size = 4;
            SpawnWeight = 20;
            ProgramCost = 600;
            TypeName = "Swarmer";
            HeadColor = new Color(190, 242, 2); // A Dream in Grellow
            TailColor = new Color(136, 196, 37); // A Dream in Lime
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Infect", "Deletes 2 cells from target.", 2, 1));
            commands.Add(new DamageCommand("Spread", "Deletes 1 cell from target.", 1, 2));
            Commands = commands;
        }
    }
}
