using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Haxxit = SmartboyDevelopments.Haxxit;

namespace SmartboyDevelopments.Haxxit.MonoGame.GameStates
{
    class TutorialMapSpawnGameState : MapSpawnGameState
    {
        public TutorialMapSpawnGameState(Haxxit.Maps.Map map)
        {
            display_map_state = new MapDisplayGameState(map);
        }

        public TutorialMapSpawnGameState(MapDisplayGameState background_state)
        {
            display_map_state = background_state;
        }

        public override void OnFinishedClick(DrawableRectangle rectangle)
        {
            display_map_state.Map.FinishedSpawning();
            //UserMapGameState new_state = new UserMapGameState(user_map_state.Map);
            //MapPlayGameState new_state = new MapPlayGameState(display_map_state);
            TutorialState new_state = new TutorialState(display_map_state);
            _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
        }
    }
}
