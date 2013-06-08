using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Haxxit = SmartboyDevelopments.Haxxit;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    static class ExtensionMethods
    {
        /// <summary>
        /// Converts a Haxxit point to an XNA Rectangle, since every point in Haxxit represents a range of pixels on screen.
        /// </summary>
        /// <param name="p">The point object that is being converted.</param>
        /// <param name="rectangle_size">The size of the squares on the screen.</param>
        /// <param name="border_size">The amount of border between each square.</param>
        /// <returns>An XNA Rectangle object of the appropriate size.</returns>
        public static Rectangle ToXNARectangle(this Haxxit.Maps.Point p, int rectangle_size, int border_size)
        {
            rectangle_size = rectangle_size < 0 ? 0 : rectangle_size;
            border_size = border_size < 0 ? 0 : border_size;
            return new Rectangle((p.X + 1) * (rectangle_size + border_size), (p.Y + 1) * (rectangle_size + border_size),
                rectangle_size, rectangle_size);
        }

        public static Haxxit.Maps.Point ToHaxxitPoint(this Point p, int rectangle_size, int border_size)
        {
            return (new Vector2(p.X, p.Y)).ToHaxxitPoint(rectangle_size, border_size);
        }

        public static Haxxit.Maps.Point ToHaxxitPoint(this Vector2 p, int rectangle_size, int border_size)
        {
            rectangle_size = rectangle_size < 0 ? 0 : rectangle_size;
            border_size = border_size < 0 ? 0 : border_size;
            // p.X = (result.X + 1) * (rectangle_size + border_size)
            // p.X / (rectangle_size + border_size) = result.X + 1
            // (p.X / (rectangle_size + border_size)) - 1 = result.X
            return new Haxxit.Maps.Point((int)Math.Floor((p.X / (rectangle_size + border_size)) - 1),
                (int)Math.Floor((p.Y / (rectangle_size + border_size)) - 1));
        }

        public static Rectangle DeepCopy(this Rectangle rectangle)
        {
            return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ScaleBy(this Rectangle rectangle, double scalar, bool ceiling=true)
        {
            double new_width = rectangle.Width * scalar;
            double new_height = rectangle.Height * scalar;
            rectangle.Width = ceiling ? (int)Math.Ceiling(new_width) : (int)Math.Floor(new_width);
            rectangle.Height = ceiling ? (int)Math.Ceiling(new_height) : (int)Math.Floor(new_height);
            return rectangle;
        }

        public static Rectangle CenterOn(this Rectangle rectangle, Rectangle other)
        {
            rectangle.X = other.X + (other.Width - rectangle.Width) / 2;
            rectangle.Y = other.Y + (other.Height - rectangle.Height) / 2;
            return rectangle;
        }

        public static IEnumerable<T> ShallowCopy<T>(this IEnumerable<T> enumerable)
        {
            List<T> list = new List<T>();
            foreach (T element in enumerable)
                list.Add(element);
            return list;
        }
    }
}
