using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Commands;
using HaxxitTest = SmartboyDevelopments.Haxxit.Tests;
using SmartboyDevelopments.Haxxit.MonoGame.Programs;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Maps
{
    class SpawnMapFactory : IFactory<Map>
    {
        ushort width, height, initial_silicoins, total_spawn_weight;
        List<Point> player1_spawns;
        List<Tuple<ProgramFactory, Point, IEnumerable<Point>>> player2_programs;
        Player player1, player2;

        public SpawnMapFactory()
        {
            width = 10;
            height = 7;
            initial_silicoins = 500;
            total_spawn_weight = 30;
            player1 = new Player("Bob");
            player2 = new PlayerAI("Jane");
            player1_spawns = new List<Point>();
            player2_programs = new List<Tuple<ProgramFactory, Point, IEnumerable<Point>>>();
            player1_spawns.Add(new Point(2, 2));
            player1_spawns.Add(new Point(3, 4));
            AddPlayer2Program(new SentinelFactory(), new Point(6, 2), new Point(6, 3), new Point(6, 4));
            player1.AddProgram(new BugFactory());
            player1.AddProgram(new HackFactory());
            player1.AddProgram(new SlingshotFactory());
        }

        private void AddPlayer2Program(ProgramFactory program, Point head, params Point[] tail)
        {
            player2_programs.Add(
                new Tuple<ProgramFactory, Point, IEnumerable<Point>>(
                    program,
                    head,
                    tail
                )
            );
        }

        public Map NewInstance()
        {
            Map map = new EnemyMap(width, height, initial_silicoins, total_spawn_weight);
            map.AddPlayer(player1);
            map.AddPlayer(player2);
            map.CreateNodes(new AvailableNodeFactory(), 0, 0, width - 1, height - 1);
            foreach (Point p in player1_spawns)
            {
                map.CreateNode(new SpawnNodeFactory(), p);
                map.SetNodeOwner(p, player1);
            }
            foreach (Tuple<ProgramFactory, Point, IEnumerable<Point>> tuple in player2_programs)
            {
                map.CreateNode(new ProgramHeadNodeFactory(tuple.Item1, player2), tuple.Item2);
                ProgramHeadNode head = map.GetNode<ProgramHeadNode>(tuple.Item2);
                ProgramNode previous = head;
                foreach (Point p in tuple.Item3)
                {
                    map.CreateNode(new ProgramTailNodeFactory(head, previous), p);
                    previous = map.GetNode<ProgramNode>(p);
                }
            }
            return map;
        }
    }
}
