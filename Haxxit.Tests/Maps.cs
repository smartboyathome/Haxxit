using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;

namespace SmartboyDevelopments.Haxxit.Tests
{
    class BasicMap: Map
    {
        public BasicMap(int x_size, int y_size) : base()
        {
            map = new MapNode[x_size, y_size];
            CreateNodes<AvailableNode>(0, 0, x_size-1, y_size-1);
        }
    }
}
