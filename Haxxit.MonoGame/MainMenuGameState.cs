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
    class MainMenuGameState : HaxxitGameState
    {
        Texture2D rectTexture, backgroundTexture;
        Rectangle startGameRect, optionsRect, creditsRect, exitRect, backgroundRect;
        SpriteFont startGameSpriteFont, titleSpriteFont;
        Color rectColor;
        Vector2 titleStringPos, startStringPos, optionsStringPos, creditsStringPos, exitStringPos;

        String titleString = "Haxxit";

        String startString = "Start";
        String optionsString = "Options";
        String creditsString = "Credits";
        String exitString = "Exit";

        int gameWindowWidth, gameWindowHeight;

        Texture2D buttonReleased, buttonPressed;

        bool isStartButtonClicked, isOptionsButtonClicked, isCreditsButtonClicked, isExitButtonClicked;

        //TODO Need to change this for the future
        Player mStartPlayer;


        public MainMenuGameState() : base()
        {

        }

        public override void Init()
        {
            gameWindowWidth = GlobalAccessors.mGame.Window.ClientBounds.Width; //use to be 640
            gameWindowHeight = GlobalAccessors.mGame.Window.ClientBounds.Height;

            isStartButtonClicked = isOptionsButtonClicked = isCreditsButtonClicked = isExitButtonClicked = false;

            startGameRect = new Rectangle(gameWindowWidth / 2 - 50, gameWindowHeight / 2, 100, 30);
            optionsRect = new Rectangle(gameWindowWidth / 2 - 50, gameWindowHeight / 2 + 33, 100, 30);
            creditsRect = new Rectangle(gameWindowWidth / 2 - 50, gameWindowHeight / 2 + 66, 100, 30);
            exitRect = new Rectangle(gameWindowWidth / 2 - 50, gameWindowHeight / 2 + 99, 100, 30);

            backgroundRect = new Rectangle(0, 0, gameWindowWidth, gameWindowHeight);
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            graphics.Clear(Color.Black);

            Init();

            startGameSpriteFont = content.Load<SpriteFont>("Arial-16px-Regular");
            titleSpriteFont = content.Load<SpriteFont>("Arial-48px-Bold");

            rectTexture = new Texture2D(graphics, 1, 1);
            rectTexture.SetData(new Color[] { Color.White });

            //File: Ogd1 layers2 License: GPLv2 or later <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html> 
            //Author Original: Traversal Technology Author Ferivative: Sreejith K <http://commons.wikimedia.org/wiki/User:Sreejithk2000>
            //backgroundTexture = content.Load<Texture2D>("Ogd1_layers2");

            backgroundTexture = content.Load<Texture2D>("Grid2D");

            buttonReleased = content.Load<Texture2D>("blackButtonReleased");
            buttonPressed = content.Load<Texture2D>("blackButtonPressed");

            titleStringPos = startStringPos = optionsStringPos = creditsStringPos = exitStringPos = new Vector2(0, 0);

            Vector2 length = titleSpriteFont.MeasureString(titleString);
            titleStringPos.X = gameWindowWidth / 2 - length.X / 2;
            titleStringPos.Y = gameWindowHeight * (1 / 4f);

            length = startGameSpriteFont.MeasureString(startString);
            startStringPos.X = startGameRect.X + ((startGameRect.Width - length.X) / 2f);
            startStringPos.Y = startGameRect.Y + ((startGameRect.Height - length.Y) / 2f);

            length = startGameSpriteFont.MeasureString(optionsString);
            optionsStringPos.X = optionsRect.X + ((optionsRect.Width - length.X) / 2f);
            optionsStringPos.Y = optionsRect.Y + ((optionsRect.Height - length.Y) / 2f);

            length = startGameSpriteFont.MeasureString(creditsString);
            creditsStringPos.X = creditsRect.X + ((creditsRect.Width - length.X) / 2f);
            creditsStringPos.Y = creditsRect.Y + ((creditsRect.Height - length.Y) / 2f);

            length = startGameSpriteFont.MeasureString(exitString);
            exitStringPos.X = exitRect.X + ((exitRect.Width - length.X) / 2f);
            exitStringPos.Y = exitRect.Y + ((exitRect.Height - length.Y) / 2f);
        }

        public override void SubscribeAll()
        {

        }

        public override void Update()
        {
            MouseState mouse_state = Mouse.GetState(); // Gets the mouse state object
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y); // creates a point for the mouse's position

            if (isStartButtonClicked)
            {
                if (mouse_state.LeftButton == ButtonState.Released)
                {
                    mStartPlayer = GlobalAccessors.mPlayer1;
                    //mStartPlayer.AddProgram(new BugFactory());
                    //mStartPlayer.AddProgram(new HackFactory());
                    mStartPlayer.AddProgram(new SlingshotFactory());
                    mStartPlayer.IsHacked = false;
                    ServerOverworldState new_state = new ServerOverworldState();
                    Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                }
            }

            if (isCreditsButtonClicked)
            {
                if (mouse_state.LeftButton == ButtonState.Released)
                {
                    CreditsGameState new_state = new CreditsGameState();
                    Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                }
            }

            if (isExitButtonClicked)
            {
                if (mouse_state.LeftButton == ButtonState.Released)
                {
                    GlobalAccessors.mGame.Exit();
                }
            }

            //Update for Start Button
            if (startGameRect.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                //rectColor = Color.Red;
                isStartButtonClicked = true;
            }
            // if hovering over rectangle
            else if (startGameRect.Contains(mouse_position))
            {
                //rectColor = Color.Yellow;
            }
            else // neither clicking nor hovering over rectangle
            {
                //rectColor = Color.Green;
                isStartButtonClicked = false;
            }

            //Update for Options Button
            if (optionsRect.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                //rectColor = Color.Red;
                isOptionsButtonClicked = true;
            }
            // if hovering over rectangle
            else if (optionsRect.Contains(mouse_position))
            {
                //rectColor = Color.Yellow;
            }
            else // neither clicking nor hovering over rectangle
            {
                //rectColor = Color.Green;
                isOptionsButtonClicked = false;
            }

            //Update for Credits Button
            if (creditsRect.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                //rectColor = Color.Red;
                isCreditsButtonClicked = true;
            }
            // if hovering over rectangle
            else if (creditsRect.Contains(mouse_position))
            {
                //rectColor = Color.Yellow;
            }
            else // neither clicking nor hovering over rectangle
            {
                //rectColor = Color.Green;
                isCreditsButtonClicked = false;
            }

            //Update for Credits Button
            if (exitRect.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                //rectColor = Color.Red;
                isExitButtonClicked = true;
            }
            // if hovering over rectangle
            else if (exitRect.Contains(mouse_position))
            {
                //rectColor = Color.Yellow;
            }
            else // neither clicking nor hovering over rectangle
            {
                //rectColor = Color.Green;
                isExitButtonClicked = false;
            }

        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            //Draw background
            sprite_batch.Draw(backgroundTexture, backgroundRect, Color.White);

            //Draw title Haxxit
            sprite_batch.DrawString(titleSpriteFont, titleString, titleStringPos + (Vector2.One * 5), Color.Black); //pop out effect
            sprite_batch.DrawString(titleSpriteFont, titleString, titleStringPos, Color.White);

            //Drawing start button
            if (isStartButtonClicked)
            {
                //if mouse click
                sprite_batch.Draw(buttonPressed, startGameRect, Color.White);
            }
            else
            {
                //else no mouse click
                sprite_batch.Draw(buttonReleased, startGameRect, Color.White);
            }

            sprite_batch.DrawString(startGameSpriteFont, startString, startStringPos + (Vector2.One * 2), Color.Black);
            sprite_batch.DrawString(startGameSpriteFont, startString, startStringPos, Color.White);

            //Drawing options button
            if (isOptionsButtonClicked)
            {
                //if mouse click
                sprite_batch.Draw(buttonPressed, optionsRect, Color.White);
            }
            else
            {
                //else no mouse click
                sprite_batch.Draw(buttonReleased, optionsRect, Color.White);
            }

            sprite_batch.DrawString(startGameSpriteFont, optionsString, optionsStringPos + (Vector2.One * 2), Color.Black);
            sprite_batch.DrawString(startGameSpriteFont, optionsString, optionsStringPos, Color.White);

            //Drawing credits button
            if (isCreditsButtonClicked)
            {
                //if mouse click
                sprite_batch.Draw(buttonPressed, creditsRect, Color.White);
            }
            else
            {
                //else no mouse click
                sprite_batch.Draw(buttonReleased, creditsRect, Color.White);
            }

            sprite_batch.DrawString(startGameSpriteFont, creditsString, creditsStringPos + (Vector2.One * 2), Color.Black);
            sprite_batch.DrawString(startGameSpriteFont, creditsString, creditsStringPos, Color.White);

            //Drawing Exit button
            if (isExitButtonClicked)
            {
                //if mouse click
                sprite_batch.Draw(buttonPressed, exitRect, Color.White);
            }
            else
            {
                //else no mouse click
                sprite_batch.Draw(buttonReleased, exitRect, Color.White);
            }

            sprite_batch.DrawString(startGameSpriteFont, exitString, exitStringPos + (Vector2.One * 2), Color.Black);
            sprite_batch.DrawString(startGameSpriteFont, exitString, exitStringPos, Color.White);
        }

        public override void NewMediator(IMediator mediator)
        {

        }

    }
}
