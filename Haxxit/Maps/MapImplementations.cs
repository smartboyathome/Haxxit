using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit.Maps
{
    public class NoWinMap : Map
    {
        public NoWinMap(int x_size, int y_size, ushort initial_silicoins=0) :
            base(x_size, y_size, initial_silicoins)
        {

        }

        protected override Player CheckIfMapHacked()
        {
            return null;
        }
    }

    public class EnemyMap : Map
    {
        public EnemyMap(int x_size, int y_size, ushort initial_silicoins=0) :
            base(x_size, y_size, initial_silicoins)
        {

        }

        protected override Player CheckIfMapHacked()
        {
            List<Player> active_players = new List<Player>();
            foreach (Point p in Low.IterateOverRange(High))
            {
                if (NodeIsType<OwnedNode>(p))
                {
                    OwnedNode node = (OwnedNode)GetNode(p);
                    bool found_player = false;
                    foreach (Player player in active_players)
                    {
                        if (player == node.Player)
                        {
                            found_player = true;
                            break;
                        }
                    }
                    if (!found_player)
                    {
                        active_players.Add(node.Player);
                    }
                }
            }
            if (active_players.Count < 2)
            {
                has_been_hacked = true;
                if (active_players.Count == 1)
                    return active_players[0];
            }
            return null;
        }
    }
}
