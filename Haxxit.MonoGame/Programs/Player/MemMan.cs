using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class MemManFactory : MonoGameProgramFactory
    {
        public MemManFactory()
        {
            Moves = 3;
            Size = 3;
            SpawnWeight = 10;
            ProgramCost = 500;
            TypeName = "MemMan";
            HeadColor = new Color(249, 212, 35); // Above Yellow
            TailColor = new Color(252, 145, 58); // Show My Your Heart
            List<Command> commands = new List<Command>();
            commands.Add(new RemoveNodeCommand("Corrupt", "Deletes one cell from the map.", 1));
            commands.Add(new AddNodeCommand("Fix", "Adds one cell to the map.", 1));
            Commands = commands;
        }
    }
}
