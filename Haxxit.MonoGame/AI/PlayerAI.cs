using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    // An AI version of the player which plans and sends its own program actions to the engine
    class PlayerAI : Player
    {
        private const int ACTIONSTALLTIME_MSECS = 250; // Time to stall between sending actions
        private AIState state;
        private Haxxit.Maps.Map map;
        private Queue<NotifyArgs> turnActions; // Queue of planned actions
        private bool waitingToSend;
        private int departureTime; // Time to send next action
        private List<Maps.ProgramNode> friendlyPrograms;
        private List<Maps.ProgramNode> enemyPrograms;
        private AINodeData[,] mapData; // A restructured grid of map data pulled from Haxxit.Maps

        // Overall status for waiting/planning/sending loop in HandleAITurn
        private enum AIState
        {
            Waiting,
            Planning,
            Sending
        };

        // Calculated behaviors for individual programs
        private enum ProgramBehavior
        {
            DoNothing,
            AttackNearest,
            DefendObjective,
            Flee,
            MoveLeft,
        };

        // Constructor
        public PlayerAI(string name = "")
            : base(name)
        {
            state = AIState.Waiting;
            map = null;
            turnActions = new Queue<NotifyArgs>();
            waitingToSend = false;
            departureTime = 0;
            friendlyPrograms = new List<Maps.ProgramNode>();
            enemyPrograms = new List<Maps.ProgramNode>();
            mapData = null;
        }

        // This is the entry point to all AI routines.  It may be called multiple times
        // concurrently by the engine, so an AIState variable is used to direct execution
        // flow after each call and prevent duplicate calculations.
        public void HandleAITurn(Haxxit.Maps.Map currentMap)
        {
            if(state == AIState.Planning)
            {
                return; // Let the AI finish planning the turn
            }
            else if (state == AIState.Sending)
            {
                SendActions();
            }
            else // state == AIState.waiting
            {
                state = AIState.Planning; // Planning to plan...
                map = currentMap;
                GetMapData(); // Load and restructure map data for AI usage
                PlanTurn();   // Begin planning
                state = AIState.Sending;
            }
        }

        // Reassembles the map data in a format that allows the AI easy access to program and node info
        private void GetMapData()
        {
            friendlyPrograms.Clear();
            enemyPrograms.Clear();
            mapData = new AINodeData[map.XSize, map.YSize]; // Instantiate the map data grid
            for (int column = 0; column < map.XSize; column++)
            {
                for (int row = 0; row < map.YSize; row++)
                {
                    mapData[column, row] = new AINodeData(column, row); // Create an AINode for each grid location
                    Maps.MapNode checkNode = map.GetNode(column, row);

                    // Typechecking of nodes from original Haxxit.Maps class
                    Type checkType = checkNode.GetType();
                    if(checkType == typeof(Maps.UnavailableNode))
                    {
                        continue;
                    }
                    else if (checkType == typeof(Maps.ProgramNode) || checkType == typeof(Maps.ProgramTailNode) || checkType == typeof(Maps.ProgramHeadNode))
                    {
                        mapData[column, row].OccupiedBy = ((Maps.ProgramNode)checkNode).Program;

                        // Detection of individual programs from original Haxxit.Maps class
                        if (checkType == typeof(Maps.ProgramHeadNode))
                        {
                            if (((Maps.ProgramHeadNode)checkNode).Player == map.CurrentPlayer)
                            {
                                friendlyPrograms.Add((Maps.ProgramHeadNode)checkNode);
                            }
                            else
                            {
                                enemyPrograms.Add((Maps.ProgramHeadNode)checkNode);
                            }
                        }
                    }
                    else if (checkType == typeof(Maps.SpawnNode))
                    {
                        mapData[column, row].IsSpawn = true;
                    }
                    else if (checkType == typeof(Maps.SilicoinNode))
                    {
                        mapData[column, row].HasSilicoin = true;
                    }
                    else if (checkType == typeof(Maps.DataNode))
                    {
                        mapData[column, row].HasData = true;
                    }
                    else // Nothing else on the node, so it must be available
                    {
                        mapData[column, row].IsAvailable = true;
                    }
                }
            }
        }

        // Plans the actions for each AI program based on the current map data
        private void PlanTurn()
        {
            foreach (Maps.ProgramHeadNode program in friendlyPrograms)
            {
                ProgramBehavior currentBehavior = DecideBehavior(program);
                switch(currentBehavior)
                {
                    case ProgramBehavior.DoNothing:
                    {
                        BehaviorDoNothing(program);
                        break;
                    }
                    case ProgramBehavior.AttackNearest:
                    {
                        BehaviorAttackNearest(program);
                        break;
                    }
                    case ProgramBehavior.DefendObjective:
                    {
                        BehaviorDefendObjective(program);
                        break;
                    }
                    case ProgramBehavior.Flee:
                    {
                        BehaviorFlee(program);
                        break;
                    }
                    case ProgramBehavior.MoveLeft:
                    {
                        BehaviorMoveLeft(program);
                        break;
                    }
                }
            }

            // Signals end of AI turn after executing each program's calculated behavior function
            turnActions.Enqueue(new NotifyArgs("haxxit.map.turn_done", this, new EventArgs()));
        }

        // Not currently implemented.  Will check mapData to determine appropriate behavior for programs
        private ProgramBehavior DecideBehavior(Maps.ProgramHeadNode program)
        {
            return ProgramBehavior.AttackNearest;
        }

        // Current program does nothing
        private void BehaviorDoNothing(Maps.ProgramHeadNode program)
        {
            return;
        }

        // Currently passes through to AStar algorithm for movement to test point
        private void BehaviorAttackNearest(Maps.ProgramHeadNode program)
        {
            Stack<Maps.Point> shortestPath = AStar(program, new Maps.Point(4, 4));
            if (shortestPath == null)
            {
                return;
            }
            Maps.Point movedHead = program.coordinate;
            int moves = program.Program.Moves.MovesLeft;
            foreach (Maps.Point point in shortestPath)
            {
                if (moves <= 0)
                {
                    break;
                }
                Maps.Point direction = point - movedHead;
                Maps.MoveEventArgs nextMove = new Maps.MoveEventArgs(movedHead, direction);
                turnActions.Enqueue(new NotifyArgs("haxxit.map.move", this, nextMove));
                movedHead += direction;
                moves--;
            }
        }

        // Not currently implemented.  Will tune AI movements to guard data nodes
        private void BehaviorDefendObjective(Maps.ProgramHeadNode program)
        {
            throw new NotImplementedException();
        }

        // Not currently implemented.  Will cause AI programs to move away from player
        // program following threat assessment.
        private void BehaviorFlee(Maps.ProgramHeadNode program)
        {
            throw new NotImplementedException();
        }

        // Moves the program directly left if possible.  This was implemented as a proof of concept.
        private void BehaviorMoveLeft(Maps.ProgramHeadNode program)
        {
            Maps.Point head = program.coordinate;
            Maps.Point moveLeft = new Maps.Point(-1, 0);
            for (int moves = 0; program.Program.Moves.MovesLeft > moves; moves++)
            {
                Maps.Point movedHead = new Maps.Point(head.X - moves, head.Y);
                if (movedHead.X - 1 < 0) // Can't move off edge of map
                {
                    break;
                }
                if (mapData[movedHead.X - 1, head.Y].canHoldCurrentProgram(program))
                {
                    Maps.MoveEventArgs moveHeadLeft = new Maps.MoveEventArgs(movedHead, moveLeft);
                    turnActions.Enqueue(new NotifyArgs("haxxit.map.move", this, moveHeadLeft));
                }
            }
        }

        // Super awesome fancy-pants AStar algorithm for efficient path-finding around obstacles!
        // See http://www.policyalmanac.org/games/aStarTutorial.htm for abstract theory.
        private Stack<Maps.Point> AStar(Maps.ProgramHeadNode program, Maps.Point destination)
        {
            int checkSourceH = Math.Abs(destination.X - program.coordinate.X) + Math.Abs(destination.Y - program.coordinate.Y);
            if (checkSourceH == 0) // If we're already at the destination then there's no work to do below
            {
                return null;
            }
            Stack<Maps.Point> path = new Stack<Maps.Point>();
            List<AINodeData> openList = new List<AINodeData>();
            List<AINodeData> closeList = new List<AINodeData>();
            AINodeData source = mapData[program.coordinate.X, program.coordinate.Y];
            source.G = 0;
            source.H = checkSourceH;
            source.F = source.G + source.H;
            openList.Add(source);
            source.AStarTrackStatus = AINodeData.AStarStatus.Open;
            AINodeData currentNode = source;
            AINodeData checkNode = null;
            while (true)
            {
                if (!openList.Any())
                {
                    return null;
                }
                currentNode = openList.First();
                foreach (AINodeData node in openList)
                {
                    if (node.F < currentNode.F)
                    {
                        currentNode = node;
                    }
                }
                openList.Remove(currentNode);
                closeList.Add(currentNode);
                currentNode.AStarTrackStatus = AINodeData.AStarStatus.Closed;
                if (currentNode.H == 0)
                {
                    break;
                }
                if (currentNode.Coordinate.X != 0)
                {
                    checkNode = mapData[currentNode.Coordinate.X - 1, currentNode.Coordinate.Y];
                    AStarHelp(currentNode, checkNode, openList, closeList, program, destination);
                }
                if (currentNode.Coordinate.Y != 0)
                {
                    checkNode = mapData[currentNode.Coordinate.X, currentNode.Coordinate.Y - 1];
                    AStarHelp(currentNode, checkNode, openList, closeList, program, destination);
                }
                if (currentNode.Coordinate.X != map.XSize - 1)
                {
                    checkNode = mapData[currentNode.Coordinate.X + 1, currentNode.Coordinate.Y];
                    AStarHelp(currentNode, checkNode, openList, closeList, program, destination);
                }
                if (currentNode.Coordinate.Y != map.YSize - 1)
                {
                    checkNode = mapData[currentNode.Coordinate.X, currentNode.Coordinate.Y + 1];
                    AStarHelp(currentNode, checkNode, openList, closeList, program, destination);
                }
            }
            while (currentNode != source)
            {
                path.Push(currentNode.Coordinate);
                currentNode = currentNode.Parent;
            }
            ClearAStarData(openList, closeList);
            return path;
        }

        // Non-recursive helper function for AStar algorithm.  Implemented to prevent code duplication for each direction above.
        private void AStarHelp(AINodeData currentNode, AINodeData checkNode, List<AINodeData> openList, List<AINodeData> closeList, Maps.ProgramHeadNode program, Maps.Point destination)
        {
            AINodeData.AStarStatus checkNodeStatus = checkNode.AStarTrackStatus;
            if (checkNodeStatus != AINodeData.AStarStatus.Closed && checkNode.canHoldCurrentProgram(program))
            {
                if (checkNodeStatus == AINodeData.AStarStatus.Unlisted)
                {
                    openList.Add(checkNode);
                    checkNode.Parent = currentNode;
                    checkNode.AStarTrackStatus = AINodeData.AStarStatus.Open;
                    checkNode.G = currentNode.G + 1;
                    checkNode.H = Math.Abs(destination.X - checkNode.Coordinate.X) + Math.Abs(destination.Y - checkNode.Coordinate.Y);
                    checkNode.F = checkNode.G + checkNode.H;
                }
                else
                {
                    int checkG = currentNode.G + 1;
                    if (checkG < checkNode.G)
                    {
                        checkNode.Parent = currentNode;
                        checkNode.G = checkG;
                        checkNode.F = checkNode.G + checkNode.H;
                    }
                }
            }
        }

        // Clear the AStar data from the nodes so it doesn't conflict with the next AStar call
        private void ClearAStarData(List<AINodeData> openList, List<AINodeData> closeList)
        {
            foreach (AINodeData node in openList)
            {
                node.Parent = null;
                node.F = int.MaxValue;
                node.G = int.MaxValue;
                node.H = int.MaxValue;
                node.AStarTrackStatus = AINodeData.AStarStatus.Unlisted;
            }
            foreach (AINodeData node in closeList)
            {
                node.Parent = null;
                node.F = int.MaxValue;
                node.G = int.MaxValue;
                node.H = int.MaxValue;
                node.AStarTrackStatus = AINodeData.AStarStatus.Unlisted;
            }
        }

        // Sends all of the planned AI actions to the engine
        private void SendActions()
        {
            if (waitingToSend)
            {
                if (System.Environment.TickCount > departureTime)
                {
                    // We're ready to send an action to the engine
                    waitingToSend = false;
                    NotifyArgs args = turnActions.Dequeue();
                    _notifiable_manager.Notify(args.EventItem, args.ObjectItem, args.EventArgsItem);
                }
            }
            else if (turnActions.Any())
            {
                // We're ready to retrieve the next action
                waitingToSend = true;
                departureTime = System.Environment.TickCount + ACTIONSTALLTIME_MSECS;
            }
            else
            {
                // We've sent all the actions
                state = AIState.Waiting;
            }
        }
    }
}
