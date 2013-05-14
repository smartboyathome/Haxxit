﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Haxxit = SmartboyDevelopments.Haxxit;

namespace SmartboyDevelopments.Haxxit.MonoGame.GameStates
{
    public class MapSpawnGameState : HaxxitGameState
    {
        const int map_rectangle_size = 24;
        const int map_border_size = 6;

        Texture2D rectangle_texture;
        Dictionary<Haxxit.Maps.Point, Tuple<Haxxit.Maps.MapNode, IEnumerable<DrawableRectangle>>> map_squares;
        List<DrawableRectangle> extra;
        DrawableRectangle turn_done_button, undo_button;
        List<Tuple<DrawableRectangle, string>> attacks;
        List<Tuple<Haxxit.Maps.Point, string>> head_nodes;
        SpriteFont arial_16px_regular, arial_12px_regular;
        Haxxit.Maps.Map map;
        Haxxit.UndoStack undo_stack;
        Dictionary<Haxxit.Player, Tuple<Color, Color>> players;
        Haxxit.Maps.Point selected_node;
        string selected_attack;

        public MapSpawnGameState(Haxxit.Maps.Map map) :
            base()
        {
            this.map = map;
            undo_stack = new UndoStack(256);
        }

        public override void NewMediator(SimplePubSub.IMediator mediator)
        {
            map.Mediator = mediator;
            undo_stack.Mediator = mediator;
        }

        public override void Init()
        {
            players = new Dictionary<Player, Tuple<Color, Color>>();
            Random rand = new Random();
            int count = 0;
            foreach (Player p in map.AllPlayers)
            {
                Color player_tail_color, player_head_color;
                if (count == 0)
                {
                    player_tail_color = Color.Gold;
                    player_head_color = Color.OrangeRed;
                }
                else if (count == 1)
                {
                    player_tail_color = Color.SandyBrown;
                    player_head_color = Color.Lerp(Color.SandyBrown, Color.Red, 0.25f);
                }
                else
                {
                    player_tail_color = Color.Peru;
                    player_head_color = Color.Orchid;
                }
                players.Add(p, new Tuple<Color, Color>(player_head_color, player_tail_color));
                count++;
            }
            map_squares = new Dictionary<Haxxit.Maps.Point, Tuple<Haxxit.Maps.MapNode, IEnumerable<DrawableRectangle>>>();
            head_nodes = new List<Tuple<Haxxit.Maps.Point, string>>();
            extra = new List<DrawableRectangle>();
            attacks = new List<Tuple<DrawableRectangle, string>>();
            selected_node = new Haxxit.Maps.Point(-1, -1);
            selected_attack = "";
        }

