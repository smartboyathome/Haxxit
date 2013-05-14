using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    // This class stores the actual parameters used in the targetOptions list of the PrioritizedCommand class.
    // these parameters include a source point from which to issue the program's command, a destination point
    // for that command, the program to be targeted by that command, and a path to get to the source point.
    // These CommandInfo objects will be sorted within the targetOptions list of the PrioritizedCommand class
    // by the length of their paths (shorter is preferable to AI).
    class CommandInfo : IComparable
    {
        private Maps.Point source;
        public Maps.Point Source { get { return source; } set { source = value; } }

        private Maps.Point target;
        public Maps.Point Target { get { return target; } set { target = value; } }

        private Maps.ProgramHeadNode targetProgram;
        public Maps.ProgramHeadNode TargetProgram { get { return targetProgram; } set { targetProgram = value; } }

        private Stack<Maps.Point> path;
        public Stack<Maps.Point> Path { get { return path; } set { path = value; } }

        // Constructor
        public CommandInfo(Maps.Point newSource, Maps.Point newTarget, Maps.ProgramHeadNode newTargetProgram, Stack<Maps.Point> newPath)
        {
            source = newSource;
            target = newTarget;
            targetProgram = newTargetProgram;
            path = newPath;
        }

        // Required for IComparable inheritance
        public int CompareTo(object obj)
        {
            CommandInfo otherCommandInfo = obj as CommandInfo;
            if (otherCommandInfo.Path.Count < path.Count)
            {
                return 1;
            }
            else if (otherCommandInfo.Path.Count == path.Count)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
