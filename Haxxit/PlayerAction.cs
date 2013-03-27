using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit
{
    public abstract class PlayerAction
    {
        public Point Source
        {
            get;
            protected set;
        }
        public Point Destination
        {
            get;
            protected set;
        }
        public abstract void Execute(MediatorManager subscriber);
    }

    public class MovementAction : PlayerAction
    {
        public MovementAction(Point source, Point destination)
        {
            Source = source;
            Destination = destination;
        }

        /*public override bool VerifyActionIsLegal(Map map)
        {
            Point difference = _destination - _source;
            return  ((Math.Abs(difference.X) == 1 && difference.Y == 0) || (Math.Abs(difference.Y) == 1 && difference.X == 0)) &&
                map.MoveProgram(_source, _destination);
        }*/

        public override void Execute(MediatorManager subscriber)
        {
            subscriber.Notify("haxxit.map.move", this, new MoveEventArgs(Source, Source - Destination));
        }
    }

    public class RunCommandAction : PlayerAction
    {
        public string Command
        {
            get;
            private set;
        }

        public RunCommandAction(Point source, Point destination, string command)
        {
            Source = source;
            Destination = destination;
            Command = command;
        }

        public override void Execute(MediatorManager subscriber)
        {
            subscriber.Notify("haxxit.map.command", this, new CommandEventArgs(Destination, Source, Command));
        }
    }
}
