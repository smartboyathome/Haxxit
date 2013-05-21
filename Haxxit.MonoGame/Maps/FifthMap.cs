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
    class FifthMapFactory : SpawnMapFactory
    {
        public FifthMapFactory()
        {
            mapType = MapType.EnemyMap;
            width = 16;
            height = 15;
            initial_silicoins = 250;
            total_spawn_weight = 40;
            player1 = GlobalAccessors.mPlayer1;
            player2 = new PlayerAI("AI");
            player1_spawns = new List<Point>();
            player2_programs = new List<Tuple<ProgramFactory, Point, IEnumerable<Point>>>();
            player1_spawns.Add(new Point(0, 1));
            player1_spawns.Add(new Point(0, 2));
            BrawlerFactory brawlerFactory = new BrawlerFactory();
            AddPlayer2Program(brawlerFactory, new Point(14, 12));
            AddPlayer2Program(brawlerFactory, new Point(14, 13));
            AddPlayer2Program(brawlerFactory, new Point(14, 14));
            unavailableNodes = new List<Point>();
            unavailableNodes.Add(new Point(0, 4));
            unavailableNodes.Add(new Point(1, 4));
            unavailableNodes.Add(new Point(2, 4));
            unavailableNodes.Add(new Point(3, 4));
            unavailableNodes.Add(new Point(4, 4));
            unavailableNodes.Add(new Point(5, 4));
            unavailableNodes.Add(new Point(6, 4));
            unavailableNodes.Add(new Point(7, 4));
            unavailableNodes.Add(new Point(15, 4));
            unavailableNodes.Add(new Point(0, 5));
            unavailableNodes.Add(new Point(1, 5));
            unavailableNodes.Add(new Point(2, 5));
            unavailableNodes.Add(new Point(3, 5));
            unavailableNodes.Add(new Point(4, 5));
            unavailableNodes.Add(new Point(5, 5));
            unavailableNodes.Add(new Point(6, 5));
            unavailableNodes.Add(new Point(14, 5));
            unavailableNodes.Add(new Point(15, 5));
            unavailableNodes.Add(new Point(0, 6));
            unavailableNodes.Add(new Point(1, 6));
            unavailableNodes.Add(new Point(2, 6));
            unavailableNodes.Add(new Point(3, 6));
            unavailableNodes.Add(new Point(4, 6));
            unavailableNodes.Add(new Point(5, 6));
            unavailableNodes.Add(new Point(13, 6));
            unavailableNodes.Add(new Point(14, 6));
            unavailableNodes.Add(new Point(15, 6));
            unavailableNodes.Add(new Point(0, 7));
            unavailableNodes.Add(new Point(1, 7));
            unavailableNodes.Add(new Point(2, 7));
            unavailableNodes.Add(new Point(3, 7));
            unavailableNodes.Add(new Point(4, 7));
            unavailableNodes.Add(new Point(12, 7));
            unavailableNodes.Add(new Point(13, 7));
            unavailableNodes.Add(new Point(14, 7));
            unavailableNodes.Add(new Point(15, 7));
            unavailableNodes.Add(new Point(0, 8));
            unavailableNodes.Add(new Point(1, 8));
            unavailableNodes.Add(new Point(2, 8));
            unavailableNodes.Add(new Point(3, 8));
            unavailableNodes.Add(new Point(11, 8));
            unavailableNodes.Add(new Point(12, 8));
            unavailableNodes.Add(new Point(13, 8));
            unavailableNodes.Add(new Point(14, 8));
            unavailableNodes.Add(new Point(15, 8));
            unavailableNodes.Add(new Point(0, 9));
            unavailableNodes.Add(new Point(1, 9));
            unavailableNodes.Add(new Point(2, 9));
            unavailableNodes.Add(new Point(10, 9));
            unavailableNodes.Add(new Point(11, 9));
            unavailableNodes.Add(new Point(12, 9));
            unavailableNodes.Add(new Point(13, 9));
            unavailableNodes.Add(new Point(14, 9));
            unavailableNodes.Add(new Point(15, 9));
            unavailableNodes.Add(new Point(0, 10));
            unavailableNodes.Add(new Point(1, 10));
            unavailableNodes.Add(new Point(9, 10));
            unavailableNodes.Add(new Point(10, 10));
            unavailableNodes.Add(new Point(11, 10));
            unavailableNodes.Add(new Point(12, 10));
            unavailableNodes.Add(new Point(13, 10));
            unavailableNodes.Add(new Point(14, 10));
            unavailableNodes.Add(new Point(15, 10));
            unavailableNodes.Add(new Point(0, 11));
            unavailableNodes.Add(new Point(8, 11));
            unavailableNodes.Add(new Point(9, 11));
            unavailableNodes.Add(new Point(10, 11));
            unavailableNodes.Add(new Point(11, 11));
            unavailableNodes.Add(new Point(12, 11));
            unavailableNodes.Add(new Point(13, 11));
            unavailableNodes.Add(new Point(14, 11));
            unavailableNodes.Add(new Point(15, 11));
        }

        public override Map NewInstance()
        {
            Map map = base.NewInstance();
            return map;
        }
    }
}
