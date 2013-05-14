using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    class OverWorldState : HaxxitGameState
    {
        Texture2D rectTexture;
        Rectangle visitShopRect;
        SpriteFont visitShopSpriteFont;
        Color rectColor;
        Vector2 stringPos;

        public OverWorldState()
            : base()
        {

        }

        public override void Init()
        {

        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            visitShopSpriteFont = content.Load<SpriteFont>("Arial");

            rectTexture = new Texture2D(graphics, 1, 1);
            rectTexture.SetData(new Color[] { Color.White });

            stringPos = new Vector2(0, 0);
            rectColor = Color.Green;

            visitShopRect = new Rectangle(50, 50, 100, 50);
        }

        public override void SubscribeAll()
        {

        }

        public override void Update()
        {
            MouseState mouse_state = Mouse.GetState(); // Gets the mouse state object
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y); // creates a point for the mouse's position

            // if clicking within rectangle
            if (visitShopRect.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                rectColor = Color.Purple;

                //WinGameState new_state = new WinGameState(map.EarnedSilicoins);
                //Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
            }
            // if hovering over rectangle
            else if (visitShopRect.Contains(mouse_position))
            {
                rectColor = Color.Orange;
            }
            else // neither clicking nor hovering over rectangle
            {
                rectColor = Color.Green;
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {

            stringPos = Vector2.Zero;

            stringPos.X = visitShopRect.X + 5;
            stringPos.Y = visitShopRect.Y + 5;

            sprite_batch.Draw(rectTexture, visitShopRect, rectColor);
            sprite_batch.DrawString(visitShopSpriteFont, "Visit Shop", stringPos, Color.White);
        }

        public virtual void NewMediator(IMediator mediator)
        {

        }
    }
}
