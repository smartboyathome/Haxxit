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

        Texture2D rectangle_texture, background, rounded_rect_back,
            rounded_rect_border, rounded_rect_full, spawn_overlay;
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
            rounded_rect_back = content.Load<Texture2D>("Map-Square-Background");
            rounded_rect_border = content.Load<Texture2D>("Map-Square-Border");
            rounded_rect_full = content.Load<Texture2D>("Map-Square-Full");
            spawn_overlay = content.Load<Texture2D>("Map-Square-Spawn");
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
            rectangles.Add(new DrawableRectangle(rounded_rect_back, square, new Color(52, 96, 141) * 0.5f));
            rectangles.Add(new DrawableRectangle(rounded_rect_border, square, new Color(24, 202, 230)));
            return rectangles;
        }

        private IEnumerable<DrawableRectangle> DrawSilicoinNode(Haxxit.Maps.Point p)
        {
            List<DrawableRectangle> rectangles = new List<DrawableRectangle>();
            Rectangle square = p.ToXNARectangle(map_rectangle_size, map_border_size);
            Rectangle extra = square.DeepCopy().ScaleBy(0.5).CenterAlignOn(square);
            rectangles.Add(new DrawableRectangle(rectangle_texture, extra, Color.Green));
            return rectangles;
        }

        private IEnumerable<DrawableRectangle> DrawDataNode(Haxxit.Maps.Point p)
        {
            List<DrawableRectangle> rectangles = new List<DrawableRectangle>();
            Rectangle square = p.ToXNARectangle(map_rectangle_size, map_border_size);
            Rectangle extra = square.DeepCopy().ScaleBy(0.5).CenterAlignOn(square);
            rectangles.Add(new DrawableRectangle(rectangle_texture, extra, Color.Red));
            return rectangles;
        }

        private IEnumerable<DrawableRectangle> DrawSpawnNode(Haxxit.Maps.Point p, Haxxit.Player player, Haxxit.Programs.ProgramFactory program=null)
        {
            List<DrawableRectangle> rectangles = new List<DrawableRectangle>();
            if (program != null)
            {
                rectangles.AddRange(DrawProgramHeadNode(p, player, program.NewInstance()));
            }
            else
            {
                rectangles.AddRange(DrawAvailableNode(p));
                rectangles.Add(new DrawableRectangle(spawn_overlay, p.ToXNARectangle(map_rectangle_size, map_border_size), Color.Purple));
            }
            return rectangles;
        }

        private IEnumerable<DrawableRectangle> DrawProgramTailNode(Haxxit.Maps.Point p, Haxxit.Maps.Point connector_direction,
            Haxxit.Player player, Haxxit.Programs.Program program)
        {
            List<DrawableRectangle> rectangles = new List<DrawableRectangle>();
            Tuple<Color, Color> player_color;
            if (!players.TryGetValue(player, out player_color))
                player_color = new Tuple<Color, Color>(Color.Transparent, Color.Transparent);
            Color node_color = player_color.Item2;
            rectangles.Add(new DrawableRectangle(rounded_rect_full, p.ToXNARectangle(map_rectangle_size, map_border_size), node_color));
            // Used for drawing connectors between nodes.
            if (!connector_direction.IsDirectional())
            {
                #if DEBUG
                throw new Exception();
                #else
                return rectangles;
                #endif
            }
            DrawableRectangle connector = new DrawableRectangle(rectangle_texture,
                p.ToXNARectangle(map_rectangle_size, map_border_size), node_color);
            Rectangle area = connector.Area;
            if (connector_direction == new Haxxit.Maps.Point(0, 1)) // Down
            {
                area.Y += map_rectangle_size;
                area.Height /= (map_rectangle_size / map_border_size);
                area.Width /= 2;
                area.X += area.Width / 2;
            }
            else if (connector_direction == new Haxxit.Maps.Point(0, -1)) // Up
            {
                area.Y -= map_border_size;
                area.Height /= (map_rectangle_size / map_border_size);
                area.Width /= 2;
                area.X += area.Width / 2;
            }
            else if (connector_direction == new Haxxit.Maps.Point(1, 0)) // Right
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
            return rectangles;
        }

        private IEnumerable<DrawableRectangle> DrawProgramHeadNode(Haxxit.Maps.Point p, Haxxit.Player player, Haxxit.Programs.Program program)
        {
            List<DrawableRectangle> rectangles = new List<DrawableRectangle>();
            Tuple<Color, Color> player_color;
            if (!players.TryGetValue(player, out player_color))
                player_color = new Tuple<Color, Color>(Color.Transparent, Color.Transparent);
            Color node_color = player_color.Item1;
            Rectangle rectangle = p.ToXNARectangle(map_rectangle_size, map_border_size);
            rectangles.Add(new DrawableRectangle(rounded_rect_back, rectangle, node_color));
            rectangles.Add(new DrawableRectangle(rounded_rect_border, rectangle, player_color.Item2));
            string programTextureName = program.TypeName;
            if (!program_textures.ContainsKey(programTextureName))
                program_textures.Add(programTextureName, content.Load<Texture2D>(programTextureName));
            rectangles.Add(new DrawableRectangle(program_textures[programTextureName], p.ToXNARectangle(map_rectangle_size, map_border_size), Color.White));
            return rectangles;
        }

        private IEnumerable<DrawableRectangle> MapNodeToRectangle(Haxxit.Maps.Point p)
        {
            List<DrawableRectangle> rectangles = new List<DrawableRectangle>();
            if (Map.NodeIsType<Haxxit.Maps.AvailableNode>(p))
            {
                rectangles.AddRange(DrawAvailableNode(p));
                if (Map.NodeIsType<Haxxit.Maps.SilicoinNode>(p))
                    rectangles.AddRange(DrawSilicoinNode(p));
                else if (Map.NodeIsType<Haxxit.Maps.DataNode>(p))
                    rectangles.AddRange(DrawDataNode(p));
            }
            else if (Map.NodeIsType<Haxxit.Maps.SpawnNode>(p))
            {
                Haxxit.Maps.SpawnNode node = Map.GetNode<Haxxit.Maps.SpawnNode>(p);
                rectangles.AddRange(DrawSpawnNode(p, node.Player, node.program));
            }
            else if (Map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
            {
                Haxxit.Maps.ProgramHeadNode node = Map.GetNode<Haxxit.Maps.ProgramHeadNode>(p);
                rectangles.AddRange(DrawProgramHeadNode(p, node.Player, node.Program));
            }
            else if (Map.NodeIsType<Haxxit.Maps.ProgramTailNode>(p))
            {
                Haxxit.Maps.ProgramTailNode node = Map.GetNode<Haxxit.Maps.ProgramTailNode>(p);
                rectangles.AddRange(DrawProgramTailNode(p, node.Head.coordinate - node.coordinate, node.Player, node.Program));
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
