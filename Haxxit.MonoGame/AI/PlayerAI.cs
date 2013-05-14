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
        private List<Maps.ProgramHeadNode> friendlyPrograms;
        private List<Maps.ProgramHeadNode> enemyPrograms;
        private AINodeData[,] mapData; // A restructured grid of map data pulled from Haxxit.Maps

        private enum MoveCode
        {
            NoMovesSpecified,
            InsufficientMoves,
            MoveWasOutOfBounds,
            MoveWasNotAdjacent,
            MoveWasBlocked,
            Success
        }

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
            friendlyPrograms = new List<Maps.ProgramHeadNode>();
            enemyPrograms = new List<Maps.ProgramHeadNode>();
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
            PrioritizedCommand prioritizedCommandHead = null;
            List<Commands.Command> commands = GetActualCommands(program);
            foreach(Commands.Command command in commands)
            {
                PrioritizedCommand newPrioritizedCommand = FindCommandOptions(program, command);
                if (prioritizedCommandHead == null)
                {
                    prioritizedCommandHead = newPrioritizedCommand;
                }
                else if(newPrioritizedCommand.Damage > prioritizedCommandHead.Damage)
                {
                    // If the damage of the new command is greater than the head then we prioritize it
                    newPrioritizedCommand.Next = prioritizedCommandHead;
                    prioritizedCommandHead = newPrioritizedCommand;
                }
                else if(newPrioritizedCommand.Damage == prioritizedCommandHead.Damage && newPrioritizedCommand.Range > prioritizedCommandHead.Range)
                {
                    // If the damage of the new command is equal but the range is greater than the head then we prioritize it
                    newPrioritizedCommand.Next = prioritizedCommandHead;
                    prioritizedCommandHead = newPrioritizedCommand;
                }
                else // The new command has less priority than the head so we must determine its position in the priority list
                {
                    PrioritizedCommand prioritizedCommandIndex = prioritizedCommandHead;
                    PrioritizedCommand prioritizedCommandNextIndex = prioritizedCommandHead.Next;
                    bool inserted = false;
                    while (prioritizedCommandNextIndex != null && inserted == false)
                    {
                        if (prioritizedCommandHead == null)
                        {
                            // This new command is the first on record, so it has priority
                            prioritizedCommandHead = newPrioritizedCommand;
                            inserted = true;
                        }
                        else if (newPrioritizedCommand.Damage > prioritizedCommandNextIndex.Damage)
                        {
                            // If the damage of the new command is greater than the next one then we prioritize it
                            prioritizedCommandIndex.Next = newPrioritizedCommand;
                            newPrioritizedCommand.Next = prioritizedCommandNextIndex;
                            inserted = true;
                        }
                        else if (newPrioritizedCommand.Damage == prioritizedCommandHead.Damage && newPrioritizedCommand.Range > prioritizedCommandHead.Range)
                        {
                            // If the damage of the new command is equal but the range is greater than the next one then we prioritize it
                            prioritizedCommandIndex.Next = newPrioritizedCommand;
                            newPrioritizedCommand.Next = prioritizedCommandNextIndex;
                            inserted = true;
                        }
                        else
                        {
                            // The new command has less priority than the next command, so we'll check further down the list
                            prioritizedCommandIndex = prioritizedCommandNextIndex;
                            prioritizedCommandNextIndex = prioritizedCommandNextIndex.Next;
                        }
                    }
                    if (!inserted)
                    {
                        // The new command has less priority than any of the existing commands so it's placed at the end of the list
                        prioritizedCommandIndex.Next = newPrioritizedCommand;
                    }
                }
            }
            // All of the valid command options for this program's current turn should now be accessible in a prioritized
            // list which is accessible from prioritizedCommandHead.

            Stack<Maps.Point> shortestPath = AStar(program, new Maps.Point(4, 4));
            if (shortestPath == null)
            {
                return;
            }
            Queue<Maps.Point> path = new Queue<Maps.Point>();
            if (shortestPath.Count < program.Program.Moves.MovesLeft)
            {
                for (int i = shortestPath.Count; i > 0; i--)
                {
                    path.Enqueue(shortestPath.Pop());
                }
            }
            else
            {
                for (int i = program.Program.Moves.MovesLeft; i > 0; i--)
                {
                    path.Enqueue(shortestPath.Pop());
                }
            }
            MoveCode moveCode = PerformMoves(program, path);
            if (moveCode != MoveCode.Success)
            {
                #if DEBUG
                throw new Exception();
                #endif
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

        // Retrieve's a program's actual command objects from the list of strings returned by
        // Haxxit.Programs.Program's GetAllCommands function.  (PS: I wish this weren't necessary)
        private List<Commands.Command> GetActualCommands(Maps.ProgramHeadNode program)
        {
            List<string> commandStrings = program.Program.GetAllCommands();
            List<Commands.Command> commands = new List<Commands.Command>();
            foreach (string lookup in commandStrings)
            {
                commands.Add(program.Program.GetCommand(lookup));
            }
            return commands;
        }

        // Fills a PrioritizeCommand object with a list of all the valid nodes from which a program can execute the
        // associated command.  This list will be used by the AI in conjunction with AStar pathfinding to determine
        // the best node to move to and issue a command from this turn.
        private PrioritizedCommand FindCommandOptions(Maps.ProgramHeadNode program, Commands.Command command)
        {
            PrioritizedCommand newPrioritizedCommand = new PrioritizedCommand(command);
            foreach (Maps.ProgramHeadNode enemyProgram in enemyPrograms)
            {
                foreach (Maps.ProgramNode node in enemyProgram.GetAllNodes())
                {
                    List<Maps.Point> commandPoints = FindNodesInRange(program, node.coordinate, newPrioritizedCommand.Range);
                    foreach (Maps.Point point in commandPoints)
                    {
                        PrioritizedCommand.CommandInfo newCommandInfo = new PrioritizedCommand.CommandInfo();
                        newCommandInfo.source = point;
                        newCommandInfo.destination = node.coordinate;
                        newCommandInfo.target = enemyProgram;
                        newPrioritizedCommand.TargetOptions.Add(newCommandInfo);
                    }
                }
            }
            return newPrioritizedCommand;
        }

        // This is a recursive helper function for finding all of the valid nodes which are within a specified range
        // of the target node.  Since programs can traverse their own nodes a program object is passed for this
        // validity check.
        private List<Maps.Point> FindNodesInRange(Maps.ProgramHeadNode program, Maps.Point target, int range)
        {
            List<Maps.Point> nodesInRange;
            if (range == 0) // Base case instantiates the actual list object
            {
                nodesInRange = new List<Maps.Point>();
                return nodesInRange;
            }
            else // Find all of the valid nodes in the current range ring (each recursive call handles a smaller ring)
            {
                nodesInRange = FindNodesInRange(program, target, range - 1);
                for (int negativeX = range * -1; negativeX < 0; negativeX++) // Find options in upper left quadrant
                {
                    Maps.Point checkPoint = new Maps.Point(target.X + negativeX, target.Y + range + negativeX);
                    if (IsInBounds(checkPoint))
                    {
                        if (mapData[checkPoint.X, checkPoint.Y].canHoldCurrentProgram(program))
                        {
                            nodesInRange.Add(checkPoint);
                        }
                    }
                }
                for (int positiveY = range; positiveY > 0; positiveY--) // Find options in upper right quadrant
                {
                    Maps.Point checkPoint = new Maps.Point(target.X + range - positiveY, target.Y + positiveY);
                    if (IsInBounds(checkPoint))
                    {
                        if (mapData[checkPoint.X, checkPoint.Y].canHoldCurrentProgram(program))
                        {
                            nodesInRange.Add(checkPoint);
                        }
                    }
                }
                for (int positiveX = range; positiveX > 0; positiveX--) // Find options in lower right quadrant
                {
                    Maps.Point checkPoint = new Maps.Point(target.X + positiveX, target.Y - range + positiveX);
                    if (IsInBounds(checkPoint))
                    {
                        if (mapData[checkPoint.X, checkPoint.Y].canHoldCurrentProgram(program))
                        {
                            nodesInRange.Add(checkPoint);
                        }
                    }
                }
                for (int negativeY = range * -1; negativeY < 0; negativeY++) // Find options in lower left quadrant
                {
                    Maps.Point checkPoint = new Maps.Point(target.X - range - negativeY, target.Y + negativeY);
                    if (IsInBounds(checkPoint))
                    {
                        if (mapData[checkPoint.X, checkPoint.Y].canHoldCurrentProgram(program))
                        {
                            nodesInRange.Add(checkPoint);
                        }
                    }
                }
            }
            return nodesInRange;
        }

        // A simple function to determine if a specified point is within the map bounds.
        // Used to prevent null reference exceptions when dereferencing the mapData array.
        public bool IsInBounds(Maps.Point point)
        {
            if (point.X < 0 || point.X >= map.XSize || point.Y < 0 || point.Y >= map.YSize)
            {
                return false;
            }
            return true;
        }

        // Super awesome fancy-pants AStar algorithm for efficient path-finding around obstacles!
        // See http://www.policyalmanac.org/games/aStarTutorial.htm for abstract theory.
        private Stack<Maps.Point> AStar(Maps.ProgramHeadNode program, Maps.Point destination)
        {
            Stack<Maps.Point> path = new Stack<Maps.Point>();
            int checkSourceH = Math.Abs(destination.X - program.coordinate.X) + Math.Abs(destination.Y - program.coordinate.Y);
            if (checkSourceH == 0) // If we're already at the destination then there's no work to do below
            {
                return path; // Empty path (already at destination)
            }
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
                    return null; // No path to destination
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
                if (currentNode.Coordinate.X != 0) // Check node to left
                {
                    checkNode = mapData[currentNode.Coordinate.X - 1, currentNode.Coordinate.Y];
                    AStarHelp(currentNode, checkNode, openList, closeList, program, destination);
                }
                if (currentNode.Coordinate.Y != 0) // Check node above
                {
                    checkNode = mapData[currentNode.Coordinate.X, currentNode.Coordinate.Y - 1];
                    AStarHelp(currentNode, checkNode, openList, closeList, program, destination);
                }
                if (currentNode.Coordinate.X != map.XSize - 1) // Check node to right
                {
                    checkNode = mapData[currentNode.Coordinate.X + 1, currentNode.Coordinate.Y];
                    AStarHelp(currentNode, checkNode, openList, closeList, program, destination);
                }
                if (currentNode.Coordinate.Y != map.YSize - 1) // Check node below
                {
                    checkNode = mapData[currentNode.Coordinate.X, currentNode.Coordinate.Y + 1];
                    AStarHelp(currentNode, checkNode, openList, closeList, program, destination);
                }
            }

            // Trace the discovered path and push the points onto a stack
            while (currentNode != source)
            {
                path.Push(currentNode.Coordinate);
                currentNode = currentNode.Parent;
            }
            ClearAStarData(openList, closeList);
            return path; // Populated path to destination
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

        // This function accepts a queue of intended moves for the specified program.  It first performs error
        // checking to ensure that the moves are possible, then updates the associated AINodeData to reflect
        // game changes as a result of these moves, and finally submits the moves to the turnActions queue for
        // sending during subsequent calls to HandleAITurn.
        private MoveCode PerformMoves(Maps.ProgramHeadNode program, Queue<Maps.Point> path)
        {
            if (path == null)
            {
                return MoveCode.NoMovesSpecified;
            }

            // Will hold all nodes touched by program both before and after move
            List<Maps.Point> programPoints = new List<Maps.Point>();

            // Get all the nodes currently associated with the program
            foreach (Maps.MapNode node in program.GetAllNodes())
            {
                programPoints.Insert(0, node.coordinate);
            }

            int movesLeft = program.Program.Moves.MovesLeft;
            Maps.Point currentHead = program.coordinate;
            foreach (Maps.Point destination in path)
            {
                Maps.Point direction = destination - currentHead;
                if (!IsInBounds(destination))
                {
                    return MoveCode.MoveWasOutOfBounds;
                }
                else if (Math.Abs(direction.X) != 1 && Math.Abs(direction.X) != 0)
                {
                    return MoveCode.MoveWasNotAdjacent;
                }
                else if (Math.Abs(direction.Y) != 1 && Math.Abs(direction.Y) != 0)
                {
                    return MoveCode.MoveWasNotAdjacent;
                }
                else if (Math.Abs(direction.X) == 0 && Math.Abs(direction.Y) == 0)
                {
                    return MoveCode.MoveWasNotAdjacent;
                }
                else if (Math.Abs(direction.X) == 1 && Math.Abs(direction.Y) == 1)
                {
                    return MoveCode.MoveWasNotAdjacent;
                }
                else if (!mapData[destination.X, destination.Y].canHoldCurrentProgram(program))
                {
                    return MoveCode.MoveWasBlocked;
                }
                else if (movesLeft <= 0)
                {
                    return MoveCode.InsufficientMoves;
                }
                movesLeft--;
                programPoints.Add(destination);
                Maps.MoveEventArgs nextMove = new Maps.MoveEventArgs(currentHead, direction);
                turnActions.Enqueue(new NotifyArgs("haxxit.map.move", this, nextMove));
                currentHead = destination;
            }
            int index = 0;
            foreach (Maps.Point point in programPoints)
            {
                if (index < programPoints.Count - program.Program.Size.MaxSize)
                {
                    mapData[point.X, point.Y].IsAvailable = true;
                    mapData[point.X, point.Y].OccupiedBy = null;
                }
                else
                {
                    mapData[point.X, point.Y].IsAvailable = false;
                    mapData[point.X, point.Y].OccupiedBy = program.Program;
                }
                index++;
            }
            return MoveCode.Success;
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
