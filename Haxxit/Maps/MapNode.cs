using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.Maps
{
    public abstract class MapNode
    {
        public abstract bool CanHoldProgram();
        public abstract bool IsTaken();

        public Point coordinate;
    }

    public class UnavailableNode : MapNode
    {
        public UnavailableNode()
        {

        }

        public override bool CanHoldProgram()
        {
            return false;
        }

        public override bool IsTaken()
        {
            return true;
        }
    }

    public class AvailableNode : MapNode
    {
        public AvailableNode()
        {
            
        }

        public override bool CanHoldProgram()
        {
            return true;
        }

        public override bool IsTaken()
        {
            return false;
        }
    }

    public abstract class OwnedNode : MapNode
    {
        public Player Player
        {
            get;
            set;
        }
    }

    public abstract class ProgramNode : OwnedNode
    {
        public ProgramTailNode Tail;
        public ProgramNode Head;
        public Program Program
        {
            get;
            protected set;
        }
        public IEnumerable<ProgramNode> GetAllNodes()
        {
            List<ProgramNode> nodes = new List<ProgramNode>();
            nodes.Add(this);
            while (nodes.First().Head != null)
                nodes.Add(nodes.First().Head);
            while (nodes.Last().Tail != null)
                nodes.Add(nodes.Last().Tail);
            return nodes;
        }
    }

    public class ProgramHeadNode : ProgramNode
    {
        public ProgramHeadNode(Program p)
        {
            Program = p;
            Tail = null;
            Head = null;
        }

        public override bool CanHoldProgram()
        {
            return true;
        }

        public override bool IsTaken()
        {
            return true;
        }
    }

    public class ProgramTailNode : ProgramNode
    {
        public ProgramTailNode(Program p, ProgramNode head)
        {
            Program = p;
            Head = head;
            Tail = null;
        }

        public ProgramTailNode(Program p, ProgramNode head, ProgramTailNode tail)
        {
            Program = p;
            Head = head;
            Tail = tail;
        }

        public override bool CanHoldProgram()
        {
            return true;
        }

        public override bool IsTaken()
        {
            return true;
        }
    }

    public class SpawnNode : OwnedNode
    {
        private Program p;

        public SpawnNode()
        {
            p = null;
        }

        public SpawnNode(Program p)
        {
            this.p = p;
        }

        public override bool CanHoldProgram()
        {
            return true;
        }

        public override bool IsTaken()
        {
            return p != null;
        }

        public Program program
        {
            get
            {
                return p;
            }
            set
            {
                p = value;
            }
        }

        public void ClearProgram()
        {
            this.p = null;
        }
    }
}
