using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit.MonoGame.Maps
{
    class MonoGameDataMap : Haxxit.Maps.DataMap
    {
        public MonoGameDataMap(int x_size, int y_size, ushort initial_silicoins=0, ushort total_spawn_weights=0) :
            base(x_size, y_size, initial_silicoins, total_spawn_weights)
        {

        }

        protected override Player CheckIfMapHacked(Haxxit.Maps.DataNode sender)
        {
            Player winner = base.CheckIfMapHacked(sender);
            if (winner != null)
                return winner;
            // Copied from EnemyMap
            List<Player> active_players = new List<Player>();
            foreach (Haxxit.Maps.Point p in Low.IterateOverRange(High))
            {
                if (NodeIsType<Haxxit.Maps.OwnedNode>(p))
                {
                    Haxxit.Maps.OwnedNode node = GetNode<Haxxit.Maps.OwnedNode>(p);
                    if (active_players.Count(x => x == node.Player) == 0)
                    {
                        active_players.Add(node.Player);
                    }
                }
            }
            if (active_players.Count < 2)
            {
                has_been_hacked = true;
                if (active_players.Count == 1 && active_players[0].GetType() == typeof(PlayerAI))
                    return active_players[0];
            }
            return null;
        }
    }
}
