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
        Player tutorialPlayer;

        public TutorialMapSpawnGameState(Haxxit.Maps.Map map)
        {
            display_map_state = new MapDisplayGameState(map);
            tutorialPlayer = GlobalAccessors.mPlayer1;
        }

        public TutorialMapSpawnGameState(MapDisplayGameState background_state)
        {
            display_map_state = background_state;
            tutorialPlayer = GlobalAccessors.mPlayer1;
        }

        public override void OnFinishedClick(DrawableRectangle rectangle)
        {
            display_map_state.Map.FinishedSpawning();
            //UserMapGameState new_state = new UserMapGameState(user_map_state.Map);
            //MapPlayGameState new_state = new MapPlayGameState(display_map_state);
            if (tutorialPlayer.CurrentNode == "Node2")
            {
                MapPlayGameState new_state = new MapPlayGameState(display_map_state);
                _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
            }
            else
            {
                TutorialState new_state = new TutorialState(display_map_state);
                _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
            }
        }

        public override void Update()
        {
            // To change a scene, just call one of these:
            // Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.pop", this, new EventArgs());

            if (tutorialPlayer.SpawnTutorial == true)
            {
                TutorialState new_state = new TutorialState(display_map_state);
                _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
            }
            else if (tutorialPlayer.level2Tutorial == true)
            {
                TutorialState new_state = new TutorialState(display_map_state);
                _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
            }

            _mediator_manager.Notify("haxxit.map.nodes.changed", this, new Haxxit.Maps.MapChangedEventArgs(spawns.Keys));

            foreach (DrawableRectangle spawn in spawns.Values)
            {
                spawn.Update();
            }

            finished_button.Update();
            leave_map_button.Update();
        }
    }
}
