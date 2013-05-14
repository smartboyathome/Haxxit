using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.Tests
{
    public class DynamicProgramFactory : ProgramFactory
    {
        public DynamicProgramFactory(ushort moves, ushort size, IEnumerable<Command> commands)
        {
            Moves = moves;
            Size = size;
            TypeName = "DynamicProgram " + moves.ToString() + "." + size.ToString("D2");
            Commands = commands;
        }
    }
}
