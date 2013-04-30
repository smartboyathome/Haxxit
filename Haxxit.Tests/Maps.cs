using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit.Tests
{
    class BasicMapFactory : IFactory<Map>
    {
        private int _x_size, _y_size;
        private IEnumerable<Point> _spawns;

        public BasicMapFactory(int x_size, int y_size)
        {
            _x_size = x_size;
            _y_size = y_size;
            _spawns = new List<Point>();
        }

        public BasicMapFactory(int x_size, int y_size, IEnumerable<Point> spawns)
        {
            _x_size = x_size;
            _y_size = y_size;
            _spawns = spawns;
        }

        public Map NewInstance()
        {
            Map map = new Map(_x_size, _y_size);
            map.Mediator = new SynchronousMediator();
            map.CreateNodes(new AvailableNodeFactory(), 0, 0, _x_size - 1, _y_size - 1);
            foreach (Point p in _spawns)
            {
                map.CreateNode(new SpawnNodeFactory(), p);
            }
            return map;
        }
    }
}
