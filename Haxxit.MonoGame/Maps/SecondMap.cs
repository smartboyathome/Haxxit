using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.MonoGame.Programs;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Maps
{
    class SecondMapFactory : SpawnMapFactory
    {
        public SecondMapFactory()
        {
            mapType = MapType.DataMap;
            width = 12;
            height = 7;
            initial_silicoins = 150;
            total_spawn_weight = 30;
            player1 = GlobalAccessors.mPlayer1;
            player2 = new PlayerAI("AI");
            player1_spawns = new List<Point>();
            player2_programs = new List<Tuple<ProgramFactory, Point, IEnumerable<Point>>>();
            player1_spawns.Add(new Point(0, 1));
            player1_spawns.Add(new Point(0, 3));
            player1_spawns.Add(new Point(0, 5));
            WatcherFactory watcherFactory = new WatcherFactory();
            AddPlayer2Program(watcherFactory, new Point(4, 0));
            AddPlayer2Program(watcherFactory, new Point(7, 0));
            AddPlayer2Program(watcherFactory, new Point(4, 6));
            AddPlayer2Program(watcherFactory, new Point(7, 6));
            unavailableNodes = new List<Point>();
            unavailableNodes.Add(new Point(3, 0));
            unavailableNodes.Add(new Point(3, 1));
            unavailableNodes.Add(new Point(3, 2));
            unavailableNodes.Add(new Point(3, 3));
            unavailableNodes.Add(new Point(3, 4));
            unavailableNodes.Add(new Point(3, 5));
            unavailableNodes.Add(new Point(3, 6));
            unavailableNodes.Add(new Point(4, 1));
            unavailableNodes.Add(new Point(4, 2));
            unavailableNodes.Add(new Point(4, 3));
            unavailableNodes.Add(new Point(4, 4));
            unavailableNodes.Add(new Point(4, 5));
            unavailableNodes.Add(new Point(5, 1));
            unavailableNodes.Add(new Point(5, 2));
            unavailableNodes.Add(new Point(5, 3));
            unavailableNodes.Add(new Point(5, 4));
            unavailableNodes.Add(new Point(5, 5));
            unavailableNodes.Add(new Point(6, 1));
            unavailableNodes.Add(new Point(6, 2));
            unavailableNodes.Add(new Point(6, 3));
            unavailableNodes.Add(new Point(6, 4));
            unavailableNodes.Add(new Point(6, 5));
            unavailableNodes.Add(new Point(7, 1));
            unavailableNodes.Add(new Point(7, 2));
            unavailableNodes.Add(new Point(7, 3));
            unavailableNodes.Add(new Point(7, 4));
            unavailableNodes.Add(new Point(7, 5));
            unavailableNodes.Add(new Point(8, 0));
            unavailableNodes.Add(new Point(8, 1));
            unavailableNodes.Add(new Point(8, 2));
            unavailableNodes.Add(new Point(8, 3));
            unavailableNodes.Add(new Point(8, 4));
            unavailableNodes.Add(new Point(8, 5));
            unavailableNodes.Add(new Point(8, 6));
            silicoinNodes.Add(new Point(5, 0));
            silicoinNodes.Add(new Point(6, 0));
            silicoinNodes.Add(new Point(5, 6));
            silicoinNodes.Add(new Point(6, 6));
            dataNodes.Add(new Point(10, 3));
        }

        public override Map NewInstance()
        {
            Map map = base.NewInstance();
            return map;
        }
    }
}
