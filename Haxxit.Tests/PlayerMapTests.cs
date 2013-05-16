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
        static AbstractPlayerMapFactory PlayerMapFactory, PreloadedMapFactory, TinyPreloadedMapFactory,
            WinnableEnemyMapFactory, WinnableDataMapFactory, TinyWinnableEnemyMapFactory;

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
            List<Tuple<Point, ProgramFactory>> player1_spawns = new List<Tuple<Point, ProgramFactory>>();
            List<Tuple<Point, ProgramFactory>> player2_spawns = new List<Tuple<Point, ProgramFactory>>();
            player1_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(0, 1), FasterProgramFactory));
            player1_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(1, 0), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(9, 8), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(8, 9), FasterProgramFactory));
            PreloadedMapFactory = new PlayerMapFactory(10, 10, player1_spawns, player2_spawns);
        }

        private static void InitializeTinyPreloadedMapFactory()
        {
            List<Tuple<Point, ProgramFactory>> player1_spawns = new List<Tuple<Point, ProgramFactory>>();
            List<Tuple<Point, ProgramFactory>> player2_spawns = new List<Tuple<Point, ProgramFactory>>();
            player1_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(0, 0), FasterProgramFactory));
            player1_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(1, 0), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(1, 1), FasterProgramFactory));
            TinyPreloadedMapFactory = new PlayerMapFactory(2, 2, player1_spawns, player2_spawns);
        }

        private static void InitializeWinnableEnemyMapFactory()
        {
            List<Tuple<Point, ProgramFactory>> player1_spawns = new List<Tuple<Point, ProgramFactory>>();
            List<Tuple<Point, ProgramFactory>> player2_spawns = new List<Tuple<Point, ProgramFactory>>();
            player1_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(0, 1), FasterProgramFactory));
            player1_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(1, 0), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(9, 8), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(8, 9), FasterProgramFactory));
            WinnableEnemyMapFactory = new WinnableEnemyMapFactory(10, 10, player1_spawns, player2_spawns);
        }

        private static void InitializeTinyWinnableEnemyMapFactory()
        {
            List<Tuple<Point, ProgramFactory>> player1_spawns = new List<Tuple<Point, ProgramFactory>>();
            List<Tuple<Point, ProgramFactory>> player2_spawns = new List<Tuple<Point, ProgramFactory>>();
            player1_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(0, 0), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(1, 0), FasterProgramFactory));
            TinyWinnableEnemyMapFactory = new WinnableEnemyMapFactory(2, 2, player1_spawns, player2_spawns);
        }

        private static void InitializeWinnableDataMapFactory()
        {
            List<Tuple<Point, ProgramFactory>> player1_spawns = new List<Tuple<Point, ProgramFactory>>();
            List<Tuple<Point, ProgramFactory>> player2_spawns = new List<Tuple<Point, ProgramFactory>>();
            player1_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(0, 1), FasterProgramFactory));
            player1_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(1, 0), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(9, 8), FasterProgramFactory));
            player2_spawns.Add(new Tuple<Point, ProgramFactory>(new Point(8, 9), FasterProgramFactory));
            WinnableDataMapFactory = new WinnableDataMapFactory(10, 10, player1_spawns, player2_spawns);
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
            InitializeTinyWinnableEnemyMapFactory();
            InitializePreloadedMapFactory();
            InitializeTinyPreloadedMapFactory();
            InitializeWinnableEnemyMapFactory();
            InitializeWinnableDataMapFactory();
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
        public void TestDontMoveOverOwnSecondProgramTail()
        {
            Map map = TinyPreloadedMapFactory.NewInstance();
            Assert.IsTrue(map.MoveProgram(new Point(0, 0), new Point(0, 1))); // Player moves left program down
            Assert.IsFalse(map.MoveProgram(new Point(1, 0), new Point(-1, 0))); // Player attempts to move right program left onto left program tail node
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 0));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(1, 0));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 1));
        }

        [TestMethod]
        public void TestDontMoveOverEnemyProgramTail()
        {
            Map map = TinyPreloadedMapFactory.NewInstance();
            Assert.IsTrue(map.MoveProgram(new Point(0, 0), new Point(0, 1))); // Player1 moves left program down
            map.TurnDone();
            Assert.IsFalse(map.MoveProgram(new Point(1, 0), new Point(-1, 0))); // Player2 attempts to move right program left onto left program tail node
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 0));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(1, 0));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 1));
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
        public void PlayersCanWinEnemyMap()
        {
            Map map = WinnableEnemyMapFactory.NewInstance();
            List<HackedEventArgs> winners = new List<HackedEventArgs>();
            Action<string, object, EventArgs> action = (x, y, z) => winners.Add((HackedEventArgs)z);
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
            Assert.IsTrue(map.HasBeenHacked);
            Assert.AreEqual<int>(1, winners.Count);
            Assert.AreEqual<Player>(WinnableEnemyMapFactory.Player2, winners[0].WinningPlayer);
            Assert.AreEqual<ushort>(0, winners[0].EarnedSilicoins);
        }

        [TestMethod]
        public void PlayersCanWinDataMap()
        {
            Map map = WinnableDataMapFactory.NewInstance();
            map.CreateNode(new DataNodeFactory(), 0, 2);
            List<HackedEventArgs> winners = new List<HackedEventArgs>();
            Action<string, object, EventArgs> action = (x, y, z) => winners.Add((HackedEventArgs)z);
            map.Mediator.Subscribe("haxxit.map.hacked", action);
            Assert.IsTrue(map.MoveProgram(new Point(0, 1), new Point(0, 1)));
            Assert.IsTrue(map.HasBeenHacked);
            Assert.AreEqual<int>(1, winners.Count);
            Assert.AreEqual<Player>(WinnableEnemyMapFactory.Player1, winners[0].WinningPlayer);
            Assert.AreEqual<ushort>(0, winners[0].EarnedSilicoins);
        }
    }
}
