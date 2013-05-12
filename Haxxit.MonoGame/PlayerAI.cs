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
        private Haxxit.Maps.Map map;

        public PlayerAI(string name = "")
            : base(name)
        {
            // Nothing
        }

        public void HandleAITurn(Haxxit.Maps.Map currentMap)
        {
            map = currentMap;
            /*
            foreach (Program program in map.CurrentPlayer.GetPrograms())
            {
                TurnActions actions = planTurn(program);
                runActions(program, actions);
            }
            */
            _notifiable_manager.Notify("haxxit.map.turn_done", this, new EventArgs());
        }

        private TurnActions planTurn(Program program)
        {
            TurnActions actions = new TurnActions();
            actions.Moves = new List<Haxxit.Maps.Point>();
            actions.CommandName = "";
            actions.CommandTarget = new Haxxit.Maps.Point();
            return actions;
        }

        private void runActions(Program program, TurnActions actions)
        {
            foreach (Haxxit.Maps.Point locationMove in actions.Moves)
            {
                Haxxit.Maps.Point startMove = findHeadNode(program);
                Haxxit.Maps.Point directionMove = locationMove - startMove;
                map.MoveProgram(startMove, directionMove);
            }
            program.RunCommand(map, actions.CommandTarget, actions.CommandName);
        }

        private Haxxit.Maps.Point findHeadNode(Program program)
        {
            Haxxit.Maps.MapNode checkNode = null;
            for (int column = 0; column < map.XSize; column++)
            {
                for (int row = 0; row < map.YSize; row++)
                {
                    checkNode = map.GetNode(column, row);
                    if (checkNode.GetType() == typeof(Haxxit.Maps.ProgramHeadNode))
                    {
                        if (((Haxxit.Maps.ProgramHeadNode)checkNode).Program == program)
                        {
                            return checkNode.coordinate;
                        }
                    }
                }
            }
            return new Haxxit.Maps.Point(-1, -1);
        }
    }
}
