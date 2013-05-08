using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartboyDevelopments.Haxxit;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit.Tests
{
    [TestClass]
    public class BasicMapTests
    {
        static DynamicProgramFactory BasicProgramFactory, FasterProgramFactory,
            BiggerFasterProgramFactory;
        static IFactory<Map> BasicMapFactory, SpawnMapFactory;

        void AssertListEqual<T>(IList<T> a, IList<T> b)
        {
            Assert.AreEqual(a.Count, b.Count);
            for (int i = 0; i < a.Count; i++)
                Assert.AreEqual<T>(a[i], b[i]);
        }

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            List<Command> commands = new List<Command>();
            commands.Add(new DynamicDamageCommand(2, 3));
            BasicProgramFactory = new DynamicProgramFactory(4, 4, commands);
            FasterProgramFactory = new DynamicProgramFactory(8, 4, commands);
            BiggerFasterProgramFactory = new DynamicProgramFactory(16, 8, commands);
            List<Point> spawns = new List<Point>();
            spawns.Add(new Point(0, 1));
            spawns.Add(new Point(1, 0));
            spawns.Add(new Point(9, 8));
            spawns.Add(new Point(8, 9));
            BasicMapFactory = new BasicMapFactory(10, 10);
            SpawnMapFactory = new BasicMapFactory(10, 10, spawns);
        }

        [TestMethod]
        public void TestCreateMap()
        {
            Map map = BasicMapFactory.NewInstance();
            for (int y = 0; y < 10; ++y)
            {
                for (int x = 0; x < 10; ++x)
                {
                    Assert.IsTrue(map.NodeIsType<AvailableNode>(x, y));
                }
            }
        }

        [TestMethod]
        public void TestAddSpawn()
        {
            Map map = SpawnMapFactory.NewInstance();
            for (int y = 0; y < 10; ++y)
            {
                for (int x = 0; x < 10; ++x)
                {
                    if ((x == 0 && y == 1) || (x == 1 && y == 0) || (x == 9 && y == 8) || (x == 8 && y == 9))
                        Assert.IsTrue(map.NodeIsType<SpawnNode>(x, y));
                    else
                        Assert.IsTrue(map.NodeIsType<AvailableNode>(x, y));
                }
            }
        }

        [TestMethod]
        public void TestSpawnProgram()
        {
            Map map = SpawnMapFactory.NewInstance();
            Assert.IsTrue(map.SpawnProgram(BasicProgramFactory, 0, 1));
            Assert.IsTrue(map.SpawnProgram(BasicProgramFactory, 1, 0));
            Assert.IsFalse(map.SpawnProgram(BasicProgramFactory, 5, 5));
            Assert.IsFalse(map.SpawnProgram(BasicProgramFactory, - 1, -1));
        }

        [TestMethod]
        public void TestFinishedSpawning()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(BasicProgramFactory, 0, 1);
            map.SpawnProgram(BasicProgramFactory, 1, 0);
            map.FinishedSpawning();
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(1, 0));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(9, 8));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(8, 9));
        }

        [TestMethod]
        public void TestMoveProgram()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(BasicProgramFactory, 0, 1);
            map.FinishedSpawning();
            Assert.IsTrue(map.MoveProgram(new Point(0, 1), new Point(0, 1)));
            Assert.IsTrue(map.MoveProgram(new Point(0, 2), new Point(0, 1)));
            Assert.IsTrue(map.MoveProgram(new Point(0, 3), new Point(0, 1)));
            Assert.IsTrue(map.MoveProgram(new Point(0, 4), new Point(0, 1)));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 3));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 4));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 5));
        }

        [TestMethod]
        public void TestDontSplitProgramFourRightFourLeft()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(BasicProgramFactory, 0, 1);
            map.FinishedSpawning();

            // Move four right
            Assert.IsTrue(map.MoveProgram(new Point(0, 1), new Point(0, 1)));
            Assert.IsTrue(map.MoveProgram(new Point(0, 2), new Point(0, 1)));
            Assert.IsTrue(map.MoveProgram(new Point(0, 3), new Point(0, 1)));
            Assert.IsTrue(map.MoveProgram(new Point(0, 4), new Point(0, 1)));

            // Cycle back to player 1's turn
            map.TurnDone();
            map.TurnDone();

            // Move four left
            Assert.IsTrue(map.MoveProgram(new Point(0, 5), new Point(0, -1)));
            Assert.IsTrue(map.MoveProgram(new Point(0, 4), new Point(0, -1)));
            Assert.IsTrue(map.MoveProgram(new Point(0, 3), new Point(0, -1)));
            Assert.IsTrue(map.MoveProgram(new Point(0, 2), new Point(0, -1)));

            // Check program location
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 5));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 4));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 3));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 0));
        }

        [TestMethod]
        public void TestAttackProgram()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(BasicProgramFactory, 0, 1);
            map.SpawnProgram(BasicProgramFactory, 1, 0);
            map.FinishedSpawning();
            map.MoveProgram(new Point(0, 1), new Point(0, 1));
            map.MoveProgram(new Point(0, 2), new Point(0, 1));
            map.MoveProgram(new Point(0, 3), new Point(0, 1));
            map.MoveProgram(new Point(0, 4), new Point(0, 1));
            Assert.IsTrue(map.RunCommand(new Point(1, 0), new Point(0, 2), "Damage") != null);
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 3));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 4));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 5));
        }

        [TestMethod]
        public void TestUndoAttackProgram()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(BasicProgramFactory, 0, 1);
            map.SpawnProgram(BasicProgramFactory, 1, 0);
            map.FinishedSpawning();
            map.MoveProgram(new Point(0, 1), new Point(0, 1));
            map.MoveProgram(new Point(0, 2), new Point(0, 1));
            map.MoveProgram(new Point(0, 3), new Point(0, 1));
            map.MoveProgram(new Point(0, 4), new Point(0, 1));
            UndoCommand command = map.RunCommand(new Point(1, 0), new Point(0, 2), "Damage");
            Assert.IsTrue(command != null);
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 3));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 4));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 5));
            Assert.IsTrue(map.RunUndoCommand(command));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 3));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 4));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 5));
        }

        [TestMethod]
        public void TestUndoMove()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(BasicProgramFactory, 0, 1);
            map.FinishedSpawning();
            map.MoveProgram(new Point(0, 1), new Point(0, 1));
            map.MoveProgram(new Point(0, 2), new Point(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 3));
            map.UndoMoveProgram(new Point(0, 2), new Point(0, 1), true, false);
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 3));
        }

        [TestMethod]
        public void TestUndoMoveProgramNotResized()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(FasterProgramFactory, 0, 1);
            map.FinishedSpawning();
            map.MoveProgram(new Point(0, 1), new Point(0, 1));
            map.MoveProgram(new Point(0, 2), new Point(0, 1));
            map.MoveProgram(new Point(0, 3), new Point(0, 1));
            map.MoveProgram(new Point(0, 4), new Point(0, 1));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 3));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 4));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 5));
            map.UndoMoveProgram(new Point(0, 4), new Point(0, 1), false, false, new Point(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 3));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 4));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 5));
        }

        [TestMethod]
        public void TestTurnDone()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(BasicProgramFactory, 0, 1);
            map.FinishedSpawning();
            map.MoveProgram(new Point(0, 1), new Point(0, 1));
            map.MoveProgram(new Point(0, 2), new Point(0, 1));
            map.MoveProgram(new Point(0, 3), new Point(0, 1));
            map.MoveProgram(new Point(0, 4), new Point(0, 1));
            map.MoveProgram(new Point(0, 5), new Point(0, 1)); //shouldn't work now
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 3));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 4));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 5));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 6));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 7));
            map.TurnDone();
            map.MoveProgram(new Point(0, 5), new Point(0, 1)); // now it should work
            map.MoveProgram(new Point(0, 6), new Point(0, 1));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(0, 3));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 4));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 5));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 6));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 7));
        }

        [TestMethod]
        public void TestMoveOverTailNode()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(FasterProgramFactory, 0, 1);
            map.FinishedSpawning();
            map.MoveProgram(new Point(0, 1), new Point(0, 1));
            map.MoveProgram(new Point(0, 2), new Point(1, 0));
            map.MoveProgram(new Point(1, 2), new Point(0, -1));
            map.MoveProgram(new Point(1, 1), new Point(-1, 0));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(1, 2));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(1, 1));
        }

        [TestMethod]
        public void TestUndoMoveOverTailNode()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(FasterProgramFactory, 0, 1);
            map.FinishedSpawning();
            map.MoveProgram(new Point(0, 1), new Point(0, 1));
            map.MoveProgram(new Point(0, 2), new Point(1, 0));
            map.MoveProgram(new Point(1, 2), new Point(0, -1));
            map.MoveProgram(new Point(1, 1), new Point(-1, 0));
            map.UndoMoveProgram(new Point(1, 1), new Point(-1, 0), false, true);
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(1, 2));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(1, 1));
        }

        [TestMethod]
        public void TestMoveOverMiddleTailNode()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(BiggerFasterProgramFactory, 0, 1);
            map.FinishedSpawning();
            map.MoveProgram(new Point(0, 1), new Point(0, 1));
            map.MoveProgram(new Point(0, 2), new Point(0, 1));
            map.MoveProgram(new Point(0, 3), new Point(0, 1));
            map.MoveProgram(new Point(0, 4), new Point(1, 0));
            map.MoveProgram(new Point(1, 4), new Point(0, -1));
            map.MoveProgram(new Point(1, 3), new Point(0, -1));
            map.MoveProgram(new Point(1, 2), new Point(0, -1));
            map.MoveProgram(new Point(1, 1), new Point(0, 1));
            map.MoveProgram(new Point(1, 2), new Point(-1, 0));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 2));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 3));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(0, 4));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(1, 1));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(1, 2));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(1, 3));
            Assert.IsTrue(map.NodeIsType<ProgramTailNode>(1, 4));
        }

        [TestMethod]
        public void TestCreateSilicoinNodes()
        {
            SilicoinNodeFactory silicoin_node_factory = new SilicoinNodeFactory(100);
            Map map = BasicMapFactory.NewInstance();
            map.CreateNode(silicoin_node_factory, 0, 1);
            map.CreateNode(silicoin_node_factory, 1, 0);
            map.CreateNode(silicoin_node_factory, 9, 8);
            map.CreateNode(silicoin_node_factory, 8, 9);
            for (int y = 0; y < 10; ++y)
            {
                for (int x = 0; x < 10; ++x)
                {
                    if ((x == 0 && y == 1) || (x == 1 && y == 0) || (x == 9 && y == 8) || (x == 8 && y == 9))
                        Assert.IsTrue(map.NodeIsType<SilicoinNode>(x, y));
                    else
                        Assert.IsTrue(!map.NodeIsType<SilicoinNode>(x, y) && map.NodeIsType<AvailableNode>(x, y));
                }
            }
        }

        [TestMethod]
        public void TestAddMediatorThenNodes()
        {
            Map map = SpawnMapFactory.NewInstance();
            map.SpawnProgram(BasicProgramFactory, 0, 1);
            map.SpawnProgram(BasicProgramFactory, 1, 0);
            map.CreateNode(new SilicoinNodeFactory(5), 0, 2);
            map.CreateNode(new SilicoinNodeFactory(5), 2, 0);
            for (int y = 0; y < 10; ++y)
            {
                for (int x = 0; x < 10; ++x)
                {
                    Assert.IsTrue(map.GetNode(x, y) != null);
                }
            }
            map.FinishedSpawning();
            map.MoveProgram(new Point(0, 1), new Point(0, 1));
            map.MoveProgram(new Point(0, 2), new Point(0, 1));
            map.MoveProgram(new Point(0, 3), new Point(0, 1));
            map.MoveProgram(new Point(0, 4), new Point(0, 1));
            map.MoveProgram(new Point(1, 0), new Point(0, 1));
            map.MoveProgram(new Point(1, 1), new Point(0, 1));
            map.MoveProgram(new Point(1, 2), new Point(0, 1));
            map.MoveProgram(new Point(1, 3), new Point(0, 1));
            for (int y = 0; y < 10; ++y)
            {
                for (int x = 0; x < 10; ++x)
                {
                    Assert.IsTrue(map.GetNode(x, y) != null);
                }
            }
        }

        [TestMethod]
        public void TestMoveOnSilicoinNode()
        {
            SilicoinNodeFactory silicoin_node_factory = new SilicoinNodeFactory(100);
            Map map = SpawnMapFactory.NewInstance();
            //List<ushort> output = new List<ushort>();
            //Action<string, object, EventArgs> action = (x, y, z) => output.Add(((SilicoinEventArgs)z).Silicoins);
            //map.Mediator.Subscribe("haxxit.silicoins.add", action);
            map.SpawnProgram(BasicProgramFactory, 0, 1);
            map.CreateNode(silicoin_node_factory, 0, 2);
            map.FinishedSpawning();
            map.MoveProgram(new Point(0, 1), new Point(0, 1));
            Assert.AreEqual<ushort>(100, map.EarnedSilicoins);
            //List<ushort> expected_output = new List<ushort>();
            //expected_output.Add(100);
            //AssertListEqual<ushort>(output, expected_output);
        }
    }
}
