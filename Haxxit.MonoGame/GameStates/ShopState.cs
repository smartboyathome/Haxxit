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
using SmartboyDevelopments.Haxxit.MonoGame.GameStates;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    class ShopState : HaxxitGameState
    {
        //window variables
        int mWindowWidth, mWindowHeight, xOffset, yOffset, containerYOffset;

        //button texture
        Texture2D buttonReleased, buttonPressed;

        //Background
        Rectangle mBackground;

        //Player
        Player mPlayer1InShop;

        //For Displaying Player's Programs
        Rectangle YourProgramsTitleRect;
        String YourProgramsString = "Your Programs";
        Vector2 YourProgramsStringPos;
        SpriteFont YourProgramsSpriteFont;
        Rectangle YourProgramsContainerRect;
        Texture2D blankRectTexture, mBackgroundTexture;

        //Players Programs selectable options
        Rectangle[] mPlayersProgramsSelectable;
        int mPlayerSingleProgramSelectedIndex;
        bool mIsAPlayerProgramSelected;
        const int MAX_SELECTABLE = 5;

        //For Displaying Program Info
        Rectangle YourProgramsInfoTitleRect;
        String YourProgramsInfoString = "Info";
        Vector2 YourProgramsInfoStringPos;
        Rectangle YourProgramsInfoContainerRect;

        //displaying program images
        Texture2D[] mPlayerProgramImages;
        Rectangle mProgramImageRect, mAvailImageRect;

        //For Displaying Available Programs
        Rectangle AvailProgramsTitleRect;
        String AvailProgramsString = "Available";
        Vector2 AvailProgramsStringPos;
        Rectangle AvailProgramsContainerRect;

        //buyable Programs
        List<ProgramFactory> mBuyablePrograms;
        Rectangle[] mAvailProgramsSelectable;
        int mAvailSingleProgramSelectedIndex;
        bool mIsAnAvailProgramSelected;

        //Buy button
        Rectangle BuyButtonRect;
        String BuyButtonString = "Download";
        Vector2 BuyButtonStringPos;
        bool isBuyButtonClicked;

        //For Displaying Buyable Program Info
        Rectangle AvailProgramsInfoTitleRect;
        String AvailProgramsInfoString = "Info";
        Vector2 AvailProgramsInfoStringPos;
        Rectangle AvailProgramsInfoContainerRect;

        //For Displaying Player options at bottom
        Rectangle PlayerOptions;
        Vector2 PlayerNamePos, PlayerSilicoinPos;
        SpriteFont PlayerUISpriteFont;
        Rectangle ExitButtonRect;
        String ExitButtonString = "Back";
        Vector2 ExitButtonStringPos;
        bool isExitButtonClicked;

        //dialog box
        bool isDialogMessageOpen;
        bool isOkayButtonClicked;

        public ShopState()
            : base()
        {

        }

        public override void Init()
        {
            mWindowHeight = GlobalAccessors.mGame.Window.ClientBounds.Height;
            mWindowWidth = GlobalAccessors.mGame.Window.ClientBounds.Width;

            xOffset = mWindowWidth / 10;
            yOffset = mWindowHeight / 15;

            containerYOffset = 7 * yOffset / 5;

            YourProgramsTitleRect = new Rectangle(xOffset / 2, yOffset, 2 * xOffset, yOffset);
            YourProgramsContainerRect = new Rectangle(xOffset / 2, 2 * yOffset, 2 * xOffset, 7 * yOffset);

            YourProgramsInfoTitleRect = new Rectangle(xOffset / 2 * 5 + 5, yOffset, 2 * xOffset, yOffset);
            YourProgramsInfoContainerRect = new Rectangle(xOffset / 2 * 5 + 5, 2 * yOffset, 2 * xOffset, 7 * yOffset);

            AvailProgramsTitleRect = new Rectangle(mWindowWidth / 2 + xOffset / 2, yOffset, 2 * xOffset, yOffset);
            AvailProgramsContainerRect = new Rectangle(mWindowWidth / 2 + xOffset / 2, 2 * yOffset, 2 * xOffset, 7 * yOffset);

            AvailProgramsInfoTitleRect = new Rectangle(mWindowWidth / 2 + xOffset / 2 * 5 + 5, yOffset, 2 * xOffset, yOffset);
            AvailProgramsInfoContainerRect = new Rectangle(mWindowWidth / 2 + xOffset / 2 * 5 + 5, 2 * yOffset, 2 * xOffset, 7 * yOffset);

            BuyButtonRect = new Rectangle(mWindowWidth / 2 + xOffset / 2, 2 * yOffset + 7 * yOffset + 5, 2 * xOffset, yOffset);

            PlayerOptions = new Rectangle(0, mWindowHeight - yOffset, mWindowWidth, yOffset);

            ExitButtonRect = new Rectangle(mWindowWidth - xOffset, mWindowHeight - yOffset, xOffset, yOffset);
            isExitButtonClicked = isBuyButtonClicked = false;

            isOkayButtonClicked = isDialogMessageOpen = false;

            //Anytime changes are made should update back into Global Accessors
            mPlayer1InShop = GlobalAccessors.mPlayer1;

            mPlayersProgramsSelectable = new Rectangle[MAX_SELECTABLE];
            mAvailProgramsSelectable = new Rectangle[MAX_SELECTABLE];
            mPlayerSingleProgramSelectedIndex = mAvailSingleProgramSelectedIndex = -1; 

            int playerProgramDisplacement = 7 * yOffset / 5; // 5 represents static number of available programs in players inventory
            int initialPlayerProgramOffset = 2 * yOffset;
            //initialize rectangle for program selection by user
            mPlayersProgramsSelectable[0] = new Rectangle(xOffset / 2, initialPlayerProgramOffset, 2 * xOffset, playerProgramDisplacement);
            mAvailProgramsSelectable[0] = new Rectangle(mWindowWidth / 2 + xOffset / 2, initialPlayerProgramOffset, 2 * xOffset, playerProgramDisplacement);
            initialPlayerProgramOffset += playerProgramDisplacement;
            for (int i = 1; i < MAX_SELECTABLE; i++)
            {
                mPlayersProgramsSelectable[i] = new Rectangle(xOffset / 2, initialPlayerProgramOffset, 2 * xOffset, playerProgramDisplacement);
                mAvailProgramsSelectable[i] = new Rectangle(mWindowWidth / 2 + xOffset / 2, initialPlayerProgramOffset, 2 * xOffset, playerProgramDisplacement);
                initialPlayerProgramOffset += playerProgramDisplacement;
            }

            mIsAPlayerProgramSelected = mIsAnAvailProgramSelected = false;

            mProgramImageRect = new Rectangle(YourProgramsInfoContainerRect.X + 5, YourProgramsInfoContainerRect.Y + 2, xOffset / 2, xOffset / 2);

            mAvailImageRect = new Rectangle(AvailProgramsInfoContainerRect.X + 5, AvailProgramsInfoContainerRect.Y + 2, xOffset / 2, xOffset / 2);
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            Init();

            YourProgramsSpriteFont = content.Load<SpriteFont>("Arial-14px-Regular");
            blankRectTexture = new Texture2D(graphics, 1, 1);
            blankRectTexture.SetData(new Color[] { Color.White });

            mBackgroundTexture = content.Load<Texture2D>("Grid2D");

            PlayerUISpriteFont = content.Load<SpriteFont>("Arial-12px-Regular");

            buttonPressed = content.Load<Texture2D>("blackButtonPressed");
            buttonReleased = content.Load<Texture2D>("blackButtonReleased");

            mBackground = new Rectangle(0, 0, mWindowWidth, mWindowHeight);

            Vector2 length = YourProgramsSpriteFont.MeasureString(YourProgramsString);
            YourProgramsStringPos = YourProgramsInfoStringPos = AvailProgramsStringPos = AvailProgramsInfoStringPos = 
                PlayerNamePos = PlayerSilicoinPos = ExitButtonStringPos = new Vector2(0, 0);

            YourProgramsStringPos.X = YourProgramsTitleRect.X + ((YourProgramsTitleRect.Width - length.X) / 2);
            YourProgramsStringPos.Y = YourProgramsTitleRect.Y + ((YourProgramsTitleRect.Height - length.Y) / 2);

            length = YourProgramsSpriteFont.MeasureString(YourProgramsInfoString);
            YourProgramsInfoStringPos.X = YourProgramsInfoTitleRect.X + ((YourProgramsInfoTitleRect.Width - length.X) / 2);
            YourProgramsInfoStringPos.Y = YourProgramsInfoTitleRect.Y + ((YourProgramsInfoTitleRect.Height - length.Y) / 2);

            length = YourProgramsSpriteFont.MeasureString(AvailProgramsString);
            AvailProgramsStringPos.X = AvailProgramsTitleRect.X + ((AvailProgramsTitleRect.Width - length.X) / 2);
            AvailProgramsStringPos.Y = AvailProgramsTitleRect.Y + ((AvailProgramsTitleRect.Height - length.Y) / 2);

            length = YourProgramsSpriteFont.MeasureString(AvailProgramsInfoString);
            AvailProgramsInfoStringPos.X = AvailProgramsInfoTitleRect.X + ((AvailProgramsInfoTitleRect.Width - length.X) / 2);
            AvailProgramsInfoStringPos.Y = AvailProgramsInfoTitleRect.Y + ((AvailProgramsInfoTitleRect.Height - length.Y) / 2);

            length = PlayerUISpriteFont.MeasureString(mPlayer1InShop.Name);
            PlayerNamePos.X = PlayerOptions.X + 5;
            PlayerNamePos.Y = PlayerOptions.Y + ((PlayerOptions.Height - length.Y) / 2);

            length = PlayerUISpriteFont.MeasureString("Silicoins: " + mPlayer1InShop.TotalSilicoins);
            PlayerSilicoinPos.X = mWindowWidth / 2 - (length.X / 2);
            PlayerSilicoinPos.Y = PlayerOptions.Y + ((PlayerOptions.Height - length.Y) / 2);

            length = PlayerUISpriteFont.MeasureString(ExitButtonString);
            ExitButtonStringPos.X = ExitButtonRect.X + ((ExitButtonRect.Width - length.X) / 2);
            ExitButtonStringPos.Y = ExitButtonRect.Y + ((ExitButtonRect.Height - length.Y) / 2);

            length = PlayerUISpriteFont.MeasureString(BuyButtonString);
            BuyButtonStringPos.X = BuyButtonRect.X + ((BuyButtonRect.Width - length.X) / 2);
            BuyButtonStringPos.Y = BuyButtonRect.Y + ((BuyButtonRect.Height - length.Y) / 2);

            //for displaying program images
            mPlayerProgramImages = new Texture2D[MAX_SELECTABLE];

            mPlayerProgramImages[0] = content.Load<Texture2D>("Hack");
            mPlayerProgramImages[1] = content.Load<Texture2D>("Bug");
            mPlayerProgramImages[2] = content.Load<Texture2D>("SlingShot");
            mPlayerProgramImages[3] = content.Load<Texture2D>("Virus");
            mPlayerProgramImages[4] = content.Load<Texture2D>("Bomb");

            //Programs Available in the shop MIGHT NEED TO CHANGE INSTANTIATION LATER
            mBuyablePrograms = new List<ProgramFactory>();

            mBuyablePrograms.Add(new SniperFactory());
            mBuyablePrograms.Add(new HackerFactory());
            mBuyablePrograms.Add(new MemManFactory());
            mBuyablePrograms.Add(new TrojanFactory());
            mBuyablePrograms.Add(new Sniper2Factory());
        }

        public override void SubscribeAll()
        {

        }

        public override void Update()
        {
            MouseState mouse_state = Mouse.GetState(); // Gets the mouse state object
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y); // creates a point for the mouse's position

            if (isDialogMessageOpen)
            {
                if (isOkayButtonClicked && mouse_state.LeftButton == ButtonState.Released)
                {
                    isDialogMessageOpen = false;
                }
            }

            if (isExitButtonClicked)
            {
                if (mouse_state.LeftButton == ButtonState.Released)
                {
                    ServerOverworldState new_state = new ServerOverworldState();
                    Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                }
            }

            if (isBuyButtonClicked && mAvailSingleProgramSelectedIndex != -1 && mAvailSingleProgramSelectedIndex < mBuyablePrograms.Count())
            {
                if (mouse_state.LeftButton == ButtonState.Released)
                {
                    //Buy Program:
                    //Check to see if Player has enough Silicoins to purchase Program
                    if (mBuyablePrograms[mAvailSingleProgramSelectedIndex].ProgramCost <= mPlayer1InShop.TotalSilicoins)
                    {
                        if (!mPlayer1InShop.OwnsProgrm(mBuyablePrograms[mAvailSingleProgramSelectedIndex]))
                        {
                            //if player does, deduct amount of program cost from player's silicoins
                            mPlayer1InShop.RemoveSilicoins((ushort)mBuyablePrograms[mAvailSingleProgramSelectedIndex].ProgramCost);

                            //Add program to Player's inventory
                            mPlayer1InShop.AddProgram(mBuyablePrograms[mAvailSingleProgramSelectedIndex]);
                        }
                        else
                        {
                            //if they do display to user they already have program
                            isDialogMessageOpen = true;
                            ShopStateDialogBox new_state = new ShopStateDialogBox(this, "You already have that program.");
                            Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
                        }
                    }
                    else
                    {
                        //if not display to user they don't have enough silicoins
                        isDialogMessageOpen = true;
                        ShopStateDialogBox new_state = new ShopStateDialogBox(this, "You don't have enough silicoins.");
                        Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
                    }

                    //released button click state
                    isBuyButtonClicked = false;
                }
            }
            else
            {
                isBuyButtonClicked = false;
            }

            //Update for Exit Button
            if (ExitButtonRect.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                isExitButtonClicked = true;
            }
            // if hovering over rectangle
            else if (ExitButtonRect.Contains(mouse_position))
            {
            }
            else // neither clicking nor hovering over rectangle
            {
                isExitButtonClicked = false;
            }

            //Update for Buy Button
            if (BuyButtonRect.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
            {
                isBuyButtonClicked = true;
            }
            // if hovering over rectangle
            else if (BuyButtonRect.Contains(mouse_position))
            {
            }
            else // neither clicking nor hovering over rectangle
            {
                isBuyButtonClicked = false;
            }

            //figuring out what program player selected out of their inventory
            for (int i = 0; i < mPlayersProgramsSelectable.Count(); i++)
            {
                // if clicking within rectangle within players programs
                if (mPlayersProgramsSelectable[i].Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
                {
                    //Update Program Info
                    //Program 
                    mPlayerSingleProgramSelectedIndex = i;
                    mIsAPlayerProgramSelected = true;
                    return;
                }
                else // neither clicking nor hovering over rectangle
                {
                    
                }
            }

            //figuring out what program player selected out of buyable programs
            for (int i = 0; i < mBuyablePrograms.Count(); i++)
            {
                // if clicking within rectangle within players programs
                if (mAvailProgramsSelectable[i].Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
                {
                    //Update Program Info
                    //Program 
                    mAvailSingleProgramSelectedIndex = i;
                    mIsAnAvailProgramSelected = true;
                    return;
                }
                else // neither clicking nor hovering over rectangle
                {

                }
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            Vector2 pos = Vector2.Zero;

            //background
            sprite_batch.Draw(mBackgroundTexture, mBackground, Color.White);

            //For displaying Player's Programs
            sprite_batch.Draw(blankRectTexture, YourProgramsContainerRect, Color.Black * .75f);
            sprite_batch.Draw(blankRectTexture, YourProgramsTitleRect, Color.Black * .75f);
            sprite_batch.DrawString(YourProgramsSpriteFont, YourProgramsString, YourProgramsStringPos, Color.White);

            //highlighting program selected by user from his/her inventory
            for (int i = 0; i < mPlayersProgramsSelectable.Length; i++)
            {
                if (mPlayerSingleProgramSelectedIndex == i)
                {
                    sprite_batch.Draw(blankRectTexture, mPlayersProgramsSelectable[i], Color.White * .25f);
                }
                else
                {
                    sprite_batch.Draw(blankRectTexture, mPlayersProgramsSelectable[i], Color.Black * .25f);
                }
            }   
            
            //displaying title and count of bought programs
            containerYOffset = 0;
            pos.X = YourProgramsContainerRect.X + 5;

            for (int i = 0; i < mPlayer1InShop.GetPrograms().Count(); i++)
            {
                pos.Y = YourProgramsContainerRect.Y + containerYOffset;
                if (mPlayerSingleProgramSelectedIndex == i)
                {
                    sprite_batch.DrawString(PlayerUISpriteFont, mPlayer1InShop.GetPrograms().ElementAt(i).TypeName,
                        pos, Color.Black);
                }
                else
                {
                    sprite_batch.DrawString(PlayerUISpriteFont, mPlayer1InShop.GetPrograms().ElementAt(i).TypeName,
                        pos, Color.White);
                }
                containerYOffset += 7 * yOffset / 5;
            }
            containerYOffset = 0;

            //Displaying info on player selected program
            sprite_batch.Draw(blankRectTexture, YourProgramsInfoContainerRect, Color.Black * .75f);
            sprite_batch.Draw(blankRectTexture, YourProgramsInfoTitleRect, Color.Black * .75f);
            sprite_batch.DrawString(YourProgramsSpriteFont, YourProgramsInfoString, YourProgramsInfoStringPos, Color.White);

            //displaying Program Info of selected players program
            containerYOffset = 0;
            if (mIsAPlayerProgramSelected && mPlayer1InShop.GetPrograms().Count() > 0 && mPlayerSingleProgramSelectedIndex <= mPlayer1InShop.GetPrograms().Count() - 1)
            {
                int buyingIndex = 0;
                for (int i = 0; i < mBuyablePrograms.Count; i++)
                {
                    if (mPlayer1InShop.GetPrograms().ElementAt(mPlayerSingleProgramSelectedIndex).TypeName == mBuyablePrograms[i].TypeName)
                    {
                        buyingIndex = i;
                        break;
                    }
                }
                sprite_batch.Draw(mPlayerProgramImages[buyingIndex],
                    mProgramImageRect, Color.White);
                pos.X = YourProgramsInfoContainerRect.X + mProgramImageRect.Width + 5;
                pos.Y = YourProgramsContainerRect.Y + containerYOffset + 2;
                sprite_batch.DrawString(PlayerUISpriteFont, "Move: " + mPlayer1InShop.GetPrograms().ElementAt(mPlayerSingleProgramSelectedIndex).Moves,
                    pos, Color.White);
                containerYOffset += 7 * yOffset / 10;
                pos.Y = YourProgramsContainerRect.Y + containerYOffset + 2;
                sprite_batch.DrawString(PlayerUISpriteFont, "Max Size: " + mPlayer1InShop.GetPrograms().ElementAt(mPlayerSingleProgramSelectedIndex).Size,
                    pos, Color.White);
                pos.X = YourProgramsInfoContainerRect.X + 5;
                containerYOffset += 7 * yOffset / 10;
                containerYOffset += 7 * yOffset / 10 / 2;
                pos.Y = YourProgramsContainerRect.Y + containerYOffset + 2;
                sprite_batch.DrawString(PlayerUISpriteFont, mPlayer1InShop.GetPrograms().ElementAt(mPlayerSingleProgramSelectedIndex).TypeName,
                    pos, Color.White);
                containerYOffset += 7 * yOffset / 10;
                containerYOffset += 7 * yOffset / 10;
                pos.Y = YourProgramsContainerRect.Y + containerYOffset + 2;
                sprite_batch.DrawString(PlayerUISpriteFont, "COMMANDS: ",
                    pos, Color.White);
                pos.X = YourProgramsInfoContainerRect.X + 20;
                containerYOffset += 7 * yOffset / 10;
                pos.Y = YourProgramsContainerRect.Y + containerYOffset + 2;
                for (int i = 0; i < mPlayer1InShop.GetPrograms().ElementAt(mPlayerSingleProgramSelectedIndex).Commands.Count(); i++)
                {
                    sprite_batch.DrawString(PlayerUISpriteFont, mPlayer1InShop.GetPrograms().ElementAt(mPlayerSingleProgramSelectedIndex).Commands.ElementAt(i).Name,
                        pos, Color.White);
                    containerYOffset += 7 * yOffset / 10;
                    pos.Y = YourProgramsContainerRect.Y + containerYOffset + 2;
                }
            }
            containerYOffset = 0;

            //displaying buyable programs
            sprite_batch.Draw(blankRectTexture, AvailProgramsContainerRect, Color.Black * .75f);
            sprite_batch.Draw(blankRectTexture, AvailProgramsTitleRect, Color.Black * .75f);
            sprite_batch.DrawString(YourProgramsSpriteFont, AvailProgramsString, AvailProgramsStringPos, Color.White);

            //highlighting program selected by user from his/her inventory
            for (int i = 0; i < mAvailProgramsSelectable.Length; i++)
            {
                if (mAvailSingleProgramSelectedIndex == i)
                {
                    sprite_batch.Draw(blankRectTexture, mAvailProgramsSelectable[i], Color.White * .25f);
                }
                else
                {
                    sprite_batch.Draw(blankRectTexture, mAvailProgramsSelectable[i], Color.Black * .25f);
                }
            }   

            //displaying available programs and silicoin amount
            containerYOffset = 0;
            for (int i = 0; i < mBuyablePrograms.Count(); i++)
            {
                pos.X = AvailProgramsContainerRect.X + 5; 
                pos.Y = AvailProgramsContainerRect.Y + containerYOffset;

                if (mAvailSingleProgramSelectedIndex == i)
                {
                    sprite_batch.DrawString(PlayerUISpriteFont, mBuyablePrograms[i].TypeName,
                        pos, Color.Black);

                    pos.X = AvailProgramsContainerRect.X + AvailProgramsContainerRect.Width - PlayerUISpriteFont.MeasureString(mBuyablePrograms[i].ProgramCost.ToString()).X - 5;

                    sprite_batch.DrawString(PlayerUISpriteFont, mBuyablePrograms[i].ProgramCost.ToString(),
                        pos, Color.Black);
                }
                else
                {
                    sprite_batch.DrawString(PlayerUISpriteFont, mBuyablePrograms[i].TypeName,
                        pos, Color.White);

                    pos.X = AvailProgramsContainerRect.X + AvailProgramsContainerRect.Width - PlayerUISpriteFont.MeasureString(mBuyablePrograms[i].ProgramCost.ToString()).X - 5;

                    sprite_batch.DrawString(PlayerUISpriteFont, mBuyablePrograms[i].ProgramCost.ToString(),
                        pos, Color.White);

                }
                containerYOffset += 7 * yOffset / 5;
            }
            containerYOffset = 0;

            //displaying info on selected buyable program
            sprite_batch.Draw(blankRectTexture, AvailProgramsInfoContainerRect, Color.Black * .75f);
            sprite_batch.Draw(blankRectTexture, AvailProgramsInfoTitleRect, Color.Black * .75f);
            sprite_batch.DrawString(YourProgramsSpriteFont, AvailProgramsInfoString, AvailProgramsInfoStringPos, Color.White);

            //displaying Program Info of selected players program
            containerYOffset = 0;
            if (mIsAnAvailProgramSelected)
            {
                sprite_batch.Draw(mPlayerProgramImages[mAvailSingleProgramSelectedIndex],
                    mAvailImageRect, Color.White);
                pos.X = AvailProgramsInfoContainerRect.X + mAvailImageRect.Width + 5;
                pos.Y = AvailProgramsContainerRect.Y + containerYOffset + 2;
                sprite_batch.DrawString(PlayerUISpriteFont, "Move: " + mBuyablePrograms.ElementAt(mAvailSingleProgramSelectedIndex).Moves,
                    pos, Color.White);
                containerYOffset += 7 * yOffset / 10;
                pos.Y = AvailProgramsContainerRect.Y + containerYOffset + 2;
                sprite_batch.DrawString(PlayerUISpriteFont, "Max Size: " + mBuyablePrograms.ElementAt(mAvailSingleProgramSelectedIndex).Size,
                    pos, Color.White);
                pos.X = AvailProgramsInfoContainerRect.X + 5;
                containerYOffset += 7 * yOffset / 10;
                containerYOffset += 7 * yOffset / 10 / 2;
                pos.Y = AvailProgramsContainerRect.Y + containerYOffset + 2;
                sprite_batch.DrawString(PlayerUISpriteFont, mBuyablePrograms.ElementAt(mAvailSingleProgramSelectedIndex).TypeName,
                    pos, Color.White);
                containerYOffset += 7 * yOffset / 10;
                containerYOffset += 7 * yOffset / 10;
                pos.Y = AvailProgramsContainerRect.Y + containerYOffset + 2;
                sprite_batch.DrawString(PlayerUISpriteFont, "COMMANDS: ",
                    pos, Color.White);
                pos.X = AvailProgramsInfoContainerRect.X + 20;
                containerYOffset += 7 * yOffset / 10;
                pos.Y = AvailProgramsContainerRect.Y + containerYOffset + 2;
                for (int i = 0; i < mBuyablePrograms.ElementAt(mAvailSingleProgramSelectedIndex).Commands.Count(); i++)
                {
                    sprite_batch.DrawString(PlayerUISpriteFont, mBuyablePrograms.ElementAt(mAvailSingleProgramSelectedIndex).Commands.ElementAt(i).Name,
                        pos, Color.White);
                    containerYOffset += 7 * yOffset / 10;
                    pos.Y = YourProgramsContainerRect.Y + containerYOffset + 2;
                }
            }
            containerYOffset = 0;

            //buy button
            if (isBuyButtonClicked)
            {
                sprite_batch.Draw(buttonPressed, BuyButtonRect, Color.White);
            }
            else
            {
                sprite_batch.Draw(buttonReleased, BuyButtonRect, Color.White);
            }
            sprite_batch.DrawString(PlayerUISpriteFont, BuyButtonString, BuyButtonStringPos + (Vector2.One * 2), Color.Black);
            sprite_batch.DrawString(PlayerUISpriteFont, BuyButtonString, BuyButtonStringPos, Color.White);

            //Draw Player UI (at bottom)
            sprite_batch.Draw(blankRectTexture, PlayerOptions, Color.Gray * .5f);
            sprite_batch.DrawString(PlayerUISpriteFont, mPlayer1InShop.Name, PlayerNamePos, Color.White);
            sprite_batch.DrawString(PlayerUISpriteFont, "Silicoins: " + mPlayer1InShop.TotalSilicoins, PlayerSilicoinPos, Color.White);
            
            if (isExitButtonClicked)
            {
                sprite_batch.Draw(buttonPressed, ExitButtonRect, Color.White);
            }
            else
            {
                sprite_batch.Draw(buttonReleased, ExitButtonRect, Color.White);
            }
            sprite_batch.DrawString(PlayerUISpriteFont, ExitButtonString, ExitButtonStringPos + (Vector2.One * 2), Color.Black);
            sprite_batch.DrawString(PlayerUISpriteFont, ExitButtonString, ExitButtonStringPos, Color.White);
        }
    }
}