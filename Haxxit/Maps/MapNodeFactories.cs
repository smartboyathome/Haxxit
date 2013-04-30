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
}
