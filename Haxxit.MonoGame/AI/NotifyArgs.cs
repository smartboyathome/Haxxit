using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    // A simple wrapper class for storing the arguments needed to use NotifiableManager.Notify.
    // This is used by the AI to store all of its intended actions in a queue while calculating its turn.
    // At the end of AI turn calculation, all of the NotifyArgs elements are pulled from the queue and
    // used to send the actual commands via NotifiableManager.Notify.
    class NotifyArgs
    {
        public enum ArgType
        {
            Move,
            Command,
            TurnDone
        }

        private string eventItem;
        public string EventItem { get { return eventItem; } set {eventItem = value; } }

        private object objectItem;
        public object ObjectItem { get { return objectItem; } set { objectItem = value; } }

        private EventArgs eventArgsItem;
        public EventArgs EventArgsItem { get { return eventArgsItem; } set { eventArgsItem = value; } }

        private ArgType argsType;
        public ArgType ArgsType { get { return argsType; } set { argsType = value; } }

        // Constructor
        public NotifyArgs(string newEventItem, object newObjectItem, EventArgs newEventArgsItem, ArgType newArgsType)
        {
            eventItem = newEventItem;
            objectItem = newObjectItem;
            eventArgsItem = newEventArgsItem;
            argsType = newArgsType;
        }
    }
}
