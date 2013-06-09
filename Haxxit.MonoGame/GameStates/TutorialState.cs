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
            sprite_batch.Draw(blankTexture, displayOutterEdgeRect, Color.Silver * .75f);
            sprite_batch.Draw(blankTexture, displayRect, Color.Black * .75f);
            overlay.Draw(sprite_batch);
            String temp;
            temp = WrapText(ArialFontSize12, storyString, displayRect.Width);
            sprite_batch.DrawString(ArialFontSize12, temp, new Vector2(displayRect.X, displayRect.Y + 5), Color.White);
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
