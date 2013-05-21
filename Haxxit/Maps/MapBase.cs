using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.Maps
{
    /// <summary>
    /// The class for a playable map.
    /// </summary>
    public abstract partial class Map
    {
        protected MapNode[,] map; // map[x,y]
        private bool spawning_finished; // Users have finished spawning
        protected bool has_been_hacked; // Whether this map has been completed already
        protected ushort earned_silicoins; // How many silicoins have been earned from this map
        protected ushort total_spawn_weights; // How many (and how large) programs can this map support?
        protected Queue<Player> players; // The player queue

        private void InitializeMap(int x_size, int y_size, ushort initial_silicoins, ushort total_spawn_weights)
        {
            map = new MapNode[x_size, y_size];
            foreach (Point p in (new Point(0, 0)).IterateOverRange(new Point(x_size - 1, y_size - 1)))
            {
                CreateNode(new UnavailableNodeFactory(), p);
            }
            players = new Queue<Player>();
            spawning_finished = false;

            has_been_hacked = false;
            earned_silicoins = initial_silicoins;
            this.total_spawn_weights = total_spawn_weights;
        }

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

        public ushort TotalSpawnWeights
        {
            get
            {
                return total_spawn_weights;
            }
        }

        public Point Low
        {
            get
            {
                return new Point(0, 0);
            }
        }

        public Point High
        {
            get
            {
                return new Point(XSize - 1, YSize - 1);
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
        /// The turn player's object.
        /// </summary>
        public Player CurrentPlayer
        {
            get
            {
                if (players.Count != 0)
                    return players.Peek();
                else
                    return null;
            }
        }

        public IEnumerable<Player> AllPlayers
        {
            get
            {
                return players.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Add a player to the player queue.
        /// </summary>
        /// <param name="player">The player being added to the queue.</param>
        public void AddPlayer(Player player)
        {
            if (players.Contains(player))
                return;
            players.Enqueue(player);
        }

        /// <summary>
        /// Remove a player from the player queue (if they are in the queue) then removes them from all owned tiles on the map.
        /// </summary>
        /// <param name="player">The player to remove from the queue.</param>
        public void RemovePlayer(Player player)
        {
            int iterations = players.Count;
            // I don't trust my own equals function for player, so here's a for loop.
            for (int i = 0; i < iterations; i++)
            {
                if (players.Peek() != player)
                    players.Enqueue(players.Dequeue());
                else
                {
                    players.Dequeue();
                    foreach(Point p in (new Point(0, 0)).IterateOverRange(new Point(XSize - 1, YSize - 1)))
                    {
                        if (NodeIsType<OwnedNode>(p))
                        {
                            OwnedNode node = (OwnedNode)GetNode(p);
                            if(node.Player == player)
                                node.Player = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remove all players from the queue and remove them all from the map.
        /// </summary>
        public void ClearPlayers()
        {
            players.Clear();
            foreach (Point p in (new Point(0, 0)).IterateOverRange(new Point(XSize - 1, YSize - 1)))
            {
                if (NodeIsType<OwnedNode>(p))
                {
                    OwnedNode node = (OwnedNode)GetNode(p);
                    node.Player = null;
                }
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
        /// <param name="factory">The factory that creates the MapNode.</param>
        /// <param name="x">The X-coordinate of where the node will be created.</param>
        /// <param name="y">The Y-coordinate of where the node will be created.</param>
        /// <param name="emit_changed_signal">Whether to emit a changed signal. Defaults to true.</param>
        /// <returns>Whether the node was able to be created.</returns>
        public bool CreateNode(IFactory<MapNode> factory, int x, int y, bool emit_changed_signal = true)
        {
            return CreateNode(factory, new Point(x, y), emit_changed_signal);
        }

        /// <summary>
        /// Creates a MapNode at each point within the rectangle bounded by the two given points, (x1, y1) and (x2, y2).
        /// </summary>
        /// <typeparam name="T">The type of MapNode that is to be created.</typeparam>
        /// <param name="x1">The X-coordinate of one corner of the rectangle.</param>
        /// <param name="y1">The Y-coordinate of one corner of the rectangle.</param>
        /// <param name="x2">The X-coordinate of the opposite corner of the rectangle.</param>
        /// <param name="y2">The Y-coordinate of the opposite corner of the rectangle.</param>
        /// <param name="emit_changed_signal">Whether to emit a changed signal. Defaults to true.</param>
        /// <returns>Whether all the nodes could be created.</returns>
        public bool CreateNodes(IFactory<MapNode> factory, int x1, int y1, int x2, int y2, bool emit_changed_signal = true)
        {
            return CreateNodes(factory, new Point(x1, y1), new Point(x2, y2), emit_changed_signal);
        }

        /// <summary>
        /// Creates a MapNode at each point within the rectangle bounded by the two given points.
        /// </summary>
        /// <typeparam name="T">The type of MapNode that is to be created.</typeparam>
        /// <param name="start">One corner of the rectangle.</param>
        /// <param name="end">The opposite corner of the rectangle.</param>
        /// <param name="emit_changed_signal">Whether to emit a changed signal. Defaults to true.</param>
        /// <returns></returns>
        public bool CreateNodes(IFactory<MapNode> factory, Point start, Point end, bool emit_changed_signal = true)
        {
            if (!IsInBounds(start) || !IsInBounds(end))
                return false;
            Point direction = start.DirectionsTo(end);
            foreach (Point p in start.IterateOverRange(end))
            {
                CreateNode(factory, p, false);
            }
            if (emit_changed_signal)
                MapChanged(start.IterateOverRange(end));
            return true;
        }

        /// <summary>
        /// Creates a MapNode at the given point.
        /// </summary>
        /// <param name="factory">The type of node that is to be created.</param>
        /// <param name="p">The point at which the node will be created.</param>
        /// <param name="emit_changed_signal">Whether to emit a changed signal. Defaults to true.</param>
        /// <returns>Whether the node was able to be created.</returns>
        public bool CreateNode(IFactory<MapNode> factory, Point p, bool emit_changed_signal = true)
        {
            if (!IsInBounds(p) || factory == null)
                return false;
            map[p.X, p.Y] = factory.NewInstance();
            map[p.X, p.Y].coordinate = p;
            map[p.X, p.Y].Notifiable = Mediator;
            if (emit_changed_signal)
                MapChanged(p);
            return true;
        }

        /// <summary>
        /// Creates a MapNode at the point (x, y).
        /// </summary>
        /// <typeparam name="T">The type of MapNode that is to be created.</typeparam>
        /// <param name="x">The X-coordinate of where the node will be created.</param>
        /// <param name="y">The Y-coordinate of where the node will be created.</param>
        /// <param name="emit_changed_signal">Whether to emit a changed signal. Defaults to true.</param>
        /// <returns>Whether the node was able to be created.</returns>
        public bool CreateNode<T>(int x, int y, bool emit_changed_signal = true) where T : MapNode, new()
        {
            return CreateNode<T>(new Point(x, y), emit_changed_signal);
        }

        /// <summary>
        /// Creates a MapNode at the given point.
        /// </summary>
        /// <typeparam name="T">The type of node that is to be created.</typeparam>
        /// <param name="p">The point at which the node will be created.</param>
        /// <param name="emit_changed_signal">Whether to emit a changed signal. Defaults to true.</param>
        /// <returns>Whether the node was able to be created.</returns>
        public bool CreateNode<T>(Point p, bool emit_changed_signal = true) where T : MapNode, new()
        {
            if (!IsInBounds(p))
                return false;
            map[p.X, p.Y] = new T();
            map[p.X, p.Y].coordinate = p;
            map[p.X, p.Y].Notifiable = Mediator;
            if (emit_changed_signal)
                MapChanged(p);
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
        /// <param name="emit_changed_signal">Whether to emit a changed signal. Defaults to true.</param>
        /// <returns>Whether all the nodes could be created.</returns>
        public bool CreateNodes<T>(int x1, int y1, int x2, int y2, bool emit_changed_signal = true) where T : MapNode, new()
        {
            return CreateNodes<T>(new Point(x1, y1), new Point(x2, y2), emit_changed_signal);
        }

        /// <summary>
        /// Creates a MapNode at each point within the rectangle bounded by the two given points.
        /// </summary>
        /// <typeparam name="T">The type of MapNode that is to be created.</typeparam>
        /// <param name="start">One corner of the rectangle.</param>
        /// <param name="end">The opposite corner of the rectangle.</param>
        /// <param name="emit_changed_signal">Whether to emit a changed signal. Defaults to true.</param>
        /// <returns></returns>
        public bool CreateNodes<T>(Point start, Point end, bool emit_changed_signal = true) where T : MapNode, new()
        {
            if (!IsInBounds(start) || !IsInBounds(end))
                return false;
            Point direction = start.DirectionsTo(end);
            foreach (Point p in start.IterateOverRange(end))
            {
                CreateNode<T>(p, false);
            }
            if (emit_changed_signal)
                MapChanged(start.IterateOverRange(end));
            return true;
        }

        /// <summary>
        /// Sets the owner of an OwnedNode to a player.
        /// </summary>
        /// <param name="p">The point at which an OwnedNode is located.</param>
        /// <param name="player">The player which will own the OwnedNode.</param>
        /// <returns></returns>
        public bool SetNodeOwner(Point p, Player player)
        {
            if (!IsInBounds(p) || !NodeIsType<OwnedNode>(p) || !players.Contains(player))
                return false;
            OwnedNode node = (OwnedNode)GetNode(p);
            node.Player = player;
            MapChanged(p);
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
        /// Gets the MapNode at the given location of the specified type then returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <returns></returns>
        public T GetNode<T>(Point p) where T: MapNode
        {
            if (!NodeIsType<T>(p))
                return null;
            return GetNode(p) as T;
        }

        /// <summary>
        /// Spawns a Program at the point (x,y).
        /// </summary>
        /// <param name="factory">The factory that will create an instance of the program.</param>
        /// <param name="x">The X-coordinate of the point where the Program will be spawned.</param>
        /// <param name="y">The Y-coordinate of the point where the Program will be spawned.</param>
        /// <returns>Whether the Program could be spawned at that point.</returns>
        public bool SpawnProgram(ProgramFactory factory, int x, int y)
        {
            return SpawnProgram(factory, new Point(x, y));
        }

        /// <summary>
        /// Spawns a Program at the given point.
        /// </summary>
        /// <param name="T">The factory that will create an instance of the program.</param>
        /// <param name="p">The point at which to spawn the Program.</param>
        /// <returns>Whether the Program could be spawned.</returns>
        public bool SpawnProgram(ProgramFactory factory, Point p)
        {
            if (!IsInBounds(p) || !NodeIsType<SpawnNode>(p))
                return false;
            if (total_spawn_weights != 0)
            {
                ushort spawn_weight_sum = 0;
                foreach (Point point in Low.IterateOverRange(High))
                    if (p != point && NodeIsType<SpawnNode>(point) && GetNode<SpawnNode>(point).program != null)
                        spawn_weight_sum += GetNode<SpawnNode>(point).program.SpawnWeight;
                if (spawn_weight_sum + factory.SpawnWeight > total_spawn_weights)
                    return false;
            }
            SpawnNode spawn = (SpawnNode)map[p.X, p.Y];
            spawn.program = factory;
            MapChanged(p);
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
            List<Point> changed_nodes = new List<Point>();
            foreach (Point p in start.IterateOverRange(end))
            {
                if (NodeIsType<SpawnNode>(p))
                {
                    SpawnNode node = (SpawnNode)map[p.X, p.Y];
                    if (node.IsTaken())
                    {
                        Program program = node.program.NewInstance();
                        ProgramHeadNode prognode = new ProgramHeadNode(program);
                        prognode.Notifiable = Mediator;
                        prognode.coordinate = p;
                        prognode.Player = node.Player;
                        map[p.X, p.Y] = prognode;
                    }
                    else
                    {
                        CreateNode(new AvailableNodeFactory(), p, false);
                    }
                    changed_nodes.Add(p);
                }
            }
            spawning_finished = true;
            MapChanged(changed_nodes);
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
        /// Moves a Program at point (start_x, start_y) in the direction of (dir_x, dir_y). (dir_x, dir_y) must move
        /// the Program one square, which means one of them must be a {+|-}1 and one must be a 0.
        /// </summary>
        /// <param name="start_x">The X-coordinate of the Program that should be moved.</param>
        /// <param name="start_y">The Y-coordinate of the Program that should be moved.</param>
        /// <param name="dir_x">The horizontal direction in which the Program should move.</param>
        /// <param name="dir_y">The vertical direction in which the Program should move.</param>
        /// <returns>Whether the move was successful.</returns>
        public bool MoveProgram(int start_x, int start_y, int dir_x, int dir_y)
        {
            return MoveProgram(new Point(start_x, start_y), new Point(dir_x, dir_y));
        }

        public bool CanMoveProgram(Point start, Point direction)
        {
            Point end = start + direction;
            if (!IsSpawningFinished() || !IsInBounds(start) || !IsInBounds(end) || !NodeIsType<ProgramHeadNode>(start)
                || !(NodeIsType<AvailableNode>(end) || NodeIsType<ProgramTailNode>(end)) || !direction.IsDirectional())
                return false;
            ProgramHeadNode head_node = (ProgramHeadNode)map[start.X, start.Y];
            if (NodeIsType<ProgramTailNode>(end) && !Object.ReferenceEquals(GetNode<ProgramTailNode>(end).Program, head_node.Program))
                return false;
            if (!head_node.Program.Moves.CanMove() || head_node.Program.AlreadyRanCommand() || CurrentPlayer != head_node.Player)
                return false;
            return true;
        }

        /// <summary>
        /// Moves a Program from the starting point in the direction of the direction point. The direction point must
        /// move the Program only one square, whih means either X must be {+|-}1 and Y must be 0, or Y must be {+|-}1
        /// and X must be 0.
        /// </summary>
        /// <param name="start">The starting point of the Program that should be moved.</param>
        /// <param name="direction"> The direction point in which to move the Program.</param>
        /// <returns>Whether the move was successful.</returns>
        public bool MoveProgram(Point start, Point direction)
        {
            Point end = start + direction;
            if (!CanMoveProgram(start, direction))
                return false;
            List<Point> changed_nodes = new List<Point>();
            bool end_is_tail_node = NodeIsType<ProgramTailNode>(end);
            ProgramHeadNode head_node = (ProgramHeadNode)map[start.X, start.Y];
            if(NodeIsType<AvailableNode>(end))
                ((AvailableNode)map[end.X, end.Y]).MovedOnto();
            ProgramTailNode tail_node = new ProgramTailNode(head_node.Program, head_node, head_node.Tail);
            tail_node.Notifiable = Mediator;
            if (head_node.Tail != null)
                head_node.Tail.Head = tail_node;
            head_node.coordinate = end;
            head_node.Tail = tail_node;
            tail_node.coordinate = start;
            tail_node.Player = head_node.Player;
            map[end.X, end.Y] = head_node;
            map[start.X, start.Y] = tail_node;
            changed_nodes.Add(start);
            changed_nodes.Add(end);

            if (end_is_tail_node)
            {
                ProgramTailNode end_node = head_node.Tail;
                while (end_node != null)
                {
                    if (end_node.coordinate == head_node.coordinate)
                    {
                        ProgramNode end_head_node = end_node.Head;
                        ProgramTailNode end_tail_node = end_node.Tail;
                        end_head_node.Tail = end_tail_node;
                        if (end_tail_node != null)
                            end_tail_node.Head = end_head_node;
                        end_node.Head = end_node.Tail = null;
                        break;
                    }
                    else
                        end_node = end_node.Tail;
                }
            }
            else if (head_node.Program.Size.IsMaxSize() && !end_is_tail_node)
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
                    changed_nodes.Add(coord);
                }
                end_node.Head.Tail = null;
            }
            else if(!end_is_tail_node)
                head_node.Program.Size.IncreaseCurrentSize(1);
            head_node.Program.Moves.Moved();
            MapChanged(changed_nodes);
            return true;
        }

        /// <summary>
        /// Undoes the movement of a Program, which means it will make it so that it looks like a Program never made a
        /// move. Unlike the MoveProgram method, this requires it also knowing whether the move being undone resized
        /// the Program or whether the Program moved over one of its own tail nodes.
        /// 
        /// This specific method allows for use of the method when the Program is not resized. If the Program was
        /// not resized, then it would require the use of a location of where the ending tail node was prior to the
        /// Program moving.
        /// </summary>
        /// <param name="start">Where the Program's movement started.</param>
        /// <param name="direction">What direction the Program originally moved in.</param>
        /// <param name="program_resized">Whether the Program's movement resized it.</param>
        /// <param name="end_was_tail_node">Whether where the Program moved to was originally its own tail node.</param>
        /// <returns>Whether the undo was sucessful. It will always return false if the Program was not resized and the
        /// end was not a tail node.</returns>
        public bool UndoMoveProgram(Point start, Point direction, bool program_resized, bool end_was_tail_node)
        {
            if (!end_was_tail_node && !program_resized)
                return false;
            return UndoMoveProgram(start, direction, program_resized, end_was_tail_node, new Point(-1, -1));
        }

        /// <summary>
        /// Undoes the movement of a Program, which means it will make it so that it looks like a Program never made a
        /// move. Unlike the MoveProgram method, this requires it also knowing whether the move being undone resized
        /// the Program or whether the Program moved over one of its own tail nodes.
        /// 
        /// This is the main UndoMoveProgram method, which allows for when the Program was not resized. The tail
        /// location is only used if the Program was neither resized nor when the end was a tail node. Otherwise, it
        /// can be any value and it doesn't matter.
        /// </summary>
        /// <param name="start">Where the Program's movement started.</param>
        /// <param name="direction">What direction the Program originally moved in.</param>
        /// <param name="program_resized">Whether the Program's movement resized it.</param>
        /// <param name="end_was_tail_node">Whether where the Program moved to was originally its own tail node.</param>
        /// <param name="tail_location">The location of the tail when the Program was not resized.</param>
        /// <returns>Whether the undo was successful or not.</returns>
        public bool UndoMoveProgram(Point start, Point direction, bool program_resized, bool end_was_tail_node, Point tail_location)
        {
            Point end = start + direction;
            if (!IsSpawningFinished() || !IsInBounds(start) || !IsInBounds(end) || !NodeIsType<ProgramHeadNode>(end)
                || !(NodeIsType<AvailableNode>(start) || NodeIsType<ProgramTailNode>(start)) || !direction.IsDirectional())
                return false;
            List<Point> changed_nodes = new List<Point>();
            ProgramHeadNode head_node = (ProgramHeadNode)map[end.X, end.Y];
            ProgramTailNode tail_node = head_node.Tail;
            if (tail_node != null && tail_node.Tail != null)
                tail_node.Tail.Head = head_node;
            if (tail_node != null)
                head_node.Tail = tail_node.Tail;
            map[start.X, start.Y] = head_node;
            head_node.coordinate = start;
            changed_nodes.Add(start);

            if (end_was_tail_node) // If the Program originally moved over a tail node
            {
                ProgramTailNode end_node = head_node.Tail;
                while (end_node.Tail != null)
                {
                    end_node = end_node.Tail;
                }
                map[end.X, end.Y] = new ProgramTailNode(head_node.Program, head_node);
                map[end.X, end.Y].coordinate = end;
                end_node.Tail = (ProgramTailNode)map[end.X, end.Y];
                ((ProgramTailNode)map[end.X, end.Y]).Head = end_node;
                changed_nodes.Add(end);
            }
            else if (!program_resized) // If the Program wasn't resized
            {
                map[end.X, end.Y] = new AvailableNode();
                map[end.X, end.Y].coordinate = end;
                changed_nodes.Add(end);
                if (IsInBounds(tail_location) && head_node.Program.Size.MaxSize != 1)
                {
                    map[tail_location.X, tail_location.Y] = new ProgramTailNode(head_node.Program, head_node);
                    map[tail_location.X, tail_location.Y].coordinate = tail_location;
                    map[tail_location.X, tail_location.Y].Notifiable = Mediator;

                    ProgramNode end_node = head_node;
                    while (end_node.Tail != null)
                    {
                        end_node = end_node.Tail;
                    }
                    end_node.Tail = (ProgramTailNode)map[tail_location.X, tail_location.Y];
                    ((ProgramTailNode)map[tail_location.X, tail_location.Y]).Head = end_node;
                    ((ProgramTailNode)map[tail_location.X, tail_location.Y]).Player = head_node.Player;
                    changed_nodes.Add(tail_location);
                }
            }
            else // The Program was resized and didn't move over a tail node
            {
                map[end.X, end.Y] = new AvailableNode();
                map[end.X, end.Y].coordinate = end;
                head_node.Program.Size.DecreaseCurrentSize(1);
            }
            head_node.Program.Moves.UndoMove();
            MapChanged(changed_nodes);
            return true;
        }

        /// <summary>
        /// Runs the specified Program's command directed at the attacked node.
        /// </summary>
        /// <param name="attacker_point">The point at which the Program's head node is currently situated.</param>
        /// <param name="attacked_point">The point the Program is attacking.</param>
        /// <param name="command">The name of the command that the Program is using.</param>
        /// <returns>An UndoCommand that will undo the actions of the command that was run, or null if no command was run.</returns>
        public virtual UndoCommand RunCommand(Point attacker_point, Point attacked_point, string command)
        {
            if (!NodeIsType<ProgramHeadNode>(attacker_point))
                return null;
            ProgramHeadNode attacker_node = (ProgramHeadNode)GetNode(attacker_point);
            if (attacker_node.Player != CurrentPlayer || !attacker_node.Program.HasCommand(command))
                return null;
            Point distance = attacker_point.DistanceBetween(attacked_point);
            Command attack = attacker_node.Program.GetCommand(command);
            if (distance.X + distance.Y > attack.Range)
                return null;
            UndoCommand undo_command = attacker_node.Program.RunCommand(this, attacked_point, command);
            if(undo_command != null)
            {
                undo_command.OriginatingPoint = attacker_point;
                undo_command.AttackedPoint = attacked_point;
            }
            return undo_command;
        }

        /// <summary>
        /// Runs an UndoCommand that was returned by the RunCommand function.
        /// </summary>
        /// <param name="command">The UndoCommand to run.</param>
        /// <returns>Whether the UndoCommand was sucessfully ran.</returns>
        public bool RunUndoCommand(UndoCommand command)
        {
            if (command == null || !NodeIsType<ProgramHeadNode>(command.OriginatingPoint))
                return false;
            ProgramHeadNode attacker_node = (ProgramHeadNode)GetNode(command.OriginatingPoint);
            attacker_node.Program.RunUndoCommand(this, command);
            return true;
        }

        /// <summary>
        /// Adds a node to a program in order to increase its size, but doesn't go past the program's maximum size.
        /// </summary>
        /// <param name="program_point">The location of a node in the program.</param>
        /// <param name="where_to_add">An available node where you want the node added.</param>
        /// <returns>Whether the node was able to be added to the program.</returns>
        public bool AddNodeToProgram(Point program_point, Point where_to_add)
        {
            if (!NodeIsType<ProgramNode>(program_point) || !NodeIsType<AvailableNode>(where_to_add))
                return false;
            ProgramNode program_node = (ProgramNode)GetNode(program_point);
            if (program_node.Program.Size.IsMaxSize())
                return false;
            while (program_node.Tail != null)
                program_node = program_node.Tail;
            ProgramTailNode new_tail = new ProgramTailNode(program_node.Program, program_node.Head);
            map[where_to_add.X, where_to_add.Y] = new_tail;
            new_tail.Tail = program_node.Tail;
            program_node.Tail = new_tail;
            program_node.Program.Size.IncreaseCurrentSize(1);
            MapChanged(where_to_add);
            return true;
        }

        /// <summary>
        /// Readds removed ProgramNodes to the map.
        /// </summary>
        /// <param name="nodes">The list of ProgramNodes that has been removed.</param>
        /// <returns>Whether the nodes could be added to the map.</returns>
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
            MapChanged(nodes.Select<ProgramNode, Point>(x => x.coordinate));
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
        /// Resets all the programs so they can be used for a new turn, then rotates the player queue.
        /// </summary>
        public void TurnDone()
        {
            if(spawning_finished)
                foreach(Point p in (new Point(0, 0)).IterateOverRange(new Point(XSize-1, YSize-1)))
                {
                    if (NodeIsType<ProgramHeadNode>(p))
                    {
                        ((ProgramHeadNode)GetNode(p)).Program.Reset();
                    }
                }
            if(players.Count != 0)
                players.Enqueue(players.Dequeue()); // Rotate the queue
        }

        //protected abstract Player CheckIfMapHacked();
    }
}
