using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;

namespace SmartboyDevelopments.Haxxit.Commands
{
    public abstract class UndoCommand
    {
        public Point OriginatingPoint
        {
            get;
            set;
        }
        public Point AttackedPoint
        {
            get;
            set;
        }
        public ushort MovesLeft
        {
            get;
            set;
        }
        public abstract bool Run(Map map);
    }
}
