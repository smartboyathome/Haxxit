using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;
using Microsoft.Xna.Framework;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.MonoGame.Programs
{
    class MonoGameProgramFactory : ProgramFactory
    {
        public Color HeadColor
        {
            get;
            protected set;
        }

        public Color TailColor
        {
            get;
            protected set;
        }

        public override Program NewInstance()
        {
            return new MonoGameProgram(Moves, Size, TypeName, Commands, HeadColor, TailColor);
        }
    }

    class MonoGameProgram : Program
    {
        public Color HeadColor
        {
            get;
            private set;
        }

        public Color TailColor
        {
            get;
            private set;
        }

        public MonoGameProgram(ushort moves, ushort size, string TypeName, IEnumerable<Command> commands,
            Color HeadColor, Color TailColor) : base(moves, size, TypeName, commands)
        {
            this.HeadColor = HeadColor;
            this.TailColor = TailColor;
        }
    }
}
