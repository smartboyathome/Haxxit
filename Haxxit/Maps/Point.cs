using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit.Maps
{
    public struct Point : IEquatable<Point>
    {
        private int x, y;

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point(Point other)
        {
            this.x = other.x;
            this.y = other.y;
        }

        public bool IsInBounds(int x_min, int x_max, int y_min, int y_max)
        {
            return x >= x_min && x <= x_max && y >= y_min && y <= y_max;
        }

        // returns 1 or -1 (step direction) for x and y from this point to another.
        public Point DirectionsTo(Point other)
        {
            return new Point(DirectionsTo(X, other.X), DirectionsTo(Y, other.Y));
        }

        private int DirectionsTo(int start, int end)
        {
            if (start == end)
                return 0;
            return (end - start) / Math.Abs(end - start);
        }

        public Point DistanceBetween(Point other)
        {
            Point retval = this - other;
            retval = new Point(Math.Abs(retval.X), Math.Abs(retval.Y));
            return retval;
        }

        public IEnumerable<Point> IterateOverRange(Point other)
        {
            Point direction = DirectionsTo(other);
            for (int y = 0; y <= other.Y; y += direction.Y)
            {
                for (int x = 0; x <= other.X; x += direction.Y)
                {
                    yield return new Point(x, y);
                }
            }
        }

        public List<Point> GetOrthologicalNeighbors()
        {
            return GetPointsWithinDistance(1);
        }

        // Recursively generates a list of points within a specified distance of this one
        public List<Point> GetPointsWithinDistance(int distance)
        {
            List<Haxxit.Maps.Point> pointsWithinDistance;
            if (distance == 0) // Base case instantiates the actual list object
            {
                pointsWithinDistance = new List<Point>();
                return pointsWithinDistance;
            }
            else // Find all of the valid points in the current range ring (each recursive call handles a smaller ring)
            {
                pointsWithinDistance = GetPointsWithinDistance(distance - 1);
                for (int negativeX = distance * -1; negativeX < 0; negativeX++) // Find options in lower left quadrant from this point
                {
                    pointsWithinDistance.Add(new Point(x + negativeX, y + distance + negativeX));
                }
                for (int positiveY = distance; positiveY > 0; positiveY--) // Find options in lower right quadrant from this point
                {
                    pointsWithinDistance.Add(new Point(x + distance - positiveY, y + positiveY));
                }
                for (int positiveX = distance; positiveX > 0; positiveX--) // Find options in upper right quadrant from this point
                {
                    pointsWithinDistance.Add(new Point(x + positiveX, y - distance + positiveX));
                }
                for (int negativeY = distance * -1; negativeY < 0; negativeY++) // Find options in upper left quadrant from this point
                {
                    pointsWithinDistance.Add(new Point(x - distance - negativeY, y + negativeY));
                }
            }
            return pointsWithinDistance;
        }

        public bool IsDirectional()
        {
            return (Math.Abs(X) == 1 && Y == 0) || (Math.Abs(Y) == 1 && X == 0);
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.x - b.x, a.y - b.y);
        }

        public static Point operator *(Point a, Point b)
        {
            return new Point(a.x * b.x, a.y * b.y);
        }

        public static Point operator /(Point a, Point b)
        {
            return new Point(a.x / b.x, a.y / b.y);
        }

        public static bool operator <(Point a, Point b)
        {
            return a.X < b.X && a.Y < b.Y;
        }

        public static bool operator <=(Point a, Point b)
        {
            return a.X <= b.X && a.Y <= b.Y;
        }

        public static bool operator >(Point a, Point b)
        {
            return a.X > b.X && a.Y > b.Y;
        }

        public static bool operator >=(Point a, Point b)
        {
            return a.X >= b.X && a.Y >= b.Y;
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator!=(Point a, Point b)
        {
            return !(a == b);
        }

        public bool Equals(Point b)
        {
            return this == b;
        }

        public override bool Equals(object other)
        {
            if(other == null || this.GetType() != other.GetType())
                return false;
            return this == (Point)other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = hash * 5 + x.GetHashCode();
                hash = hash * 7 + y.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return "(" + X.ToString() + ", " + Y.ToString() + ")";
        }
    }
}
