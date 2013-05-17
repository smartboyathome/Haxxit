using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SmartboyDevelopments.Haxxit;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    static class GlobalAccessors
    {
        public static Player mPlayer1{get; set;}
        public static HaxxitEngine mGame { get; set; }
        public static GameTime mGameTime { get; set; }
    }
}
