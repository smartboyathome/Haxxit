using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;

namespace SmartboyDevelopments.Haxxit
{
    public enum GameStates
    {
        Network,
        Map
    }

    public class Game
    {
        private Server _selected_server;
        private IMediator _mediator;
        private UndoStack _undo_stack;

        public Network GameNetwork
        {
            get;
            private set;
        }

        public Player Attacker
        {
            get;
            private set;
        }

        public GameStates State
        {
            get;
            private set;
        }

        public Game(Network game_network, Player current_player)
        {
            _mediator = new SynchronousMediator();
            GameNetwork = game_network;
            Attacker = current_player;
            _selected_server = null;
            _undo_stack = new UndoStack(64, _mediator);
            State = GameStates.Network;
        }

        public bool SelectServer(Server server)
        {
            if (!GameNetwork.IsInNetwork(server))
                return false;
            _selected_server = server;
            return true;
        }
    }
}