using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SmartboyDevelopments.SimplePubSub.ExtensionMethods;

namespace SmartboyDevelopments.SimplePubSub
{
    public delegate void SubscribableListener(string channel, object sender, EventArgs args);

    public interface ISubscribable
    {
        bool Subscribe(string channel, SubscribableListener del);
        void PatternSubscribe(string channel_pattern, SubscribableListener del);
        bool Unsubscribe(string channel, SubscribableListener del);
        void PatternUnsubscribe(string channel_pattern, SubscribableListener del);
    }

    public interface INotifiable
    {
        void Notify(string channel, object sender, EventArgs args);
        void PatternNotify(string channel_pattern, object sender, EventArgs args);
    }

    public interface IMediator : ISubscribable, INotifiable {}

    public class SynchronousMediator : IMediator
    {
        Dictionary<string, List<SubscribableListener>> channels;
        Dictionary<string, List<SubscribableListener>> pattern_channels;

        public SynchronousMediator()
        {
            channels = new Dictionary<string, List<SubscribableListener>>();
            pattern_channels = new Dictionary<string, List<SubscribableListener>>();
        }

        private List<SubscribableListener> DefaultChannelValue(string key)
        {
            List<SubscribableListener> subscribers = new List<SubscribableListener>();
            foreach (KeyValuePair<string, List<SubscribableListener>> pattern_channel in pattern_channels)
            {
                if (Regex.Match(key, pattern_channel.Key).Success)
                {
                    foreach (SubscribableListener subscriber in pattern_channel.Value)
                    {
                        if (!subscribers.Contains(subscriber))
                        {
                            subscribers.Add(subscriber);
                        }
                    }
                }
            }
            return subscribers;
        }

        private List<SubscribableListener> DefaultPatternValue(string key)
        {
            // Implemented as a method so that a new list is only created when needed.
            return new List<SubscribableListener>();
        }

        public bool Subscribe(string channel, SubscribableListener del)
        {
            List<SubscribableListener> subscribers = channels.GetValueOrDefault(channel, DefaultChannelValue);
            if (subscribers.Contains(del))
            {
                return false;
            }
            subscribers.Add(del);
            return true;
        }

        public void PatternSubscribe(string channel_pattern, SubscribableListener del)
        {
            List<SubscribableListener> pattern_channel = pattern_channels.GetValueOrDefault(channel_pattern, DefaultPatternValue);
            if (!pattern_channel.Contains(del))
            {
                pattern_channel.Add(del);
            }
            foreach(KeyValuePair<string, List<SubscribableListener>> channel in channels.GetAllMatchingRegex(channel_pattern))
            {
                if (!channel.Value.Contains(del))
                {
                    channel.Value.Add(del);
                }
            }
        }

        public bool Unsubscribe(string channel, SubscribableListener del)
        {
            List<SubscribableListener> subscribers = channels.GetValueOrDefault(channel, DefaultChannelValue);
            if (!subscribers.Contains(del))
            {
                return false;
            }
            subscribers.Remove(del);
            return true;
        }

        public void PatternUnsubscribe(string channel_pattern, SubscribableListener del)
        {
            List<SubscribableListener> pattern_channel = pattern_channels.GetValueOrDefault(channel_pattern, DefaultPatternValue);
            if (pattern_channel.Contains(del))
            {
                pattern_channel.Remove(del);
            }
            IEnumerable<KeyValuePair<string, List<SubscribableListener>>> matching_channels =
                channels.GetAllMatchingRegex(channel_pattern);
            List<string> channels_to_remove = new List<string>();
            foreach (KeyValuePair<string, List<SubscribableListener>> channel in matching_channels)
            {
                if (channel.Value.Contains(del))
                {
                    channel.Value.Remove(del);
                }
                if (channel.Value.Count == 0)
                {
                    channels_to_remove.Add(channel.Key);
                }
            }
        }

        public void Notify(string channel, object sender, EventArgs args)
        {
            foreach (SubscribableListener subscriber in channels.GetValueOrDefault(channel, DefaultChannelValue).ShallowCopy())
            {
                subscriber.Invoke(channel, sender, args);
            }
        }

        public void PatternNotify(string channel_pattern, object sender, EventArgs args)
        {
            foreach (KeyValuePair<string, List<SubscribableListener>> channel in channels.GetAllMatchingRegex(channel_pattern).ShallowCopy())
            {
                foreach (SubscribableListener subscriber in channel.Value.ShallowCopy())
                {
                    subscriber.Invoke(channel.Key, sender, args);
                }
            }
        }
    }
}
