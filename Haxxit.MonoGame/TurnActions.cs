using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    public class TurnActions
    {
        private List<Haxxit.Maps.Point> moves;
        public List<Haxxit.Maps.Point> Moves { get { return moves; } set { moves = value; } }

        private string commandName;
        public string CommandName { get { return commandName; } set { commandName = value; } }

        private Haxxit.Maps.Point commandTarget;
        public Haxxit.Maps.Point CommandTarget { get { return commandTarget; } set { commandTarget = value; } }

        public TurnActions()
        {
            moves = new List<Haxxit.Maps.Point>();
            commandName = "";
            commandTarget = new Haxxit.Maps.Point();
        }
    }
}
