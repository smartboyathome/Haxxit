using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.SimplePubSub;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.Maps
{
    /*
     * Emitted signals:
     *     haxxit.map.move
     *     haxxit.map.move.undo
     *     haxxit.map.turn_done
     *     haxxit.map.command
     *     haxxit.map.command.undo
     *     haxxit.map.hacked
     *     haxxit.map.hacked.check
     *     haxxit.map.silicoins.add
     *     haxxit.map.nodes.create
     */
    public abstract partial class Map
    {
        private MediatorManager _mediator_manager;
        public IMediator Mediator
        {
            get
            {
                return _mediator_manager.Mediator;
            }
            set
            {
                _mediator_manager.Mediator = value;
                foreach (MapNode node in map)
                {
                    node.Notifiable = value;
                }
            }
        }

        public Map(int x_size, int y_size, ushort initial_silicoins=0, ushort total_spawn_weights=0)
        {
            DefaultSubscribableManager subscribable_manager = new DefaultSubscribableManager();
            subscribable_manager.OnSubscribe += SubscribeAll;
            _mediator_manager = new MediatorManager(subscribable_manager);
            InitializeMap(x_size, y_size, initial_silicoins, total_spawn_weights);
        }

        private void SubscribeAll()
        {
            _mediator_manager.Subscribe("haxxit.map.move", MoveListener);
            _mediator_manager.Subscribe("haxxit.map.move.undo", UndoMoveListener);
            _mediator_manager.Subscribe("haxxit.map.turn_done", TurnDoneListener);
            _mediator_manager.Subscribe("haxxit.map.command", CommandListener);
            _mediator_manager.Subscribe("haxxit.map.command.undo", UndoCommandListener);
            _mediator_manager.Subscribe("haxxit.map.silicoins.add", AddSilicoinListener);
            _mediator_manager.Subscribe("haxxit.map.hacked.check", CheckIfHackedListener);
        }

        public void TurnDoneListener(string channel, object sender, EventArgs args)
        {
            TurnDone();
            _mediator_manager.Notify("haxxit.undo_stack.clear", this, new EventArgs());
        }

        public void MoveListener(string channel, object sender, EventArgs args)
        {
            MoveEventArgs move_args = (MoveEventArgs)args;
            bool program_resized = NodeIsType<ProgramHeadNode>(move_args.Start) &&
                !GetNode<ProgramHeadNode>(move_args.Start).Program.Size.IsMaxSize();
            bool end_was_tailnode = this.NodeIsType<ProgramTailNode>(move_args.Start + move_args.Direction);
            Point tail_location = new Point(-1, -1);
            if(!program_resized)
            {
                ProgramNode program_node = (ProgramNode)GetNode(move_args.Start);
                while (program_node.Tail != null)
                    program_node = program_node.Tail;
                tail_location = program_node.coordinate;
            }
            if(MoveProgram(move_args.Start, move_args.Direction))
                _mediator_manager.Notify("haxxit.undo_stack.push", this,
                    new UndoEventArgs("haxxit.map.move.undo",
                        new UndoMoveEventArgs(move_args, program_resized, end_was_tailnode, tail_location)
                    )
                );
        }

        public void UndoMoveListener(string channel, object sender, EventArgs args)
        {
            UndoMoveEventArgs undo_move_args = (UndoMoveEventArgs)args;
            bool result = UndoMoveProgram(undo_move_args.Start, undo_move_args.Direction, undo_move_args.ProgramSizeIncreased,
                undo_move_args.EndWasTailNode, undo_move_args.TailNodeLocation);
        }

        public void CommandListener(string channel, object sender, EventArgs args)
        {
            CommandEventArgs attack_args = (CommandEventArgs)args;
            UndoCommand undo_command = RunCommand(attack_args.AttackerPoint, attack_args.AttackedPoint, attack_args.Command);
            if (undo_command != null)
            {
                _mediator_manager.Notify("haxxit.undo_stack.push", this,
                    new UndoEventArgs("haxxit.map.command.undo",
                        new UndoCommandEventArgs(undo_command)
                    )
                );
            }
        }

        public void UndoCommandListener(string channel, object sender, EventArgs args)
        {
            UndoCommandEventArgs undo_command_args = (UndoCommandEventArgs)args;
            RunUndoCommand(undo_command_args._undo_command);
        }

        public void AddSilicoinListener(string channel, object sender, EventArgs args)
        {
            SilicoinEventArgs event_args = (SilicoinEventArgs)args;
            IncreraseEarnedSilicoins(event_args.Silicoins);
        }

        public void CreateNodeListener(string channel, object sender, EventArgs args)
        {
            CreateNodeEventArgs event_args = (CreateNodeEventArgs)args;
            CreateNode(event_args.NodeFactory, event_args.NodeLocation);
        }

        public abstract void CheckIfHackedListener(string channel, object sender, EventArgs args);
    }
}
