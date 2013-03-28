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

    public abstract class ProgramNode : MapNode
    {
        public ProgramTailNode Tail;
        public ProgramNode Head;
        protected Program _program;
        public Program program
        {
            get
            {
                return _program;
            }
        }
    }

    public class ProgramHeadNode : ProgramNode
    {
        public ProgramHeadNode(Program p)
        {
            _program = p;
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
            _program = p;
            Head = head;
            Tail = null;
        }

        public ProgramTailNode(Program p, ProgramNode head, ProgramTailNode tail)
        {
            _program = p;
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

    public class SpawnNode : MapNode
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
