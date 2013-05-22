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
    public class TempDialogGameState : HaxxitGameState
    {
        Texture2D rectangle_texture;
        SpriteFont arial_36px_bold;
        HaxxitGameState background_state;
        string dialog_text;
        int dialog_delay, start_time;
        DrawableRectangle dialog_background;
        Point text_position;

        public TempDialogGameState(HaxxitGameState background_state, string dialog_text, int dialog_delay)
        {
            this.dialog_text = dialog_text;
            this.dialog_delay = dialog_delay;
            this.background_state = background_state;
        }

        public override void Init()
        {
            start_time = System.Environment.TickCount;
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            arial_36px_bold = content.Load<SpriteFont>("Arial-36px-Bold");
            Vector2 text_size = arial_36px_bold.MeasureString(dialog_text);
            text_position = new Point((int)Math.Floor((800 - text_size.X)/2), (int)Math.Floor((480 - text_size.Y)/2));
            dialog_background = new DrawableRectangle(rectangle_texture,
                new Rectangle(text_position.X - 5, text_position.Y - 5, (int)Math.Floor(text_size.X + 10), (int)Math.Floor(text_size.Y + 10)),
                Color.Green);
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

            // Fixed, you can just check it on each update to see if it worked. --Alex
            if (System.Environment.TickCount >= start_time + dialog_delay)
            {
                Mediator.Notify("haxxit.engine.state.pop", this, new EventArgs());
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            //sprite_batch.Draw(rectangle_texture, rectangle, rectangle_color);
            background_state.Draw(sprite_batch);
            dialog_background.Draw(sprite_batch);
            sprite_batch.DrawString(arial_36px_bold, dialog_text, new Vector2(text_position.X, text_position.Y), Color.White);
        }
    }
}
