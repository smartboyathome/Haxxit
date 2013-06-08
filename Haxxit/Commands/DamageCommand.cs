using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.Commands
{
    public class DamageCommand : Command
    {
        public ushort Strength
        {
            get;
            protected set;
        }

        public DamageCommand()
        {
            Name = "";
            Description = "";
            Strength = 0;
            Range = 0;
        }

        public DamageCommand(string name, string description, ushort strength, ushort range)
        {
            Name = name;
            Description = description;
            Strength = strength;
            Range = range;
        }

        public override UndoCommand Run(Map map, Point attacked_point)
        {
            if (!CanAttack(map, attacked_point))
                return null;
            ProgramNode attacked_node = map.GetNode<ProgramNode>(attacked_point);
            Program attacked_program = attacked_node.Program;
            IEnumerable<ProgramNode> removed_nodes = DamageProgram(map, attacked_node, Strength);
            return new UndoDamageCommand(removed_nodes);
        }

        public override bool CanAttack(Map map, Point attacked_point)
        {
            if (!map.NodeIsType<ProgramNode>(attacked_point))
                return false;
            ProgramNode attacked_node = (ProgramNode)map.GetNode(attacked_point);
            if (map.CurrentPlayer != null && attacked_node.Player == map.CurrentPlayer)
                return false;
            return true;
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
