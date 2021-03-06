﻿#region Using Statements
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
    /// <summary>
    /// The EventArgs subclass for pushing or changing a state (used with
    /// haxxit.engine.state.change and haxxit.engine.state.push).
    /// </summary>
    public class ChangeStateEventArgs : EventArgs
    {
        /// <summary>
        /// The game state to be pushed onto the stack.
        /// </summary>
        public HaxxitGameState State
        {
            get;
            private set;
        }

        public ChangeStateEventArgs(HaxxitGameState state)
        {
            State = state;
        }
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class HaxxitEngine : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Stack<HaxxitGameState> state_stack;
        IMediator mediator;

        /*public HaxxitMap.Map GenerateMap()
        {
            List<HaxxitCom.Command> commands = new List<HaxxitCom.Command>();
            commands.Add(new HaxxitCom.DamageCommand("Pong", "Removes 3 cells from target", 3, 1));
            commands.Add(new HaxxitCom.DamageCommand("Ping", "Removes 2 cells from target", 2, 2));
            HaxxitTest.DynamicProgramFactory program_factory = new HaxxitTest.DynamicProgramFactory(4, 4, commands);
            List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>> player1_spawns =
                new List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>>();
            List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>> player2_spawns =
                new List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>>();
            player1_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(0, 1), program_factory));
            player1_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(1, 0), program_factory));
            player2_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(9, 8), program_factory));
            player2_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(8, 9), program_factory));
            return (new HaxxitTest.WinnableEnemyMapFactory(10, 10, player1_spawns, player2_spawns)).NewInstance();
        }

        public HaxxitMap.Map GenerateTinyMap()
        {
            List<HaxxitCom.Command> commands = new List<HaxxitCom.Command>();
            commands.Add(new HaxxitTest.DynamicDamageCommand(3, 1, "Pong"));
            commands.Add(new HaxxitTest.DynamicDamageCommand(2, 2, "Ping"));
            HaxxitTest.DynamicProgramFactory program_factory = new HaxxitTest.DynamicProgramFactory(4, 4, commands);
            List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>> player1_spawns =
                new List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>>();
            List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>> player2_spawns =
                new List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>>();
            player1_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(0, 0), program_factory));
            player1_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(1, 0), program_factory));
            player2_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(1, 1), program_factory));
            return (new HaxxitTest.PlayerMapFactory(2, 2, player1_spawns, player2_spawns)).NewInstance();
        }

        public HaxxitMap.Map GenerateTinyEnemyMap()
        {
            List<HaxxitCom.Command> commands = new List<HaxxitCom.Command>();
            commands.Add(new HaxxitTest.DynamicDamageCommand(3, 1, "Pong"));
            commands.Add(new HaxxitTest.DynamicDamageCommand(2, 2, "Ping"));
            HaxxitTest.DynamicProgramFactory program_factory = new HaxxitTest.DynamicProgramFactory(4, 4, commands);
            List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>> player1_spawns =
                new List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>>();
            List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>> player2_spawns =
                new List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>>();
            player1_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(0, 0), program_factory));
            player2_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(1, 0), program_factory));
            return (new HaxxitTest.WinnableEnemyMapFactory(2, 2, player1_spawns, player2_spawns)).NewInstance();
        }

        public HaxxitMap.Map GenerateAwesomeMap()
        {
            List<HaxxitCom.Command> commands = new List<HaxxitCom.Command>();
            commands.Add(new HaxxitTest.DynamicDamageCommand(3, 1, "Pong"));
            commands.Add(new HaxxitTest.DynamicDamageCommand(2, 2, "Ping"));
            HaxxitTest.DynamicProgramFactory program_factory = new HaxxitTest.DynamicProgramFactory(4, 4, commands);
            List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>> player1_spawns =
                new List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>>();
            List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>> player2_spawns =
                new List<Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>>();
            
            player1_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(0, 2), program_factory));
            player1_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(0, 3), program_factory));
            player1_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(0, 6), program_factory));
            player1_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(0, 7), program_factory));
            player2_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(19, 1), program_factory));
            player2_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(19, 2), program_factory));
            player2_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(19, 4), program_factory));
            player2_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(19, 5), program_factory));
            player2_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(19, 7), program_factory));
            player2_spawns.Add(new Tuple<HaxxitMap.Point, HaxxitProg.ProgramFactory>(new HaxxitMap.Point(19, 8), program_factory));
            Haxxit.Maps.Map Map = (new Haxxit.MonoGame.WinnableEnemyMapFactory(20, 10, player1_spawns, player2_spawns)).NewInstance();
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(3, 3));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(3, 4));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(3, 5));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(3, 6));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(7, 2));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(6, 1));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(5, 0));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(5, 9));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(6, 8));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(7, 7));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(9, 4));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(9, 5));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(10, 4));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(10, 5));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(16, 3));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(16, 4));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(16, 5));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(16, 6));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(12, 2));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(13, 1));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(14, 0));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(12, 7));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(13, 8));
            Map.CreateNode(new Haxxit.Maps.UnavailableNodeFactory(), new Haxxit.Maps.Point(14, 9));
            return Map;
        }*/

        public HaxxitEngine()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            state_stack = new Stack<HaxxitGameState>();
            mediator = new SynchronousMediator();
            mediator.Subscribe("haxxit.engine.state.change", ChangeStateListener);
            mediator.Subscribe("haxxit.engine.state.push", PushStateListener);
            mediator.Subscribe("haxxit.engine.state.pop", PopStateListener);
            mediator.Subscribe("haxxit.engine.state.clear_all", ClearAllStateListener);
            mediator.Subscribe("haxxit.engine.state.clear_change", ClearChangeStateListener);

            GlobalAccessors.mGame = this;
            GlobalAccessors.mPlayer1 = new Player("Bob");
            GlobalAccessors.mPlayer1.AddSilicoins(1400);

            PushState(new IntroductionState());
            //PushState(new MapSpawnGameState((new SpawnMapFactory()).NewInstance()));
            //PushState(new UserMapGameState(GenerateTinyMap())); // For 2x2 testing
            //PushState(new UserMapGameState(GenerateTinyEnemyMap())); // For 2x2 testing with enemy
            //PushState(new UserMapGameState(GenerateAwesomeMap()));
        }

        /// <summary>
        /// A listener for notifications to change states sent through haxxit.engine.state.change.
        /// </summary>
        /// <param name="channel">The channel the notification is being sent through (haxxit.engine.state.change).</param>
        /// <param name="sender">The sender object of this notification.</param>
        /// <param name="args">The arguments for this notification (only takes ChangeStateEventArgs).</param>
        public void ChangeStateListener(SimplePubSub.INotifiable notifiable, string channel, object sender, EventArgs args)
        {
            ChangeStateEventArgs event_args = (ChangeStateEventArgs)args;
            ChangeState(event_args.State);
        }

        /// <summary>
        /// A listener for notifications to push states sent through haxxit.engine.state.push.
        /// </summary>
        /// <param name="channel">The channel the notification is being sent through (haxxit.engine.state.push).</param>
        /// <param name="sender">The sender object of this notification.</param>
        /// <param name="args">The arguments for this notification (only takes ChangeStateEventArgs).</param>
        public void PushStateListener(SimplePubSub.INotifiable notifiable, string channel, object sender, EventArgs args)
        {
            ChangeStateEventArgs event_args = (ChangeStateEventArgs)args;
            PushState(event_args.State);
        }

        /// <summary>
        /// A listener for notifications to pop states sent through haxxit.engine.state.pop.
        /// </summary>
        /// <param name="channel">The channel the notification is being sent through (haxxit.engine.state.pop).</param>
        /// <param name="sender">The sender object of this notification.</param>
        /// <param name="args">The arguments for this notification (requires no arguments).</param>
        public void PopStateListener(SimplePubSub.INotifiable notifiable, string channel, object sender, EventArgs args)
        {
            PopState();
        }

        /// <summary>
        /// A listener for notifications to clear the state stack sent through haxxit.engine.state.clear_all
        /// </summary>
        /// <param name="channel">The channel the notification is being sent through (haxxit.engine.state.clear_all).</param>
        /// <param name="sender">The sender object of this notification.</param>
        /// <param name="args">The arguments for this notification (requires no arguments).</param>
        public void ClearAllStateListener(SimplePubSub.INotifiable notifiable, string channel, object sender, EventArgs args)
        {
            ClearStates();
        }

        /// <summary>
        /// A listener for notifications to clear the state stack then push a new state onto it, sent through haxxit.engine.state.clear_change.
        /// </summary>
        /// <param name="channel">The channel the notification is being sent through (haxxit.engine.state.clear_change).</param>
        /// <param name="sender">The sender object of this notification.</param>
        /// <param name="args">The arguments for this notification (only takes ChangeStateEventArgs).</param>
        public void ClearChangeStateListener(SimplePubSub.INotifiable notifiable, string channel, object sender, EventArgs args)
        {
            ChangeStateEventArgs event_args = (ChangeStateEventArgs)args;
            ClearStates();
            PushState(event_args.State);
        }

        /// <summary>
        /// Removes all states currently on the stack then adds passed in state to the stack.
        /// </summary>
        /// <param name="state">The state to add to the stack after all other states are dropped.</param>
        public void ChangeState(HaxxitGameState state)
        {
            if (state_stack.Count != 0)
            {
                state_stack.Pop().Mediator = null;
            }
            state_stack.Push(state);
            state_stack.Peek().Mediator = mediator;
            state_stack.Peek().Init();
            state_stack.Peek().LoadContent(GraphicsDevice, spriteBatch, Content);
        }

        /// <summary>
        /// Pushes a state onto the stack.
        /// </summary>
        /// <param name="state">The state to push onto the top of the stack.</param>
        public void PushState(HaxxitGameState state)
        {
            if(state_stack.Count != 0)
                state_stack.Peek().Mediator = null;
            state_stack.Push(state);
            state.Mediator = mediator;
            state.Init();
            state_stack.Peek().LoadContent(GraphicsDevice, spriteBatch, Content);
        }

        /// <summary>
        /// Pops a state from the stack.
        /// </summary>
        public void PopState()
        {
            if (state_stack.Count != 0)
            {
                state_stack.Peek().Mediator = null;
                state_stack.Pop();
                if(state_stack.Count != 0)
                    state_stack.Peek().Mediator = mediator;
            }
        }

        /// <summary>
        /// Clears the state stack.
        /// </summary>
        public void ClearStates()
        {
            if(state_stack.Count != 0)
            {
                state_stack.Peek().Mediator = null;
            }
            state_stack.Clear();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            foreach (HaxxitGameState state in state_stack)
            {
                state.Init();
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            foreach (HaxxitGameState state in state_stack)
            {
                state.LoadContent(GraphicsDevice, spriteBatch, Content);
            }
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GlobalAccessors.mGameTime = gameTime;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);

            if(state_stack.Count > 0)
                state_stack.Peek().Update();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            /*spriteBatch.Begin();
            spriteBatch.Draw(rectangle_texture, new Rectangle(10, 10, 10, 10), Color.Red);
            spriteBatch.End();*/

            if (state_stack.Count > 0)
            {
                spriteBatch.Begin();
                state_stack.Peek().Draw(spriteBatch);
                spriteBatch.End();
            }
        }
    }
}
