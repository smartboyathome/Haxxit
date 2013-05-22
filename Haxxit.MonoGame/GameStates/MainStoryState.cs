using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SmartboyDevelopments.Haxxit.MonoGame.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    class MainStoryState : HaxxitGameState
    {
        int mWindowWidth, mWindowHeight;
        Player mPlayer1Story;

        //Background art
        Rectangle backgroundRect;
        Texture2D backgroundTexture;

        //black rect
        Rectangle displayRect, displayOutterEdgeRect;
        Texture2D blankTexture;

        //story synopsis
        //String storyString = "The year is 2029.\nA rogue AI, known as the Sentient Autonomous Network Traffic Analyzer, has taken over most of the computers on the internet. Only a small number of computers have not been hacked.\nAs part of the Elite Liberation Force, you are tasked with taking back the net and shutting down S.A.N.T.A.\nHurry, there's no time to lose! . . . . . ";
        String storyString;
        Vector2 storyPos;
        SpriteFont storyStringSpriteFont;
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

        //Continue button
        int xOffset, yOffset;
        Rectangle ContinueButtonRect;
        String ContinueButtonString = "Continue";
        Vector2 ContinueButtonStringPos;
        bool isContinueButtonClicked;
        Texture2D buttonPressed, buttonReleased;

        public MainStoryState()
        {

        }

        public override void Init()
        {
            mWindowWidth = GlobalAccessors.mGame.Window.ClientBounds.Width;
            mWindowHeight = GlobalAccessors.mGame.Window.ClientBounds.Height;

            mPlayer1Story = GlobalAccessors.mPlayer1;

            xOffset = mWindowWidth / 10;
            yOffset = mWindowHeight / 15;
     
            backgroundRect = new Rectangle(0, 0, mWindowWidth, mWindowHeight);
            ContinueButtonRect = new Rectangle(mWindowWidth - xOffset, mWindowHeight - yOffset, xOffset, yOffset);

            trackingTime = trackBlink = isContinueButtonClicked = false;
            blinkIconIsShown = true;
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            Init();

            //STORY STRINGS----------------------------------------------------------
            //TODO turn these into a resource file for easier editing
            if (mPlayer1Story.CurrentNode == "Node1")
            {
                storyString = "Excellent work Cadet.  Your next mission will be to retrieve a piece of critical data for us from the node you just gained access to.  Our snooper programs are reporting that this node has two lines of defenses, the first are powerful stationary defense programs, and second is intentional memory corruption that has isolated the information we need from the rest of the node.  You will need to procure new programs from the black market store node you also gained access to.  You will need a MemMan program to repair the corruption to gain access to the data.";
            }
            else if (mPlayer1Story.CurrentNode == "Node2")
            {
                storyString = "This is it; the data you acquired for us has given us our chance.  We have found a weakness in S.A.N.T.A’s AI, if we can take the node that the data server is connected to handles most of S.A.N.T.A’s higher functions.  If we can take this node we will be able to end this war.  Purchase the heaviest hitting programs you can, this node is defending by multiple heavy duty defense programs; you’re going to have to wipe them all out to take the node.  Happy Hunting.";
            }
            else
            {
                storyString = "Welcome to ELF, Cadet.  Your first task will be to break into a low security server node as a staging point to infiltrate S.A.N.T.A’s network.  You have been outfit with a simple attack program to deal with this node’s defenses.  Remember in this struggle to take back the internet some sacrifices will be required.  Good Luck.";
            }
            //------------------------------------------------------------------------

            backgroundTexture = content.Load<Texture2D>("OverworldBackground");

            storyStringSpriteFont = content.Load<SpriteFont>("Arial-12px-Regular");

            blankTexture = new Texture2D(graphics, 1, 1);
            blankTexture.SetData(new Color[] { Color.White });

            buttonPressed = content.Load<Texture2D>("blackButtonPressed");
            buttonReleased = content.Load<Texture2D>("blackButtonReleased");

            storyPos = new Vector2(mWindowWidth / 2 - 200, 100);

            displayRect = new Rectangle(mWindowWidth / 2 - 210, 90, 420, 235);
            displayOutterEdgeRect = new Rectangle(mWindowWidth / 2 - 220, 80, 440, 255);

            Vector2 length = storyStringSpriteFont.MeasureString("E");

            blinkingIconAfterStoryCharRect = new Rectangle((int)(storyPos.X), (int)storyPos.Y, (int) length.X, (int)(length.Y - 2));

            length = storyStringSpriteFont.MeasureString(ContinueButtonString);
            ContinueButtonStringPos.X = ContinueButtonRect.X + ((ContinueButtonRect.Width - length.X) /  2);
            ContinueButtonStringPos.Y = ContinueButtonRect.Y + ((ContinueButtonRect.Height - length.Y) / 2);
        }

        public override void SubscribeAll()
        {

        }

        public override void Update()
        {
            MouseState mouse_state = Mouse.GetState(); // Gets the mouse state object
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y); // creates a point for the mouse's position

            if (isContinueButtonClicked)
            {
                if (mouse_state.LeftButton == ButtonState.Released)
                {
                    ServerOverworldState new_state = new ServerOverworldState();
                    Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                }
            }

            //Update for Continue Button
            if (ContinueButtonRect.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                isContinueButtonClicked = true;
            }
            // if hovering over rectangle
            else if (ContinueButtonRect.Contains(mouse_position))
            {
            }
            else // neither clicking nor hovering over rectangle
            {
                isContinueButtonClicked = false;
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
            //draw background art
            sprite_batch.Draw(backgroundTexture, backgroundRect, Color.White);

            //displaying story rectangle and texture
            sprite_batch.Draw(blankTexture, displayOutterEdgeRect, Color.Silver * .75f);
            sprite_batch.Draw(blankTexture, displayRect, Color.Black * .75f);

            //for displaying story one char at a time
            Vector2 length;
            Vector2 charPos;
            charPos.X = (int) storyPos.X;
            charPos.Y = (int) storyPos.Y;

            //displaying the string one char at a time to depict someone is typing story
            for (int i = 0; i < nextCharInString; i++)
            {
                length = storyStringSpriteFont.MeasureString(storyString.ElementAt(i).ToString());

                sprite_batch.DrawString(storyStringSpriteFont, storyString.ElementAt(i).ToString(), charPos + (Vector2.One * 2), Color.Black);
                sprite_batch.DrawString(storyStringSpriteFont, storyString.ElementAt(i).ToString(), charPos , Color.White);

                charPos.X += (int) length.X;

                //go to next line
                if (charPos.X > 560 && storyString.ElementAt(i).ToString() == " " || storyString.ElementAt(i).ToString() == "\n")
                {
                    charPos.X = (int) storyPos.X;
                    charPos.Y += (int) length.Y;
                }
            }

            //blinking icon
            if (blinkIconIsShown)
            {
                sprite_batch.Draw(blankTexture, blinkingIconAfterStoryCharRect, Color.White);
            }

            //update blinking icon
            blinkingIconAfterStoryCharRect.X = (int) charPos.X;
            blinkingIconAfterStoryCharRect.Y = (int) charPos.Y;

            //continue button
            if (isContinueButtonClicked)
            {
                sprite_batch.Draw(buttonPressed, ContinueButtonRect, Color.White);
            }
            else
            {
                sprite_batch.Draw(buttonReleased, ContinueButtonRect, Color.White);
            }
            sprite_batch.DrawString(storyStringSpriteFont, ContinueButtonString, ContinueButtonStringPos + (Vector2.One * 2), Color.Black);
            sprite_batch.DrawString(storyStringSpriteFont, ContinueButtonString, ContinueButtonStringPos, Color.White);

        }
    }
}
