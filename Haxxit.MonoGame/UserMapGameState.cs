using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Haxxit = SmartboyDevelopments.Haxxit;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    public class UserMapGameState : HaxxitGameState
    {
        const int map_rectangle_size = 24;
        const int map_border_size = 6;

        Texture2D rectangle_texture;
        //List<DrawableRectangle> map_squares;
        //Dictionary<Haxxit.Maps.MapNode, DrawableRectangle> map_squares;
        Dictionary<Haxxit.Maps.Point, Tuple<Haxxit.Maps.MapNode, IEnumerable<DrawableRectangle>>> map_squares;
        List<DrawableRectangle> extra;
        List<Tuple<Haxxit.Maps.Point, string>> head_nodes;
        SpriteFont arial_16px_regular;
        Haxxit.Maps.Map map;
        Dictionary<Haxxit.Player, Color> players;
        Maps.Point selected_node;

        public UserMapGameState(Haxxit.Maps.Map map) :
            base()
        {
            this.map = map;
        }

        public override void Init()
        {
            players = new Dictionary<Player, Color>();
            Random rand = new Random();
            int count = 0;
            foreach (Player p in map.AllPlayers)
            {
                Color player_color;
                if (count == 0)
                    player_color = Color.Gold;
                else if (count == 1)
                    player_color = Color.Purple;
                else
                    player_color = Color.Peru;
                players.Add(p, player_color);
                count++;
            }
            map_squares = new Dictionary<Haxxit.Maps.Point, Tuple<Maps.MapNode, IEnumerable<DrawableRectangle>>>();
            head_nodes = new List<Tuple<Maps.Point, string>>();
            extra = new List<DrawableRectangle>();
            selected_node = new Maps.Point(-1, -1);
        }

        public void OnRectangleClick(DrawableRectangle rectangle)
        {
            Haxxit.Maps.Point haxxit_location = rectangle.Area.Center.ToHaxxitPoint(map_rectangle_size, map_border_size);
            if (map.NodeIsType<Maps.ProgramHeadNode>(haxxit_location))
            {
                if (map_squares.ContainsKey(selected_node) && map_squares[selected_node].Item2.Count() > 0)
                {
                    map_squares[selected_node].Item2.First().BorderSize = 0;
                }
                if (map_squares.ContainsKey(haxxit_location) && map_squares[haxxit_location].Item2.Count() > 0)
                {
                    map_squares[haxxit_location].Item2.First().BorderSize = 2;
                    List<Maps.Point> points = haxxit_location.GetOrthologicalNeighbors().ToList();
                    foreach (Maps.Point p in points)
                    {
                        if (map.NodeIsType<Maps.AvailableNode>(p))
                        {
                            extra.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, map_border_size),
                                new Color(Color.White, 0.375f)));
                        }
                    }
                }
                selected_node = haxxit_location;
            }
            else
            {
                if (map_squares.ContainsKey(selected_node) && map_squares[selected_node].Item2.Count() > 0)
                {
                    map_squares[selected_node].Item2.First().BorderSize = 0;
                }
                extra.Clear();
                selected_node = new Maps.Point(-1, -1);
            }
        }

        public void OnRectangleInside(DrawableRectangle rectangle)
        {
            rectangle.BorderSize = 2;
        }

        public void OnRectangleOutside(DrawableRectangle rectangle)
        {
            rectangle.BorderSize = 0;
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            arial_16px_regular = content.Load<SpriteFont>("Arial");
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
                Color player_color;
                if (!players.TryGetValue(program_node.Player, out player_color))
                    player_color = Color.Transparent;
                rectangles.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, 6), player_color));
                if (map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
                {
                    Haxxit.Maps.ProgramHeadNode head_node = (Haxxit.Maps.ProgramHeadNode)map.GetNode(p);
                    string Name = head_node.Program.TypeName[0].ToString();
                    head_nodes.Add(new Tuple<Haxxit.Maps.Point, string>(p, Name));
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

            extra.Clear();
            foreach (SmartboyDevelopments.Haxxit.Maps.Point p in map.Low.IterateOverRange(map.High))
            {
                Haxxit.Maps.MapNode node = map.GetNode(p);
                if (!map_squares.ContainsKey(p))
                {
                    IEnumerable<DrawableRectangle> rectangles = MapNodeToRectangle(p);
                    map_squares.Add(p,
                        new Tuple<Maps.MapNode, IEnumerable<DrawableRectangle>>(node, rectangles));
                }
                else
                {
                    Tuple<Maps.MapNode, IEnumerable<DrawableRectangle>> old = map_squares[p];
                    if (!Object.ReferenceEquals(old.Item1, map.GetNode(p)))
                    {
                        IEnumerable<DrawableRectangle> rectangles = MapNodeToRectangle(p);
                        map_squares[p] = new Tuple<Maps.MapNode, IEnumerable<DrawableRectangle>>(map.GetNode(p), rectangles);
                    }
                }
                foreach (DrawableRectangle rectangle in map_squares[p].Item2)
                {
                    rectangle.Update();
                }
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            foreach (Tuple<Maps.MapNode, IEnumerable<DrawableRectangle>> tuple in map_squares.Values)
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
            foreach (Tuple<Haxxit.Maps.Point, string> tuple in head_nodes)
            {
                Rectangle position = tuple.Item1.ToXNARectangle(map_rectangle_size, 6);
                Vector2 text_size = arial_16px_regular.MeasureString(tuple.Item2);
                sprite_batch.DrawString(arial_16px_regular, tuple.Item2,
                    new Vector2(position.X + (position.Width - text_size.X)/2, position.Y + (position.Height - text_size.Y)/2), Color.White);
            }
        }
    }
}
