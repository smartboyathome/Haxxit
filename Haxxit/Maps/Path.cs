using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit.Maps
{
    struct Path
    {
        private Point start, end;
        private Map map;
        private Random random;

        public Path(Point _start, Point _end, Map _map)
        {
            start = _start;
            end = _end;
            map = _map;
            random = new Random();
        }

        private Tuple<bool, List<Point>> FindShortestPath(Point start, Point end, Point last, int moves_left)
        {
            Point diff = end - start;
            if (moves_left == 0)
            {
                return Tuple.Create(false, new List<Point>());
            }
            if ((diff == new Point(1, 0) || diff == new Point(-1, 0) || diff == new Point(0, 1) || diff == new Point(0, -1)) && map.NodeIsType<AvailableNode>(end))
            {
                List<Point> retlist = new List<Point>();
                retlist.Add(end);
                retlist.Add(start);
                return Tuple.Create(true, retlist);
            }
            if (diff.X != 0 && diff.Y != 0)
            {
                FindShortestPath_Both(start, end, last, moves_left);
            }
            else if (diff.X == 0)
            {
                FindShortestPath_Vertical(start, end, last, moves_left);
            }
            else if (diff.Y == 0)
            {
                FindShortestPath_Horizontal(start, end, last, moves_left);
            }
            return Tuple.Create(false, new List<Point>());
        }

        private Tuple<bool, List<Point>> FindShortestPath_Both(Point start, Point end, Point last, int moves_left)
        {
            Point diff = end - start;
            Point dirs = start.DirectionsTo(end);
            Point next_horizontal = start + new Point(dirs.X, 0);
            Point next_vertical = start + new Point(0, dirs.Y);
            bool vertical_is_available = map.NodeIsType<AvailableNode>(next_vertical) && next_vertical != last;
            bool horizontal_is_available = map.NodeIsType<AvailableNode>(next_horizontal) && next_horizontal != last;
            bool extra_moves_available = diff.X + diff.Y < moves_left;
            if (!horizontal_is_available && vertical_is_available)
            {
                return FindShortestPath_Vertical(start, end, last, moves_left);
            }
            else if (!vertical_is_available && horizontal_is_available)
            {
                return FindShortestPath_Horizontal(start, end, last, moves_left);
            }
            else if (vertical_is_available && horizontal_is_available)
            {
                Tuple<bool, List<Point>> retval_vertical = FindShortestPath(next_vertical, end, start, moves_left - 1);
                Tuple<bool, List<Point>> retval_horizontal = FindShortestPath(next_horizontal, end, start, moves_left - 1);
                if (!retval_horizontal.Item1 && retval_vertical.Item1)
                    return retval_vertical;
                if (!retval_vertical.Item1 && retval_horizontal.Item1)
                    return retval_horizontal;
                if (retval_vertical.Item1 && retval_horizontal.Item1)
                {
                    if (retval_vertical.Item2.Count < retval_horizontal.Item2.Count)
                        return retval_vertical;
                    if (retval_horizontal.Item2.Count < retval_vertical.Item2.Count)
                        return retval_horizontal;
                    bool choose_vertical = random.Next(2) == 0;
                    if (choose_vertical)
                        return retval_vertical;
                    return retval_horizontal;
                }
            }
            return FindShortestPath_Backwards(start, end, last, moves_left);
        }

        private Tuple<bool, List<Point>> FindShortestPath_Backwards(Point start, Point end, Point last, int moves_left)
        {
            Point diff = end - start;
            Point dirs = start.DirectionsTo(end) * new Point(-1, -1);
            Point next_horizontal = start + new Point(dirs.X, 0);
            Point next_vertical = start + new Point(0, dirs.Y);
            bool vertical_is_available = map.NodeIsType<AvailableNode>(next_vertical) && next_vertical != last;
            bool horizontal_is_available = map.NodeIsType<AvailableNode>(next_horizontal) && next_horizontal != last;
            bool extra_moves_available = diff.X + diff.Y < moves_left;
            if (!horizontal_is_available && vertical_is_available)
            {
                return FindShortestPath_Vertical(start, end, last, moves_left);
            }
            else if (!vertical_is_available && horizontal_is_available)
            {
                Tuple<bool, List<Point>> retval = FindShortestPath(next_horizontal, end, start, moves_left - 1);
                if (!retval.Item1)
                    return retval;
                retval.Item2.Add(start);
                return retval;
            }
            else if (vertical_is_available && horizontal_is_available)
            {
                Tuple<bool, List<Point>> retval_vertical = FindShortestPath(next_vertical, end, start, moves_left - 1);
                Tuple<bool, List<Point>> retval_horizontal = FindShortestPath(next_horizontal, end, start, moves_left - 1);
                if (!retval_horizontal.Item1 && retval_vertical.Item1)
                    return retval_vertical;
                if (!retval_vertical.Item1 && retval_horizontal.Item1)
                    return retval_horizontal;
                if (retval_vertical.Item1 && retval_horizontal.Item1)
                {
                    if (retval_vertical.Item2.Count < retval_horizontal.Item2.Count)
                        return retval_vertical;
                    if (retval_horizontal.Item2.Count < retval_vertical.Item2.Count)
                        return retval_horizontal;
                    bool choose_vertical = random.Next(2) == 0;
                    if (choose_vertical)
                        return retval_vertical;
                    return retval_horizontal;
                }
            }
            return Tuple.Create(false, new List<Point>());
        }

        private Tuple<bool, List<Point>> FindShortestPath_Vertical(Point start, Point end, Point last, int moves_left)
        {
            Point diff = end - start;
            Point dirs = start.DirectionsTo(end) * new Point(-1, -1);
            Point next_vertical = start + new Point(0, dirs.Y);
            bool extra_moves_available = diff.X + diff.Y < moves_left;
            Tuple<bool, List<Point>> retval = FindShortestPath(next_vertical, end, start, moves_left - 1);
            if (!retval.Item1 && extra_moves_available)
            {
                return FindShortestPath_Backwards(start, end, last, moves_left);
            }
            else if (!retval.Item1)
            {
                return retval;
            }
            retval.Item2.Add(start);
            return retval;
        }

        private Tuple<bool, List<Point>> FindShortestPath_Horizontal(Point start, Point end, Point last, int moves_left)
        {
            Point diff = end - start;
            Point dirs = start.DirectionsTo(end) * new Point(-1, -1);
            Point next_horizontal = start + new Point(dirs.X, 0);
            bool extra_moves_available = diff.X + diff.Y < moves_left;
            Tuple<bool, List<Point>> retval = FindShortestPath(next_horizontal, end, start, moves_left - 1);
            if (!retval.Item1 && extra_moves_available)
            {
                return FindShortestPath_Backwards(start, end, last, moves_left);
            }
            else if (!retval.Item1)
            {
                return retval;
            }
            retval.Item2.Add(start);
            return retval;
        }
    }
}
