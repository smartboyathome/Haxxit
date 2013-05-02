using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.Tests
{
    [TestClass]
    public class PlayerMapTests
    {
        static DynamicProgramFactory BasicProgramFactory, FasterProgramFactory,
            BiggerFasterProgramFactory;
        static AbstractPlayerMapFactory PlayerMapFactory, PreloadedMapFactory,
            WinnableMapFactory;

        private static void InitializePlayerMapFactory()
        {
            List<Point> player1_spawns = new List<Point>();
            List<Point> player2_spawns = new List<Point>();
            player1_spawns.Add(new Point(0, 1));
            player1_spawns.Add(new Point(1, 0));
            player2_spawns.Add(new Point(9, 8));
            player2_spawns.Add(new Point(8, 9));
            PlayerMapFactory = new PlayerMapFactory(10, 10, player1_spawns, player2_spawns);
        }

        private static void InitializePreloadedMapFactory()
        {
            List<Tuple<Point, IFactory<Program>>> player1_spawns = new List<Tuple<Point, IFactory<Program>>>();
            List<Tuple<Point, IFactory<Program>>> player2_spawns = new List<Tuple<Point, IFactory<Program>>>();
            player1_spawns.Add(new Tuple<Point, IFactory<Program>>(new Point(0, 1), FasterProgramFactory));
            player1_spawns.Add(new Tuple<Point, IFactory<Program>>(new Point(1, 0), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, IFactory<Program>>(new Point(9, 8), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, IFactory<Program>>(new Point(8, 9), FasterProgramFactory));
            PreloadedMapFactory = new PlayerMapFactory(10, 10, player1_spawns, player2_spawns);
        }

        private static void InitializeWinnableMapFactory()
        {
            List<Tuple<Point, IFactory<Program>>> player1_spawns = new List<Tuple<Point, IFactory<Program>>>();
            List<Tuple<Point, IFactory<Program>>> player2_spawns = new List<Tuple<Point, IFactory<Program>>>();
            player1_spawns.Add(new Tuple<Point, IFactory<Program>>(new Point(0, 1), FasterProgramFactory));
            player1_spawns.Add(new Tuple<Point, IFactory<Program>>(new Point(1, 0), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, IFactory<Program>>(new Point(9, 8), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, IFactory<Program>>(new Point(8, 9), FasterProgramFactory));
            WinnableMapFactory = new WinnableMapFactory(10, 10, player1_spawns, player2_spawns);
        }

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            List<Command> commands = new List<Command>();
            commands.Add(new DynamicDamageCommand(2, 3));
            BasicProgramFactory = new DynamicProgramFactory(4, 4, commands);
            FasterProgramFactory = new DynamicProgramFactory(8, 4, commands);
            BiggerFasterProgramFactory = new DynamicProgramFactory(16, 8, commands);
            InitializePlayerMapFactory();
            InitializePreloadedMapFactory();
            InitializeWinnableMapFactory();
        }

        [TestMethod]
        public void OwnedNodesCanHavePlayers()
        {
            Map map = PlayerMapFactory.NewInstance();
            Assert.AreEqual<Player>(PlayerMapFactory.Player1, map.CurrentPlayer);
            map.SpawnProgram(BasicProgramFactory, 0, 1);
            map.SpawnProgram(BasicProgramFactory, 1, 0);
            Assert.IsTrue(map.NodeIsType<OwnedNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<OwnedNode>(1, 0));
            Assert.AreEqual<Player>(PlayerMapFactory.Player1, ((OwnedNode)map.GetNode(0, 1)).Player);
            Assert.AreEqual<Player>(PlayerMapFactory.Player1, ((OwnedNode)map.GetNode(1, 0)).Player);
            map.TurnDone();
            Assert.AreEqual<Player>(PlayerMapFactory.Player2, map.CurrentPlayer);
            map.SpawnProgram(BasicProgramFactory, 9, 8);
            map.SpawnProgram(BasicProgramFactory, 8, 9);
            Assert.IsTrue(map.NodeIsType<OwnedNode>(9, 8));
            Assert.IsTrue(map.NodeIsType<OwnedNode>(8, 9));
            Assert.AreEqual<Player>(PlayerMapFactory.Player2, ((OwnedNode)map.GetNode(9, 8)).Player);
            Assert.AreEqual<Player>(PlayerMapFactory.Player2, ((OwnedNode)map.GetNode(8, 9)).Player);
            map.TurnDone();
            Assert.AreEqual<Player>(PlayerMapFactory.Player1, map.CurrentPlayer);
        }

        [TestMethod]
        public void PlayersCanMoveOnlyOwnedPrograms()
        {
            Map map = PreloadedMapFactory.NewInstance();
            Assert.AreEqual<Player>(PreloadedMapFactory.Player1, map.CurrentPlayer);
            Assert.IsTrue(map.NodeIsType<OwnedNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<OwnedNode>(1, 0));
            Assert.IsTrue(map.NodeIsType<OwnedNode>(9, 8));
            Assert.IsTrue(map.NodeIsType<OwnedNode>(8, 9));
            Assert.AreEqual<Player>(PlayerMapFactory.Player1, ((OwnedNode)map.GetNode(0, 1)).Player);
            Assert.AreEqual<Player>(PlayerMapFactory.Player1, ((OwnedNode)map.GetNode(1, 0)).Player);
            Assert.AreEqual<Player>(PlayerMapFactory.Player2, ((OwnedNode)map.GetNode(9, 8)).Player);
            Assert.AreEqual<Player>(PlayerMapFactory.Player2, ((OwnedNode)map.GetNode(8, 9)).Player);
            Assert.IsFalse(map.MoveProgram(new Point(9, 8), new Point(0, -1)));
            Assert.IsFalse(map.MoveProgram(new Point(8, 9), new Point(0, -1)));
            Assert.IsTrue(map.MoveProgram(new Point(1, 0), new Point(0, 1)));
            Assert.IsTrue(map.MoveProgram(new Point(0, 1), new Point(0, 1)));
            map.TurnDone();
            Assert.AreEqual<Player>(PreloadedMapFactory.Player2, map.CurrentPlayer);
            Assert.IsTrue(map.MoveProgram(new Point(9, 8), new Point(0, -1)));
            Assert.IsTrue(map.MoveProgram(new Point(8, 9), new Point(0, -1)));
            Assert.IsFalse(map.MoveProgram(new Point(1, 1), new Point(0, 1)));
            Assert.IsFalse(map.MoveProgram(new Point(0, 2), new Point(0, 1)));
        }

        [TestMethod]
        public void PlayersCanOnlyAttackOwnedPrograms()
        {
            Map map = PreloadedMapFactory.NewInstance();
            List<Point> moves = new List<Point>();
            moves.Add(new Point(0, 1));
            moves.Add(new Point(0, 1));
            moves.Add(new Point(0, 1));
            moves.Add(new Point(0, 1));
            moves.Add(new Point(1, 0));
            moves.Add(new Point(1, 0));
            moves.Add(new Point(1, 0));
            moves.Add(new Point(1, 0));
            Point p1_start = new Point(0, 1);
            Point p2_start = new Point(1, 0);
            foreach(Point p in moves)
            {
                Assert.IsTrue(map.MoveProgram(p1_start, p));
                Assert.IsTrue(map.MoveProgram(p2_start, p));
                p1_start += p;
                p2_start += p;
            }
            map.TurnDone();
            moves.RemoveAt(7);
            Point p3_start = new Point(8, 9);
            Point p4_start = new Point(9, 8);
            foreach(Point p in moves)
            {
                Point _p = p * new Point(-1, -1);
                Assert.IsTrue(map.MoveProgram(p3_start, _p));
                Assert.IsTrue(map.MoveProgram(p4_start, _p));
                p3_start += _p;
                p4_start += _p;
            }
            Assert.IsNull(map.RunCommand(p4_start, p3_start, "Damage"));
            Assert.IsNotNull(map.RunCommand(p3_start, p1_start, "Damage"));
        }

        [TestMethod]
        public void PlayersCanWinGame()
        {
            Map map = WinnableMapFactory.NewInstance();
            List<Player> winners = new List<Player>();
            Action<string, object, EventArgs> action = (x, y, z) => winners.Add(((HackedEventArgs)z).WinningPlayer);
            map.Mediator.Subscribe("haxxit.map.hacked", action);
            List<Point> moves = new List<Point>();
            moves.Add(new Point(0, 1));
            moves.Add(new Point(0, 1));
            moves.Add(new Point(0, 1));
            moves.Add(new Point(0, 1));
            moves.Add(new Point(1, 0));
            moves.Add(new Point(1, 0));
            moves.Add(new Point(1, 0));
            moves.Add(new Point(1, 0));
            Point p1_start = new Point(0, 1);
            Point p2_start = new Point(1, 0);
            foreach (Point p in moves)
            {
                Assert.IsTrue(map.MoveProgram(p1_start, p));
                Assert.IsTrue(map.MoveProgram(p2_start, p));
                p1_start += p;
                p2_start += p;
            }
            map.TurnDone();
            moves.RemoveAt(7);
            Point p3_start = new Point(8, 9);
            Point p4_start = new Point(9, 8);
            foreach (Point p in moves)
            {
                Point _p = p * new Point(-1, -1);
                Assert.IsTrue(map.MoveProgram(p3_start, _p));
                Assert.IsTrue(map.MoveProgram(p4_start, _p));
                p3_start += _p;
                p4_start += _p;
            }
            Assert.IsNotNull(map.RunCommand(p3_start, p1_start, "Damage"));
            Assert.IsNotNull(map.RunCommand(p4_start, p1_start, "Damage"));
            map.TurnDone();
            map.TurnDone();
            Assert.IsNotNull(map.RunCommand(p3_start, p2_start, "Damage"));
            Assert.IsNotNull(map.RunCommand(p4_start, p2_start, "Damage"));
            Assert.AreEqual(1, winners.Count);
            Assert.AreEqual<Player>(WinnableMapFactory.Player2, winners[0]);
        }
    }
}
