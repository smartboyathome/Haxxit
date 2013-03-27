using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit.GameEngine
{
    public abstract class GameState
    {
        protected MediatorManager _mediator_manager;
        public IMediator Mediator
        {
            get
            {
                return _mediator_manager.Mediator;
            }
            set
            {
                BeforeStateChange();
                _mediator_manager.Mediator = value;
                AfterStateChange();
            }
        }

        public abstract void Init();
        public abstract void Cleanup();
        public virtual void BeforeStateChange() { }
        public virtual void AfterStateChange() { }

        // Useful for bringing up menu screens
        public virtual void Pause() { }
        public virtual void Resume() { }

        // The methods that must be implemented
        protected abstract IEnumerable<StateAction> RetrieveEvents();
        protected abstract void Redraw(GameEngine engine);

        protected virtual void HandleActions(GameEngine engine, IEnumerable<StateAction> actions)
        {
            if (actions == null || actions.Count() == 0)
                return;
            foreach (StateAction action in actions)
            {
                if(action != null)
                    action.Execute(engine);
            }
        }
        public virtual void Update(GameEngine engine)
        {
            HandleActions(engine, RetrieveEvents());
            Redraw(engine);
        }
    }
}
