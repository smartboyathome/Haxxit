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
    class CreditsGameState : HaxxitGameState
    {
        //title
        String titleString = "Credits";
        SpriteFont titleSpriteFont;
        Vector2 titleStringPos;

        //black rect
        Rectangle displayRect, displayOutterEdgeRect, backgroundRect;
        Texture2D blankTexture, backgroundTexture;

        //credits
        String storyString = "Programmers:\n     Alex Abbott\n     Alex Johnson\n     Brian Hayward\n     Marcel Okello\n\nArt provided by:\n     Maggie Roy";
        Vector2 storyPos;
        //SpriteFont storyStringSpriteFont;
        int nextCharInString = 0;

        //blinking icon effect
        Rectangle blinkingIconAfterStoryCharRect;
        bool trackBlink;
        double blinkingTimer;
        const int MAX_BLINK = 250;
        bool blinkIconIsShown;

        //time values
        double timeElasped = 0;
        bool trackingTime;
        const int MAX_TIME_BEFORE_NEXT_CHAR = 30;

        //back button
        SpriteFont backSpriteFont;
        String backString = "Back";
        Vector2 backStringPos;
        Rectangle backRect;
        Texture2D buttonReleased, buttonPressed;
        bool isBackButtonClicked;
        int xOffset, yOffset;

        int mWindowWidth, mWindowHeight;

        public CreditsGameState() : base()
        {

        }

        public override void Init()
        {
            mWindowWidth = 800;
            mWindowHeight = 480;

            isBackButtonClicked = trackBlink = trackingTime = false;
            blinkIconIsShown = true;

            xOffset = mWindowWidth / 10;
            yOffset = mWindowHeight / 15;

            backgroundRect = new Rectangle(0, 0, mWindowWidth, mWindowHeight);

            backRect = new Rectangle(mWindowWidth - xOffset, mWindowHeight - yOffset, xOffset, yOffset);

            backStringPos.X = backRect.X + ((backRect.Width - backString.Length * 10) / 2f);
            backStringPos.Y = backRect.Y + 3;

            titleStringPos = new Vector2(0, 0);
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            backSpriteFont = content.Load<SpriteFont>("Arial-12px-Regular");
            titleSpriteFont = content.Load<SpriteFont>("Arial-48px-Bold");

            backgroundTexture = content.Load<Texture2D>("Grid2D");

            buttonReleased = content.Load<Texture2D>("blackButtonReleased");
            buttonPressed = content.Load<Texture2D>("blackButtonPressed");

            blankTexture = new Texture2D(graphics, 1, 1);
            blankTexture.SetData(new Color[] { Color.White });

            displayRect = new Rectangle(mWindowWidth / 2 - 105, 150, 210, 235);
            displayOutterEdgeRect = new Rectangle(mWindowWidth / 2 - 115, 140, 230, 255);

            storyPos = new Vector2(mWindowWidth / 2 - 95, 160);

            Vector2 length = backSpriteFont.MeasureString("E");

            blinkingIconAfterStoryCharRect = new Rectangle((int)(storyPos.X), (int)storyPos.Y, (int)length.X, (int)(length.Y - 2));

            length = titleSpriteFont.MeasureString(titleString);
            titleStringPos.X = mWindowWidth / 2 - length.X / 2;
            titleStringPos.Y = mWindowHeight * (1 / 8f);
        }

        public override void SubscribeAll()
        {

        }

        public override void Update()
        {
            MouseState mouse_state = Mouse.GetState(); // Gets the mouse state object
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y); // creates a point for the mouse's position

            if (isBackButtonClicked)
            {
                if (mouse_state.LeftButton == ButtonState.Released)
                {
                    MainMenuGameState new_state = new MainMenuGameState();
                    Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                }
            }

            //Update for Start Button
            if (backRect.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                isBackButtonClicked = true;
            }
            // if hovering over rectangle
            else if (backRect.Contains(mouse_position))
            {
            }
            else // neither clicking nor hovering over rectangle
            {
                isBackButtonClicked = false;
            }

            //check to start new time capture
            if (!trackingTime)
            {
                timeElasped = GlobalAccessors.mGameTime.TotalGameTime.TotalMilliseconds;
                trackingTime = true;
            }

            //position to next char in string array if certain amount of time has expired
            if (GlobalAccessors.mGameTime.TotalGameTime.TotalMilliseconds - timeElasped >= MAX_TIME_BEFORE_NEXT_CHAR)
            {
                if (nextCharInString + 1 <= storyString.Count())
                {
                    nextCharInString++;
                }

                trackingTime = false;
            }

            //track blinking icon
            if (!trackBlink)
            {
                blinkingTimer = GlobalAccessors.mGameTime.TotalGameTime.TotalMilliseconds;
                trackBlink = true;
            }

            //switch whether icon is shown or not based on amount of time that has passed
            if (GlobalAccessors.mGameTime.TotalGameTime.TotalMilliseconds - blinkingTimer >= MAX_BLINK)
            {
                blinkIconIsShown = !blinkIconIsShown;
                trackBlink = false;
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            //Draw background
            sprite_batch.Draw(backgroundTexture, backgroundRect, Color.White);

            //draw title
            sprite_batch.DrawString(titleSpriteFont, titleString, titleStringPos + (Vector2.One * 5), Color.Black); //pop out effect
            sprite_batch.DrawString(titleSpriteFont, titleString, titleStringPos, Color.White);

            //displaying story rectangle and texture
            sprite_batch.Draw(blankTexture, displayOutterEdgeRect, Color.Silver * .75f);
            sprite_batch.Draw(blankTexture, displayRect, Color.Black * .75f);

            //for displaying story one char at a time
            Vector2 length;
            Vector2 charPos;
            charPos.X = (int)storyPos.X;
            charPos.Y = (int)storyPos.Y;

            //displaying the string one char at a time to depict someone is typing story
            for (int i = 0; i < nextCharInString; i++)
            {
                length = backSpriteFont.MeasureString(storyString.ElementAt(i).ToString());

                sprite_batch.DrawString(backSpriteFont, storyString.ElementAt(i).ToString(), charPos + (Vector2.One * 2), Color.Black);
                sprite_batch.DrawString(backSpriteFont, storyString.ElementAt(i).ToString(), charPos, Color.White);

                charPos.X += (int)length.X;

                //go to next line
                if (charPos.X > 560 && storyString.ElementAt(i).ToString() == " ")
                {
                    charPos.X = (int)storyPos.X;
                    charPos.Y += (int)length.Y;
                }
                else if (storyString.ElementAt(i).ToString() == "\n")
                {
                    charPos.X = (int)storyPos.X;
                    charPos.Y += (int)length.Y / 2;
                }
            }

            //blinking icon
            if (blinkIconIsShown)
            {
                sprite_batch.Draw(blankTexture, blinkingIconAfterStoryCharRect, Color.White);
            }

            //update blinking icon
            blinkingIconAfterStoryCharRect.X = (int)charPos.X;
            blinkingIconAfterStoryCharRect.Y = (int)charPos.Y;

            //back button
            if (isBackButtonClicked)
            {
                //draw back button
                sprite_batch.Draw(buttonPressed, backRect, Color.White);
            }
            else
            {
                //draw back button
                sprite_batch.Draw(buttonReleased, backRect, Color.White);
            }
            sprite_batch.DrawString(backSpriteFont, backString, backStringPos + (Vector2.One * 2), Color.Black);
            sprite_batch.DrawString(backSpriteFont, backString, backStringPos, Color.White);

        }
    }
}
