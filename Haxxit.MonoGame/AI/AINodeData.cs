using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    // These nodes form part of a grid which is used by the AI to store and
    // calculate information used during its turn.
    class AINodeData
    {
        // X,Y Coordinate of this node
        private Haxxit.Maps.Point coordinate;
        public Haxxit.Maps.Point Coordinate { get { return coordinate; } }

        // If there's nothing on this node
        private bool isAvailable;
        public bool IsAvailable { get { return isAvailable; } set { isAvailable = value; } }

        // If there's a silicoin on this node
        bool hasSilicoin;
        public bool HasSilicoin { get { return hasSilicoin; } set { hasSilicoin = value; } }

        // If there's a data (end of level) item on this node
        bool hasData;
        public bool HasData { get { return hasData; } set { hasData = value; } }

        // If there's a program spawn on this node
        bool isSpawn;
        public bool IsSpawn { get { return isSpawn; } set { isSpawn = value; } }

        // Link to the program currently occupying this node
        private Haxxit.Maps.ProgramHeadNode occupiedBy;
        public Haxxit.Maps.ProgramHeadNode OccupiedBy { get { return occupiedBy; } set { occupiedBy = value; } }

        // This is used on a node by node basis during
        // calls fromthe AStar shortest path algorithm
        // In the PlayerAI class
        public enum AStarStatus
        {
            Unlisted,
            Open,
            Closed
        }
        private AStarStatus aStarTrackStatus;
        public AStarStatus AStarTrackStatus { get { return aStarTrackStatus; } set { aStarTrackStatus = value; } }

        // This is used to track the path back to a moving program
        // while determining the shortest path to a destination node
        // during calls from the AStar algorithm in the PlayerAI class
        private AINodeData parent;
        public AINodeData Parent { get { return parent; } set { parent = value; } }

        // These are variables used during calls from the AStar algorithm
        // in the PlayerAI class.  Modified nodes are tracked during algorithm
        // execution and are reset to default values once the shortest path
        // has been found
        private int f;
        public int F { get { return f; } set { f = value; } }
        private int g;
        public int G { get { return g; } set { g = value; } }
        private int h;
        public int H { get { return h; } set { h = value; } }

        // Constructor
        public AINodeData(int column, int row)
        {
            coordinate = new Haxxit.Maps.Point(column, row);
            isAvailable = false;
            hasSilicoin = false;
            hasData = false;
            isSpawn = false;
            occupiedBy = null;
            aStarTrackStatus = AStarStatus.Unlisted;
            parent = null;
            f = int.MaxValue;
            g = int.MaxValue;
            h = int.MaxValue;
        }

        // Checks to see if this node is either available or
        // part of the program that wants to move into it (allowed).
        public bool canHoldCurrentProgram(Haxxit.Maps.ProgramHeadNode program)
        {
            if (isAvailable)
            {
                return true;
            }
            else if (occupiedBy == program)
            {
                return true;
            }
            return false;
        }
    }
}
