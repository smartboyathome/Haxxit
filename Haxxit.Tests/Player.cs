using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit.Tests
{
    class TestPlayer : Player
    {
        Stack<PlayerAction> _actions;

        public TestPlayer(Stack<PlayerAction> actions)
        {
            _actions = actions;
        }
    }
}
