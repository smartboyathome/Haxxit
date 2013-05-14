using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    class PlayerAI : Player
    {
        public void HandleAITurn(Haxxit.Maps.Map map)
        {
            _notifiable_manager.Notify("haxxit.map.turn_done", this, new EventArgs());
        }
    }
}
