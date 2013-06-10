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
    class ThirdMapFactory : SpawnMapFactory
    {
        public ThirdMapFactory()
        {
            mapType = MapType.EnemyMap;
            width = 20;
            height = 10;
            initial_silicoins = 1000;
            total_spawn_weight = 100;
            player1 = GlobalAccessors.mPlayer1;
            player2 = new PlayerAI("AI");
            player1_spawns = new List<Point>();
            player2_programs = new List<Tuple<ProgramFactory, Point, IEnumerable<Point>>>();
            player1_spawns.Add(new Point(0, 2));
            player1_spawns.Add(new Point(0, 3));
            player1_spawns.Add(new Point(0, 6));
            player1_spawns.Add(new Point(0, 7));
            SwarmerFactory swarmerFactory = new SwarmerFactory();
            AddPlayer2Program(swarmerFactory, new Point(19, 1));
            AddPlayer2Program(swarmerFactory, new Point(19, 2));
            AddPlayer2Program(swarmerFactory, new Point(19, 4));
            AddPlayer2Program(swarmerFactory, new Point(19, 5));
            AddPlayer2Program(swarmerFactory, new Point(19, 7));
            AddPlayer2Program(swarmerFactory, new Point(19, 8));
            unavailableNodes = new List<Point>();
            unavailableNodes.Add(new Point(3, 3));
            unavailableNodes.Add(new Point(3, 4));
            unavailableNodes.Add(new Point(3, 5));
            unavailableNodes.Add(new Point(3, 6));
            unavailableNodes.Add(new Point(7, 2));
            unavailableNodes.Add(new Point(6, 1));
            unavailableNodes.Add(new Point(5, 0));
            unavailableNodes.Add(new Point(5, 9));
            unavailableNodes.Add(new Point(6, 8));
            unavailableNodes.Add(new Point(7, 7));
            unavailableNodes.Add(new Point(9, 4));
            unavailableNodes.Add(new Point(9, 5));
            unavailableNodes.Add(new Point(10, 4));
            unavailableNodes.Add(new Point(10, 5));
            unavailableNodes.Add(new Point(16, 3));
            unavailableNodes.Add(new Point(16, 4));
            unavailableNodes.Add(new Point(16, 5));
            unavailableNodes.Add(new Point(16, 6));
            unavailableNodes.Add(new Point(12, 2));
            unavailableNodes.Add(new Point(13, 1));
            unavailableNodes.Add(new Point(13, 0));
            unavailableNodes.Add(new Point(12, 7));
            unavailableNodes.Add(new Point(13, 8));
            unavailableNodes.Add(new Point(14, 9));
        }

        public override Map NewInstance()
        {
            Map map = base.NewInstance();
            return map;
        }
    }
}
