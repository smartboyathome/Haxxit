using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    public class DefaultGameState : HaxxitGameState
    {
        Texture2D test_texture;
        SpriteFont test_font;

        public DefaultGameState() :
            base()
        {

        }

        public override void Init()
        {
            
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            test_texture = new Texture2D(graphics, 1, 1);
            test_texture.SetData(new Color[] { Color.White });
            test_font = content.Load<SpriteFont>("Arial");
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

        public override void Draw(GraphicsDevice graphics, SpriteBatch sprite_batch)
        {
            sprite_batch.Begin();
            sprite_batch.Draw(test_texture, new Rectangle(10, 10, 10, 10), Color.Red);
            sprite_batch.DrawString(test_font, "TEST", new Vector2(25, 25), Color.Orange);
            sprite_batch.End();
        }
    }
}
