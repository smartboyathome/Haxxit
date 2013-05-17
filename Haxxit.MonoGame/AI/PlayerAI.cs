using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit;
using SmartboyDevelopments.SimplePubSub;

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
        private List<Haxxit.Maps.ProgramHeadNode> friendlyPrograms;
        private List<Haxxit.Maps.ProgramHeadNode> enemyPrograms;
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

        private enum CommandCode
        {
            NoCommandSpecified,
            TargetWasOutOfBounds,
            NoProgramAtTarget,
            ProgramOutOfRange,
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
            friendlyPrograms = new List<Haxxit.Maps.ProgramHeadNode>();
            enemyPrograms = new List<Haxxit.Maps.ProgramHeadNode>();
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
                    Haxxit.Maps.MapNode checkNode = map.GetNode(column, row);

                    // Typechecking of nodes from original Haxxit.Maps class
                    Type checkType = checkNode.GetType();
                    if(checkType == typeof(Haxxit.Maps.UnavailableNode))
                    {
                        continue;
                    }
                    else if (checkType == typeof(Haxxit.Maps.ProgramTailNode))
                    {
                        while (true)
                        {
                            if (((Haxxit.Maps.ProgramNode)checkNode).Head != null)
                            {
                                checkNode = ((Haxxit.Maps.ProgramNode)checkNode).Head;
                            }
                            else
                            {
                                break;
                            }
                        }
                        mapData[column, row].OccupiedBy = (Haxxit.Maps.ProgramHeadNode)checkNode;
                    }
                    else if (checkType == typeof(Haxxit.Maps.ProgramHeadNode))
                    {
                        mapData[column, row].OccupiedBy = (Haxxit.Maps.ProgramHeadNode)checkNode;
                        if (((Haxxit.Maps.ProgramHeadNode)checkNode).Player == map.CurrentPlayer)
                        {
                            friendlyPrograms.Add((Haxxit.Maps.ProgramHeadNode)checkNode);
                        }
                        else
                        {
                            enemyPrograms.Add((Haxxit.Maps.ProgramHeadNode)checkNode);
                        }
                    }
                    else if (checkType == typeof(Haxxit.Maps.SpawnNode))
                    {
                        mapData[column, row].IsSpawn = true;
                    }
                    else if (checkType == typeof(Haxxit.Maps.SilicoinNode))
                    {
                        mapData[column, row].HasSilicoin = true;
                    }
                    else if (checkType == typeof(Haxxit.Maps.DataNode))
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
            foreach (Haxxit.Maps.ProgramHeadNode program in friendlyPrograms)
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
        private ProgramBehavior DecideBehavior(Haxxit.Maps.ProgramHeadNode program)
        {
            return ProgramBehavior.AttackNearest;
        }

        // Current program does nothing
        private void BehaviorDoNothing(Haxxit.Maps.ProgramHeadNode program)
        {
            return;
        }

        // Locates the nearest move/attack combination and attempts to use it
        private void BehaviorAttackNearest(Haxxit.Maps.ProgramHeadNode program)
        {
            List<Commands.Command> commands = GetActualCommands(program);
            List<PrioritizedCommand> prioritizedCommands = new List<PrioritizedCommand>();

            // For each of the current program's available commands...
            foreach(Commands.Command command in commands)
            {
                // Record all potential options for using that command
                PrioritizedCommand newPrioritizedCommand = FindCommandOptions(program, command);
                prioritizedCommands.Add(newPrioritizedCommand);
            }

            // Sort the different commands (and their lists of usage options) by usefulness priority (IE: Damage & range).
            // The PrioritizedCommand class has an internal comparable interface for performing this sort.
            prioritizedCommands.Sort();

            Stack<Haxxit.Maps.Point> chosenPath = null;
            Haxxit.Maps.Point chosenSource = new Haxxit.Maps.Point(-1, -1);
            Haxxit.Maps.Point chosenTarget = new Haxxit.Maps.Point(-1, -1);
            Commands.Command chosenCommand = null;
            bool optionChosen = false;

            // For each of the current program's prioritized commands...
            foreach(PrioritizedCommand prioritizedCommand in prioritizedCommands)
            {
                if (optionChosen)
                {
                    break;
                }
                if (prioritizedCommand.TargetOptions.Count != 0) // If the command has any potential usages...
                {
                    CommandInfo closestOption = prioritizedCommand.TargetOptions.First();
                    if (closestOption.Path.Count <= program.Program.Moves.MovesLeft)
                    {
                        // We can use the command this turn, so we queue up the moves!
                        chosenPath = closestOption.Path;
                        chosenSource = closestOption.Source;
                        chosenTarget = closestOption.Target;
                        chosenCommand = prioritizedCommand.Command;
                        optionChosen = true;
                    }
                }
            }

            // If the program isn't close enough to use any commands this turn after moving...
            if (!optionChosen)
            {
                // For each of the current program's prioritized commands...
                foreach (PrioritizedCommand prioritizedCommand in prioritizedCommands)
                {
                    if (optionChosen)
                    {
                        break;
                    }
                    if (prioritizedCommand.TargetOptions.Count != 0) // If the command has any potential usages...
                    {
                        // We begin moving towards usage points for next turn.
                        CommandInfo closestOption = prioritizedCommand.TargetOptions.First();
                        chosenPath = closestOption.Path;
                        optionChosen = true;
                    }
                }
            }

            // If there are no accessible positions from which to use a command anywhere on the map...
            if (!optionChosen)
            {
                // We have nowhere we want to move.
                return;
            }
            Queue<Haxxit.Maps.Point> finalPath = new Queue<Haxxit.Maps.Point>();
            if (chosenPath.Count < program.Program.Moves.MovesLeft)
            {
                for (int i = chosenPath.Count; i > 0; i--)
                {
                    finalPath.Enqueue(chosenPath.Pop());
                }
            }
            else
            {
                for (int i = program.Program.Moves.MovesLeft; i > 0; i--)
                {
                    finalPath.Enqueue(chosenPath.Pop());
                }
            }
            MoveCode moveCode = MoveCode.Success;
            if (finalPath != null && finalPath.Any())
            {
                moveCode = PerformMoves(program, finalPath);
            }
            if (moveCode != MoveCode.Success)
            {
                #if DEBUG
                throw new Exception();
                #endif
            }
            CommandCode commandCode = CommandCode.Success;
            if(chosenCommand != null)
            {
                commandCode = PerformCommand(chosenTarget, chosenSource, chosenCommand);
            }
            if (commandCode != CommandCode.Success)
            {
                #if DEBUG
                throw new Exception();
                #endif
            }
        }

        // Not currently implemented.  Will tune AI movements to guard data nodes
        private void BehaviorDefendObjective(Haxxit.Maps.ProgramHeadNode program)
        {
            throw new NotImplementedException();
        }

        // Not currently implemented.  Will cause AI programs to move away from player
        // program following threat assessment.
        private void BehaviorFlee(Haxxit.Maps.ProgramHeadNode program)
        {
            throw new NotImplementedException();
        }

        // Moves the program directly left if possible.  This was implemented as a proof of concept.
        // May be useful for future testing.
        private void BehaviorMoveLeft(Haxxit.Maps.ProgramHeadNode program)
        {
            Haxxit.Maps.Point head = program.coordinate;
            Haxxit.Maps.Point moveLeft = new Haxxit.Maps.Point(-1, 0);
            for (int moves = 0; program.Program.Moves.MovesLeft > moves; moves++)
            {
                Haxxit.Maps.Point movedHead = new Haxxit.Maps.Point(head.X - moves, head.Y);
                if (movedHead.X - 1 < 0) // Can't move off edge of map
                {
                    break;
                }
                if (mapData[movedHead.X - 1, head.Y].canHoldCurrentProgram(program))
                {
                    Haxxit.Maps.MoveEventArgs moveHeadLeft = new Haxxit.Maps.MoveEventArgs(movedHead, moveLeft);
                    turnActions.Enqueue(new NotifyArgs("haxxit.map.move", this, moveHeadLeft));
                }
            }
        }

        // Retrieves a program's actual command objects from the list of strings returned by
        // Haxxit.Programs.Program's GetAllCommands function.  (PS: I wish this weren't necessary)
        private List<Commands.Command> GetActualCommands(Haxxit.Maps.ProgramHeadNode program)
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
        private PrioritizedCommand FindCommandOptions(Haxxit.Maps.ProgramHeadNode program, Commands.Command command)
        {
            PrioritizedCommand newPrioritizedCommand = new PrioritizedCommand(command);

            // For every enemy program...
            foreach (Haxxit.Maps.ProgramHeadNode enemyProgram in enemyPrograms)
            {
                // For every node in that enemy program...
                foreach (Haxxit.Maps.ProgramNode node in enemyProgram.GetAllNodes())
                {
                    List<Haxxit.Maps.Point> commandPoints = new List<Haxxit.Maps.Point>();

                    // Remember that GetAllNodes is from Haxxit.Maps, which is unaware of pending changes in mapData here.
                    // Second check is necessary in the extremely rare event that friendly programs have both eliminated the enemy program node here and expanded into it themselves earlier this turn.
                    if (mapData[node.coordinate.X, node.coordinate.Y].OccupiedBy != null && mapData[node.coordinate.X, node.coordinate.Y].OccupiedBy.Program == enemyProgram.Program)
                    {
                        commandPoints = FindNodesInRange(program, node.coordinate, newPrioritizedCommand.Range);
                    }

                    // For every point in range of that node in the enemy program...
                    foreach (Haxxit.Maps.Point point in commandPoints)
                    {
                        // Attempt to find a path to that point
                        Stack<Haxxit.Maps.Point> path = AStar(program, point);
                        if (path != null)
                        {
                            // Record the shortest path to that point as well as the command and target information
                            CommandInfo newCommandInfo = new CommandInfo(point, node.coordinate, enemyProgram, path);
                            newPrioritizedCommand.TargetOptions.Add(newCommandInfo);
                        }
                    }
                }
            }

            // Sort the different usage options for this command by the length of the path required to use them.
            // The CommandInfo class has an internal comparable interface for performing this sort.
            newPrioritizedCommand.TargetOptions.Sort();
            return newPrioritizedCommand;
        }

        // This is a recursive helper function for finding all of the valid nodes which are within a specified range
        // of the target node.  Since programs can traverse their own nodes a program object is passed for this
        // validity check.
        private List<Haxxit.Maps.Point> FindNodesInRange(Haxxit.Maps.ProgramHeadNode program, Haxxit.Maps.Point target, int range)
        {
            List<Haxxit.Maps.Point> nodesInRange;
            if (range == 0) // Base case instantiates the actual list object
            {
                nodesInRange = new List<Haxxit.Maps.Point>();
                return nodesInRange;
            }
            else // Find all of the valid nodes in the current range ring (each recursive call handles a smaller ring)
            {
                nodesInRange = FindNodesInRange(program, target, range - 1);
                for (int negativeX = range * -1; negativeX < 0; negativeX++) // Find options in lower left quadrant from target
                {
                    Haxxit.Maps.Point checkPoint = new Haxxit.Maps.Point(target.X + negativeX, target.Y + range + negativeX);
                    if (IsInBounds(checkPoint))
                    {
                        if (mapData[checkPoint.X, checkPoint.Y].canHoldCurrentProgram(program))
                        {
                            nodesInRange.Add(checkPoint);
                        }
                    }
                }
                for (int positiveY = range; positiveY > 0; positiveY--) // Find options in lower right quadrant from target
                {
                    Haxxit.Maps.Point checkPoint = new Haxxit.Maps.Point(target.X + range - positiveY, target.Y + positiveY);
                    if (IsInBounds(checkPoint))
                    {
                        if (mapData[checkPoint.X, checkPoint.Y].canHoldCurrentProgram(program))
                        {
                            nodesInRange.Add(checkPoint);
                        }
                    }
                }
                for (int positiveX = range; positiveX > 0; positiveX--) // Find options in upper right quadrant from target
                {
                    Haxxit.Maps.Point checkPoint = new Haxxit.Maps.Point(target.X + positiveX, target.Y - range + positiveX);
                    if (IsInBounds(checkPoint))
                    {
                        if (mapData[checkPoint.X, checkPoint.Y].canHoldCurrentProgram(program))
                        {
                            nodesInRange.Add(checkPoint);
                        }
                    }
                }
                for (int negativeY = range * -1; negativeY < 0; negativeY++) // Find options in upper left quadrant from target
                {
                    Haxxit.Maps.Point checkPoint = new Haxxit.Maps.Point(target.X - range - negativeY, target.Y + negativeY);
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
        public bool IsInBounds(Haxxit.Maps.Point point)
        {
            if (point.X < 0 || point.X >= map.XSize || point.Y < 0 || point.Y >= map.YSize)
            {
                return false;
            }
            return true;
        }

        // Super awesome fancy-pants AStar algorithm for efficient path-finding around obstacles!
        // See http://www.policyalmanac.org/games/aStarTutorial.htm for abstract theory.
        private Stack<Haxxit.Maps.Point> AStar(Haxxit.Maps.ProgramHeadNode program, Haxxit.Maps.Point destination)
        {
            Stack<Haxxit.Maps.Point> path = new Stack<Haxxit.Maps.Point>();
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
                    ClearAStarData(openList, closeList);
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
        private void AStarHelp(AINodeData currentNode, AINodeData checkNode, List<AINodeData> openList, List<AINodeData> closeList, Haxxit.Maps.ProgramHeadNode program, Haxxit.Maps.Point destination)
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
        private MoveCode PerformMoves(Haxxit.Maps.ProgramHeadNode program, Queue<Haxxit.Maps.Point> path)
        {
            if (path == null)
            {
                return MoveCode.NoMovesSpecified;
            }
            else if (!path.Any())
            {
                return MoveCode.NoMovesSpecified;
            }

            // Will hold all nodes touched by program both before and after move
            List<Haxxit.Maps.Point> programPoints = new List<Haxxit.Maps.Point>();

            // Get all the nodes currently associated with the program
            foreach (Haxxit.Maps.MapNode node in program.GetAllNodes())
            {
                programPoints.Insert(0, node.coordinate);
            }

            int movesLeft = program.Program.Moves.MovesLeft;
            Haxxit.Maps.Point currentHead = program.coordinate;
            foreach (Haxxit.Maps.Point destination in path)
            {
                Haxxit.Maps.Point direction = destination - currentHead;
                if (!IsInBounds(destination))
                {
                    return MoveCode.MoveWasOutOfBounds;
                }
                else if (Math.Abs(direction.X) == 1 && Math.Abs(direction.Y) != 0)
                {
                    return MoveCode.MoveWasNotAdjacent;
                }
                else if (Math.Abs(direction.Y) == 1 && Math.Abs(direction.X) != 0)
                {
                    return MoveCode.MoveWasNotAdjacent;
                }
                else if (Math.Abs(direction.X) != 1 && Math.Abs(direction.Y) != 1)
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
                Haxxit.Maps.MoveEventArgs nextMove = new Haxxit.Maps.MoveEventArgs(currentHead, direction);
                turnActions.Enqueue(new NotifyArgs("haxxit.map.move", this, nextMove));
                currentHead = destination;
            }
            int index = 0;
            foreach (Haxxit.Maps.Point point in programPoints)
            {
                if (index < programPoints.Count - program.Program.Size.MaxSize)
                {
                    mapData[point.X, point.Y].IsAvailable = true;
                    mapData[point.X, point.Y].OccupiedBy = null;
                }
                else
                {
                    mapData[point.X, point.Y].IsAvailable = false;
                    mapData[point.X, point.Y].OccupiedBy = program;
                }
                index++;
            }
            return MoveCode.Success;
        }

        // This function accepts program, target, and intended command.  It first performs error checking to
        // ensure that the command is valid, then updates the associated AINodeData and program list to reflect
        // game changes as a result of this command, and finally submits the command to the turnActions queue for
        // sending during subsequent calls to HandleAITurn.
        private CommandCode PerformCommand(Haxxit.Maps.Point target, Haxxit.Maps.Point source, Commands.Command command)
        {
            if (command == null)
            {
                return CommandCode.NoCommandSpecified;
            }
            else if (!IsInBounds(target))
            {
                return CommandCode.TargetWasOutOfBounds;
            }
            else if (mapData[target.X, target.Y].OccupiedBy == null)
            {
                return CommandCode.NoProgramAtTarget;
            }
            else if (command.Range < Math.Abs(target.X - source.X) + Math.Abs(target.Y - source.Y))
            {
                return CommandCode.ProgramOutOfRange;
            }

            // Will hold all nodes touched by target program both before and after command
            List<Haxxit.Maps.Point> programPoints = new List<Haxxit.Maps.Point>();

            // Get all the nodes currently associated with the program
            Haxxit.Maps.ProgramHeadNode targetProgram = mapData[target.X, target.Y].OccupiedBy;
            foreach (Haxxit.Maps.MapNode node in targetProgram.GetAllNodes())
            {
                programPoints.Insert(0, node.coordinate);
            }

            Haxxit.Maps.CommandEventArgs commandArgs = new Haxxit.Maps.CommandEventArgs(target, source, command.Name);
            turnActions.Enqueue(new NotifyArgs("haxxit.map.command", this, commandArgs));
            //if(command.GetType() == typeof(Tests.DynamicDamageCommand))
            if(command.GetType() == typeof(Commands.DamageCommand))
            {
                //int damage = ((Tests.DynamicDamageCommand)command).Strength;
                int damage = ((Commands.DamageCommand)command).Strength;
                int index = 0;
                foreach (Haxxit.Maps.Point point in programPoints)
                {
                    if (index < damage)
                    {
                        mapData[point.X, point.Y].IsAvailable = true;
                        mapData[point.X, point.Y].OccupiedBy = null;
                    }
                    index++;
                }
                if (index <= damage)
                {
                    enemyPrograms.Remove(targetProgram);
                }
            }
            else if (command.GetType() == typeof(Commands.DamageCommand))
            {
                int damage = ((Commands.DamageCommand)command).Strength;
                int index = 0;
                foreach (Haxxit.Maps.Point point in programPoints)
                {
                    if (index < damage)
                    {
                        mapData[point.X, point.Y].IsAvailable = true;
                        mapData[point.X, point.Y].OccupiedBy = null;
                    }
                    index++;
                }
                if (index <= damage)
                {
                    enemyPrograms.Remove(targetProgram);
                }
            }
            return CommandCode.Success;
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
