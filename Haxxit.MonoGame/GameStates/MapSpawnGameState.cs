using System;
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
        SpriteFont arial_16px_regular, arial_12px_regular;
        Haxxit.Maps.Map map;
        Haxxit.UndoStack undo_stack;
        Dictionary<Haxxit.Player, Tuple<Color, Color>> players;
        Vector2 finished_text_size;
        DrawableRectangle finished_button;

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
        }

        public void OnRectangleClick(DrawableRectangle rectangle)
        {
            Haxxit.Maps.Point haxxit_location = rectangle.Area.Center.ToHaxxitPoint(map_rectangle_size, map_border_size);
            if (map.NodeIsType<Haxxit.Maps.SpawnNode>(haxxit_location)
                && map.GetNode<Haxxit.Maps.SpawnNode>(haxxit_location).Player == map.CurrentPlayer)
            {
                SpawnDialogGameState new_state = new SpawnDialogGameState(this, map, haxxit_location);
                _mediator_manager.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
            }
        }

        public void OnFinishedClick(DrawableRectangle rectangle)
        {
            map.FinishedSpawning();
            UserMapGameState new_state = new UserMapGameState(map);
            _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            arial_16px_regular = content.Load<SpriteFont>("Arial-16px-Regular");
            arial_12px_regular = content.Load<SpriteFont>("Arial-12px-Regular");
            finished_text_size = arial_16px_regular.MeasureString("Finished");
            finished_button = new DrawableRectangle(
                rectangle_texture,
                new Rectangle(
                    800 - (int)Math.Floor(finished_text_size.X) - 15,
                    480 - (int)Math.Floor(finished_text_size.Y) - 15,
                    (int)Math.Floor(finished_text_size.X) + 10,
                    (int)Math.Floor(finished_text_size.Y) + 10
                ),
                Color.Green
            );
            finished_button.OnMouseLeftClick += OnFinishedClick;
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
                rectangles.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, 6), node_color));
                if (map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
                {
                    Haxxit.Maps.ProgramHeadNode head_node = (Haxxit.Maps.ProgramHeadNode)map.GetNode(p);
                    string Name = head_node.Program.TypeName[0].ToString();
                }
                else if (map.NodeIsType<Haxxit.Maps.ProgramTailNode>(p))
                {

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

                foreach (DrawableRectangle rectangle in map_squares[p].Item2)
                {
                    rectangle.Update();
                }
            }
            finished_button.Update();
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            finished_button.Draw(sprite_batch);
            Vector2 finished_text_position = new Vector2(finished_button.Area.X + (finished_button.Area.Width - finished_text_size.X) / 2,
                finished_button.Area.Y + (finished_button.Area.Height - finished_text_size.Y) / 2);
            sprite_batch.DrawString(arial_16px_regular, "Finished", finished_text_position, Color.White);

            foreach (Tuple<Haxxit.Maps.MapNode, IEnumerable<DrawableRectangle>> tuple in map_squares.Values)
            {
                foreach (DrawableRectangle rectangle in tuple.Item2)
                {
                    rectangle.Draw(sprite_batch);
                }
            }

            List<string> bottom_status = new List<string>();
            bottom_status.Add(map.CurrentPlayer.Name + "'s turn.");
            for (int i = 0; i < bottom_status.Count; i++)
            {
                Vector2 bottom_status_position = new Vector2(10, 450 - 18 * (bottom_status.Count - i - 1));
                sprite_batch.DrawString(arial_12px_regular, bottom_status[i], bottom_status_position, Color.White);
            }
        }
    }
}
