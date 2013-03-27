using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.Maps
{
    public abstract partial class Map
    {
        protected MapNode[,] map; // map[x,y]
        private bool spawning_finished;
        protected bool has_been_hacked;
        protected ushort earned_silicoins;

        /// <summary>
        /// Whether this map has been hacked (completed) yet.
        /// </summary>
        public bool HasBeenHacked
        {
            get
            {
                return has_been_hacked;
            }
        }

        /// <summary>
        /// How many Silicoins have been earned from this map so far.
        /// </summary>
        public ushort EarnedSilicoins
        {
            get
            {
                return earned_silicoins;
            }
        }

        /// <summary>
        /// The width of the map.
        /// </summary>
        public int XSize
        {
            get
            {
                return map.GetLength(0);
            }
        }

        /// <summary>
        /// The height of the map.
        /// </summary>
        public int YSize
        {
            get
            {
                return map.GetLength(1);
            }
        }

        /// <summary>
        /// Checks if the point (x, y) is within the bounds of the map.
        /// </summary>
        /// <param name="x">The X-coordinate of the point to check.</param>
        /// <param name="y">The Y-coordinate of the point to check.</param>
        /// <returns>Whether the point is in bounds.</returns>
        public bool IsInBounds(int x, int y)
        {
            return IsInBounds(new Point(x, y));
        }

        /// <summary>
        /// Checks if a point is within the bounds of the map.
        /// </summary>
        /// <param name="p">The point to check.</param>
        /// <returns>Whether the point is in bounds.</returns>
        public bool IsInBounds(Point p)
        {
            return p.IsInBounds(0, XSize - 1, 0, YSize - 1);
        }

        /// <summary>
        /// Creates a MapNode at the point (x, y).
        /// </summary>
        /// <typeparam name="T">The type of MapNode that is to be created.</typeparam>
        /// <param name="x">The X-coordinate of where the node will be created.</param>
        /// <param name="y">The Y-coordinate of where the node will be created.</param>
        /// <returns>Whether the node was able to be created.</returns>
        public bool CreateNode<T>(int x, int y) where T : MapNode, new()
        {
            return CreateNode<T>(new Point(x, y));
        }

        /// <summary>
        /// Creates a MapNode at the given point.
        /// </summary>
        /// <typeparam name="T">The type of node that is to be created.</typeparam>
        /// <param name="p">The point at which the node will be created.</param>
        /// <returns>Whether the node was able to be created.</returns>
        public bool CreateNode<T>(Point p) where T : MapNode, new()
        {
            if (!IsInBounds(p))
                return false;
            map[p.X, p.Y] = new T();
            map[p.X, p.Y].coordinate = p;
            return true;
        }

        /// <summary>
        /// Creates a MapNode at each point within the rectangle bounded by the two given points, (x1, y1) and (x2, y2).
        /// </summary>
        /// <typeparam name="T">The type of MapNode that is to be created.</typeparam>
        /// <param name="x1">The X-coordinate of one corner of the rectangle.</param>
        /// <param name="y1">The Y-coordinate of one corner of the rectangle.</param>
        /// <param name="x2">The X-coordinate of the opposite corner of the rectangle.</param>
        /// <param name="y2">The Y-coordinate of the opposite corner of the rectangle.</param>
        /// <returns>Whether all the nodes could be created.</returns>
        public bool CreateNodes<T>(int x1, int y1, int x2, int y2) where T : MapNode, new()
        {
            return CreateNodes<T>(new Point(x1, y1), new Point(x2, y2));
        }

        /// <summary>
        /// Creates a MapNode at each point within the rectangle bounded by the two given points.
        /// </summary>
        /// <typeparam name="T">The type of MapNode that is to be created.</typeparam>
        /// <param name="start">One corner of the rectangle.</param>
        /// <param name="end">The opposite corner of the rectangle.</param>
        /// <returns></returns>
        public bool CreateNodes<T>(Point start, Point end) where T : MapNode, new()
        {
            if (!IsInBounds(start) || !IsInBounds(end))
                return false;
            Point direction = start.DirectionsTo(end);
            foreach (Point p in start.IterateOverRange(end))
            {
                CreateNode<T>(p);
            }
            return true;
        }

        /// <summary>
        /// Checks the type of the node at the point (x,y).
        /// </summary>
        /// <typeparam name="T">The type to check the node at point (x,y) against.</typeparam>
        /// <param name="x">The X-coordinate of the node to be checked.</param>
        /// <param name="y">The Y-coordinate of the node to be checked.</param>
        /// <returns>Whether the node is of the given type.</returns>
        public bool NodeIsType<T>(int x, int y) where T : MapNode
        {
            return NodeIsType<T>(new Point(x, y));
        }

        /// <summary>
        /// Checks the type of the node at the given point.
        /// </summary>
        /// <typeparam name="T">The type to check the node at the given point against.</typeparam>
        /// <param name="p">The point of the node to be checked.</param>
        /// <returns>Whether the node is of the given type.</returns>
        public bool NodeIsType<T>(Point p) where T : MapNode
        {
            if (!IsInBounds(p))
                return false;
            return map[p.X, p.Y].GetType() == typeof(T) || map[p.X, p.Y].GetType().IsSubclassOf(typeof(T));
        }

        /// <summary>
        /// Gets the MapNode at point (x, y).
        /// </summary>
        /// <param name="x">The X-coordinate of the MapNode to be returned.</param>
        /// <param name="y">The Y-coordinate of the MapNode to be returned.</param>
        /// <returns>The MapNode at the given point.</returns>
        public MapNode GetNode(int x, int y)
        {
            return GetNode(new Point(x, y));
        }

        /// <summary>
        /// Gets the MapNode at the given point.
        /// </summary>
        /// <param name="p">The point of the MapNode to be returned.</param>
        /// <returns>The MapNode at the given point.</returns>
        public MapNode GetNode(Point p)
        {
            if (!IsInBounds(p))
                return null;
            return map[p.X, p.Y];
        }

        /// <summary>
        /// Spawns a program at the point (x,y).
        /// </summary>
        /// <typeparam name="T">The type of program to spawn.</typeparam>
        /// <param name="x">The X-coordinate of the point where the program will be spawned.</param>
        /// <param name="y">The Y-coordinate of the point where the program will be spawned.</param>
        /// <returns>Whether the program could be spawned at that point.</returns>
        public bool SpawnProgram<T>(int x, int y) where T : Program, new()
        {
            return SpawnProgram<T>(new Point(x, y));
        }

        /// <summary>
        /// Spawns a program at the given point.
        /// </summary>
        /// <typeparam name="T">The type of program to spawn.</typeparam>
        /// <param name="p">The point at which to spawn the program.</param>
        /// <returns>Whether the program could be spawned.</returns>
        public bool SpawnProgram<T>(Point p) where T : Program, new()
        {
            if (!IsInBounds(p) || !NodeIsType<SpawnNode>(p))
                return false;
            SpawnNode spawn = (SpawnNode)map[p.X, p.Y];
            spawn.program = new T();
            return true;
        }

        /// <summary>
        /// Converts all spawns that have programs to ProgramHeadNodes and spawns that don't have programs to
        /// AvailableNodes after a user has finished selecting their programs to use with this map.
        /// </summary>
        public void FinishedSpawning()
        {
            Point start = new Point(0, 0);
            Point end = new Point(XSize - 1, YSize - 1);
            foreach (Point p in start.IterateOverRange(end))
            {
                if (NodeIsType<SpawnNode>(p))
                {
                    SpawnNode node = (SpawnNode)map[p.X, p.Y];
                    if (node.IsTaken())
                    {
                        Program program = node.program;
                        ProgramHeadNode prognode = new ProgramHeadNode(program);
                        prognode.coordinate = p;
                        map[p.X, p.Y] = prognode;
                    }
                    else
                    {
                        map[p.X, p.Y] = new AvailableNode();
                    }
                }
            }
            spawning_finished = true;
        }

        /// <summary>
        /// Determines if a user has finished spawning their programs for use on this map.
        /// </summary>
        /// <returns>Whether the user has finished spawning their programs.</returns>
        public bool IsSpawningFinished()
        {
            return spawning_finished;
        }

        /// <summary>
        /// Moves a program at point (start_x, start_y) in the direction of (dir_x, dir_y). (dir_x, dir_y) must move
        /// the program one square, which means one of them must be a {+|-}1 and one must be a 0.
        /// </summary>
        /// <param name="start_x">The X-coordinate of the program that should be moved.</param>
        /// <param name="start_y">The Y-coordinate of the program that should be moved.</param>
        /// <param name="dir_x">The horizontal direction in which the program should move.</param>
        /// <param name="dir_y">The vertical direction in which the program should move.</param>
        /// <returns>Whether the move was successful.</returns>
        public bool MoveProgram(int start_x, int start_y, int dir_x, int dir_y)
        {
            return MoveProgram(new Point(start_x, start_y), new Point(dir_x, dir_y));
        }

        /// <summary>
        /// Moves a program from the starting point in the direction of the direction point. The direction point must
        /// move the program only one square, whih means either X must be {+|-}1 and Y must be 0, or Y must be {+|-}1
        /// and X must be 0.
        /// </summary>
        /// <param name="start">The starting point of the program that should be moved.</param>
        /// <param name="direction"> The direction point in which to move the program.</param>
        /// <returns>Whether the move was successful.</returns>
        public bool MoveProgram(Point start, Point direction)
        {
            Point end = start + direction;
            if (!IsSpawningFinished() || !IsInBounds(start) || !IsInBounds(end) || !NodeIsType<ProgramHeadNode>(start)
                || !(NodeIsType<AvailableNode>(end) || NodeIsType<ProgramTailNode>(end)) || !direction.IsDirectional())
                return false;
            bool end_is_tail_node = NodeIsType<ProgramTailNode>(end);
            ProgramHeadNode head_node = (ProgramHeadNode)map[start.X, start.Y];
            if (!head_node.program.Moves.CanMove() || head_node.program.AlreadyRanCommand())
                return false;
            ProgramTailNode tail_node = new ProgramTailNode(head_node.program, head_node, head_node.Tail);
            if (head_node.Tail != null)
                head_node.Tail.Head = tail_node;
            head_node.coordinate = end;
            head_node.Tail = tail_node;
            tail_node.coordinate = start;
            map[end.X, end.Y] = head_node;
            map[start.X, start.Y] = tail_node;

            if (head_node.program.Size.IsMaxSize() && !end_is_tail_node)
            {
                ProgramTailNode end_node = head_node.Tail;
                while (end_node.Tail != null)
                {
                    end_node = end_node.Tail;
                }
                Point coord = end_node.coordinate;
                if (this.NodeIsType<ProgramTailNode>(coord))
                {
                    map[coord.X, coord.Y] = new AvailableNode();
                    map[coord.X, coord.Y].coordinate = coord;
                }
                end_node.Head.Tail = null;
            }
            else if(!end_is_tail_node)
                head_node.program.Size.IncreaseCurrentSize(1);
            head_node.program.Moves.Moved();
            return true;
        }

        /// <summary>
        /// Undoes the movement of a program, which means it will make it so that it looks like a program never made a
        /// move. Unlike the MoveProgram method, this requires it also knowing whether the move being undone resized
        /// the program or whether the program moved over one of its own tail nodes.
        /// 
        /// This specific method allows for use of the method when the program is not resized. If the program was
        /// not resized, then it would require the use of a location of where the ending tail node was prior to the
        /// program moving.
        /// </summary>
        /// <param name="start">Where the program's movement started.</param>
        /// <param name="direction">What direction the program originally moved in.</param>
        /// <param name="program_resized">Whether the program's movement resized it.</param>
        /// <param name="end_was_tail_node">Whether where the program moved to was originally its own tail node.</param>
        /// <returns>Whether the undo was sucessful. It will always return false if the program was not resized and the
        /// end was not a tail node.</returns>
        public bool UndoMoveProgram(Point start, Point direction, bool program_resized, bool end_was_tail_node)
        {
            if (!end_was_tail_node && !program_resized)
                return false;
            return UndoMoveProgram(start, direction, program_resized, end_was_tail_node, new Point(-1, -1));
        }

        /// <summary>
        /// Undoes the movement of a program, which means it will make it so that it looks like a program never made a
        /// move. Unlike the MoveProgram method, this requires it also knowing whether the move being undone resized
        /// the program or whether the program moved over one of its own tail nodes.
        /// 
        /// This is the main UndoMoveProgram method, which allows for when the program was not resized. The tail
        /// location is only used if the program was neither resized nor when the end was a tail node. Otherwise, it
        /// can be any value and it doesn't matter.
        /// </summary>
        /// <param name="start">Where the program's movement started.</param>
        /// <param name="direction">What direction the program originally moved in.</param>
        /// <param name="program_resized">Whether the program's movement resized it.</param>
        /// <param name="end_was_tail_node">Whether where the program moved to was originally its own tail node.</param>
        /// <param name="tail_location">The location of the tail when the program was not resized.</param>
        /// <returns>Whether the undo was successful or not.</returns>
        public bool UndoMoveProgram(Point start, Point direction, bool program_resized, bool end_was_tail_node, Point tail_location)
        {
            Point end = start + direction;
            if (!IsSpawningFinished() || !IsInBounds(start) || !IsInBounds(end) || !NodeIsType<ProgramHeadNode>(end)
                || !(NodeIsType<AvailableNode>(start) || NodeIsType<ProgramTailNode>(start)) || !direction.IsDirectional())
                return false;
            ProgramHeadNode head_node = (ProgramHeadNode)map[end.X, end.Y];
            ProgramTailNode tail_node = head_node.Tail;
            if (tail_node != null && tail_node.Tail != null)
                tail_node.Tail.Head = head_node;
            if (tail_node != null)
                head_node.Tail = tail_node.Tail;
            map[start.X, start.Y] = head_node;
            head_node.coordinate = start;

            if (end_was_tail_node) // If the program originally moved over a tail node
            {
                ProgramTailNode end_node = head_node.Tail;
                while (end_node.Tail != null)
                {
                    end_node = end_node.Tail;
                }
                map[end.X, end.Y] = new ProgramTailNode(head_node.program, head_node);
                map[end.X, end.Y].coordinate = end;
                end_node.Tail = (ProgramTailNode)map[end.X, end.Y];
                ((ProgramTailNode)map[end.X, end.Y]).Head = end_node;
            }
            else if (!program_resized) // If the program wasn't resized
            {
                map[end.X, end.Y] = new AvailableNode();
                map[end.X, end.Y].coordinate = end;
                if (IsInBounds(tail_location))
                {
                    map[tail_location.X, tail_location.Y] = new ProgramTailNode(head_node.program, head_node);
                    map[tail_location.X, tail_location.Y].coordinate = tail_location;
                    ProgramTailNode end_node = head_node.Tail;
                    while (end_node.Tail != null)
                    {
                        end_node = end_node.Tail;
                    }
                    end_node.Tail = (ProgramTailNode)map[tail_location.X, tail_location.Y];
                    ((ProgramTailNode)map[tail_location.X, tail_location.Y]).Head = end_node;
                }
            }
            else // The program was resized and didn't move over a tail node
            {
                map[end.X, end.Y] = new AvailableNode();
                map[end.X, end.Y].coordinate = end;
                head_node.program.Size.DecreaseCurrentSize(1);
            }
            head_node.program.Moves.UndoMove();
            return true;
        }

        /// <summary>
        /// Runs the specified program's command directed at the attacked node.
        /// </summary>
        /// <param name="attacker_point">The point at which the program's head node is currently situated.</param>
        /// <param name="attacked_point">The point the program is attacking.</param>
        /// <param name="command">The name of the command that the program is using.</param>
        /// <returns></returns>
        public UndoCommand RunCommand(Point attacker_point, Point attacked_point, string command)
        {
            if (!NodeIsType<ProgramHeadNode>(attacker_point))
                return null;
            ProgramHeadNode attacker_node = (ProgramHeadNode)GetNode(attacker_point);
            if (!attacker_node.program.HasCommand(command))
                return null;
            Point distance = attacker_point.DistanceBetween(attacked_point);
            Command attack = attacker_node.program.GetCommand(command);
            if (distance.X + distance.Y > attack.Range)
                return null;
            UndoCommand undo_command = attacker_node.program.RunCommand(this, attacked_point, command);
            if(undo_command != null)
            {
                undo_command.OriginatingPoint = attacker_point;
                undo_command.AttackedPoint = attacked_point;
            }
            return undo_command;
        }

        public bool RunUndoCommand(UndoCommand command)
        {
            if (command == null || !NodeIsType<ProgramHeadNode>(command.OriginatingPoint))
                return false;
            ProgramHeadNode attacker_node = (ProgramHeadNode)GetNode(command.OriginatingPoint);
            attacker_node.program.RunUndoCommand(this, command);
            return true;
        }

        public bool AddNodeToProgram(Point program_point, Point where_to_add)
        {
            if (!NodeIsType<ProgramNode>(program_point) || !NodeIsType<AvailableNode>(where_to_add))
                return false;
            ProgramNode program_node = (ProgramNode)GetNode(program_point);
            while (program_node.Tail != null)
                program_node = program_node.Tail;
            ProgramTailNode new_tail = new ProgramTailNode(program_node.program, program_node.Head);
            map[where_to_add.X, where_to_add.Y] = new_tail;
            new_tail.Tail = program_node.Tail;
            program_node.Tail = new_tail;
            program_node.program.Size.IncreaseCurrentSize(1);
            return true;
        }

        public bool AddProgramNodes(IEnumerable<ProgramNode> nodes)
        {
            if (nodes.Count() == 0
                || (nodes.First().Head != null && !NodeIsType<ProgramNode>(nodes.First().Head.coordinate)))
                return false;
            foreach (ProgramNode node in nodes)
                if (!NodeIsType<AvailableNode>(node.coordinate))
                    return false;
            if (nodes.First().Head != null)
                nodes.First().Head.Tail = (ProgramTailNode)nodes.First();
            foreach (ProgramNode node in nodes)
            {
                map[node.coordinate.X, node.coordinate.Y] = node;
            }
            return true;
        }

        /// <summary>
        /// Increases the amount of Silicoins the player earned from this map.
        /// </summary>
        /// <param name="amount">The amount of Silicoins to add to the pool of earned Silicoins.</param>
        public void IncreraseEarnedSilicoins(ushort amount)
        {
            if (earned_silicoins + amount > earned_silicoins) // in order to avoid overflows.
                earned_silicoins += amount;
        }

        /// <summary>
        /// Decreases the amount of Silicoins the player earned from this map.
        /// </summary>
        /// <param name="amount">The amount of Silicoins to remove from the pool of earned Silicoins.</param>
        public void DecreaseEarnedSilicoins(ushort amount)
        {
            if (earned_silicoins < amount) // if it is, this would cause an underflow.
                earned_silicoins = 0;
            else
                earned_silicoins -= amount;
        }

        /// <summary>
        /// Resets all the programs so they can be used for a new turn.
        /// </summary>
        public void TurnDone()
        {
            foreach(Point p in (new Point(0, 0)).IterateOverRange(new Point(XSize-1, YSize-1)))
            {
                if (NodeIsType<ProgramHeadNode>(p))
                {
                    ((ProgramHeadNode)GetNode(p)).program.Reset();
                }
            }
        }
    }
}
