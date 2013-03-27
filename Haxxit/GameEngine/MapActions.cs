using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;

namespace SmartboyDevelopments.Haxxit.GameEngine
{
    public abstract class MapAction : StateAction
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
    }

    public class MovementAction : MapAction
    {
        public MovementAction(Point source, Point destination)
        {
            Source = source;
            Destination = destination;
        }

        public override void Execute(GameEngine engine)
        {
            _notifiable_manager.Notify("haxxit.map.move", this, new MoveEventArgs(Source, Source - Destination));
        }
    }

    public class RunCommandAction : MapAction
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

        public override void Execute(GameEngine engine)
        {
            _notifiable_manager.Notify("haxxit.map.command", this, new CommandEventArgs(Destination, Source, Command));
        }
    }
}
