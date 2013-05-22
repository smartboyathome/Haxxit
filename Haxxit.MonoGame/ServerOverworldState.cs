#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using SmartboyDevelopments.SimplePubSub;
using HaxxitCom = SmartboyDevelopments.Haxxit.Commands;
using HaxxitProg = SmartboyDevelopments.Haxxit.Programs;
using HaxxitMap = SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.MonoGame.Programs;
using SmartboyDevelopments.Haxxit.MonoGame.Maps;
using SmartboyDevelopments.Haxxit.MonoGame.GameStates;
#endregion


namespace SmartboyDevelopments.Haxxit.MonoGame
{
    class ServerOverworldState : HaxxitGameState
    {
        int mWindowWidth;
        int mWindowHeight;

        Player mPlayer1InOverWorld;

        int mMouseClickedRectangleIndex;

        bool mouseClicked = false;

        //For Displayings Player's name, Silicoins, and exit options
        Rectangle PlayerOptions;

        Texture2D rectTexture, backgroundTexture, availableTexture, availableTextureShadow,
            hackedTexture, hackedTextureShadow, unAvailableTexture, unAvailableTextureShadow,
            shopTexture, shopTextureShadow, serverIconTexture;
        Rectangle backgroundRect;

        Color rectanglesGreenColor = Color.Green;
        Color rectanglesRedColor = Color.Red;
        Texture2D test_text;

        SpriteBatch sprite_Batch;
        SpriteFont ArialFontSize14;
        SpriteFont ArialFontSize12;

        String[] mPlayerProgramNames = new String[5];
        Texture2D[] mPlayerProgramImages = new Texture2D[5];

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
        Rectangle startNodeShadow;
        Rectangle tier2Node1;
        Rectangle tier2Node1Shadow;
        Rectangle tier2Node2;
        Rectangle tier2Node2Shadow;
        Rectangle tier3Node1;
        Rectangle tier3Node1Shadow;
        Rectangle tier3Node2;

        Rectangle connector1;
        Rectangle connector2;
        Rectangle connector3;
        Rectangle connector4;

        Rectangle[] nodeArray;
        bool[] isNodeHacked;
        bool[] isNodeClickable;

        bool firstLevel;
        bool secondLevel;
        bool thirdLevel;
        bool shop;

        #endregion



