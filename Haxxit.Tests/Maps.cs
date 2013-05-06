using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit.Tests
{
    public class BasicMapFactory : IFactory<Map>
    {
        private int _x_size, _y_size;
        private IEnumerable<Point> _spawns;

        public BasicMapFactory(int x_size, int y_size) :
            this(x_size, y_size, new List<Point>())
        {

        }

        public BasicMapFactory(int x_size, int y_size, IEnumerable<Point> spawns)
        {
            _x_size = x_size;
            _y_size = y_size;
            _spawns = spawns;
        }

        public Map NewInstance()
        {
            Map map = new NoWinMap(_x_size, _y_size);
            map.Mediator = new SynchronousMediator();
            map.CreateNodes(new AvailableNodeFactory(), 0, 0, _x_size - 1, _y_size - 1);
            foreach (Point p in _spawns)
            {
                map.CreateNode(new SpawnNodeFactory(), p);
            }
            return map;
        }
    }

    public abstract class AbstractPlayerMapFactory : IFactory<Map>
    {
        public Player Player1
        {
            get;
            private set;
        }

        public Player Player2
        {
            get;
            private set;
        }

        private int _x_size, _y_size;
        private IEnumerable<Point> _player1_spawns, _player2_spawns;
        private IEnumerable<Tuple<Point, IFactory<Program>>> _player1_spawns_programs, _player2_spawns_programs;

        private AbstractPlayerMapFactory(int x_size, int y_size)
        {
            _x_size = x_size;
            _y_size = y_size;
            Player1 = new Player("Player1");
            Player2 = new Player("Player2");
        }

        public AbstractPlayerMapFactory(int x_size, int y_size, IEnumerable<Point> player1_spawns, IEnumerable<Point> player2_spawns) :
            this(x_size, y_size)
        {
            _player1_spawns = player1_spawns;
            _player2_spawns = player2_spawns;
            _player1_spawns_programs = new List<Tuple<Point, IFactory<Program>>>();
            _player2_spawns_programs = new List<Tuple<Point, IFactory<Program>>>();
        }

        public AbstractPlayerMapFactory(int x_size, int y_size,
            IEnumerable<Tuple<Point, IFactory<Program>>> player1_spawns_programs,
            IEnumerable<Tuple<Point, IFactory<Program>>> player2_spawns_programs) :
            this(x_size, y_size)
        {
            _player1_spawns = new List<Point>();
            _player2_spawns = new List<Point>();
            _player1_spawns_programs = player1_spawns_programs;
            _player2_spawns_programs = player2_spawns_programs;
        }

        public Map NewInstance()
        {
            Map map = CreateMap(_x_size, _y_size);
            map.Mediator = new SynchronousMediator();
            map.AddPlayer(Player1);
            map.AddPlayer(Player2);
            map.CreateNodes(new AvailableNodeFactory(), 0, 0, _x_size - 1, _y_size - 1);
            if (_player1_spawns.Count() > 0 && _player2_spawns.Count() > 0)
            {
                foreach (Point p in _player1_spawns)
                {
                    map.CreateNode(new SpawnNodeFactory(), p);
                    map.SetNodeOwner(p, Player1);
                }
                foreach (Point p in _player2_spawns)
                {
                    map.CreateNode(new SpawnNodeFactory(), p);
                    map.SetNodeOwner(p, Player2);
                }
            }
            else if (_player1_spawns_programs.Count() > 0 && _player2_spawns_programs.Count() > 0)
            {
                foreach (Tuple<Point, IFactory<Program>> tuple in _player1_spawns_programs)
                {
                    map.CreateNode(new SpawnNodeFactory(), tuple.Item1);
                    map.SetNodeOwner(tuple.Item1, Player1);
                    map.SpawnProgram(tuple.Item2, tuple.Item1);
                }
                map.TurnDone();
                foreach (Tuple<Point, IFactory<Program>> tuple in _player2_spawns_programs)
                {
                    map.CreateNode(new SpawnNodeFactory(), tuple.Item1);
                    map.SetNodeOwner(tuple.Item1, Player2);
                    map.SpawnProgram(tuple.Item2, tuple.Item1);
                }
                map.TurnDone();
                map.FinishedSpawning();
            }
            return map;
        }

        protected abstract Map CreateMap(int x_size, int y_size);
    }

    public class PlayerMapFactory : AbstractPlayerMapFactory
    {
        public PlayerMapFactory(int x_size, int y_size, IEnumerable<Point> player1_spawns, IEnumerable<Point> player2_spawns) :
            base(x_size, y_size, player1_spawns, player2_spawns)
        {

        }

        public PlayerMapFactory(int x_size, int y_size,
            IEnumerable<Tuple<Point, IFactory<Program>>> player1_spawns_programs,
            IEnumerable<Tuple<Point, IFactory<Program>>> player2_spawns_programs) :
            base(x_size, y_size, player1_spawns_programs, player2_spawns_programs)
        {
            
        }

        protected override Map CreateMap(int x_size, int y_size)
        {
            return new NoWinMap(x_size, y_size);
        }
    }

    public class WinnableEnemyMapFactory : AbstractPlayerMapFactory
    {
        public WinnableEnemyMapFactory(int x_size, int y_size, IEnumerable<Point> player1_spawns, IEnumerable<Point> player2_spawns) :
            base(x_size, y_size, player1_spawns, player2_spawns)
        {

        }

        public WinnableEnemyMapFactory(int x_size, int y_size,
            IEnumerable<Tuple<Point, IFactory<Program>>> player1_spawns_programs,
            IEnumerable<Tuple<Point, IFactory<Program>>> player2_spawns_programs) :
            base(x_size, y_size, player1_spawns_programs, player2_spawns_programs)
        {

        }

        protected override Map CreateMap(int x_size, int y_size)
        {
            return new EnemyMap(x_size, y_size);
        }
    }

    public class WinnableDataMapFactory : AbstractPlayerMapFactory
    {
        public WinnableDataMapFactory(int x_size, int y_size, IEnumerable<Point> player1_spawns, IEnumerable<Point> player2_spawns) :
            base(x_size, y_size, player1_spawns, player2_spawns)
        {

        }

        public WinnableDataMapFactory(int x_size, int y_size,
            IEnumerable<Tuple<Point, IFactory<Program>>> player1_spawns_programs,
            IEnumerable<Tuple<Point, IFactory<Program>>> player2_spawns_programs) :
            base(x_size, y_size, player1_spawns_programs, player2_spawns_programs)
        {

        }

        protected override Map CreateMap(int x_size, int y_size)
        {
            return new DataMap(x_size, y_size);
        }
    }
}
