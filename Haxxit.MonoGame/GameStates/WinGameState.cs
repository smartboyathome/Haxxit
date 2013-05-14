using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace SmartboyDevelopments.Haxxit.MonoGame.GameStates
{
    public class WinGameState : HaxxitGameState
    {
        Texture2D rectangle_texture;
        DrawableRectangle overlay;
        SpriteFont win_font;
        SpriteFont sub_font;
        ushort earned_silicoins;
        HaxxitGameState background_state;

        public WinGameState(ushort earned_silicoins, HaxxitGameState background_state) :
            base()
        {
            this.earned_silicoins = earned_silicoins;
            this.background_state = background_state;
        }

        public override void Init()
        {
            
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            overlay = new DrawableRectangle(rectangle_texture, new Rectangle(0, 0, 800, 480), Color.Black * 0.5f);
            win_font = content.Load<SpriteFont>("Arial-36px-Bold");
            sub_font = content.Load<SpriteFont>("Arial-16px-Regular");
            
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
            

        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            //sprite_batch.Draw(rectangle_texture, rectangle, rectangle_color);
            background_state.Draw(sprite_batch);
            overlay.Draw(sprite_batch);
            sprite_batch.DrawString(win_font, "YOU WIN!", new Vector2(125, 125), Color.Orange);
            sprite_batch.DrawString(sub_font, "You earned " + earned_silicoins.ToString() + " silicoins.", new Vector2(125, 175), Color.Orange);
        }
    }
}
