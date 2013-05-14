#region Using Statements
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
#endregion


namespace SmartboyDevelopments.Haxxit.MonoGame
{
    class ServerOverworldState : HaxxitGameState
    {
        int mWindowWidth;
        int mWindowHeight;

        int mMouseHoveringRectangleIndex;
        int mMouseClickedRectangleIndex;
        bool updateProgramInfo = false;
        bool updateShopInfo = false;

        bool mouseClicked = false;

        //For Displayings Player's name, Silicoins, and exit options
        Rectangle PlayerOptions;

        Color rectanglesGreenColor = Color.Green;
        Color rectanglesRedColor = Color.Red;
        Texture2D test_text;

        //Shop Prototype variables
        Player mPlayer1;
        SpriteBatch sprite_Batch;
        SpriteFont ArialFontSize14;
        SpriteFont ArialFontSize12;
        ContentManager content;

        String[] mPlayerProgramNames = new String[5];
        Texture2D[] mPlayerProgramImages = new Texture2D[5];
        Program[] mBuyablePrograms = new Program[5];
        Rectangle ProgramImageRect;

        #region Overworld Variables
        int columnWidth;
        int column1;
        int column2;
        int column3;
        int column4;
        int column5;
        int column6;
        int column7;
        int column8;

        Rectangle startNode;
        Rectangle tier2Node1;
        Rectangle tier2Node2;
        Rectangle tier3Node1;
        Rectangle tier3Node2;

        Rectangle connector1;
        Rectangle connector2;
        Rectangle connector3;
        Rectangle connector4;

        Rectangle[] nodeArray;
        bool[] isNodeHacked;
        bool[] isNodeVisible;

        #endregion



        public override void Init()
        {
            //mPlayer1.AddProgram
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            graphics.Clear(Color.CornflowerBlue);

            mWindowHeight = 480;
            mWindowWidth = 800;

            mPlayer1 = new Player("ELF Cadet");
            columnWidth = mWindowWidth / 8;
            column1 = 0;
            column2 = columnWidth;
            column3 = columnWidth * 2;
            column4 = columnWidth * 3;
            column5 = columnWidth * 4;
            column6 = columnWidth * 5;
            column7 = columnWidth * 6;


            int yPos = (mWindowHeight / 2) - (columnWidth / 2);
            startNode = new Rectangle(column2, yPos, columnWidth, columnWidth);

            int yPos2 = (mWindowHeight / 2) - (columnWidth * 3 / 2);
            tier2Node1 = new Rectangle(column4, yPos2, columnWidth, columnWidth);

            int yPos3 = (mWindowHeight / 2) + (columnWidth / 2);
            tier2Node2 = new Rectangle(column4, yPos3, columnWidth, columnWidth);

            int yPos4 = (mWindowHeight / 2) - (columnWidth / 2);
            tier3Node1 = new Rectangle(column6, yPos4, columnWidth, columnWidth);

            int yPos5 = (mWindowHeight / 2) + (columnWidth * 3 / 2) - 10;
            tier3Node2 = new Rectangle(column6, yPos5, columnWidth, columnWidth);

            connector1 = new Rectangle(3, 3, 3, 3);
            connector2 = new Rectangle(startNode.Center.X, startNode.Center.Y, tier2Node2.Center.X - startNode.Center.X, 10);

            PlayerOptions = new Rectangle(0, 0, mWindowWidth, 30);

            nodeArray = new Rectangle[5];
            nodeArray[0] = startNode;
            nodeArray[1] = tier2Node1;
            nodeArray[2] = tier2Node2;
            nodeArray[3] = tier3Node1;
            nodeArray[4] = tier3Node2;


            test_text = new Texture2D(graphics, 1, 1);
            test_text.SetData(new Color[] { Color.White });

            sprite_Batch = sprite_batch;

            ArialFontSize14 = content.Load<SpriteFont>("Arial");
            ArialFontSize12 = content.Load<SpriteFont>("Arial-12px-Regular");

            bool mouseClicked = false;

            isNodeHacked = new bool[5];
            for (int i = 0; i < 5; i++)
            {
                isNodeHacked[i] = false;
            }

            isNodeVisible = new bool[5];
            isNodeVisible[0] = true;
            for (int j = 1; j < 5; j++)
            {
                isNodeVisible[j] = false;
            }

        }

        public override void SubscribeAll()
        {
            //Mediator.Subscribe("mouse_click", mMouse_Click);
        }

        //public void mMouse_Click(string channel, object sender, EventArgs args) { }

