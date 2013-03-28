using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.Tests
{
    [TestClass]
    public class MapTests
    {
        private BasicMap CreateMapWithSpawns()
        {
            BasicMap map = new BasicMap(10, 10);
            map.CreateNode<SpawnNode>(0, 1);
            map.CreateNode<SpawnNode>(1, 0);
            map.CreateNode<SpawnNode>(9, 8);
            map.CreateNode<SpawnNode>(8, 9);
            return map;
        }

        private DynamicProgramFactory CreateBasicProgramFactory()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new DynamicDamageCommand(2, 3));
            return new DynamicProgramFactory(4, 4, commands);
        }

        private DynamicProgramFactory CreateFasterProgramFactory()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new DynamicDamageCommand(2, 3));
            return new DynamicProgramFactory(8, 4, commands);
        }

        private DynamicProgramFactory CreateBiggerFasterProgramFactory()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new DynamicDamageCommand(3, 2));
            return new DynamicProgramFactory(16, 8, commands);
        }

        [TestMethod]
        public void TestCreateMap()
        {
            BasicMap map = new BasicMap(10, 10);
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
            BasicMap map = CreateMapWithSpawns();
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
            BasicMap map = CreateMapWithSpawns();
            Assert.IsTrue(map.SpawnProgram(CreateBasicProgramFactory(), 0, 1));
            Assert.IsTrue(map.SpawnProgram(CreateBasicProgramFactory(), 1, 0));
            Assert.IsFalse(map.SpawnProgram(CreateBasicProgramFactory(), 5, 5));
            Assert.IsFalse(map.SpawnProgram(CreateBasicProgramFactory(), - 1, -1));
        }

        [TestMethod]
        public void TestFinishedSpawning()
        {
            BasicMap map = CreateMapWithSpawns();
            map.SpawnProgram(CreateBasicProgramFactory(), 0, 1);
            map.SpawnProgram(CreateBasicProgramFactory(), 1, 0);
            map.FinishedSpawning();
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(0, 1));
            Assert.IsTrue(map.NodeIsType<ProgramHeadNode>(1, 0));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(9, 8));
            Assert.IsTrue(map.NodeIsType<AvailableNode>(8, 9));
        }

        [TestMethod]
        public void TestMoveProgram()
        {
            BasicMap map = CreateMapWithSpawns();
            map.SpawnProgram(CreateBasicProgramFactory(), 0, 1);
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
        public void TestAttackProgram()
        {
            BasicMap map = CreateMapWithSpawns();
            map.SpawnProgram(CreateBasicProgramFactory(), 0, 1);
            map.SpawnProgram(CreateBasicProgramFactory(), 1, 0);
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
            BasicMap map = CreateMapWithSpawns();
            map.SpawnProgram(CreateBasicProgramFactory(), 0, 1);
            map.SpawnProgram(CreateBasicProgramFactory(), 1, 0);
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
            BasicMap map = CreateMapWithSpawns();
            map.SpawnProgram(CreateBasicProgramFactory(), 0, 1);
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
            BasicMap map = CreateMapWithSpawns();
            map.SpawnProgram(CreateFasterProgramFactory(), 0, 1);
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
            BasicMap map = CreateMapWithSpawns();
            map.SpawnProgram(CreateBasicProgramFactory(), 0, 1);
            map.FinishedSpawning();
            map.MoveProgram(new Point(0, 1), new Point(0, 1));
            map.MoveProgram(new Point(0, 2), new Point(0, 1));
            map.MoveProgram(new Point(0, 3), new Point(0, 1));
            map.MoveProgram(new Point(0, 4), new Point(0, 1));
            map.MoveProgram(new Point(0, 5), new Point(0, 1)); //shoudln't work now
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
            BasicMap map = CreateMapWithSpawns();
            map.SpawnProgram(CreateFasterProgramFactory(), 0, 1);
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
            BasicMap map = CreateMapWithSpawns();
            map.SpawnProgram(CreateFasterProgramFactory(), 0, 1);
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
            BasicMap map = CreateMapWithSpawns();
            map.SpawnProgram(CreateBiggerFasterProgramFactory(), 0, 1);
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
    }
}
