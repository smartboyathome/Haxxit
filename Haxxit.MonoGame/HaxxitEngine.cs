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
            PushState(new DefaultGameState());
        }

        /// <summary>
        /// A listener for notifications to change states sent through haxxit.engine.state.change.
        /// </summary>
        /// <param name="channel">The channel the notification is being sent through (haxxit.engine.state.change).</param>
        /// <param name="sender">The sender object of this notification.</param>
        /// <param name="args">The arguments for this notification (only takes ChangeStateEventArgs).</param>
        public void ChangeStateListener(string channel, object sender, EventArgs args)
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
        public void PushStateListener(string channel, object sender, EventArgs args)
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
        public void PopStateListener(string channel, object sender, EventArgs args)
        {
            PopState();
        }

        /// <summary>
        /// Removes all states currently on the stack then adds passed in state to the stack.
        /// </summary>
        /// <param name="state">The state to add to the stack after all other states are dropped.</param>
        public void ChangeState(HaxxitGameState state)
        {
            if (state_stack.Count != 0)
            {
                state_stack.Peek().Mediator = null;
                state_stack.Clear();
            }
            state_stack.Push(state);
            state_stack.Peek().Mediator = mediator;
            state_stack.Peek().Init();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);

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
            spriteBatch.Draw(test_texture, new Rectangle(10, 10, 10, 10), Color.Red);
            spriteBatch.End();*/

            state_stack.Peek().Draw(GraphicsDevice, spriteBatch);
        }
    }
}
