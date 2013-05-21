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
    class IntroductionState : HaxxitGameState
    {
        //game dimensions
        int mWindowWidth;
        int mWindowHeight;

        //background
        Rectangle backgroundRect;
        Texture2D blankTexture;

        //Title
        String titleStringLine1 = "Smartboy";
        String titleStringLine2 = "Developments";
        Vector2 titleStringLine1Pos, titleStringLine2Pos;
        SpriteFont titleStringSpriteFont;

        //Logo Picture
        Rectangle logoPictureRect;
        Texture2D logoPictureTexture;

        //int for chaning alpha value
        float alphaValue = 0.0f;

        //time values
        double timeElasped;
        bool trackingTime;
        const int MAX_TIME_BEFORE_CHANGE = 1;
        const int STAY_ON_SCREEN = 1250;

        //for timed evenets
        bool fadeIn;
        bool startFadeIn;

        bool userWantsToSkip;

        public IntroductionState()
        {

        }

        public override void Init()
        {
            mWindowWidth = GlobalAccessors.mGame.Window.ClientBounds.Width;
            mWindowHeight = GlobalAccessors.mGame.Window.ClientBounds.Height;

            backgroundRect = new Rectangle(0, 0, mWindowWidth, mWindowHeight);

            timeElasped = 0;
            trackingTime = startFadeIn = userWantsToSkip = false;
            fadeIn = true;
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            Init();

            graphics.Clear(Color.Black);

            titleStringSpriteFont = content.Load<SpriteFont>("Arial-48px-Bold");
            blankTexture = new Texture2D(graphics, 1, 1);
            blankTexture.SetData(new Color[] { Color.White });

            //puzzle piece image from:
            //http://office.microsoft.com/en-us/images/results.aspx?qu=PNG&ex=1#ai:MC900439604|
            logoPictureTexture = content.Load<Texture2D>("PuzzlePiece");

            Vector2 length = titleStringSpriteFont.MeasureString(titleStringLine1);

            titleStringLine1Pos.X = mWindowWidth / 2 - length.X / 2;
            titleStringLine1Pos.Y = mWindowHeight / 6;

            length = titleStringSpriteFont.MeasureString(titleStringLine2);

            titleStringLine2Pos.X = mWindowWidth / 2 - length.X / 2;
            titleStringLine2Pos.Y = titleStringLine1Pos.Y + length.Y;

            logoPictureRect = new Rectangle(mWindowWidth / 2 - 125, (int) (titleStringLine2Pos.Y + length.Y) + 10, 250, 250);
        }

        public override void SubscribeAll()
        {

        }

        public override void Update()
        {
            MouseState mouse_state = Mouse.GetState(); // Gets the mouse state object
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y); // creates a point for the mouse's position

            if (userWantsToSkip)
            {
                if (mouse_state.LeftButton == ButtonState.Released)
                {
                    MainMenuGameState new_state = new MainMenuGameState();
                    Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                }
            }

            //User clicks anywhere within window
            if (backgroundRect.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                userWantsToSkip = true;
            }

            //check to start new time capture
            if (!trackingTime)
            {
                timeElasped = GlobalAccessors.mGameTime.TotalGameTime.TotalMilliseconds;
                trackingTime = true;
            }

            //check to start logo animation
            if (GlobalAccessors.mGameTime.TotalGameTime.TotalMilliseconds - timeElasped >= STAY_ON_SCREEN)
            {
                startFadeIn = true;
                trackingTime = false;
            }

            //check to fade in, keep on screen or fade out
            if (startFadeIn)
            {
                if (fadeIn && alphaValue >= 1f)
                {
                    if (GlobalAccessors.mGameTime.TotalGameTime.TotalMilliseconds - timeElasped >= STAY_ON_SCREEN)
                    {
                        fadeIn = false;
                    }
                }
                else if (fadeIn)
                {
                    if (GlobalAccessors.mGameTime.TotalGameTime.TotalMilliseconds - timeElasped >= MAX_TIME_BEFORE_CHANGE)
                    {
                        alphaValue += .008f;
                        trackingTime = false;
                    }
                }
                else if (!fadeIn && alphaValue >= 0)
                {
                    if (GlobalAccessors.mGameTime.TotalGameTime.TotalMilliseconds - timeElasped >= MAX_TIME_BEFORE_CHANGE)
                    {
                        alphaValue -= .008f;
                        trackingTime = false;
                    }
                }
                else
                {
                    MainMenuGameState new_state = new MainMenuGameState();
                    Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                }
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            //black background, stays at alpha = 1
            sprite_batch.Draw(blankTexture, backgroundRect, Color.Black);

            //white background, fades in and then out
            sprite_batch.Draw(blankTexture, backgroundRect, Color.White * alphaValue);

            //title line 1, fades in and then out
            sprite_batch.DrawString(titleStringSpriteFont, titleStringLine1, titleStringLine1Pos, Color.Black * alphaValue);

            //title line 2, fades in and then out
            sprite_batch.DrawString(titleStringSpriteFont, titleStringLine2, titleStringLine2Pos, Color.Black * alphaValue);

            //logo, fades in and then out
            sprite_batch.Draw(logoPictureTexture, logoPictureRect, Color.White * (alphaValue - .05f));
        }
    }
}
