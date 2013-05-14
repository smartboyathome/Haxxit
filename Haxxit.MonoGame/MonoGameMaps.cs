using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    public abstract class AbstractMapFactory : IFactory<Map>
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
        private IEnumerable<Tuple<Point, ProgramFactory>> _player1_spawns_programs, _player2_spawns_programs;

        private AbstractMapFactory(int x_size, int y_size)
        {
            _x_size = x_size;
            _y_size = y_size;
            Player1 = CreatePlayer1();
            Player2 = CreatePlayer2();
        }

        public AbstractMapFactory(int x_size, int y_size, IEnumerable<Point> player1_spawns, IEnumerable<Point> player2_spawns) :
            this(x_size, y_size)
        {
            _player1_spawns = player1_spawns;
            _player2_spawns = player2_spawns;
            _player1_spawns_programs = new List<Tuple<Point, ProgramFactory>>();
            _player2_spawns_programs = new List<Tuple<Point, ProgramFactory>>();
        }

        public AbstractMapFactory(int x_size, int y_size,
            IEnumerable<Tuple<Point, ProgramFactory>> player1_spawns_programs,
            IEnumerable<Tuple<Point, ProgramFactory>> player2_spawns_programs) :
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
                foreach (Tuple<Point, ProgramFactory> tuple in _player1_spawns_programs)
                {
                    map.CreateNode(new SpawnNodeFactory(), tuple.Item1);
                    map.SetNodeOwner(tuple.Item1, Player1);
                    map.SpawnProgram(tuple.Item2, tuple.Item1);
                }
                map.TurnDone();
                foreach (Tuple<Point, ProgramFactory> tuple in _player2_spawns_programs)
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
        protected abstract Player CreatePlayer1();
        protected abstract Player CreatePlayer2();
    }
    public class NoWinMapFactory : AbstractMapFactory
    {
        public NoWinMapFactory(int x_size, int y_size, IEnumerable<Point> player1_spawns, IEnumerable<Point> player2_spawns) :
            base(x_size, y_size, player1_spawns, player2_spawns)
        {

        }

        public NoWinMapFactory(int x_size, int y_size,
            IEnumerable<Tuple<Point, ProgramFactory>> player1_spawns_programs,
            IEnumerable<Tuple<Point, ProgramFactory>> player2_spawns_programs) :
            base(x_size, y_size, player1_spawns_programs, player2_spawns_programs)
        {

        }

        protected override Map CreateMap(int x_size, int y_size)
        {
            return new NoWinMap(x_size, y_size);
        }

        protected override Player CreatePlayer1()
        {
            return new Player("Player1");
        }

        protected override Player CreatePlayer2()
        {
            return new Player("Player2");
        }
    }

    public class WinnableEnemyMapFactory : AbstractMapFactory
    {
        public WinnableEnemyMapFactory(int x_size, int y_size, IEnumerable<Point> player1_spawns, IEnumerable<Point> player2_spawns) :
            base(x_size, y_size, player1_spawns, player2_spawns)
        {

        }

        public WinnableEnemyMapFactory(int x_size, int y_size,
            IEnumerable<Tuple<Point, ProgramFactory>> player1_spawns_programs,
            IEnumerable<Tuple<Point, ProgramFactory>> player2_spawns_programs) :
            base(x_size, y_size, player1_spawns_programs, player2_spawns_programs)
        {

        }

        protected override Map CreateMap(int x_size, int y_size)
        {
            return new EnemyMap(x_size, y_size);
        }

        protected override Player CreatePlayer1()
        {
            return new Player("Player1");
        }

        protected override Player CreatePlayer2()
        {
            return new Player("Player2");
        }
    }

    public class WinnableDataMapFactory : AbstractMapFactory
    {
        public WinnableDataMapFactory(int x_size, int y_size, IEnumerable<Point> player1_spawns, IEnumerable<Point> player2_spawns) :
            base(x_size, y_size, player1_spawns, player2_spawns)
        {

        }

        public WinnableDataMapFactory(int x_size, int y_size,
            IEnumerable<Tuple<Point, ProgramFactory>> player1_spawns_programs,
            IEnumerable<Tuple<Point, ProgramFactory>> player2_spawns_programs) :
            base(x_size, y_size, player1_spawns_programs, player2_spawns_programs)
        {

        }

        protected override Map CreateMap(int x_size, int y_size)
        {
            return new DataMap(x_size, y_size);
        }

        protected override Player CreatePlayer1()
        {
            return new Player("Player1");
        }

        protected override Player CreatePlayer2()
        {
            return new Player("Player2");
        }
    }
}
