using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit.GameEngine
{
    public abstract class GameEngine
    {
        private Stack<GameState> _current_states;
        public GameState CurrentState
        {
            get
            {
                return _current_states.Peek();
            }
        }
        public bool IsRunning { get; set; }
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
                foreach (GameState state in _current_states)
                {
                    state.Mediator = value;
                }
            }
        }

        public GameEngine(GameState InitialState=null)
        {
            _current_states = new Stack<GameState>();
            IsRunning = true;
            _mediator_manager = new MediatorManager(new DefaultSubscribableManager());
            if (InitialState != null)
            {
                InitialState.Mediator = Mediator;
                _current_states.Push(InitialState);
            }
        }

        public void Cleanup()
        {
            while (_current_states.Count != 0)
            {
                _current_states.Pop().Cleanup();
            }
        }

        public void ChangeState(GameState _state)
        {
            if (_current_states.Count != 0)
            {
                _current_states.Pop().Cleanup();
            }
            _current_states.Push(_state);
            _current_states.Peek().Init();
        }

        public void PushState(GameState _state)
        {
            if (_current_states.Count != 0)
            {
                _current_states.Peek().Pause();
            }
            _current_states.Push(_state);
            _current_states.Peek().Init();
        }

        public void PopState(GameState _state)
        {
            if (_current_states.Count != 0)
            {
                _current_states.Pop().Cleanup();
            }
            if (_current_states.Count != 0)
            {
                _current_states.Peek().Resume();
            }
        }

        public void RunLoop()
        {
            while (IsRunning)
            {
                _current_states.Peek().Update(this);
            }
        }
    }
}
