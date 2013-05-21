using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.Programs;
using SmartboyDevelopments.SimplePubSub;
using SmartboyDevelopments.Haxxit.MonoGame.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.GameStates
{
    class ShopStateDialogBox : HaxxitGameState
    {
        //background state, should be shop state
        HaxxitGameState backgroundState;

        int mWindowWidth, mWindowHeight, xOffset, yOffset;

        //button texture
        Texture2D buttonReleased, buttonPressed;

        //blank rect texture
        Texture2D blankRectTexture;

        //dialog message
        Rectangle mDialogMessageRect;
        String dialogMessageString;
        Vector2 dialogMessagePos;

        //okay button on dialog screen
        Rectangle mOkayButtonRect;
        String mOkayButtonString = "OK";
        Vector2 mOkayButtonStringPos;
        bool isOkayButtonClicked;

        //
        SpriteFont PlayerUISpriteFont;

        public ShopStateDialogBox(HaxxitGameState backgroundUnselectable, String message = "")
        {
            backgroundState = backgroundUnselectable;
            dialogMessageString = message;
        }

        public override void Init()
        {
            mWindowHeight = GlobalAccessors.mGame.Window.ClientBounds.Height;
            mWindowWidth = GlobalAccessors.mGame.Window.ClientBounds.Width;

            xOffset = mWindowWidth / 10;
            yOffset = mWindowHeight / 15;

            isOkayButtonClicked = false;

            mDialogMessageRect = new Rectangle(mWindowWidth / 4, mWindowHeight / 4, mWindowWidth / 2, mWindowHeight / 2);
            mOkayButtonRect = new Rectangle(mDialogMessageRect.X + mDialogMessageRect.Width / 2 - xOffset / 2, mDialogMessageRect.Y + mDialogMessageRect.Height - mDialogMessageRect.Height / 4, xOffset, yOffset);
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            Init();

            PlayerUISpriteFont = content.Load<SpriteFont>("Arial-12px-Regular");

            buttonPressed = content.Load<Texture2D>("blackButtonPressed");
            buttonReleased = content.Load<Texture2D>("blackButtonReleased");

            blankRectTexture = new Texture2D(graphics, 1, 1);
            blankRectTexture.SetData(new Color[] { Color.White });

            Vector2 length = PlayerUISpriteFont.MeasureString(mOkayButtonString);
            mOkayButtonStringPos.X = mOkayButtonRect.X + ((mOkayButtonRect.Width - length.X) / 2);
            mOkayButtonStringPos.Y = mOkayButtonRect.Y + ((mOkayButtonRect.Height - length.Y) / 2);

            length = PlayerUISpriteFont.MeasureString(dialogMessageString);
            dialogMessagePos.X = mDialogMessageRect.X + ((mDialogMessageRect.Width - length.X) / 2);
            dialogMessagePos.Y = mDialogMessageRect.Y + ((mDialogMessageRect.Height - length.Y) / 2);
        }

        public override void SubscribeAll()
        {

        }

        public override void Update()
        {
            MouseState mouse_state = Mouse.GetState(); // Gets the mouse state object
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y); // creates a point for the mouse's position

            if (isOkayButtonClicked && mouse_state.LeftButton == ButtonState.Released)
            {
                Mediator.Notify("haxxit.engine.state.pop", this, new EventArgs());
            }

            //Update for Okay Button
            if (mOkayButtonRect.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                isOkayButtonClicked = true;
            }
            // if hovering over rectangle
            else if (mOkayButtonRect.Contains(mouse_position))
            {
            }
            else // neither clicking nor hovering over rectangle
            {
                isOkayButtonClicked = false;
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            backgroundState.Draw(sprite_batch);

            sprite_batch.Draw(blankRectTexture, mDialogMessageRect, Color.Silver);
            sprite_batch.DrawString(PlayerUISpriteFont, dialogMessageString, dialogMessagePos, Color.Black);

            if (isOkayButtonClicked)
            {
                sprite_batch.Draw(buttonPressed, mOkayButtonRect, Color.White);
            }
            else
            {
                sprite_batch.Draw(buttonReleased, mOkayButtonRect, Color.White);
            }
            sprite_batch.DrawString(PlayerUISpriteFont, mOkayButtonString, mOkayButtonStringPos, Color.White);
        }
    }
}
