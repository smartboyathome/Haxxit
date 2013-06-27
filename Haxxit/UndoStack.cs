using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit
{
    public class UndoEventArgs : EventArgs
    {
        public string Channel { get; private set; }
        public EventArgs Args { get; private set; }
        public UndoEventArgs(string channel, EventArgs args)
        {
            Channel = channel;
            Args = args;
        }
    }

    public class UndoStack
    {
        private Stack<UndoEventArgs> undo_stack;
        private int _max_size;
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

        public UndoStack(int max_size, IMediator mediator=null)
        {
            _max_size = max_size;
            undo_stack = new Stack<UndoEventArgs>();
            DefaultSubscribableManager subscribable_manager = new DefaultSubscribableManager();
            subscribable_manager.OnSubscribe += SubscribeAll;
            _mediator_manager = new MediatorManager(subscribable_manager);
            _mediator_manager.Mediator = mediator;
        }

        public void SubscribeAll()
        {
            _mediator_manager.Subscribe("haxxit.undo_stack.push", PushUndoListener);
            _mediator_manager.Subscribe("haxxit.undo_stack.trigger", TriggerUndoListener);
            _mediator_manager.Subscribe("haxxit.undo_stack.clear", ClearUndoListener);
        }

        public void PushUndoListener(INotifiable notifiable, string channel, object sender, EventArgs args)
        {
            if (undo_stack.Count >= _max_size)
                undo_stack.Pop();
            undo_stack.Push((UndoEventArgs)args);
        }

        public void TriggerUndoListener(INotifiable notifiable, string channel, object sender, EventArgs args)
        {
            if (undo_stack.Count != 0)
            {
                UndoEventArgs undo = undo_stack.Pop();
                _mediator_manager.Notify(undo.Channel, this, undo.Args);
            }
        }

        public void ClearUndoListener(INotifiable notifiable, string channel, object sender, EventArgs args)
        {
            undo_stack.Clear();
        }
    }
}
