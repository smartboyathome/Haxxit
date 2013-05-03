using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.Maps
{
    public class MoveEventArgs : EventArgs
    {
        public Point Start { get; private set; }
        public Point Direction { get; private set; }

        public MoveEventArgs(Point start, Point direction) :
            base()
        {
            Start = start;
            Direction = direction;
        }

        public MoveEventArgs(int start_x, int start_y, int direction_x, int direction_y)
            : base()
        {
            Start = new Point(start_x, start_y);
            Direction = new Point(direction_x, direction_y);
        }

        public MoveEventArgs(MoveEventArgs other)
            : base()
        {
            Start = other.Start;
            Direction = other.Direction;
        }
    }

    public class UndoMoveEventArgs : MoveEventArgs
    {
        public bool ProgramSizeIncreased { get; private set; }
        public bool EndWasTailNode { get; private set; }
        public Point TailNodeLocation { get; private set; }

        public UndoMoveEventArgs(Point start, Point direction, bool size_increased, bool end_was_tail_node)
            : base(start, direction)
        {
            ProgramSizeIncreased = size_increased;
            EndWasTailNode = end_was_tail_node;
            TailNodeLocation = new Point(-1, -1);
        }

        public UndoMoveEventArgs(Point start, Point direction, bool size_increased, bool end_was_tail_node, Point tail_node_location)
            : base(start, direction)
        {
            ProgramSizeIncreased = size_increased;
            EndWasTailNode = end_was_tail_node;
            TailNodeLocation = tail_node_location;
        }

        public UndoMoveEventArgs(int start_x, int start_y, int direction_x, int direction_y, bool size_increased, bool end_was_tail_node)
            : base(start_x, start_y, direction_x, direction_y)
        {
            ProgramSizeIncreased = size_increased;
            EndWasTailNode = end_was_tail_node;
            TailNodeLocation = new Point(-1, -1);
        }

        public UndoMoveEventArgs(int start_x, int start_y, int direction_x, int direction_y, bool size_increased, bool end_was_tail_node, Point tail_node_location)
            : base(start_x, start_y, direction_x, direction_y)
        {
            ProgramSizeIncreased = size_increased;
            EndWasTailNode = end_was_tail_node;
            TailNodeLocation = tail_node_location;
        }

        public UndoMoveEventArgs(MoveEventArgs other, bool size_increased, bool end_was_tail_node)
            : base(other)
        {
            ProgramSizeIncreased = size_increased;
            EndWasTailNode = end_was_tail_node;
            TailNodeLocation = new Point(-1, -1);
        }

        public UndoMoveEventArgs(MoveEventArgs other, bool size_increased, bool end_was_tail_node, Point tail_node_location)
            : base(other)
        {
            ProgramSizeIncreased = size_increased;
            EndWasTailNode = end_was_tail_node;
            TailNodeLocation = tail_node_location;
        }

        public UndoMoveEventArgs(UndoMoveEventArgs other)
            : base(other.Start, other.Direction)
        {
            ProgramSizeIncreased = other.ProgramSizeIncreased;
            EndWasTailNode = other.EndWasTailNode;
            TailNodeLocation = other.TailNodeLocation;
        }
    }

    public class CommandEventArgs : EventArgs
    {
        public Point AttackedPoint { get; private set; }
        public Point AttackerPoint { get; private set; }
        public string Command { get; private set; }
        public CommandEventArgs(Point attacked_point, Point attacker_point, string command)
        {
            AttackedPoint = attacked_point;
            AttackerPoint = attacker_point;
            Command = command;
        }
    }

    public class UndoCommandEventArgs : EventArgs
    {
        public UndoCommand _undo_command { get; private set; }
        public UndoCommandEventArgs(UndoCommand undo_command)
        {
            _undo_command = undo_command;
        }
    }

    public class HackedEventArgs : EventArgs
    {
        public Player WinningPlayer
        {
            get;
            private set;
        }
        public ushort EarnedSilicoins
        {
            get;
            private set;
        }

        public HackedEventArgs(Player winning_player, ushort earned_silicoins)
        {
            WinningPlayer = winning_player;
            EarnedSilicoins = earned_silicoins;
        }
    }
}