        public override void Update()
        {
            MouseState mouse_state = Mouse.GetState(); // Gets the mouse state object
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y); // creates a point for the mouse's position

            if (mouseClicked)
            {
                if (mouse_state.LeftButton == ButtonState.Pressed)
                {
                    return;
                }
                else
                {
                    //mouseClicked = false;
                    ShopState new_state = new ShopState();
                    Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                }
            }

            //if (isExitButtonClicked)
            //{
            //    if (mouse_state.LeftButton == ButtonState.Released)
            //    {
            //        ServerOverworldState new_state = new ServerOverworldState();
            //        Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
            //    }
            //}

            for (int i = 0; i < 5; i++)
            {
                if (nodeArray[i].Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed && !mouseClicked && isNodeVisible[i] == true)
                {
                    //Node slection
                    mMouseClickedRectangleIndex = i;
                    isNodeHacked[i] = true;
                    updateProgramInfo = true;
                    mouseClicked = true;
                    return;
                }
            }

            
        }

        public override void Draw(SpriteBatch sprite_batch)
        {

            sprite_batch.Draw(test_text, new Vector2(PlayerOptions.X, PlayerOptions.Y), PlayerOptions, Color.Green);
            if (isNodeHacked[0] == false)
            {
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Cadet", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
            }

            if (isNodeHacked[0] == true)
            {
                sprite_batch.Draw(test_text, new Vector2(startNode.X, startNode.Y), startNode, Color.Red);
                isNodeVisible[1] = true;
                isNodeVisible[2] = true;
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Private", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
            }
            else
            {
                sprite_batch.Draw(test_text, new Vector2(startNode.X, startNode.Y), startNode, Color.Black);
            }

            if (isNodeVisible[1] == true && isNodeHacked[1] == false)
            {
                sprite_batch.Draw(test_text, new Vector2(tier2Node1.X, tier2Node1.Y), tier2Node1, Color.Black);
            }
            else if (isNodeVisible[1] == true && isNodeHacked[1] == true)
            {
                sprite_batch.Draw(test_text, new Vector2(tier2Node1.X, tier2Node1.Y), tier2Node1, Color.Red);
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Private", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
            }

            if (isNodeVisible[2] == true && isNodeHacked[2] == false)
            {
                sprite_batch.Draw(test_text, new Vector2(tier2Node2.X, tier2Node2.Y), tier2Node2, Color.Black);
            }
            else if (isNodeVisible[2] == true && isNodeHacked[2] == true)
            {
                sprite_batch.Draw(test_text, new Vector2(tier2Node2.X, tier2Node2.Y), tier2Node2, Color.Red);
                isNodeVisible[3] = true;
                isNodeVisible[4] = true;
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Sergeant", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
            }

            if (isNodeVisible[3] == true && isNodeHacked[3] == false)
            {
                sprite_batch.Draw(test_text, new Vector2(tier3Node1.X, tier3Node1.Y), tier3Node1, Color.Black);
            }
            else if (isNodeVisible[3] == true && isNodeHacked[3] == true)
            {
                sprite_batch.Draw(test_text, new Vector2(tier3Node1.X, tier3Node1.Y), tier3Node1, Color.Red);
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Captain", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
            }

            if (isNodeVisible[4] == true && isNodeHacked[4] == false)
            {
                sprite_batch.Draw(test_text, new Vector2(tier3Node2.X, tier3Node2.Y), tier3Node2, Color.Black);
            }
            else if (isNodeVisible[4] == true && isNodeHacked[4] == true)
            {
                sprite_batch.Draw(test_text, new Vector2(tier3Node2.X, tier3Node2.Y), tier3Node2, Color.Red);
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Captain", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
            }

            //sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Cadet", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);

            /*
            float angle = 15;
            Matrix rotate = Matrix.CreateRotationZ(angle);
            Vector2 original = new Vector2(connector2.X, connector2.Y);
            Vector2 temp = Vector2.Normalize(original) * 100.0f;
            Vector2 transformedPoint = Vector2.Transform(original, rotate);

            sprite_batch.Draw(test_text, transformedPoint, connector2, Color.Yellow);

            sprite_batch.Draw(test_text, temp, Color.Yellow);
             * */
        }

        /// <summary>
        /// For letting ServerOverworldState know the Game Window Bounds (i.e. height and width)
        /// </summary>
        /// <param name="window"></param>
        //public override void SaveGameWindowSize(GameWindow window)
        //{
        //    mWindowHeight = window.ClientBounds.Height;
        //    mWindowWidth = window.ClientBounds.Width;
        //}

    }
}
