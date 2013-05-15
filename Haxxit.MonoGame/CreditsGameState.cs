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
        Texture2D creditsTexture, backgroundTexture, rectTexture;
        Rectangle creditsRectBorder, creditsRect, backgroundRect;

        SpriteFont backSpriteFont;
        String backString = "Back";
        Vector2 backStringPos;
        Rectangle backRect;
        Texture2D buttonReleased, buttonPressed;

        int gameWindowWidth, gameWindowHeight;

        bool isBackButtonClicked;

        public CreditsGameState() : base()
        {

        }

        public override void Init()
        {
            isBackButtonClicked = false;

            gameWindowWidth = 800; //use to be 640
            gameWindowHeight = 480;

            backgroundRect = new Rectangle(0, 0, gameWindowWidth, gameWindowHeight);

            creditsRectBorder = new Rectangle(gameWindowWidth / 2 - 100, gameWindowHeight / 2 - 100, 200, 200);
            creditsRect = new Rectangle(gameWindowWidth / 2 - 95, gameWindowHeight / 2 - 95, 190, 190);

            backRect = new Rectangle(creditsRect.X + ((creditsRect.Width - 100) / 2), creditsRect.Y + creditsRect.Height + 8, 100, 30);

            backStringPos.X = backRect.X + ((backRect.Width - backString.Length * 10) / 2f);
            backStringPos.Y = backRect.Y + 3;
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            backSpriteFont = content.Load<SpriteFont>("Arial");

            creditsTexture = content.Load<Texture2D>("Border");
            backgroundTexture = content.Load<Texture2D>("Grid2D");

            buttonReleased = content.Load<Texture2D>("blackButtonReleased");
            buttonPressed = content.Load<Texture2D>("blackButtonPressed");

            rectTexture = new Texture2D(graphics, 1, 1);
            rectTexture.SetData(new Color[] { Color.White });
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
                //rectColor = Color.Red;
                isBackButtonClicked = true;
            }
            // if hovering over rectangle
            else if (backRect.Contains(mouse_position))
            {
                //rectColor = Color.Yellow;
            }
            else // neither clicking nor hovering over rectangle
            {
                //rectColor = Color.Green;
                isBackButtonClicked = false;
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            //Draw background
            sprite_batch.Draw(backgroundTexture, backgroundRect, Color.White);

            //draw credits container
            //sprite_batch.Draw(rectTexture, creditsRect, Color.Black);
            //sprite_batch.Draw(creditsTexture, creditsRectBorder, Color.White);

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
