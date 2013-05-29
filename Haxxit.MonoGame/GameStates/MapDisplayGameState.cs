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
    public class MapDisplayGameState : HaxxitGameState
    {
        const int map_rectangle_size = 28;
        const int map_border_size = 7;

        Texture2D rectangle_texture, background;
        DrawableRectangle background_rectangle;
        Dictionary<Haxxit.Maps.Point, IEnumerable<DrawableRectangle>> map_squares;
        Dictionary<Haxxit.Player, Tuple<Color, Color>> players;
        Dictionary<string, Texture2D> program_textures;
        ContentManager content;
        GraphicsDevice graphics;

        public Haxxit.Maps.Map Map
        {
            get;
            private set;
        }

        public Haxxit.UndoStack UndoStack
        {
            get;
            private set;
        }
        
        public MapDisplayGameState(Haxxit.Maps.Map map) :
            base()
        {
            Map = map;
            UndoStack = new UndoStack(256);
        }

        public override void NewMediator(SimplePubSub.IMediator mediator)
        {
            Map.Mediator = mediator;
            UndoStack.Mediator = mediator;
            foreach (Player player in Map.AllPlayers)
                player.Notifiable = mediator;
        }

        public override void Init()
        {
            players = new Dictionary<Player, Tuple<Color, Color>>();
            int count = 0;
            foreach (Player p in Map.AllPlayers)
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
            map_squares = new Dictionary<Haxxit.Maps.Point, IEnumerable<DrawableRectangle>>();
            program_textures = new Dictionary<string, Texture2D>();
        }

        public override void LoadContent(GraphicsDevice newGraphics, SpriteBatch sprite_batch, ContentManager newContent)
        {
            graphics = newGraphics;
            content = newContent;
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            background = content.Load<Texture2D>("Map-Background-1");
            background_rectangle = new DrawableRectangle(background, new Rectangle(0, 0, 800, 480), Color.White);
            map_squares.Clear();
            foreach (SmartboyDevelopments.Haxxit.Maps.Point p in Map.Low.IterateOverRange(Map.High))
                map_squares[p] = MapNodeToRectangle(p);
        }

        public Rectangle HaxxitPointToXnaRectangle(Haxxit.Maps.Point p)
        {
            return p.ToXNARectangle(map_rectangle_size, map_border_size);
        }

        public Haxxit.Maps.Point XnaVector2ToHaxxitPoint(Vector2 v)
        {
            return v.ToHaxxitPoint(map_rectangle_size, map_border_size);
        }

        public Haxxit.Maps.Point XnaPointToHaxxitPoint(Point p)
        {
            return p.ToHaxxitPoint(map_rectangle_size, map_border_size);
        }

        private void MapChangedListener(string channel, object sender, EventArgs args)
        {
            Haxxit.Maps.MapChangedEventArgs event_args = (Haxxit.Maps.MapChangedEventArgs)args;
            foreach (Haxxit.Maps.Point p in event_args.ChangedNodes)
                map_squares[p] = MapNodeToRectangle(p);
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

            _mediator_manager.Subscribe("haxxit.map.nodes.changed", MapChangedListener);
        }

        private IEnumerable<DrawableRectangle> DrawAvailableNode(Haxxit.Maps.Point p)
        {
            List<DrawableRectangle> rectangles = new List<DrawableRectangle>();
            Rectangle square = p.ToXNARectangle(map_rectangle_size, map_border_size);
            rectangles.Add(new DrawableRectangle(rectangle_texture, square, Color.LightBlue * 0.5f));
            int extra_width = square.Width / 2;
            int extra_height = square.Height / 2;
            int extra_x = square.X + (square.Width - extra_width)/2;
            int extra_y = square.Y + (square.Height - extra_height)/2;
            if (Map.NodeIsType<Haxxit.Maps.SilicoinNode>(p))
                rectangles.Add(new DrawableRectangle(rectangle_texture, new Rectangle(extra_x, extra_y, extra_width, extra_height), Color.Green));
            else if (Map.NodeIsType<Haxxit.Maps.DataNode>(p))
                rectangles.Add(new DrawableRectangle(rectangle_texture, new Rectangle(extra_x, extra_y, extra_width, extra_height), Color.Red));
            return rectangles;
        }

        private IEnumerable<DrawableRectangle> DrawSpawnNode(Haxxit.Maps.Point p)
        {
            List<DrawableRectangle> rectangles = new List<DrawableRectangle>();
            Haxxit.Maps.SpawnNode node = Map.GetNode<Haxxit.Maps.SpawnNode>(p);
            if (node.program != null)
            {
                Color node_color = players[node.Player].Item1;
                rectangles.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, map_border_size), node_color));
                string programTextureName = node.program.TypeName;
                if (!program_textures.ContainsKey(programTextureName))
                    program_textures.Add(programTextureName, content.Load<Texture2D>(programTextureName));
                rectangles.Add(new DrawableRectangle(program_textures[programTextureName], p.ToXNARectangle(map_rectangle_size, map_border_size), Color.White));
            }
            else
            {
                rectangles.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, map_border_size), Color.Purple));
            }
            return rectangles;
        }

        private IEnumerable<DrawableRectangle> DrawProgramNode(Haxxit.Maps.Point p)
        {
            List<DrawableRectangle> rectangles = new List<DrawableRectangle>();
            Haxxit.Maps.ProgramNode program_node = (Haxxit.Maps.ProgramNode)Map.GetNode(p);
            Tuple<Color, Color> player_color;
            if (!players.TryGetValue(program_node.Player, out player_color))
                player_color = new Tuple<Color, Color>(Color.Transparent, Color.Transparent);
            Color node_color = player_color.Item2;
            if (Map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
            {
                node_color = player_color.Item1;
            }
            rectangles.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, map_border_size), node_color));
            if (Map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
            {
                string programTextureName = ((Haxxit.Maps.ProgramHeadNode)program_node).Program.TypeName;
                if (!program_textures.ContainsKey(programTextureName))
                    program_textures.Add(programTextureName, content.Load<Texture2D>(programTextureName));
                rectangles.Add(new DrawableRectangle(program_textures[programTextureName], p.ToXNARectangle(map_rectangle_size, map_border_size), Color.White));
            }
            else if (Map.NodeIsType<Haxxit.Maps.ProgramTailNode>(p))
            {
                // Used for drawing connectors between nodes.
                Haxxit.Maps.ProgramTailNode this_node = Map.GetNode<Haxxit.Maps.ProgramTailNode>(p);
                Haxxit.Maps.Point difference = this_node.Head.coordinate - this_node.coordinate;
                if (!difference.IsDirectional())
                {
                    #if DEBUG
                    throw new Exception();
                    #endif
                }
                else
                {
                    DrawableRectangle connector = new DrawableRectangle(rectangle_texture, p.ToXNARectangle(map_rectangle_size, map_border_size), Color.Gray);
                    Rectangle area = connector.Area;
                    if (difference == new Haxxit.Maps.Point(0, 1)) // Down
                    {
                        area.Y += map_rectangle_size;
                        area.Height /= (map_rectangle_size / map_border_size);
                        area.Width /= 2;
                        area.X += area.Width / 2;
                    }
                    else if (difference == new Haxxit.Maps.Point(0, -1)) // Up
                    {
                        area.Y -= map_border_size;
                        area.Height /= (map_rectangle_size / map_border_size);
                        area.Width /= 2;
                        area.X += area.Width / 2;
                    }
                    else if (difference == new Haxxit.Maps.Point(1, 0)) // Right
                    {
                        area.X += map_rectangle_size;
                        area.Width /= (map_rectangle_size / map_border_size);
                        area.Height /= 2;
                        area.Y += area.Height / 2;
                    }
                    else // Left
                    {
                        area.X -= map_border_size;
                        area.Width /= (map_rectangle_size / map_border_size);
                        area.Height /= 2;
                        area.Y += area.Height / 2;
                    }
                    connector.Area = area;
                    rectangles.Add(connector);
                }
            }
            return rectangles;
        }

        private IEnumerable<DrawableRectangle> MapNodeToRectangle(Haxxit.Maps.Point p)
        {
            List<DrawableRectangle> rectangles = new List<DrawableRectangle>();
            if (Map.NodeIsType<Haxxit.Maps.AvailableNode>(p))
            {
                return DrawAvailableNode(p);
            }
            else if (Map.NodeIsType<Haxxit.Maps.SpawnNode>(p))
            {
                return DrawSpawnNode(p);
            }
            else if (Map.NodeIsType<Haxxit.Maps.ProgramNode>(p))
            {
                return DrawProgramNode(p);
            }
            return rectangles;
        }

        public override void Update()
        {
            // To change a scene, just call one of these:
            // Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.pop", this, new EventArgs());
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            background_rectangle.Draw(sprite_batch);
            foreach (IEnumerable<DrawableRectangle> rectangle_list in map_squares.Values)
                foreach (DrawableRectangle rectangle in rectangle_list)
                    rectangle.Draw(sprite_batch);
        }
    }
}
