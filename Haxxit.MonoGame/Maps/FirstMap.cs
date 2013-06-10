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
    class FirstMapFactory : SpawnMapFactory
    {
        public FirstMapFactory()
        {
            mapType = MapType.EnemyMap;
            width = 7;
            height = 7;
            initial_silicoins = 500;
            total_spawn_weight = 20;
            player1 = GlobalAccessors.mPlayer1;
            player2 = new PlayerAI("AI");
            player1_spawns = new List<Point>();
            player2_programs = new List<Tuple<ProgramFactory, Point, IEnumerable<Point>>>();
            player1_spawns.Add(new Point(0, 2));
            player1_spawns.Add(new Point(0, 4));
            AddPlayer2Program(new SentryFactory(), new Point(6, 3));
            unavailableNodes = new List<Point>();
            unavailableNodes.Add(new Point(3, 2));
            unavailableNodes.Add(new Point(2, 3));
            unavailableNodes.Add(new Point(3, 3));
            unavailableNodes.Add(new Point(4, 3));
            unavailableNodes.Add(new Point(3, 4));
        }

        public override Map NewInstance()
        {
            Map map = base.NewInstance();
            return map;
        }
    }
}