        private void DrawProgramExtras(Haxxit.Maps.Point head_location)
        {
            if (!map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(head_location))
                return;
            Haxxit.Maps.ProgramHeadNode node = map.GetNode<Haxxit.Maps.ProgramHeadNode>(head_location);
            extra.Clear();
            attacks.Clear();
            if (node.Program.Moves.CanMove())
            {
                List<Haxxit.Maps.Point> points = head_location.GetOrthologicalNeighbors().ToList();
                foreach (Haxxit.Maps.Point p in points)
                {
                    if (map.NodeIsType<Haxxit.Maps.AvailableNode>(p) ||
                        (map.NodeIsType<Haxxit.Maps.ProgramNode>(p) && map.GetNode<Haxxit.Maps.ProgramNode>(p).Program == node.Program))
                    {
                        extra.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, map_border_size),
                            Color.White * 0.375f));
                    }
                }
            }
            if (!node.Program.AlreadyRanCommand())
            {
                foreach (string command in node.Program.GetAllCommands())
                {
                    Vector2 text_size = arial_16px_regular.MeasureString(command);
                    DrawableRectangle rectangle = new DrawableRectangle(rectangle_texture,
                            new Rectangle(760 - (int)Math.Floor(text_size.Y), 10 + ((int)Math.Floor(text_size.Y) + 15) * attacks.Count, 
                                (int)Math.Floor(text_size.X) + 10, (int)Math.Floor(text_size.Y) + 10),
                            Color.Red);
                    rectangle.OnMouseLeftClick += OnAttackClick;
                    attacks.Add(new Tuple<DrawableRectangle, string>(rectangle, command));
                }
            }
        }

        public void OnRectangleClick(DrawableRectangle rectangle)
        {
            Haxxit.Maps.Point haxxit_location = rectangle.Area.Center.ToHaxxitPoint(map_rectangle_size, map_border_size);
            if (map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(haxxit_location)
                && map.GetNode<Haxxit.Maps.ProgramHeadNode>(haxxit_location).Player == map.CurrentPlayer)
            {
                if (map_squares.ContainsKey(selected_node) && map_squares[selected_node].Item2.Count() > 0)
                {
                    map_squares[selected_node].Item2.First().BorderSize = 0;
                }
                if (map_squares.ContainsKey(haxxit_location) && map_squares[haxxit_location].Item2.Count() > 0)
                {
                    map_squares[haxxit_location].Item2.First().BorderSize = 2;
                    DrawProgramExtras(haxxit_location);

                }
                selected_node = haxxit_location;
            }
            else if (map.NodeIsType<Haxxit.Maps.SpawnNode>(haxxit_location)
                && map.GetNode<Haxxit.Maps.SpawnNode>(haxxit_location).Player == map.CurrentPlayer)
            {
                SpawnDialogGameState new_state = new SpawnDialogGameState(this, map, haxxit_location);
                _mediator_manager.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
            }
            else
            {
                Haxxit.Maps.Point difference = haxxit_location - selected_node;
                bool can_move = map.CanMoveProgram(selected_node, difference);
                if (selected_attack != "")
                {
                    Haxxit.Maps.Point attacked_point = rectangle.Area.Center.ToHaxxitPoint(map_rectangle_size, map_border_size);
                    _mediator_manager.Notify("haxxit.map.command", this,
                        new Haxxit.Maps.CommandEventArgs(attacked_point, selected_node, selected_attack));
                    //map.RunCommand(selected_node, attacked_point, selected_attack);
                    selected_attack = "";
                    extra.Clear();
                    attacks.Clear();
                }
                if (can_move)
                {
                    _mediator_manager.Notify("haxxit.map.move", this, new Haxxit.Maps.MoveEventArgs(selected_node, difference));
                    //map.MoveProgram(selected_node, difference);
                    DrawProgramExtras(haxxit_location);
                    selected_node = haxxit_location;
                }
                else
                {
                    if (map_squares.ContainsKey(selected_node) && map_squares[selected_node].Item2.Count() > 0)
                    {
                        map_squares[selected_node].Item2.First().BorderSize = 0;
                    }
                    extra.Clear();
                    attacks.Clear();
                    selected_node = new Haxxit.Maps.Point(-1, -1);
                }
            }
        }

        public void OnUndoClick(DrawableRectangle rectangle)
        {
            _mediator_manager.Notify("haxxit.undo_stack.trigger", this, new EventArgs());
        }

        public void OnRectangleInside(DrawableRectangle rectangle)
        {
            rectangle.BorderSize = 2;
        }

        public void OnRectangleOutside(DrawableRectangle rectangle)
        {
            rectangle.BorderSize = 0;
        }

        public void OnTurnDoneClick(DrawableRectangle rectangle)
        {
            _mediator_manager.Notify("haxxit.map.turn_done", this, new EventArgs());
            extra.Clear();
            attacks.Clear();
            if (map_squares.ContainsKey(selected_node))
            {
                map_squares[selected_node].Item2.First().BorderSize = 0;
                selected_node = new Haxxit.Maps.Point(-1, -1);
            }
        }

        public void OnAttackClick(DrawableRectangle rectangle)
        {
            extra.Clear();
            foreach (Tuple<DrawableRectangle, string> attack in attacks)
            {
                if (attack.Item1 == rectangle)
                {
                    Commands.Command command = map.GetNode<Haxxit.Maps.ProgramHeadNode>(selected_node).Program.GetCommand(attack.Item2);
                    foreach (Haxxit.Maps.Point p in selected_node.GetPointsWithinDistance(command.Range))
                    {
                        if (map.IsInBounds(p))
                        {
                            DrawableRectangle attack_node = new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, map_border_size), Color.Red * 0.25f);
                            extra.Add(attack_node);
                        }
                    }
                    selected_attack = attack.Item2;
                }
            }
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            arial_16px_regular = content.Load<SpriteFont>("Arial-16px-Regular");
            arial_12px_regular = content.Load<SpriteFont>("Arial-12px-Regular");
            turn_done_button = new DrawableRectangle(rectangle_texture, new Rectangle(690, 440, 100, 30), Color.Green);
            turn_done_button.OnMouseLeftClick += OnTurnDoneClick;
            undo_button = new DrawableRectangle(rectangle_texture, new Rectangle(580, 440, 100, 30), Color.Orange);
            undo_button.OnMouseLeftClick += OnUndoClick;
        }

        private void MapHackedListener(string channel, object sender, EventArgs args)
        {
            Haxxit.Maps.HackedEventArgs event_args = args as Haxxit.Maps.HackedEventArgs;
            //WinGameState new_state = new WinGameState(event_args.EarnedSilicoins, this);
            WinGameState new_state = new WinGameState(500, this);
            _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
        }

        public override void SubscribeAll()
        {
            // Add any channels you are subscribing to here by doing the following:
            // Mediator.Subscribe("haxxit.xxx.yyy.zzz", NameOfListener);
            //
            // You must also have a function in this class with the following structure:
            // public void NameOfListener(string channel, object sender, EventArgs args)
            // With channel being the channel the notification is sent through, sender
            // being the object that sent the notification, and EventArgs being the
            // arguments for the listener. If you need arguments, create a subclass of
            // EventArgs with the arguments as properties.

            _mediator_manager.Subscribe("haxxit.map.hacked", MapHackedListener);
        }

        private IEnumerable<DrawableRectangle> MapNodeToRectangle(Haxxit.Maps.Point p)
        {
            List<DrawableRectangle> rectangles = new List<DrawableRectangle>();
            if (map.NodeIsType<Haxxit.Maps.AvailableNode>(p))
            {
                Rectangle square = p.ToXNARectangle(map_rectangle_size, 6);
                rectangles.Add(new DrawableRectangle(rectangle_texture, square, Color.Blue));
                if (map.NodeIsType<Haxxit.Maps.SilicoinNode>(p))
                    rectangles.Add(new DrawableRectangle(rectangle_texture, new Rectangle(square.X + 10, square.Y + 10, 4, 4), Color.Green));
                else if (map.NodeIsType<Haxxit.Maps.DataNode>(p))
                    rectangles.Add(new DrawableRectangle(rectangle_texture, new Rectangle(square.X + 10, square.Y + 10, 4, 4), Color.Red));
            }
            else if (map.NodeIsType<Haxxit.Maps.SpawnNode>(p))
            {
                rectangles.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, 6), Color.Purple, 2, new Color(255, 255, 255, 127)));
            }
            else if (map.NodeIsType<Haxxit.Maps.ProgramNode>(p))
            {
                Haxxit.Maps.ProgramNode program_node = (Haxxit.Maps.ProgramNode)map.GetNode(p);
                Tuple<Color, Color> player_color;
                if (!players.TryGetValue(program_node.Player, out player_color))
                    player_color = new Tuple<Color,Color>(Color.Transparent, Color.Transparent);
                Color node_color = player_color.Item2;
                if (map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
                    node_color = player_color.Item1;
                if (p == selected_node)
                    rectangles.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, 6), node_color, 2, Color.White));
                else
                    rectangles.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, 6), node_color));
                if (map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
                {
                    Haxxit.Maps.ProgramHeadNode head_node = (Haxxit.Maps.ProgramHeadNode)map.GetNode(p);
                    string Name = head_node.Program.TypeName[0].ToString();
                    head_nodes.Add(new Tuple<Haxxit.Maps.Point, string>(p, Name));
                }
                else if (map.NodeIsType<Haxxit.Maps.ProgramTailNode>(p))
                {
                    /* NOT FINISHED! Used for drawing connectors between nodes.
                    Maps.ProgramTailNode this_node = map.GetNode<Maps.ProgramTailNode>(p);
                    foreach (Maps.ProgramNode node in this_node.GetAllNodes())
                    {
                        Maps.Point difference = node.coordinate - this_node.coordinate;
                        Point center = rectangles.First().Area.Center;
                        int width = rectangles.First().Area.Width;
                        int height = rectangles.First().Area.Height;
                        if (!difference.IsDirectional())
                            continue;
                        else if (difference == new Maps.Point(0, 1)) // Down
                        {
                            width /= 4;
                        }
                        else if (difference == new Maps.Point(0, -1)) // Up
                        {

                        }
                        else if (difference == new Maps.Point(1, 0)) // Right
                        {

                        }
                        else // Left
                        {

                        }
                    }*/
                }
            }
            if (rectangles.Count() > 0)
            {
                rectangles.First().OnMouseLeftClick += OnRectangleClick;
                rectangles.First().OnMouseMiddleClick += OnRectangleClick;
                rectangles.First().OnMouseRightClick += OnRectangleClick;
            }
            return rectangles;
        }

        public override void Update()
        {
            // To change a scene, just call one of these:
            // Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.pop", this, new EventArgs());

            head_nodes.Clear();
            foreach (SmartboyDevelopments.Haxxit.Maps.Point p in map.Low.IterateOverRange(map.High))
            {
                Haxxit.Maps.MapNode node = map.GetNode(p);
                if (!map_squares.ContainsKey(p))
                {
                    IEnumerable<DrawableRectangle> rectangles = MapNodeToRectangle(p);
                    map_squares.Add(p,
                        new Tuple<Haxxit.Maps.MapNode, IEnumerable<DrawableRectangle>>(node, rectangles));
                }
                else
                {
                    Tuple<Haxxit.Maps.MapNode, IEnumerable<DrawableRectangle>> old = map_squares[p];
                    if (!Object.ReferenceEquals(old.Item1, map.GetNode(p)))
                    {
                        IEnumerable<DrawableRectangle> rectangles = MapNodeToRectangle(p);
                        map_squares[p] = new Tuple<Haxxit.Maps.MapNode, IEnumerable<DrawableRectangle>>(map.GetNode(p), rectangles);
                    }
                }

                if (map.CurrentPlayer.GetType() != typeof(Haxxit.MonoGame.PlayerAI))
                {
                    foreach (DrawableRectangle rectangle in map_squares[p].Item2)
                    {
                        rectangle.Update();
                    }
                }
            }

            if (map.CurrentPlayer.GetType() != typeof(Haxxit.MonoGame.PlayerAI))
            {
                foreach (DrawableRectangle rectangle in extra)
                {
                    rectangle.Update();
                }
                foreach (Tuple<DrawableRectangle, string> attack in attacks)
                {
                    attack.Item1.Update();
                }
                turn_done_button.Update();
                undo_button.Update();
            }
            else // AI is in charge
            {
                ((Haxxit.MonoGame.PlayerAI)map.CurrentPlayer).HandleAITurn(map);
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            turn_done_button.Draw(sprite_batch);
            Vector2 turn_done_size = arial_16px_regular.MeasureString("Turn Done");
            Vector2 turn_done_position = new Vector2(turn_done_button.Area.X + (turn_done_button.Area.Width - turn_done_size.X)/2,
                turn_done_button.Area.Y + (turn_done_button.Area.Height - turn_done_size.Y)/2);
            sprite_batch.DrawString(arial_16px_regular, "Turn Done", turn_done_position, Color.White);

            undo_button.Draw(sprite_batch);
            Vector2 undo_text_size = arial_16px_regular.MeasureString("Undo");
            Vector2 undo_text_position = new Vector2(undo_button.Area.X + (undo_button.Area.Width - undo_text_size.X)/2,
                undo_button.Area.Y + (undo_button.Area.Height - undo_text_size.Y)/2);
            sprite_batch.DrawString(arial_16px_regular, "Undo", undo_text_position, Color.White);

            foreach (Tuple<Haxxit.Maps.MapNode, IEnumerable<DrawableRectangle>> tuple in map_squares.Values)
            {
                foreach (DrawableRectangle rectangle in tuple.Item2)
                {
                    rectangle.Draw(sprite_batch);
                }
            }
            foreach (DrawableRectangle rectangle in extra)
            {
                rectangle.Draw(sprite_batch);
            }
            foreach (Tuple<DrawableRectangle, string> attack in attacks)
            {
                attack.Item1.Draw(sprite_batch);
                Vector2 command_size = arial_16px_regular.MeasureString(attack.Item2);
                sprite_batch.DrawString(arial_16px_regular, attack.Item2,
                    new Vector2(attack.Item1.Area.X + (attack.Item1.Area.Width - command_size.X) / 2,
                        attack.Item1.Area.Y + (attack.Item1.Area.Width - command_size.X) / 2), Color.White);
            }
            foreach (Tuple<Haxxit.Maps.Point, string> tuple in head_nodes)
            {
                Rectangle position = tuple.Item1.ToXNARectangle(map_rectangle_size, 6);
                Vector2 text_size = arial_16px_regular.MeasureString(tuple.Item2);
                sprite_batch.DrawString(arial_16px_regular, tuple.Item2,
                    new Vector2(position.X + (position.Width - text_size.X)/2, position.Y + (position.Height - text_size.Y)/2), Color.White);
            }

            List<string> bottom_status = new List<string>();
            bottom_status.Add(map.CurrentPlayer.Name + "'s turn.");
            if (map.IsInBounds(selected_node) && map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(selected_node))
            {
                Haxxit.Maps.ProgramHeadNode node = map.GetNode<Haxxit.Maps.ProgramHeadNode>(selected_node);
                //bottom_status += " " + node.Program.TypeName + " has " + node.Program.Moves.MovesLeft + " moves left.";
                string extra_status = bottom_status.First() + " " + node.Program.TypeName + " has " + node.Program.Moves.MovesLeft + " moves left.";
                bottom_status.Remove(bottom_status.First());
                bottom_status.Add(extra_status);
                if (selected_attack != "")
                {
                    Commands.Command command = node.Program.GetCommand(selected_attack);
                    bottom_status.Add(selected_attack + ": " + command.Description);
                }
            }
            for (int i = 0; i < bottom_status.Count; i++)
            {
                Vector2 bottom_status_position = new Vector2(10, 450 - 18 * (bottom_status.Count - i - 1));
                sprite_batch.DrawString(arial_12px_regular, bottom_status[i], bottom_status_position, Color.White);
            }
        }
    }
}