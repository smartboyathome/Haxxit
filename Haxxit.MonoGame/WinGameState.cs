using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    public class WinGameState : HaxxitGameState
    {
        Texture2D rectangle_texture;
        Color rectangle_color;
        Rectangle rectangle;
        SpriteFont win_font;
        SpriteFont sub_font;
        ushort earned_silicoins;

        public WinGameState(ushort earned_silicoins) :
            base()
        {
            this.earned_silicoins = earned_silicoins;
        }

        public override void Init()
        {
            
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            rectangle_color = Color.Red;
            rectangle = new Rectangle(50, 50, 50, 50);
            win_font = content.Load<SpriteFont>("WinText");
            sub_font = content.Load<SpriteFont>("Arial");
            
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
            /*MouseState mouse_state = Mouse.GetState(); // Gets the mouse state object
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y); // creates a point for the mouse's position
            // if clicking within rectangle
            if (rectangle.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                rectangle_color = Color.Purple;
            }
            // if hovering over rectangle
            else if (rectangle.Contains(mouse_position))
            {
                rectangle_color = Color.Yellow;
            }
            else // neither clicking nor hovering over rectangle
            {
                rectangle_color = Color.Red;
            }*/
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            //sprite_batch.Draw(rectangle_texture, rectangle, rectangle_color);
            sprite_batch.DrawString(win_font, "YOU WIN!", new Vector2(125, 125), Color.Orange);
            sprite_batch.DrawString(sub_font, "You earned " + earned_silicoins.ToString() + " silicoins.", new Vector2(125, 175), Color.Orange);
        }
    }
}
