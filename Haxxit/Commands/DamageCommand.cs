using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.Commands
{
    public abstract class DamageCommand : Command
    {
        protected ushort _strength;
        public ushort Strength
        {
            get
            {
                return _strength;
            }
        }

        public override UndoCommand Run(Map map, Point attacked_point)
        {
            if (!map.NodeIsType<ProgramNode>(attacked_point))
                return null;
            ProgramNode attacked_node = (ProgramNode)map.GetNode(attacked_point);
            Program attacked_program = attacked_node.program;
            IEnumerable<ProgramNode> removed_nodes = DamageProgram(map, attacked_node, _strength);
            return new UndoDamageCommand(removed_nodes);
        }
    }

    public class UndoDamageCommand : UndoCommand
    {
        private IEnumerable<ProgramNode> _removed_nodes;

        public UndoDamageCommand(IEnumerable<ProgramNode> removed_nodes)
        {
            _removed_nodes = removed_nodes;
        }

        public override bool Run(Map map)
        {
            return map.AddProgramNodes(_removed_nodes);
        }
    }
}
