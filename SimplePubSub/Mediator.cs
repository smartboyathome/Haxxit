using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SmartboyDevelopments.SimplePubSub.ExtensionMethods;

namespace SmartboyDevelopments.SimplePubSub
{
    public interface ISubscribable
    {
        bool Subscribe(string channel, Action<string, object, EventArgs> del);
        void PatternSubscribe(string channel_pattern, Action<string, object, EventArgs> del);
        bool Unsubscribe(string channel, Action<string, object, EventArgs> del);
        void PatternUnsubscribe(string channel_pattern, Action<string, object, EventArgs> del);
    }

    public interface INotifiable
    {
        void Notify(string channel, object sender, EventArgs args);
        void PatternNotify(string channel_pattern, object sender, EventArgs args);
    }

    public interface IMediator : ISubscribable, INotifiable {}

    public class SynchronousMediator : IMediator
    {
        Dictionary<string, List<Action<string, object, EventArgs>>> channels;
        Dictionary<string, List<Action<string, object, EventArgs>>> pattern_channels;

        public SynchronousMediator()
        {
            channels = new Dictionary<string, List<Action<string, object, EventArgs>>>();
            pattern_channels = new Dictionary<string, List<Action<string, object, EventArgs>>>();
        }

        private List<Action<string, object, EventArgs>> DefaultChannelValue(string key)
        {
            List<Action<string, object, EventArgs>> subscribers = new List<Action<string, object, EventArgs>>();
            foreach (KeyValuePair<string, List<Action<string, object, EventArgs>>> pattern_channel in pattern_channels)
            {
                if (Regex.Match(key, pattern_channel.Key).Success)
                {
                    foreach (Action<string, object, EventArgs> subscriber in pattern_channel.Value)
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

        private List<Action<string, object, EventArgs>> DefaultPatternValue(string key)
        {
            // Implemented as a method so that a new list is only created when needed.
            return new List<Action<string, object, EventArgs>>();
        }

        public bool Subscribe(string channel, Action<string, object, EventArgs> del)
        {
            List<Action<string, object, EventArgs>> subscribers = channels.GetValueOrDefault(channel, DefaultChannelValue);
            if (subscribers.Contains(del))
            {
                return false;
            }
            subscribers.Add(del);
            return true;
        }

        public void PatternSubscribe(string channel_pattern, Action<string, object, EventArgs> del)
        {
            List<Action<string, object, EventArgs>> pattern_channel = pattern_channels.GetValueOrDefault(channel_pattern, DefaultPatternValue);
            if (!pattern_channel.Contains(del))
            {
                pattern_channel.Add(del);
            }
            foreach(KeyValuePair<string, List<Action<string, object, EventArgs>>> channel in channels.GetAllMatchingRegex(channel_pattern))
            {
                if (!channel.Value.Contains(del))
                {
                    channel.Value.Add(del);
                }
            }
        }

        public bool Unsubscribe(string channel, Action<string, object, EventArgs> del)
        {
            List<Action<string, object, EventArgs>> subscribers = channels.GetValueOrDefault(channel, DefaultChannelValue);
            if (!subscribers.Contains(del))
            {
                return false;
            }
            subscribers.Remove(del);
            return true;
        }

        public void PatternUnsubscribe(string channel_pattern, Action<string, object, EventArgs> del)
        {
            List<Action<string, object, EventArgs>> pattern_channel = pattern_channels.GetValueOrDefault(channel_pattern, DefaultPatternValue);
            if (pattern_channel.Contains(del))
            {
                pattern_channel.Remove(del);
            }
            IEnumerable<KeyValuePair<string, List<Action<string, object, EventArgs>>>> matching_channels =
                channels.GetAllMatchingRegex(channel_pattern);
            List<string> channels_to_remove = new List<string>();
            foreach (KeyValuePair<string, List<Action<string, object, EventArgs>>> channel in matching_channels)
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
            foreach (Action<string, object, EventArgs> subscriber in channels.GetValueOrDefault(channel, DefaultChannelValue))
            {
                subscriber.Invoke(channel, sender, args);
            }
        }

        public void PatternNotify(string channel_pattern, object sender, EventArgs args)
        {
            foreach (KeyValuePair<string, List<Action<string, object, EventArgs>>> channel in channels.GetAllMatchingRegex(channel_pattern))
            {
                foreach (Action<string, object, EventArgs> subscriber in channel.Value)
                {
                    subscriber.Invoke(channel.Key, sender, args);
                }
            }
        }
    }
}
