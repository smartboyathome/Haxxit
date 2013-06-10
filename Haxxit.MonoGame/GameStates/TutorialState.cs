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

    public class TutorialState : HaxxitGameState
    {
        int mWindowWidth, mWindowHeight;
        Texture2D rectangle_texture;
        DrawableRectangle overlay;
        SpriteFont win_font;
        SpriteFont sub_font;
        SpriteFont ArialFontSize12;
        MapDisplayGameState display_map_state;
        Player mPlayer1Tutorial;
        int departureTime;

        //black rect
        Rectangle displayRect, displayOutterEdgeRect;
        Texture2D blankTexture;
        Rectangle tutorialRect1, tutorialEdgeRect1, tutorialRect2, tutorialEdgeRect2,
            tutorialRect3, tutorialEdgeRect3, tutorialRect4, tutorialEdgeRect4, tutorialRect5, tutorialEdgeRect5;

        //story synopsis
        String storyString;
        Vector2 storyPos;
        SpriteFont storyStringSpriteFont;

        //Continue button
        int xOffset, yOffset;
        Rectangle ContinueButtonRect;
        String ContinueButtonString = "Continue";
        Vector2 ContinueButtonStringPos;
        bool isContinueButtonClicked;
        Texture2D buttonPressed, buttonReleased;

        public TutorialState(MapDisplayGameState display_map_state) :
            base()
        {
            this.display_map_state = display_map_state;
        }

        public override void Init()
        {
            display_map_state.Update();
            departureTime = System.Environment.TickCount + 3000;
            mWindowWidth = GlobalAccessors.mGame.Window.ClientBounds.Width;
            mWindowHeight = GlobalAccessors.mGame.Window.ClientBounds.Height;

            mPlayer1Tutorial = GlobalAccessors.mPlayer1;

            xOffset = mWindowWidth / 10;
            yOffset = mWindowHeight / 15;

            ContinueButtonRect = new Rectangle(mWindowWidth - xOffset, mWindowHeight - yOffset, xOffset, yOffset);
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            ArialFontSize12 = content.Load<SpriteFont>("Arial-12px-Regular");
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            overlay = new DrawableRectangle(rectangle_texture, new Rectangle(0, 0, 800, 480), Color.Black * 0.5f);
            win_font = content.Load<SpriteFont>("Arial-36px-Bold");
            sub_font = content.Load<SpriteFont>("Arial-16px-Regular");

            //-------------------------------------------------------------------------

            storyStringSpriteFont = content.Load<SpriteFont>("Arial-12px-Regular");

            blankTexture = new Texture2D(graphics, 1, 1);
            blankTexture.SetData(new Color[] { Color.White });

            buttonPressed = content.Load<Texture2D>("blackButtonPressed");
            buttonReleased = content.Load<Texture2D>("blackButtonReleased");

            storyPos = new Vector2(mWindowWidth / 2 - 200, 100);

            
            displayRect = new Rectangle(mWindowWidth / 2 - 210, 90, 420, 235);
            displayOutterEdgeRect = new Rectangle(mWindowWidth / 2 - 220, 80, 440, 255);

            tutorialRect1 = new Rectangle(mWindowWidth - 700, mWindowHeight - 170, 170, 110);
            tutorialEdgeRect1 = new Rectangle(mWindowWidth - 710, mWindowHeight -180, 190, 130);

            tutorialRect2 = new Rectangle(mWindowWidth - 450, mWindowHeight / 6, 170, 110);
            tutorialEdgeRect2 = new Rectangle(mWindowWidth - 460, mWindowHeight / 6 - 10, 190, 130);

            tutorialRect3 = new Rectangle(mWindowWidth - 450, mWindowHeight / 2 + 20, 320, 110);
            tutorialEdgeRect3 = new Rectangle(mWindowWidth - 460, mWindowHeight / 2 + 10, 340, 130);

            Vector2 length = storyStringSpriteFont.MeasureString("E");

            length = storyStringSpriteFont.MeasureString(ContinueButtonString);
            ContinueButtonStringPos.X = ContinueButtonRect.X + ((ContinueButtonRect.Width - length.X) / 2);
            ContinueButtonStringPos.Y = ContinueButtonRect.Y + ((ContinueButtonRect.Height - length.Y) / 2);

            //-------------------------------------------------------------------------

            if (mPlayer1Tutorial.SpawnTutorial == true)
            {
                storyString = "Spawn Tutorial";
            }
            else if (mPlayer1Tutorial.CurrentNode == "Node1")
            {
                storyString = "Level 1 Tutorial";
            }
            else if (mPlayer1Tutorial.CurrentNode == "Node2")
            {
                storyString = "Level 2 Tutorial";
            }
            //storyString = "This is only a placeholder.";

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
            MouseState mouse_state = Mouse.GetState(); // Gets the mouse state object
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y); // creates a point for the mouse's position

            if (isContinueButtonClicked)
            {
                if (mouse_state.LeftButton == ButtonState.Released)
                {
                    if (mPlayer1Tutorial.SpawnTutorial == true)
                    {
                        mPlayer1Tutorial.SpawnTutorial = false;
                        TutorialMapSpawnGameState new_state = new TutorialMapSpawnGameState(display_map_state);
                        _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                    }
                    else if (mPlayer1Tutorial.level2Tutorial == true)
                    {
                        mPlayer1Tutorial.level2Tutorial = false;
                        TutorialMapSpawnGameState new_state = new TutorialMapSpawnGameState(display_map_state);
                        _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));

                    }
                    else
                    {
                        MapPlayGameState new_state = new MapPlayGameState(display_map_state);
                        _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                    }
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
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            display_map_state.Draw(sprite_batch);
            //sprite_batch.Draw(blankTexture, displayOutterEdgeRect, Color.Silver * .75f);
            //sprite_batch.Draw(blankTexture, displayRect, Color.Black * .75f);
            if (mPlayer1Tutorial.SpawnTutorial == true)
            {
                String box1 = "Purple squares represent locations that you can spawn units in, left click on them to spawn a program.";
                String box2 = "You can see where the enemy units begin the game, they are represented by icons that look like this.";
                String box3 = "When spawning units you must also take into account the spawn weights, each map has a spawn weight limit and each program has a spawn weight, your spawned units must be equal to or less than the spawn weight.";
                sprite_batch.Draw(blankTexture, tutorialEdgeRect1, Color.Silver * .75f);
                sprite_batch.Draw(blankTexture, tutorialRect1, Color.Black * .75f);
                sprite_batch.Draw(blankTexture, tutorialEdgeRect2, Color.Silver * .75f);
                sprite_batch.Draw(blankTexture, tutorialRect2, Color.Black * .75f);
                sprite_batch.Draw(blankTexture, tutorialEdgeRect3, Color.Silver * .75f);
                sprite_batch.Draw(blankTexture, tutorialRect3, Color.Black * .75f);
                String temp1;
                String temp2;
                String temp3;
                temp1 = WrapText(ArialFontSize12, box1, tutorialRect1.Width);
                temp2 = WrapText(ArialFontSize12, box2, tutorialRect2.Width);
                temp3 = WrapText(ArialFontSize12, box3, tutorialRect3.Width);
                sprite_batch.DrawString(ArialFontSize12, temp1, new Vector2(tutorialRect1.X, tutorialRect1.Y + 5), Color.White);
                sprite_batch.DrawString(ArialFontSize12, temp2, new Vector2(tutorialRect2.X, tutorialRect2.Y + 5), Color.White);
                sprite_batch.DrawString(ArialFontSize12, temp3, new Vector2(tutorialRect3.X, tutorialRect3.Y + 5), Color.White);
                PrimiviteDrawing.DrawLineSegment(blankTexture, sprite_batch, new Vector2(tutorialEdgeRect1.Center.X, tutorialEdgeRect1.Top),
                    new Vector2(40, 120), Color.Gold, 5);
                PrimiviteDrawing.DrawLineSegment(blankTexture, sprite_batch, new Vector2(tutorialEdgeRect2.Left, tutorialEdgeRect2.Center.Y),
                    new Vector2(260, 160), Color.Gold, 5);
                
            }
            else if (mPlayer1Tutorial.CurrentNode == "Node1")
            {
                String box1 = "Left click on your units to move them to an adjacent square or to use one of their abilites or attacks.";
                String box2 = "Left clicking on enemy units will show how many moves they have and what attack and abilites they can use.";
                String box3 = "The objective of this level is to wipe out all enemy units, move your programs around the map to get them into range to defeat the enemy program.";
                sprite_batch.Draw(blankTexture, tutorialEdgeRect1, Color.Silver * .75f);
                sprite_batch.Draw(blankTexture, tutorialRect1, Color.Black * .75f);
                sprite_batch.Draw(blankTexture, tutorialEdgeRect2, Color.Silver * .75f);
                sprite_batch.Draw(blankTexture, tutorialRect2, Color.Black * .75f);
                sprite_batch.Draw(blankTexture, tutorialEdgeRect3, Color.Silver * .75f);
                sprite_batch.Draw(blankTexture, tutorialRect3, Color.Black * .75f);
                String temp1;
                String temp2;
                String temp3;
                temp1 = WrapText(ArialFontSize12, box1, tutorialRect1.Width);
                temp2 = WrapText(ArialFontSize12, box2, tutorialRect2.Width);
                temp3 = WrapText(ArialFontSize12, box3, tutorialRect3.Width);
                sprite_batch.DrawString(ArialFontSize12, temp1, new Vector2(tutorialRect1.X, tutorialRect1.Y + 5), Color.White);
                sprite_batch.DrawString(ArialFontSize12, temp2, new Vector2(tutorialRect2.X, tutorialRect2.Y + 5), Color.White);
                sprite_batch.DrawString(ArialFontSize12, temp3, new Vector2(tutorialRect3.X, tutorialRect3.Y + 5), Color.White);
                PrimiviteDrawing.DrawLineSegment(blankTexture, sprite_batch, new Vector2(tutorialEdgeRect1.Center.X, tutorialEdgeRect1.Top),
                    new Vector2(40, 120), Color.Gold, 5);

                PrimiviteDrawing.DrawLineSegment(blankTexture, sprite_batch, new Vector2(tutorialEdgeRect2.Left, tutorialEdgeRect2.Center.Y),
                    new Vector2(260, 160), Color.Gold, 5);
            }
            else if (mPlayer1Tutorial.CurrentNode == "Node2")
            {
                String box1 = "Purple squares represent locations that you can spawn units in, left click on them to spawn a program.";
                String box2 = "You can see where the enemy units begin the game, they are represented by icons that look like this.";
                String box3 = "When spawning units you must also take into account the spawn weights, each map has a spawn weight limit and each program has a spawn weight, your spawned units must be equal to or less than the spawn weight.";
                sprite_batch.Draw(blankTexture, tutorialEdgeRect1, Color.Silver * .75f);
                sprite_batch.Draw(blankTexture, tutorialRect1, Color.Black * .75f);
                sprite_batch.Draw(blankTexture, tutorialEdgeRect2, Color.Silver * .75f);
                sprite_batch.Draw(blankTexture, tutorialRect2, Color.Black * .75f);
                sprite_batch.Draw(blankTexture, tutorialEdgeRect3, Color.Silver * .75f);
                sprite_batch.Draw(blankTexture, tutorialRect3, Color.Black * .75f);
                String temp1;
                String temp2;
                String temp3;
                temp1 = WrapText(ArialFontSize12, box1, tutorialRect1.Width);
                temp2 = WrapText(ArialFontSize12, box2, tutorialRect2.Width);
                temp3 = WrapText(ArialFontSize12, box3, tutorialRect3.Width);
                sprite_batch.DrawString(ArialFontSize12, temp1, new Vector2(tutorialRect1.X, tutorialRect1.Y + 5), Color.White);
                sprite_batch.DrawString(ArialFontSize12, temp2, new Vector2(tutorialRect2.X, tutorialRect2.Y + 5), Color.White);
                sprite_batch.DrawString(ArialFontSize12, temp3, new Vector2(tutorialRect3.X, tutorialRect3.Y + 5), Color.White);
                PrimiviteDrawing.DrawLineSegment(blankTexture, sprite_batch, new Vector2(tutorialEdgeRect1.Center.X, tutorialEdgeRect1.Top),
                    new Vector2(40, 120), Color.Gold, 5);

                PrimiviteDrawing.DrawLineSegment(blankTexture, sprite_batch, new Vector2(tutorialEdgeRect2.Left, tutorialEdgeRect2.Center.Y),
                    new Vector2(260, 160), Color.Gold, 5);
            }
            overlay.Draw(sprite_batch);
            String temp;
            temp = WrapText(ArialFontSize12, storyString, displayRect.Width);
            //sprite_batch.DrawString(ArialFontSize12, temp, new Vector2(displayRect.X, displayRect.Y + 5), Color.White);
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

        //Code for text wrapping used from:
        //http://www.xnawiki.com/index.php/Basic_Word_Wrapping
        public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');

            StringBuilder sb = new StringBuilder();

            float lineWidth = 0f;

            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
    }
}
