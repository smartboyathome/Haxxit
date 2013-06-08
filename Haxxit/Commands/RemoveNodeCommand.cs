using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.Commands
{
    public class RemoveNodeCommand : Command
    {
        public RemoveNodeCommand()
        {
            Name = "";
            Description = "";
            Range = 0;
        }

        public RemoveNodeCommand(string name, string description, ushort range)
        {
            Name = name;
            Description = description;
            Range = range;
        }

        public override UndoCommand Run(Map map, Point attacked_point)
        {
            if(!CanAttack(map, attacked_point))
                return null;
            map.CreateNode(new UnavailableNodeFactory(), attacked_point);
            return new UndoRemoveNodeCommand(attacked_point);
        }

        public override bool CanAttack(Map map, Point attacked_point)
        {
            return map.GetNode(attacked_point).GetType() == typeof(AvailableNode);
        }
    }

    public class UndoRemoveNodeCommand : UndoCommand
    {
        private Command command;
        private Point attacked_point;

        public UndoRemoveNodeCommand(Point attacked_point)
        {
            this.command = new AddNodeCommand();
            this.attacked_point = attacked_point;
        }

        public override bool Run(Map map)
        {
            return command.Run(map, attacked_point) != null;
        }
    }
}
