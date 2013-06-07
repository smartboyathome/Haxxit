using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Haxxit = SmartboyDevelopments.Haxxit;

namespace SmartboyDevelopments.Haxxit.MonoGame.GameStates
{
    public class SpawnDialogGameState : HaxxitGameState
    {
        const int SCROLLBARHEIGHT = 100;
        const int SCROLLBARWIDTH = 10;

        Texture2D rectangle_texture;
        DrawableRectangle overlay, popup_window, scrollUpButton, scrollDownButton, scrollBar, scrollGrip;
        SpriteFont arial_16px_regular, arial_14px_regular, arial_12px_regular, arial_10px_regular;
        HaxxitGameState background_state;
        Haxxit.Maps.Map map;
        Haxxit.Maps.Point spawn_point;
        //Dictionary<Haxxit.Programs.ProgramFactory, DrawableRectangle> program_rectangles;
        List<Tuple<Haxxit.Programs.ProgramFactory, DrawableRectangle>> program_rectangles;
        int totalPrograms;
        int currentScrollLevel;
        int total_spawn_points;

        public SpawnDialogGameState(HaxxitGameState background_state, Haxxit.Maps.Map map, Haxxit.Maps.Point spawn_point) :
            base()
        {
            this.background_state = background_state;
            this.map = map;
            this.spawn_point = spawn_point;
        }

        public override void Init()
        {
            totalPrograms = 0;
            currentScrollLevel = 0;
            //program_rectangles = new Dictionary<Haxxit.Programs.ProgramFactory, DrawableRectangle>();
            program_rectangles = new List<Tuple<Haxxit.Programs.ProgramFactory, DrawableRectangle>>();
            total_spawn_points = 0;

            foreach (Haxxit.Maps.Point p in map.Low.IterateOverRange(map.High))
            {
                if (p != spawn_point && map.NodeIsType<Haxxit.Maps.SpawnNode>(p) && map.GetNode<Haxxit.Maps.SpawnNode>(p).program != null)
                {
                    total_spawn_points += map.GetNode<Haxxit.Maps.SpawnNode>(p).program.SpawnWeight;
                }
            }
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            overlay = new DrawableRectangle(rectangle_texture, new Rectangle(0, 0, 800, 480), Color.Black * 0.5f);
            popup_window = new DrawableRectangle(rectangle_texture, new Rectangle(200, 90, 400, 350), Color.DarkBlue);
            scrollUpButton = new DrawableRectangle(rectangle_texture, new Rectangle(610, 90, 50, 50), Color.Yellow);
            scrollUpButton.OnMouseLeftClick += OnScrollUp;
            scrollDownButton = new DrawableRectangle(rectangle_texture, new Rectangle(610, 150, 50, 50), Color.Yellow);
            scrollDownButton.OnMouseLeftClick += OnScrollDown;

            scrollBar = new DrawableRectangle(rectangle_texture, new Rectangle(700, 90, 10, 100), Color.DarkGray);
            scrollGrip = new DrawableRectangle(rectangle_texture, new Rectangle(700, 90, 10, 10), Color.Yellow);

            arial_16px_regular = content.Load<SpriteFont>("Arial-16px-Regular");
            arial_14px_regular = content.Load<SpriteFont>("Arial-14px-Regular");
            arial_12px_regular = content.Load<SpriteFont>("Arial-12px-Regular");
            arial_10px_regular = content.Load<SpriteFont>("Arial-10px-Regular");

            List<Haxxit.Programs.ProgramFactory> programs = new List<Haxxit.Programs.ProgramFactory>();
            programs.Add(null);
            foreach (Haxxit.Programs.ProgramFactory program in programs.Concat(map.CurrentPlayer.GetPrograms()))
            {
                Rectangle program_location = new Rectangle(popup_window.Area.X + 10,
                    popup_window.Area.Y + program_rectangles.Count * 72 + 48, popup_window.Area.Width - 20, 60);
                DrawableRectangle program_rectangle = new DrawableRectangle(rectangle_texture, program_location, Color.White * 0.2f);
                program_rectangle.OnMouseLeftClick += OnProgramSelect;
                program_rectangles.Add(new Tuple<Haxxit.Programs.ProgramFactory, DrawableRectangle>(program, program_rectangle));
                totalPrograms++;
            }
            
        }

        public override void SubscribeAll()
        {
            // Add any channels you are subscribing to here by doing the following:
            // Mediator.Subscribe("haxxit.xxx.yyy.zzz", NameOfListener);
            //
            // You must also have a function in this class with the following structure:
            // public void NameOfListener(string channel, object sender, EventArgs args)
            // With channel being the channel the notification is sent through, sender
            // being the object that sent the notification, and EventArgs being the
            // arguments for the listener. If you need arguments, create a subclass of
            // EventArgs with the arguments as properties.
        }

        private void OnScrollUp(DrawableRectangle rectangle)
        {
            if (currentScrollLevel > 0)
            {
                foreach (DrawableRectangle program_rectangle in program_rectangles.Select(x => x.Item2))
                {
                    Rectangle area = program_rectangle.Area;
                    area.Y += 72;
                    program_rectangle.Area = area;
                }
                currentScrollLevel--;
            }
        }

