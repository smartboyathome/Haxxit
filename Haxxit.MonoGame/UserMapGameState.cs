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
        Texture2D rectangle_texture;
        List<DrawableRectangle> map_squares;
        List<Tuple<Haxxit.Maps.Point, string>> head_nodes;
        SpriteFont arial_16px_regular;
        Haxxit.Maps.Map map;
        Dictionary<Haxxit.Player, Color> players;

        public UserMapGameState(Haxxit.Maps.Map map) :
            base()
        {
            this.map = map;
        }

        public override void Init()
        {
            players = new Dictionary<Player, Color>();
            Random rand = new Random();
            foreach (Player p in map.AllPlayers)
            {
                players.Add(p, new Color(rand.Next(256), rand.Next(256), rand.Next(256)));
            }
            map_squares = new List<DrawableRectangle>();
            head_nodes = new List<Tuple<Maps.Point, string>>();
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

        public override void Update()
        {
            // To change a scene, just call one of these:
            // Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.pop", this, new EventArgs());

            map_squares.Clear();
            head_nodes.Clear();
            foreach (SmartboyDevelopments.Haxxit.Maps.Point p in map.Low.IterateOverRange(map.High))
            {
                if (map.NodeIsType<Haxxit.Maps.UnavailableNode>(p))
                    continue;
                else if (map.NodeIsType<Haxxit.Maps.AvailableNode>(p))
                {
                    Rectangle square = p.ToXNARectangle(24, 6);
                    map_squares.Add(new DrawableRectangle(rectangle_texture, square, Color.Blue));
                    if (map.NodeIsType<Haxxit.Maps.SilicoinNode>(p))
                        map_squares.Add(new DrawableRectangle(rectangle_texture, new Rectangle(square.X + 10, square.Y + 10, 4, 4), Color.Green));
                    else if (map.NodeIsType<Haxxit.Maps.DataNode>(p))
                        map_squares.Add(new DrawableRectangle(rectangle_texture, new Rectangle(square.X + 10, square.Y + 10, 4, 4), Color.Red));
                }
                else if (map.NodeIsType<Haxxit.Maps.SpawnNode>(p))
                {
                    map_squares.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(24, 6), Color.Purple, 2, new Color(255, 255, 255, 127)));
                }
                else if (map.NodeIsType<Haxxit.Maps.ProgramNode>(p))
                {
                    Haxxit.Maps.ProgramNode program_node = (Haxxit.Maps.ProgramNode)map.GetNode(p);
                    Color player_color;
                    if (!players.TryGetValue(program_node.Player, out player_color))
                        player_color = Color.Transparent;
                    map_squares.Add(new DrawableRectangle(rectangle_texture, p.ToXNARectangle(24, 6), player_color));
                    if(map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
                    {
                        Haxxit.Maps.ProgramHeadNode head_node = (Haxxit.Maps.ProgramHeadNode)map.GetNode(p);
                        string Name = head_node.Program.TypeName[0].ToString();
                        head_nodes.Add(new Tuple<Haxxit.Maps.Point, string>(p, Name));
                    }
                }
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            foreach (DrawableRectangle rectangle in map_squares)
            {
                rectangle.Draw(sprite_batch);
            }
            foreach (Tuple<Haxxit.Maps.Point, string> tuple in head_nodes)
            {
                Rectangle position = tuple.Item1.ToXNARectangle(24, 6);
                Vector2 text_size = arial_16px_regular.MeasureString(tuple.Item2);
                sprite_batch.DrawString(arial_16px_regular, tuple.Item2,
                    new Vector2(position.X + (position.Width - text_size.X)/2, position.Y + (position.Height - text_size.Y)/2), Color.White);
            }
        }
    }
}
