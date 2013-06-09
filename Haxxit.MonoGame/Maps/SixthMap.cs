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
    class SixthMapFactory : SpawnMapFactory
    {
        public SixthMapFactory()
        {
            mapType = MapType.EnemyMap;
            width = 11;
            height = 11;
            initial_silicoins = 2000;
            total_spawn_weight = 100;
            player1 = GlobalAccessors.mPlayer1;
            player2 = new PlayerAI("AI");
            player1_spawns = new List<Point>();
            player2_programs = new List<Tuple<ProgramFactory, Point, IEnumerable<Point>>>();
            player1_spawns.Add(new Point(0, 3));
            player1_spawns.Add(new Point(0, 4));
            player1_spawns.Add(new Point(0, 5));
            player1_spawns.Add(new Point(0, 6));
            player1_spawns.Add(new Point(0, 7));
            SantaFactory santaFactory = new SantaFactory();
            Point[] santaTailNodes = new Point[24];
            santaTailNodes[0] = new Point(7, 5);
            santaTailNodes[1] = new Point(8, 5);
            santaTailNodes[2] = new Point(8, 6);
            santaTailNodes[3] = new Point(7, 6);
            santaTailNodes[4] = new Point(6, 6);
            santaTailNodes[5] = new Point(6, 7);
            santaTailNodes[6] = new Point(7, 7);
            santaTailNodes[7] = new Point(8, 7);
            santaTailNodes[8] = new Point(9, 7);
            santaTailNodes[9] = new Point(9, 6);
            santaTailNodes[10] = new Point(9, 5);
            santaTailNodes[11] = new Point(9, 4);
            santaTailNodes[12] = new Point(8, 4);
            santaTailNodes[13] = new Point(7, 4);
            santaTailNodes[14] = new Point(6, 4);
            santaTailNodes[15] = new Point(6, 3);
            santaTailNodes[16] = new Point(7, 3);
            santaTailNodes[17] = new Point(8, 3);
            santaTailNodes[18] = new Point(9, 3);
            santaTailNodes[19] = new Point(10, 3);
            santaTailNodes[20] = new Point(10, 4);
            santaTailNodes[21] = new Point(10, 5);
            santaTailNodes[22] = new Point(10, 6);
            santaTailNodes[23] = new Point(10, 7);
            AddPlayer2Program(santaFactory, new Point(6, 5), santaTailNodes);
            unavailableNodes = new List<Point>();
            unavailableNodes.Add(new Point(0, 0));
            unavailableNodes.Add(new Point(1, 0));
            unavailableNodes.Add(new Point(2, 0));
            unavailableNodes.Add(new Point(8, 0));
            unavailableNodes.Add(new Point(9, 0));
            unavailableNodes.Add(new Point(10, 0));
            unavailableNodes.Add(new Point(0, 1));
            unavailableNodes.Add(new Point(1, 1));
            unavailableNodes.Add(new Point(9, 1));
            unavailableNodes.Add(new Point(10, 1));
            unavailableNodes.Add(new Point(0, 2));
            unavailableNodes.Add(new Point(10, 2));
            unavailableNodes.Add(new Point(5, 5));
            unavailableNodes.Add(new Point(0, 8));
            unavailableNodes.Add(new Point(10, 8));
            unavailableNodes.Add(new Point(0, 9));
            unavailableNodes.Add(new Point(1, 9));
            unavailableNodes.Add(new Point(9, 9));
            unavailableNodes.Add(new Point(10, 9));
            unavailableNodes.Add(new Point(0, 10));
            unavailableNodes.Add(new Point(1, 10));
            unavailableNodes.Add(new Point(2, 10));
            unavailableNodes.Add(new Point(8, 10));
            unavailableNodes.Add(new Point(9, 10));
            unavailableNodes.Add(new Point(10, 10));
        }

        public override Map NewInstance()
        {
            Map map = base.NewInstance();
            return map;
        }
    }
}
