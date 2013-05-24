using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.SimplePubSub
{
    public abstract class SubscribableManager : ISubscribable
    {
        private ISubscribable _subscribable;

        public ISubscribable Subscribable
        {
            get
            {
                return _subscribable;
            }
            set
            {
                if (_subscribable != null)
                {
                    UnsubscribeAll();
                }
                _subscribable = value;
                if (_subscribable != null)
                {
                    SubscribeAll();
                }
            }
        }

        public abstract void UnsubscribeAll();
        public abstract void SubscribeAll();

        public virtual bool Subscribe(string channel, Action<string, object, EventArgs> del)
        {
            if (_subscribable != null)
                return _subscribable.Subscribe(channel, del);
            return false;
        }

        public virtual void PatternSubscribe(string channel_pattern, Action<string, object, EventArgs> del)
        {
            if (_subscribable != null)
                _subscribable.PatternSubscribe(channel_pattern, del);
        }

        public virtual bool Unsubscribe(string channel, Action<string, object, EventArgs> del)
        {
            if (_subscribable != null)
                return _subscribable.Unsubscribe(channel, del);
            return false;
        }

        public virtual void PatternUnsubscribe(string channel_pattern, Action<string, object, EventArgs> del)
        {
            if (_subscribable != null)
                _subscribable.PatternSubscribe(channel_pattern, del);
        }
    }

    public class NotifiableManager : INotifiable
    {
        public INotifiable Notifiable
        {
            get;
            set;
        }

        public virtual void Notify(string channel, object sender, EventArgs args)
        {
            if (Notifiable != null)
                Notifiable.Notify(channel, sender, args);
        }

        public virtual void PatternNotify(string channel_pattern, object sender, EventArgs args)
        {
            if (Notifiable != null)
                Notifiable.Notify(channel_pattern, sender, args);
        }
    }

    public class MediatorManager : IMediator
    {
        private IMediator _mediator;

        public NotifiableManager NotifiableManager
        {
            get;
            private set;
        }

        public SubscribableManager SubscribableManager
        {
            get;
            private set;
        }

        public IMediator Mediator
        {
            get
            {
                return _mediator;
            }
            set
            {
                SubscribableManager.Subscribable = value;
                NotifiableManager.Notifiable = value;
                _mediator = value;
            }
        }

        public MediatorManager(SubscribableManager subscribable_manager, NotifiableManager notifiable_manager)
        {
            SubscribableManager = subscribable_manager;
            NotifiableManager = notifiable_manager;
        }

        public MediatorManager(SubscribableManager subscribable_manager)
        {
            SubscribableManager = subscribable_manager;
            NotifiableManager = new NotifiableManager();
        }

        public bool Subscribe(string channel, Action<string, object, EventArgs> del)
        {
            return SubscribableManager.Subscribe(channel, del);
        }

        public void PatternSubscribe(string channel_pattern, Action<string, object, EventArgs> del)
        {
            SubscribableManager.PatternSubscribe(channel_pattern, del);
        }

        public bool Unsubscribe(string channel, Action<string, object, EventArgs> del)
        {
            return SubscribableManager.Unsubscribe(channel, del);
        }

        public void PatternUnsubscribe(string channel_pattern, Action<string, object, EventArgs> del)
        {
            SubscribableManager.PatternUnsubscribe(channel_pattern, del);
        }

        public void Notify(string channel, object sender, EventArgs args)
        {
            NotifiableManager.Notify(channel, sender, args);
        }

        public void PatternNotify(string channel_pattern, object sender, EventArgs args)
        {
            NotifiableManager.PatternNotify(channel_pattern, sender, args);
        }
    }

    public class DefaultSubscribableManager : SubscribableManager
    {
        public event Action OnSubscribe;
        private List<Tuple<string, Action<string, object, EventArgs>>> subscribed_delegates;
        private List<Tuple<string, Action<string, object, EventArgs>>> pattern_subscribed_delegates;

        public DefaultSubscribableManager()
        {
            subscribed_delegates = new List<Tuple<string, Action<string, object, EventArgs>>>();
            pattern_subscribed_delegates = new List<Tuple<string, Action<string, object, EventArgs>>>();
        }

        public override void SubscribeAll()
        {
            OnSubscribe();
        }

        public override void  UnsubscribeAll()
        {
 	        foreach(Tuple<string, Action<string, object, EventArgs>> item in subscribed_delegates)
            {
                base.Unsubscribe(item.Item1, item.Item2);
            }
            foreach(Tuple<string, Action<string, object, EventArgs>> item in pattern_subscribed_delegates)
            {
                base.PatternUnsubscribe(item.Item1, item.Item2);
            }
            subscribed_delegates.Clear();
            pattern_subscribed_delegates.Clear();
        }

        public override bool Subscribe(string channel, Action<string, object, EventArgs> del)
        {
            bool retval = base.Subscribe(channel, del);
            if (retval)
                subscribed_delegates.Add(new Tuple<string, Action<string, object, EventArgs>>(channel, del));
            return retval;
        }

        public override void PatternSubscribe(string channel_pattern, Action<string, object, EventArgs> del)
        {
            base.PatternSubscribe(channel_pattern, del);
            pattern_subscribed_delegates.Add(new Tuple<string, Action<string, object, EventArgs>>(channel_pattern, del));
        }

        public override bool Unsubscribe(string channel, Action<string, object, EventArgs> del)
        {
            bool retval = base.Unsubscribe(channel, del);
            if (retval)
            {
                int index = -1;
                for (int i = 0; i < subscribed_delegates.Count; i++)
                {
                    if (subscribed_delegates[i].Item1 == channel && subscribed_delegates[i].Item2 == del)
                    {
                        index = i;
                        break;
                    }
                }
                if (index > -1)
                {
                    subscribed_delegates.RemoveAt(index);
                }
            }
            return retval;
        }

        public override void PatternUnsubscribe(string channel_pattern, Action<string, object, EventArgs> del)
        {
            base.PatternUnsubscribe(channel_pattern, del);
            int index = -1;
            for (int i = 0; i < subscribed_delegates.Count; i++)
            {
                if (subscribed_delegates[i].Item1 == channel_pattern && subscribed_delegates[i].Item2 == del)
                {
                    index = i;
                    break;
                }
            }
            if (index > -1)
            {
                subscribed_delegates.RemoveAt(index);
            }
        }
    }

    public class FlexibleSubscribableManager : SubscribableManager
    {
        public event Action OnSubscribe;
        public event Action OnUnsubscribe;

        public override void UnsubscribeAll()
        {
            OnUnsubscribe();
        }

        public override void SubscribeAll()
        {
            OnSubscribe();
        }
    }
}
