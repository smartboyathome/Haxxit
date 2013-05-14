using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.Maps
{
    public class NoWinMap : Map
    {
        public NoWinMap(int x_size, int y_size, ushort initial_silicoins=0, ushort total_spawn_weights=0) :
            base(x_size, y_size, initial_silicoins, total_spawn_weights)
        {

        }

        public override void CheckIfHackedListener(string channel, object sender, EventArgs args)
        {
            
        }
    }

    public class EnemyMap : Map
    {
        public EnemyMap(int x_size, int y_size, ushort initial_silicoins=0, ushort total_spawn_weights=0) :
            base(x_size, y_size, initial_silicoins, total_spawn_weights)
        {

        }

        public override UndoCommand RunCommand(Point attacker_point, Point attacked_point, string command)
        {
            UndoCommand retval = base.RunCommand(attacker_point, attacked_point, command);
            Mediator.Notify("haxxit.map.hacked.check", this, new EventArgs());
            return retval;
        }

        public override void CheckIfHackedListener(string channel, object sender, EventArgs args)
        {
            Player winner = CheckIfMapHacked();
            if (has_been_hacked)
                Mediator.Notify("haxxit.map.hacked", this, new HackedEventArgs(winner, EarnedSilicoins));
        }

        protected Player CheckIfMapHacked()
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

    public class DataMap : Map
    {
        public DataMap(int x_size, int y_size, ushort initial_silicoins=0, ushort total_spawn_weights=0) :
            base(x_size, y_size, initial_silicoins, total_spawn_weights)
        {

        }

        public override void CheckIfHackedListener(string channel, object sender, EventArgs args)
        {
            if (sender.GetType() != typeof(DataNode))
                return;
            Player winner = CheckIfMapHacked((DataNode)sender);
            if (has_been_hacked)
                Mediator.Notify("haxxit.map.hacked", this, new HackedEventArgs(winner, EarnedSilicoins));
        }

        protected Player CheckIfMapHacked(DataNode sender)
        {
            bool has_data_nodes = false;
            foreach (Point p in Low.IterateOverRange(High))
            {
                if (NodeIsType<DataNode>(p) && p != sender.coordinate)
                {
                    has_data_nodes = true;
                    break;
                }
            }
            if (!has_data_nodes)
            {
                has_been_hacked = true;
                return CurrentPlayer;
            }
            return null;
        }
    }
}
