using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class KelvinFactory : MonoGameProgramFactory
    {
        public KelvinFactory()
        {
            Moves = 99;
            Size = 99;
            SpawnWeight = 1;
            ProgramCost = 1;
            TypeName = "Kelvin";
            HeadColor = new Color(223, 21, 26); // rainbow Jell-O red
            TailColor = new Color(253, 134, 3); // Orange Jell-O
            List<Command> commands = new List<Command>();
            commands.Add(new DamageCommand("Nuke", "Deletes target.", 99, 99));
            commands.Add(new RemoveNodeCommand("Corrupt", "Deletes one cell from the map.", 1));
            commands.Add(new AddNodeCommand("Fix", "Adds one cell to the map.", 1));
            Commands = commands;
        }
    }
}
