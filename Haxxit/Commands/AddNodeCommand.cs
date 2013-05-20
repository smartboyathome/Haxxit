using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.Commands
{
    public class AddNodeCommand : Command
    {
        public AddNodeCommand()
        {
            Name = "";
            Description = "";
            Range = 0;
        }

        public AddNodeCommand(string name, string description, ushort range)
        {
            Name = name;
            Description = description;
            Range = range;
        }

        public override UndoCommand Run(Map map, Point attacked_point)
        {
            if (map.GetNode(attacked_point).GetType() == typeof(UnavailableNode))
            {
                map.CreateNode(new AvailableNodeFactory(), attacked_point);
                return new UndoAddNodeCommand(attacked_point);
            }
            return null;
        }
    }

    public class UndoAddNodeCommand : UndoCommand
    {
        private Command command;
        private Point attacked_point;

        public UndoAddNodeCommand(Point attacked_point)
        {
            this.command = new RemoveNodeCommand();
            this.attacked_point = attacked_point;
        }

        public override bool Run(Map map)
        {
            return command.Run(map, attacked_point) != null;
        }
    }
}