        public override void Init()
        {
            
            mPlayer1InOverWorld = GlobalAccessors.mPlayer1;
            firstLevel = false;
            secondLevel = false;
            thirdLevel = false;
            shop = false;
            mWindowWidth = GlobalAccessors.mGame.Window.ClientBounds.Width; //use to be 640
            mWindowHeight = GlobalAccessors.mGame.Window.ClientBounds.Height;
            backgroundRect = new Rectangle(0, 0, mWindowWidth, mWindowHeight);

        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            graphics.Clear(Color.Black);

            rectTexture = new Texture2D(graphics, 1, 1);
            rectTexture.SetData(new Color[] { Color.White });

            //File: Ogd1 layers2 License: GPLv2 or later <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html> 
            //Author Original: Traversal Technology Author Ferivative: Sreejith K <http://commons.wikimedia.org/wiki/User:Sreejithk2000>
            //backgroundTexture = content.Load<Texture2D>("Ogd1_layers2");

            backgroundTexture = content.Load<Texture2D>("OverworldBackground");
            serverIconTexture = content.Load<Texture2D>("ServerIcon");
            //------------------------------------------------------------------------------------
            columnWidth = mWindowWidth / 8;
            column1 = 0;
            column2 = columnWidth;
            column3 = columnWidth * 2;
            column4 = columnWidth * 3;
            column5 = columnWidth * 4;
            column6 = columnWidth * 5;
            column7 = columnWidth * 6;

            availableTexture = CreateBG(graphics, columnWidth, columnWidth, "blue");
            availableTextureShadow = CreateBG(graphics, columnWidth, columnWidth, "darkblue");
            hackedTexture = CreateBG(graphics, columnWidth, columnWidth, "yellow");
            hackedTextureShadow = CreateBG(graphics, columnWidth, columnWidth, "darkyellow");
            unAvailableTexture = CreateBG(graphics, columnWidth, columnWidth, "red");
            unAvailableTextureShadow = CreateBG(graphics, columnWidth, columnWidth, "darkred");
            shopTexture = CreateBG(graphics, columnWidth, columnWidth, "purple");
            shopTextureShadow = CreateBG(graphics, columnWidth, columnWidth, "darkpurple");



            int yPos = (mWindowHeight / 2) - (columnWidth / 2);
            startNode = new Rectangle(column2, yPos, columnWidth, columnWidth);
            startNodeShadow = new Rectangle(column2 + 5, yPos + 5, columnWidth, columnWidth);

            int yPos2 = (mWindowHeight / 2) - (columnWidth * 3 / 2);
            tier2Node1 = new Rectangle(column4, yPos2, columnWidth, columnWidth);
            tier2Node1Shadow = new Rectangle(column4 + 5, yPos2 + 5, columnWidth, columnWidth);

            int yPos3 = (mWindowHeight / 2) + (columnWidth / 2);
            tier2Node2 = new Rectangle(column4, yPos3, columnWidth, columnWidth);
            tier2Node2Shadow = new Rectangle(column4 + 5, yPos3 + 5, columnWidth, columnWidth);

            int yPos4 = (mWindowHeight / 2) - (columnWidth / 2);
            tier3Node1 = new Rectangle(column6, yPos4, columnWidth, columnWidth);
            tier3Node1Shadow = new Rectangle(column6 + 5, yPos4 + 5, columnWidth, columnWidth);

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

            ArialFontSize14 = content.Load<SpriteFont>("Arial-16px-Regular");
            ArialFontSize12 = content.Load<SpriteFont>("Arial-12px-Regular");

            isNodeHacked = new bool[5];
            for (int i = 0; i < 5; i++)
            {
                isNodeHacked[i] = false;
            }

            isNodeClickable = new bool[5];
            isNodeClickable[0] = true;
            for (int j = 1; j < 5; j++)
            {
                isNodeClickable[j] = false;
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
                else if (firstLevel == true)
                {
                    mPlayer1InOverWorld.CurrentNode = "Node1";
                    MapSpawnGameState new_state = new MapSpawnGameState((new FirstMapFactory()).NewInstance());
                    Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
                }
                else if (secondLevel == true)
                {
                    mPlayer1InOverWorld.CurrentNode = "Node2";
                    MapSpawnGameState new_state = new MapSpawnGameState((new SecondMapFactory()).NewInstance());
                    Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
                }
                else if (thirdLevel == true)
                {
                    mPlayer1InOverWorld.CurrentNode = "Node3";
                    MapSpawnGameState new_state = new MapSpawnGameState((new ThirdMapFactory()).NewInstance());
                    Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
                }
                else if (shop == true)
                {
                    ShopState new_state = new ShopState();
                    Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
                }
                else
                {
                    mouseClicked = false;
                    //PushState(new MapSpawnGameState((new SpawnMapFactory()).NewInstance()));
                    //ShopState new_state = new ShopState();
                    //MapSpawnGameState new_state = new MapSpawnGameState((new SpawnMapFactory()).NewInstance());
                    //Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
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
                if (nodeArray[i].Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed && !mouseClicked && isNodeClickable[i] == true)
                {
                    //Node slection
                    mMouseClickedRectangleIndex = i;
                    //isNodeHacked[i] = true;
                    mouseClicked = true;

                    if (i == 0 && mPlayer1InOverWorld.IsNodeHacked("Node1") == false)
                    {
                        firstLevel = true;
                    }
                    else if (i == 1)
                    {
                        shop = true;
                    }
                    else if (i == 2)
                    {
                        secondLevel = true;
                    }
                    else if (i == 3)
                    {
                        thirdLevel = true;
                    }

                    return;
                }
            }

            
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(backgroundTexture, backgroundRect, Color.White);
            sprite_batch.Draw(test_text, new Vector2(PlayerOptions.X, PlayerOptions.Y), PlayerOptions, Color.Green);

            //REDO OF LOGIC TO ENHANCE DRAW PERFORMANCE
            if (mPlayer1InOverWorld.IsNodeHacked("Node5") == true)
            {

            }
            else if (mPlayer1InOverWorld.IsNodeHacked("Node4") == true)
            {

            }
            else if (mPlayer1InOverWorld.IsNodeHacked("Node3") == true)
            {
                //Needs to be done for beta
            }
            else if (mPlayer1InOverWorld.IsNodeHacked("Node2") == true)
            {
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Corporal", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
                sprite_batch.Draw(hackedTextureShadow, startNodeShadow, Color.White);
                sprite_batch.Draw(shopTextureShadow, tier2Node1Shadow, Color.White);
                sprite_batch.Draw(hackedTextureShadow, tier2Node2Shadow, Color.White);
                sprite_batch.Draw(availableTextureShadow, tier3Node1Shadow, Color.White);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(startNode.Center.X, startNode.Center.Y),
                    new Vector2(tier2Node1.Center.X, tier2Node1.Center.Y), Color.Violet, 10);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(startNode.Center.X, startNode.Center.Y),
                    new Vector2(tier2Node2.Center.X, tier2Node2.Center.Y), Color.Violet, 10);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(tier2Node2.Center.X, tier2Node2.Center.Y),
                    new Vector2(tier3Node1.Center.X, tier3Node1.Center.Y), Color.Violet, 10);
                sprite_batch.Draw(hackedTexture, startNode, Color.White);
                sprite_batch.Draw(shopTexture, tier2Node1, Color.White);
                sprite_batch.Draw(hackedTexture, tier2Node2, Color.White);
                sprite_batch.Draw(availableTexture, tier3Node1, Color.White);
                //sprite_batch.Draw(serverIconTexture, tier3Node1, Color.White);
                isNodeClickable[0] = true;
                isNodeClickable[1] = true;
                isNodeClickable[2] = true;
                isNodeClickable[3] = true;
            }
            else if (mPlayer1InOverWorld.IsNodeHacked("Node1") == true)
            {
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Private", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
                sprite_batch.Draw(hackedTextureShadow, startNodeShadow, Color.White);
                sprite_batch.Draw(shopTextureShadow, tier2Node1Shadow, Color.White);
                sprite_batch.Draw(availableTextureShadow, tier2Node2Shadow, Color.White);
                sprite_batch.Draw(unAvailableTextureShadow, tier3Node1Shadow, Color.White);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(startNode.Center.X, startNode.Center.Y),
                    new Vector2(tier2Node1.Center.X, tier2Node1.Center.Y), Color.Violet, 10);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(startNode.Center.X, startNode.Center.Y),
                    new Vector2(tier2Node2.Center.X, tier2Node2.Center.Y), Color.Violet, 10);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(tier2Node2.Center.X, tier2Node2.Center.Y),
                    new Vector2(tier3Node1.Center.X, tier3Node1.Center.Y), Color.Violet, 10);
                sprite_batch.Draw(hackedTexture, startNode, Color.White);
                sprite_batch.Draw(shopTexture, tier2Node1, Color.White);
                sprite_batch.Draw(availableTexture, tier2Node2, Color.White);
                sprite_batch.Draw(unAvailableTexture, tier3Node1, Color.White);
                isNodeClickable[0] = true;
                isNodeClickable[1] = true;
                isNodeClickable[2] = true;
                
            }
            else
            {
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Cadet", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
                sprite_batch.Draw(availableTextureShadow, startNodeShadow, Color.White);
                sprite_batch.Draw(unAvailableTextureShadow, tier2Node2Shadow, Color.White);
                sprite_batch.Draw(unAvailableTextureShadow, tier2Node1Shadow, Color.White);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(startNode.Center.X, startNode.Center.Y),
                    new Vector2(tier2Node1.Center.X, tier2Node1.Center.Y), Color.Violet, 10);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(startNode.Center.X, startNode.Center.Y),
                    new Vector2(tier2Node2.Center.X, tier2Node2.Center.Y), Color.Violet, 10);
                sprite_batch.Draw(availableTexture, startNode, Color.White);
                sprite_batch.Draw(unAvailableTexture, tier2Node1, Color.White);
                sprite_batch.Draw(unAvailableTexture, tier2Node2, Color.White);
                isNodeClickable[0] = true;
            }
            //-----------------------------------------------------------------------------------------------
            /*
            if (mPlayer1InOverWorld.IsNodeHacked("Node1") == false)
            {
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Cadet", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
            }

            if (mPlayer1InOverWorld.IsNodeHacked("Node1") == true)
            {
                sprite_batch.Draw(hackedTextureShadow, startNodeShadow, Color.White);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(startNode.Center.X, startNode.Center.Y),
                    new Vector2(tier2Node1.Center.X, tier2Node1.Center.Y), Color.Violet, 10);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(startNode.Center.X, startNode.Center.Y),
                    new Vector2(tier2Node2.Center.X, tier2Node2.Center.Y), Color.Violet, 10);
                sprite_batch.Draw(hackedTexture, startNode, Color.White);
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Private", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
                isNodeClickable[1] = true;
                isNodeClickable[2] = true;
            }
            else
            {
                sprite_batch.Draw(availableTextureShadow, startNodeShadow, Color.White);
                sprite_batch.Draw(unAvailableTextureShadow, tier2Node2Shadow, Color.White);
                sprite_batch.Draw(unAvailableTextureShadow, tier2Node1Shadow, Color.White);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(startNode.Center.X, startNode.Center.Y),
                    new Vector2(tier2Node1.Center.X, tier2Node1.Center.Y), Color.Violet, 10);
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(startNode.Center.X, startNode.Center.Y),
                    new Vector2(tier2Node2.Center.X, tier2Node2.Center.Y), Color.Violet, 10);
                sprite_batch.Draw(availableTexture, startNode, Color.White);
                sprite_batch.Draw(unAvailableTexture, tier2Node1, Color.White);
                sprite_batch.Draw(unAvailableTexture, tier2Node2, Color.White);
            }

            if (isNodeClickable[1] == true && isNodeHacked[1] == false)
            {
                sprite_batch.Draw(shopTextureShadow, tier2Node1Shadow, Color.White);
                sprite_batch.Draw(shopTexture, tier2Node1, Color.White);
                //PrimiviteDrawing.DrawCircle(test_text, sprite_batch, new Vector2(tier2Node1.Center.X, tier2Node1.Center.Y), 50, Color.White, 2, 16);
            }
            else if (isNodeClickable[1] == true && isNodeHacked[1] == true)
            {
                //sprite_batch.Draw(test_text, new Vector2(tier2Node1.X, tier2Node1.Y), tier2Node1, Color.Red);
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Private", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
            }

            if (isNodeClickable[2] == true && mPlayer1InOverWorld.IsNodeHacked("Node2") == false)
            {
                PrimiviteDrawing.DrawLineSegment(test_text, sprite_batch, new Vector2(tier2Node2.Center.X, tier2Node2.Center.Y),
                   new Vector2(tier3Node1.Center.X, tier3Node1.Center.Y), Color.Violet, 10);
                sprite_batch.Draw(availableTextureShadow, tier3Node1Shadow, Color.White);
                sprite_batch.Draw(availableTexture, tier3Node1, Color.White);
                sprite_batch.Draw(availableTextureShadow, tier2Node2Shadow, Color.White);
                sprite_batch.Draw(availableTexture, tier2Node2, Color.White);
            }
            else if (isNodeClickable[2] == true && mPlayer1InOverWorld.IsNodeHacked("Node2") == true)
            {
                sprite_batch.Draw(test_text, new Vector2(tier2Node2.X, tier2Node2.Y), tier2Node2, Color.Red);
                isNodeClickable[3] = true;
                //isNodeClickable[4] = true;
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Sergeant", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
            }
            */

            /*
            if (isNodeClickable[3] == true && isNodeHacked[3] == false)
            {
                sprite_batch.Draw(test_text, new Vector2(tier3Node1.X, tier3Node1.Y), tier3Node1, Color.Black);
            }
            else if (isNodeClickable[3] == true && isNodeHacked[3] == true)
            {
                sprite_batch.Draw(test_text, new Vector2(tier3Node1.X, tier3Node1.Y), tier3Node1, Color.Red);
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Captain", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
            }

            if (isNodeClickable[4] == true && isNodeHacked[4] == false)
            {
                sprite_batch.Draw(test_text, new Vector2(tier3Node2.X, tier3Node2.Y), tier3Node2, Color.Black);
            }
            else if (isNodeClickable[4] == true && isNodeHacked[4] == true)
            {
                sprite_batch.Draw(test_text, new Vector2(tier3Node2.X, tier3Node2.Y), tier3Node2, Color.Red);
                sprite_batch.DrawString(ArialFontSize12, "Rank: ELF Captain", new Vector2(PlayerOptions.X, PlayerOptions.Y + 5), Color.White);
            }
             * */

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

        //---------------------------------------------------------------------------------------------
        //Code for gradient textures used from:
        //https://mattryder.wordpress.com/2011/02/02/xna-creating-a-gradient-styled-texture2d/
        /// <summary>
        /// Creates a pretty cool gradient texture!
        /// Used for a background Texture!
        /// </summary>
        /// <param name="width">The width of the current viewport</param>
        /// <param name="height">The height of the current viewport</param>
        /// A Texture2D with a gradient applied.
        private Texture2D CreateBG(GraphicsDevice graphics, int width, int height, string request)
        {
            Texture2D backgroundTex = new Texture2D(graphics, width, height);
            Color[] bgc = new Color[height * width];
            int texColour = 0;          // Defines the colour of the gradient.
            int gradientThickness = 2;  // Defines how "diluted" the gradient gets. I've found 2 works great, and 16 is a very fine gradient.

            for (int i = 0; i < bgc.Length; i++)
            {
                texColour = (i / (height / gradientThickness));

                if (request == "blue")
                {
                    bgc[i] = new Color(0, 100, texColour +200); 
                }
                else if (request == "darkblue")
                {
                    bgc[i] = new Color(0, 0, texColour + 100); 
                }
                else if (request == "yellow")
                {
                    bgc[i] = new Color(texColour + 150, texColour + 150, 0); 
                }
                else if (request == "darkyellow")
                {
                    bgc[i] = new Color(texColour + 25, texColour + 25, 0); 
                }
                else if (request == "red")
                {
                    bgc[i] = new Color(texColour + 150, 0, 0); 
                }
                else if (request == "darkred")
                {
                    bgc[i] = new Color(texColour + 50, 0, 0); 
                }
                else if (request == "purple")
                {
                    bgc[i] = new Color(texColour + 175, 0, texColour + 175); 
                }
                else if (request == "darkpurple")
                {
                    bgc[i] = new Color(texColour + 75, 0, texColour + 75); 
                }
            }
            backgroundTex.SetData(bgc);
            return backgroundTex;
        }
        //-------------------------------------------------------------------------------------------

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
