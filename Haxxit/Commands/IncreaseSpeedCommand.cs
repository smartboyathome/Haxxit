using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.Commands
{
    public class IncreaseSpeedCommand : Command
    {
        public ushort Speed
        {
            get;
            protected set;
        }

        public IncreaseSpeedCommand()
        {
            Name = "";
            Description = "";
            Speed = 0;
            Range = 0;
        }

        public IncreaseSpeedCommand(string name, string description, ushort speed, ushort range)
        {
            Name = name;
            Description = description;
            Speed = speed;
            Range = range;
        }

        public override UndoCommand Run(Map map, Point attacked_point)
        {
            if (!map.NodeIsType<ProgramNode>(attacked_point))
                return null;
            ProgramNode attacked_node = (ProgramNode)map.GetNode(attacked_point);
            Program attacked_program = attacked_node.Program;
            IncreaseProgramSpeed(attacked_program, Speed);
            return null;
        }
    }

    public class UndoIncreaseSpeedCommand : UndoCommand
    {
        private DecreaseSpeedCommand command;
        private Point attacked_point;

        public UndoIncreaseSpeedCommand(ushort speed, Point attacked_point)
        {
            this.command = new DecreaseSpeedCommand("", "", speed, 0);
            this.attacked_point = attacked_point;
        }

        public override bool Run(Map map)
        {
            return command.Run(map, attacked_point) != null;
        }
    }
}