        private void OnScrollDown(DrawableRectangle rectangle)
        {
            if (currentScrollLevel < totalPrograms - 4)
            {
                foreach (DrawableRectangle program_rectangle in program_rectangles.Select(x => x.Item2))
                {
                    Rectangle area = program_rectangle.Area;
                    area.Y -= 72;
                    program_rectangle.Area = area;
                }
                currentScrollLevel++;
            }
        }

        private void OnProgramSelect(DrawableRectangle rectangle)
        {
            foreach (Tuple<Haxxit.Programs.ProgramFactory, DrawableRectangle> tuple in program_rectangles)
            {
                if (Object.ReferenceEquals(tuple.Item2, rectangle))
                {
                    map.SpawnProgram(tuple.Item1, spawn_point);
                    break;
                }
            }
            _mediator_manager.Notify("haxxit.engine.state.pop", this, new EventArgs());
        }

        public override void Update()
        {
            // To change a scene, just call one of these:
            // Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.pop", this, new EventArgs());
            scrollUpButton.Update();
            scrollDownButton.Update();
            foreach (DrawableRectangle program_rectangle in program_rectangles.Select(x => x.Item2))
            {
                program_rectangle.Update();
            }
            MouseState mouse_state = Mouse.GetState();
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y);
            bool mouse_within_element = scrollDownButton.Area.Contains(mouse_position) || scrollUpButton.Area.Contains(mouse_position)
                || popup_window.Area.Contains(mouse_position);
            if (!mouse_within_element && mouse_state.LeftButton == ButtonState.Pressed)
                _mediator_manager.Notify("haxxit.engine.state.pop", this, new EventArgs());
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            background_state.Draw(sprite_batch);
            overlay.Draw(sprite_batch);
            popup_window.Draw(sprite_batch);
            scrollUpButton.Draw(sprite_batch);
            sprite_batch.DrawString(arial_12px_regular, "Scroll\nUp", new Vector2(scrollUpButton.Area.X + 5, scrollUpButton.Area.Y + 5), Color.Black);
            scrollDownButton.Draw(sprite_batch);
            sprite_batch.DrawString(arial_12px_regular, "Scroll\nDown", new Vector2(scrollDownButton.Area.X + 5, scrollDownButton.Area.Y + 5), Color.Black);
            scrollBar.Draw(sprite_batch);
            scrollGrip.Draw(sprite_batch);
            Vector2 title_size = arial_16px_regular.MeasureString("Select a program");
            Vector2 title_location = new Vector2(popup_window.Area.X + (popup_window.Area.Width - title_size.X)/2, popup_window.Area.Y + 10);
            sprite_batch.DrawString(arial_16px_regular, "Select a program", title_location, Color.White);
            int currentProgram = 0;
            foreach (Tuple<Haxxit.Programs.ProgramFactory, DrawableRectangle> tuple in program_rectangles)
            {
                if (currentProgram >= currentScrollLevel && currentProgram < currentScrollLevel + 4)
                {
                    tuple.Item2.Draw(sprite_batch);
                    string type_name = tuple.Item1 == null ? "None" : tuple.Item1.TypeName;
                    int spawn_weight = tuple.Item1 == null ? 0 : tuple.Item1.SpawnWeight;
                    int moves = tuple.Item1 == null ? 0 : tuple.Item1.Moves;
                    int size = tuple.Item1 == null ? 0 : tuple.Item1.Size;
                    Vector2 name_size = arial_16px_regular.MeasureString(type_name);
                    Vector2 name_location = new Vector2(tuple.Item2.Area.X + 2, tuple.Item2.Area.Y + 2);
                    if(spawn_weight <= map.TotalSpawnWeights - total_spawn_points)
                        sprite_batch.DrawString(arial_16px_regular, type_name, name_location, Color.White);
                    else
                        sprite_batch.DrawString(arial_16px_regular, type_name, name_location, Color.Gray);
                    string info = "Moves: " + moves.ToString() + "   Size: " + size.ToString()
                        + "   Spawn Weight: " + spawn_weight.ToString();
                    if (tuple.Item1 != null)
                    {
                        info += "\nCommands: ";
                        bool first = true;
                        foreach (Haxxit.Commands.Command command in tuple.Item1.Commands)
                        {
                            if (!first)
                                info += ", ";
                            else
                                first = false;
                            info += command.Name;
                            //info += " (Range: " + command.Range.ToString() + "; " + command.Description + ")";
                        }
                    }
                    else
                    {
                        info += "\nRemoves the current program from the spawn.";
                    }
                    Vector2 move_size = arial_10px_regular.MeasureString(info);
                    Vector2 move_location = new Vector2(tuple.Item2.Area.X + 2, tuple.Item2.Area.Y + 24);
                    sprite_batch.DrawString(arial_10px_regular, info, move_location, Color.White);
                }
                currentProgram++;
            }
            if (currentScrollLevel > 0)
            {
                sprite_batch.DrawString(arial_16px_regular, "...", new Vector2(popup_window.Area.X + 10, popup_window.Area.Y + 20), Color.White);
            }
            if (currentScrollLevel < totalPrograms - 4)
            {
                sprite_batch.DrawString(arial_16px_regular, "...", new Vector2(popup_window.Area.X + 10, popup_window.Area.Y + popup_window.Area.Height - 30), Color.White);
            }
        }
    }
}
