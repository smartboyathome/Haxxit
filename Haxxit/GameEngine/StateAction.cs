using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit.GameEngine
{
    public abstract class StateAction
    {
        protected NotifiableManager _notifiable_manager;
        public INotifiable Notifiable
        {
            get
            {
                return _notifiable_manager.Notifiable;
            }
            set
            {
                _notifiable_manager.Notifiable = value;
            }
        }
        public abstract void Execute(GameEngine engine);
    }

    public class QuitGameState : StateAction
    {
        public override void Execute(GameEngine engine)
        {
            engine.IsRunning = false;
        }
    }
}
