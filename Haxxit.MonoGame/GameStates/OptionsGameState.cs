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
    class OptionsGameState : HaxxitGameState
    {
        //silly white pixel thing for drawable rectangle
        Texture2D white_pixel;

        //title
        String titleString = "Options";
        SpriteFont titleSpriteFont;
        Vector2 titleStringPos;

        //black rect
        Rectangle displayRect, displayOutterEdgeRect, backgroundRect;
        Texture2D blankTexture, backgroundTexture;

        //options
        SpriteFont optionSpriteFont;
        Texture2D optionSelectedTexture, optionNotSelectedTexture;
        String kelvinString = "Kelvin Mode";
        Vector2 kelvinStringPos;
        Color kelvinOptionBackgroundRectColor;
        Rectangle kelvinOptionBackgroundRect, kelvinOptionRect;
        DrawableRectangle drawableKelvinOptionRect;
        String soundString = "Sound";
        Vector2 soundStringPos;
        Color soundOptionBackgroundRectColor;
        Rectangle soundOptionBackgroundRect, soundOptionRect;
        DrawableRectangle drawableSoundOptionRect;
        String musicString = "Music";
        Vector2 musicStringPos;
        Color musicOptionBackgroundRectColor;
        Rectangle musicOptionBackgroundRect, musicOptionRect;
        DrawableRectangle drawableMusicOptionRect;

        //back button
        SpriteFont backSpriteFont;
        String backString = "Back";
        Vector2 backStringPos;
        Rectangle backRect;
        Texture2D buttonReleased, buttonPressed;
        bool isBackButtonClicked;
        int xOffset, yOffset;

        Player player;

        int mWindowWidth, mWindowHeight;

        public OptionsGameState()
            : base()
        {
            player = GlobalAccessors.mPlayer1;
        }

        public override void Init()
        {
            mWindowWidth = 800;
            mWindowHeight = 480;

            xOffset = mWindowWidth / 10;
            yOffset = mWindowHeight / 15;

            backgroundRect = new Rectangle(0, 0, mWindowWidth, mWindowHeight);

            backRect = new Rectangle(mWindowWidth - xOffset, mWindowHeight - yOffset, xOffset, yOffset);

            backStringPos.X = backRect.X + ((backRect.Width - backString.Length * 10) / 2f);
            backStringPos.Y = backRect.Y + 3;

            titleStringPos = new Vector2(0, 0);

            kelvinStringPos = new Vector2(mWindowWidth / 2 - 95, 190);
            soundStringPos = new Vector2(mWindowWidth / 2 - 95, 250);
            musicStringPos = new Vector2(mWindowWidth / 2 - 95, 310);
            kelvinOptionBackgroundRectColor = Color.White;
            kelvinOptionBackgroundRect = new Rectangle((int)kelvinStringPos.X + 148, (int)kelvinStringPos.Y, 24, 24);
            kelvinOptionRect = new Rectangle((int)kelvinStringPos.X + 150, (int)kelvinStringPos.Y + 2, 20, 20);
            drawableKelvinOptionRect = new DrawableRectangle(white_pixel, kelvinOptionRect, Color.White);
            drawableKelvinOptionRect.OnMouseInside += delegate { kelvinOptionBackgroundRectColor = Color.Cyan; };
            drawableKelvinOptionRect.OnMouseOutside += delegate { kelvinOptionBackgroundRectColor = Color.White; };
            drawableKelvinOptionRect.OnMouseLeftClick += OnKelvinOptionClick;
            soundOptionBackgroundRectColor = Color.White;
            soundOptionBackgroundRect = new Rectangle((int)soundStringPos.X + 148, (int)soundStringPos.Y, 24, 24);
            soundOptionRect = new Rectangle((int)soundStringPos.X + 150, (int)soundStringPos.Y + 2, 20, 20);
            drawableSoundOptionRect = new DrawableRectangle(white_pixel, soundOptionRect, Color.White);
            drawableSoundOptionRect.OnMouseInside += delegate { soundOptionBackgroundRectColor = Color.Cyan; };
            drawableSoundOptionRect.OnMouseOutside += delegate { soundOptionBackgroundRectColor = Color.White; };
            drawableSoundOptionRect.OnMouseLeftClick += OnSoundOptionClick;
            musicOptionBackgroundRectColor = Color.White;
            musicOptionBackgroundRect = new Rectangle((int)musicStringPos.X + 148, (int)musicStringPos.Y, 24, 24);
            musicOptionRect = new Rectangle((int)musicStringPos.X + 150, (int)musicStringPos.Y + 2, 20, 20);
            drawableMusicOptionRect = new DrawableRectangle(white_pixel, musicOptionRect, Color.White);
            drawableMusicOptionRect.OnMouseInside += delegate { musicOptionBackgroundRectColor = Color.Cyan; };
            drawableMusicOptionRect.OnMouseOutside += delegate { musicOptionBackgroundRectColor = Color.White; };
            drawableMusicOptionRect.OnMouseLeftClick += OnMusicOptionClick;
        }

        private void OnKelvinOptionClick(DrawableRectangle rectangle)
        {
            if (player.KelvinMode)
            {
                player.KelvinMode = false;
            }
            else
            {
                player.KelvinMode = true;
            }
        }

        private void OnSoundOptionClick(DrawableRectangle rectangle)
        {
            if (player.Sound)
            {
                player.Sound = false;
            }
            else
            {
                player.Sound = true;
            }
        }

        private void OnMusicOptionClick(DrawableRectangle rectangle)
        {
            if (player.Music)
            {
                player.Music = false;
            }
            else
            {
                player.Music = true;
            }
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            white_pixel = new Texture2D(graphics, 1, 1);
            white_pixel.SetData(new Color[] { Color.White });

            backSpriteFont = content.Load<SpriteFont>("Arial-12px-Regular");
            titleSpriteFont = content.Load<SpriteFont>("Arial-48px-Bold");
            optionSpriteFont = content.Load<SpriteFont>("Arial-12px-Regular");

            backgroundTexture = content.Load<Texture2D>("Grid2D");

            optionSelectedTexture = content.Load<Texture2D>("OptionSelected");
            optionNotSelectedTexture = content.Load<Texture2D>("OptionNotSelected");

            buttonReleased = content.Load<Texture2D>("blackButtonReleased");
            buttonPressed = content.Load<Texture2D>("blackButtonPressed");

            blankTexture = new Texture2D(graphics, 1, 1);
            blankTexture.SetData(new Color[] { Color.White });

            displayRect = new Rectangle(mWindowWidth / 2 - 105, 150, 210, 235);
            displayOutterEdgeRect = new Rectangle(mWindowWidth / 2 - 115, 140, 230, 255);

            Vector2 length = backSpriteFont.MeasureString("E");

            length = titleSpriteFont.MeasureString(titleString);
            titleStringPos.X = mWindowWidth / 2 - length.X / 2;
            titleStringPos.Y = mWindowHeight * (1 / 8f);
        }

        public override void SubscribeAll()
        {

        }

        public override void Update()
        {
            drawableKelvinOptionRect.Update();
            drawableSoundOptionRect.Update();
            drawableMusicOptionRect.Update();

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

            //draw options
            sprite_batch.DrawString(optionSpriteFont, kelvinString, kelvinStringPos + (Vector2.One * 2), Color.Black); //pop out effect
            sprite_batch.DrawString(optionSpriteFont, kelvinString, kelvinStringPos, Color.White);
            sprite_batch.Draw(blankTexture, kelvinOptionBackgroundRect, kelvinOptionBackgroundRectColor);
            if (player.KelvinMode)
            {
                sprite_batch.Draw(optionSelectedTexture, kelvinOptionRect, Color.White);
            }
            else
            {
                sprite_batch.Draw(optionNotSelectedTexture, kelvinOptionRect, Color.White);
            }
            sprite_batch.DrawString(optionSpriteFont, soundString, soundStringPos + (Vector2.One * 2), Color.Black); //pop out effect
            sprite_batch.DrawString(optionSpriteFont, soundString, soundStringPos, Color.White);
            sprite_batch.Draw(blankTexture, soundOptionBackgroundRect, soundOptionBackgroundRectColor);
            if (player.Sound)
            {
                sprite_batch.Draw(optionSelectedTexture, soundOptionRect, Color.White);
            }
            else
            {
                sprite_batch.Draw(optionNotSelectedTexture, soundOptionRect, Color.White);
            }
            sprite_batch.DrawString(optionSpriteFont, musicString, musicStringPos + (Vector2.One * 2), Color.Black); //pop out effect
            sprite_batch.DrawString(optionSpriteFont, musicString, musicStringPos, Color.White);
            sprite_batch.Draw(blankTexture, musicOptionBackgroundRect, musicOptionBackgroundRectColor);
            if (player.Music)
            {
                sprite_batch.Draw(optionSelectedTexture, musicOptionRect, Color.White);
            }
            else
            {
                sprite_batch.Draw(optionNotSelectedTexture, musicOptionRect, Color.White);
            }

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
