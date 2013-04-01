using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.Commands
{
    public abstract class IncreaseSpeedCommand : Command
    {
        protected ushort _speed;
        public ushort Speed
        {
            get
            {
                return _speed;
            }
        }

        public override UndoCommand Run(Map map, Point attacked_point)
        {
            if (!map.NodeIsType<ProgramNode>(attacked_point))
                return null;
            ProgramNode attacked_node = (ProgramNode)map.GetNode(attacked_point);
            Program attacked_program = attacked_node.Program;
            IncreaseProgramSpeed(attacked_program, _speed);
            return null; // TODO: implement UndoIncreaseSpeedCommand
        }
    }
}
