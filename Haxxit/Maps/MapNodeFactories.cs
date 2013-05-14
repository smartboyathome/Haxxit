using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.Maps
{
    public class UnavailableNodeFactory : IFactory<MapNode>
    {
        public MapNode NewInstance()
        {
            return new UnavailableNode();
        }
    }

    public class AvailableNodeFactory : IFactory<MapNode>
    {
        public MapNode NewInstance()
        {
            return new AvailableNode();
        }
    }

    public class SpawnNodeFactory : IFactory<MapNode>
    {
        public MapNode NewInstance()
        {
            return new SpawnNode();
        }
    }

    public class SilicoinNodeFactory : IFactory<MapNode>
    {
        ushort num_silicoins;

        public SilicoinNodeFactory(ushort num_silicoins)
        {
            this.num_silicoins = num_silicoins;
        }

        public MapNode NewInstance()
        {
            return new SilicoinNode(num_silicoins);
        }
    }

    public class DataNodeFactory : IFactory<MapNode>
    {
        public DataNodeFactory()
        {

        }

        public MapNode NewInstance()
        {
            return new DataNode();
        }
    }

    public class ProgramHeadNodeFactory : IFactory<MapNode>
    {
        ProgramFactory program;
        Player player;

        public ProgramHeadNodeFactory(ProgramFactory program, Player player)
        {
            this.program = program;
            this.player = player;
        }

        public MapNode NewInstance()
        {
            ProgramHeadNode node = new ProgramHeadNode(program.NewInstance());
            node.Player = player;
            return node;
        }
    }

    public class ProgramTailNodeFactory : IFactory<MapNode>
    {
        ProgramHeadNode head;
        ProgramNode previous;
        ProgramTailNode next;

        public ProgramTailNodeFactory(ProgramHeadNode head, ProgramNode previous, ProgramTailNode next=null)
        {
            this.head = head;
            this.previous = previous;
            this.next = next;
        }

        public MapNode NewInstance()
        {
            ProgramTailNode node = new ProgramTailNode(head);
            if (previous != null)
            {
                node.Head = previous;
                previous.Tail = node;
            }
            if (next != null)
            {
                node.Tail = next;
                next.Head = node;
            }
            node.Player = head.Player;
            return node;
        }
    }
}
