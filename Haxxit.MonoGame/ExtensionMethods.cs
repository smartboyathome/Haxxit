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
        public static Rectangle ToXNARectangle(this Haxxit.Maps.Point p, int rectangle_size, int border_size)
        {
            rectangle_size = rectangle_size < 0 ? 0 : rectangle_size;
            border_size = border_size < 0 ? 0 : border_size;
            return new Rectangle((p.X + 1) * (rectangle_size + border_size), (p.Y + 1) * (rectangle_size + border_size),
                rectangle_size, rectangle_size);
        }
    }
}
