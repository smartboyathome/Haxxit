using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    public abstract class HaxxitGameState
    {
        private MediatorManager _mediator_manager;
        public IMediator Mediator
        {
            get
            {
                return _mediator_manager.Mediator;
            }
            set
            {
                _mediator_manager.Mediator = value;
            }
        }

        public HaxxitGameState()
        {
            DefaultSubscribableManager subscribable_manager = new DefaultSubscribableManager();
            subscribable_manager.OnSubscribe += SubscribeAll;
            _mediator_manager = new MediatorManager(subscribable_manager);
        }

        public abstract void Init();
        public abstract void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch);
        public abstract void SubscribeAll();
        public abstract void Update();
        public abstract void Draw(GraphicsDevice graphics, SpriteBatch sprite_batch);
    }
}
