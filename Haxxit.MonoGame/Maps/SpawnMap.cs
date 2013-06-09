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
    abstract class SpawnMapFactory : IFactory<Map>
    {
        protected enum MapType
        {
            EnemyMap,
            DataMap
        }

        protected ushort width, height, initial_silicoins, total_spawn_weight;
        protected List<Point> player1_spawns;
        protected List<Tuple<ProgramFactory, Point, IEnumerable<Point>>> player2_programs;
        protected Player player1, player2;
        protected List<Point> unavailableNodes;
        protected Dictionary<Point, ushort> silicoinNodes;
        protected List<Point> dataNodes;
        protected MapType mapType;

        public SpawnMapFactory()
        {
            mapType = MapType.EnemyMap;
            width = 0;
            height = 0;
            initial_silicoins = 0;
            total_spawn_weight = 0;
            player1 = GlobalAccessors.mPlayer1;
            player2 = new PlayerAI("AI");
            player1_spawns = new List<Point>();
            player2_programs = new List<Tuple<ProgramFactory, Point, IEnumerable<Point>>>();
            unavailableNodes = new List<Point>();
            silicoinNodes = new Dictionary<Point, ushort>();
            dataNodes = new List<Point>();
        }

        protected void AddPlayer2Program(ProgramFactory program, Point head, params Point[] tail)
        {
            player2_programs.Add(
                new Tuple<ProgramFactory, Point, IEnumerable<Point>>(
                    program,
                    head,
                    tail
                )
            );
        }

        public virtual Map NewInstance()
        {
            Map map = null;
            if (mapType == MapType.EnemyMap)
            {
                map = new EnemyMap(width, height, initial_silicoins, total_spawn_weight);
            }
            else if (mapType == MapType.DataMap)
            {
                map = new MonoGameDataMap(width, height, initial_silicoins, total_spawn_weight);
            }
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

            UnavailableNodeFactory unavailableNodeFactory = new UnavailableNodeFactory();
            foreach (Point unavailableNode in unavailableNodes)
            {
                map.CreateNode(unavailableNodeFactory, unavailableNode);
            }

            foreach (Point node in silicoinNodes.Keys)
            {
                map.CreateNode(new SilicoinNodeFactory(silicoinNodes[node]), node);
            }

            DataNodeFactory dataNodeFactory = new DataNodeFactory();
            foreach (Point dataNode in dataNodes)
            {
                map.CreateNode(dataNodeFactory, dataNode);
            }

            return map;
        }
    }
}
