using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit.Maps
{
    public abstract class MapNode
    {
        private NotifiableManager _notifiable_manager;
        public INotifiable Notifiable
        {
            get
            {
                return _notifiable_manager.Notifiable;
            }
            set
            {
                _notifiable_manager.Notifiable = value;
            }
        }

        public abstract bool CanHoldProgram();
        public abstract bool IsTaken();

        protected MapNode()
        {
            _notifiable_manager = new NotifiableManager();
        }

        public Point coordinate;
    }

    public class UnavailableNode : MapNode
    {
        public UnavailableNode() :
            base()
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
        public AvailableNode() :
            base()
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

        public virtual void MovedOnto()
        {

        }
    }

    public class SilicoinNode : AvailableNode
    {
        ushort num_silicoins;

        public SilicoinNode(ushort num_silicoins) :
            base()
        {
            this.num_silicoins = num_silicoins;
        }
        
        public override void MovedOnto()
        {
            Notifiable.Notify("haxxit.map.silicoins.add", this, new SilicoinEventArgs(num_silicoins));
        }
    }

    public class DataNode : AvailableNode
    {
        public DataNode() : base()
        {

        }

        public override void MovedOnto()
        {
            Notifiable.Notify("haxxit.map.hacked.check", this, new EventArgs());
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
        public ProgramHeadNode(Program p) :
            base()
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
        public ProgramTailNode(ProgramNode head) : base()
        {
            Program = head.Program;
            Head = head;
            Tail = null;
        }

        public ProgramTailNode(Program p, ProgramNode head) :
            base()
        {
            Program = p;
            Head = head;
            Tail = null;
        }

        public ProgramTailNode(Program p, ProgramNode head, ProgramTailNode tail) :
            base()
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
        private ProgramFactory p;

        public SpawnNode() :
            base()
        {
            p = null;
        }

        public SpawnNode(ProgramFactory p) :
            base()
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

        public ProgramFactory program
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
