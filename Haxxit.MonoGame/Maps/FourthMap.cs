﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Maps;
using SmartboyDevelopments.Haxxit.Commands;
using SmartboyDevelopments.Haxxit.MonoGame.Programs;
using SmartboyDevelopments.Haxxit.Programs;

namespace SmartboyDevelopments.Haxxit.MonoGame.Maps
{
    class FourthMapFactory : SpawnMapFactory
    {
        public FourthMapFactory()
        {
            width = 10;
            height = 10;
            initial_silicoins = 250;
            total_spawn_weight = 40;
            player1 = GlobalAccessors.mPlayer1;
            player2 = new PlayerAI("AI");
            player1_spawns = new List<Point>();
            player2_programs = new List<Tuple<ProgramFactory, Point, IEnumerable<Point>>>();
            player1_spawns.Add(new Point(0, 9));
            SwarmerFactory swarmerFactory = new SwarmerFactory();
            AddPlayer2Program(swarmerFactory, new Point(2, 8));
            AddPlayer2Program(swarmerFactory, new Point(6, 4));
            AddPlayer2Program(swarmerFactory, new Point(6, 6));
            WatcherFactory watcherFactory = new WatcherFactory();
            AddPlayer2Program(watcherFactory, new Point(2, 3));
            AddPlayer2Program(watcherFactory, new Point(4, 4));
            AddPlayer2Program(watcherFactory, new Point(6, 1));
            unavailableNodes = new List<Point>();
            unavailableNodes.Add(new Point(1, 1));
            unavailableNodes.Add(new Point(1, 2));
            unavailableNodes.Add(new Point(1, 3));
            unavailableNodes.Add(new Point(1, 4));
            unavailableNodes.Add(new Point(1, 5));
            unavailableNodes.Add(new Point(1, 6));
            unavailableNodes.Add(new Point(1, 7));
            unavailableNodes.Add(new Point(1, 8));
            unavailableNodes.Add(new Point(1, 9));
            unavailableNodes.Add(new Point(2, 9));
            unavailableNodes.Add(new Point(3, 0));
            unavailableNodes.Add(new Point(3, 1));
            unavailableNodes.Add(new Point(3, 3));
            unavailableNodes.Add(new Point(3, 4));
            unavailableNodes.Add(new Point(3, 5));
            unavailableNodes.Add(new Point(3, 6));
            unavailableNodes.Add(new Point(3, 7));
            unavailableNodes.Add(new Point(3, 9));
            unavailableNodes.Add(new Point(4, 3));
            unavailableNodes.Add(new Point(4, 7));
            unavailableNodes.Add(new Point(5, 1));
            unavailableNodes.Add(new Point(5, 2));
            unavailableNodes.Add(new Point(5, 3));
            unavailableNodes.Add(new Point(5, 4));
            unavailableNodes.Add(new Point(5, 5));
            unavailableNodes.Add(new Point(5, 7));
            unavailableNodes.Add(new Point(5, 9));
            unavailableNodes.Add(new Point(6, 2));
            unavailableNodes.Add(new Point(6, 5));
            unavailableNodes.Add(new Point(6, 9));
            unavailableNodes.Add(new Point(7, 0));
            unavailableNodes.Add(new Point(7, 2));
            unavailableNodes.Add(new Point(7, 3));
            unavailableNodes.Add(new Point(7, 5));
            unavailableNodes.Add(new Point(7, 6));
            unavailableNodes.Add(new Point(7, 7));
            unavailableNodes.Add(new Point(7, 8));
            unavailableNodes.Add(new Point(7, 9));
            unavailableNodes.Add(new Point(8, 0));
            unavailableNodes.Add(new Point(8, 6));
            unavailableNodes.Add(new Point(9, 2));
            unavailableNodes.Add(new Point(9, 3));
            unavailableNodes.Add(new Point(9, 4));
            unavailableNodes.Add(new Point(9, 8));
            unavailableNodes.Add(new Point(9, 9));
            silicoinNodes.Add(new Point(4, 9));
            silicoinNodes.Add(new Point(6, 3));
            silicoinNodes.Add(new Point(9, 0));
            dataNodes.Add(new Point(8, 9));
        }
    }
}
