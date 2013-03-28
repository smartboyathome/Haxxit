using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.Commands
{
    public abstract class Command : IEquatable<Command>
    {
        public int Range
        {
            get;
            protected set;
        }
        public string Name
        {
            get;
            protected set;
        }
        public string Description
        {
            get;
            protected set;
        }
        protected Program _current_program;
        public abstract UndoCommand Run(Map map, Point attacked_point);

        protected IEnumerable<ProgramNode> GetProgramNodes(ProgramNode program_node)
        {
            // A stack to hold all the nodes from the attacked node to the head node.
            Stack<ProgramNode> nodes_to_head = new Stack<ProgramNode>();
            nodes_to_head.Push(program_node);
            //ProgramNode temp_node = attacked_node;
            while (nodes_to_head.Peek().Head != null)
            {
                nodes_to_head.Push(nodes_to_head.Peek().Head);
            }
            // The nodes_to_head stack is actually reversed in this constructor since it acts like it is popping off of
            // the old stack onto this one. Neat, huh? :D
            Stack<ProgramNode> nodes_to_tail = new Stack<ProgramNode>(nodes_to_head.AsEnumerable());
            while (nodes_to_tail.Peek().Tail != null)
            {
                nodes_to_tail.Push(nodes_to_tail.Peek().Tail);
            }
            return nodes_to_tail;
        }

        protected IEnumerable<ProgramNode> DamageProgram(Map map, ProgramNode attacked_node, ushort strength)
        {
            IEnumerable<ProgramNode> nodes_list = GetProgramNodes(attacked_node);
            // Reverse() is used because taking something from one stack to another puts it on backwards.
            Stack<ProgramNode> nodes_stack = new Stack<ProgramNode>(nodes_list.Reverse());
            List<ProgramNode> removed_nodes = new List<ProgramNode>();
            for (int i = 0; i < strength; ++i)
            {
                ProgramNode current_node = nodes_stack.Pop();
                map.CreateNode<AvailableNode>(current_node.coordinate);
                removed_nodes.Add(current_node);
                if (current_node.Head == null)
                    break;
            }
            if(nodes_stack.Count != 0)
                nodes_stack.Peek().Tail = null;
            attacked_node.program.Size.DecreaseCurrentSize(strength);
            removed_nodes.Reverse();
            return removed_nodes;
        }

        protected void IncreaseProgramSpeed(Program attacked_program, ushort _speed)
        {
            attacked_program.Moves.IncreaseSpeed(_speed);
        }

        protected void DecreaseProgramSpeed(Program attacked_program, ushort _speed)
        {
            attacked_program.Moves.DecreaseSpeed(_speed);
        }

        public bool Equals(Command other)
        {
            return Range == other.Range && Name == other.Name && Description == other.Description;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Range.GetHashCode();
                hash = hash * 2 + Name.GetHashCode();
                hash = hash * 3 + Description.GetHashCode();
                return hash;
            }
        }
    }

    // TODO: Create increase Size and add nodes to program Commands.

    // TODO: Create remove square and add square Commands.
}
