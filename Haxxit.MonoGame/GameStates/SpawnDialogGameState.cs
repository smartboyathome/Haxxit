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
        Texture2D rectangle_texture;
        DrawableRectangle overlay, popup_window, scrollUpButton, scrollDownButton;
        SpriteFont arial_16px_regular, arial_14px_regular, arial_12px_regular, arial_10px_regular;
        HaxxitGameState background_state;
        Haxxit.Maps.Map map;
        Haxxit.Maps.Point spawn_point;
        Dictionary<Haxxit.Programs.ProgramFactory, DrawableRectangle> program_rectangles;
        int totalPrograms;
        int currentScrollLevel;

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
            program_rectangles = new Dictionary<Haxxit.Programs.ProgramFactory, DrawableRectangle>();
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
            arial_16px_regular = content.Load<SpriteFont>("Arial-16px-Regular");
            arial_14px_regular = content.Load<SpriteFont>("Arial-14px-Regular");
            arial_12px_regular = content.Load<SpriteFont>("Arial-12px-Regular");
            arial_10px_regular = content.Load<SpriteFont>("Arial-10px-Regular");
            
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
            if (currentScrollLevel < totalPrograms - 4)
            {
                foreach (KeyValuePair<Haxxit.Programs.ProgramFactory, DrawableRectangle> pair in program_rectangles)
                {
                    DrawableRectangle program_rectangle = pair.Value;
                    Rectangle area = program_rectangle.Area;
                    area.Y -= 72;
                    program_rectangle.Area = area;
                }
                currentScrollLevel++;
            }
        }

        private void OnScrollDown(DrawableRectangle rectangle)
        {
            if (currentScrollLevel > 0)
            {
                foreach (KeyValuePair<Haxxit.Programs.ProgramFactory, DrawableRectangle> pair in program_rectangles)
                {
                    DrawableRectangle program_rectangle = pair.Value;
                    Rectangle area = program_rectangle.Area;
                    area.Y += 72;
                    program_rectangle.Area = area;
                }
                currentScrollLevel--;
            }
        }

        private void OnProgramSelect(DrawableRectangle rectangle)
        {
            foreach (KeyValuePair<Haxxit.Programs.ProgramFactory, DrawableRectangle> pair in program_rectangles)
            {
                if (Object.ReferenceEquals(pair.Value, rectangle))
                {
                    map.SpawnProgram(pair.Key, spawn_point);
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
            foreach (Haxxit.Programs.ProgramFactory program in map.CurrentPlayer.GetPrograms())
            {
                if (!program_rectangles.ContainsKey(program))
                {
                    Rectangle program_location = new Rectangle(popup_window.Area.X + 10,
                        popup_window.Area.Y + program_rectangles.Count * 72 + 48, popup_window.Area.Width - 20, 60);
                    DrawableRectangle program_rectangle = new DrawableRectangle(rectangle_texture, program_location, Color.White * 0.2f);
                    program_rectangle.OnMouseLeftClick += OnProgramSelect;
                    program_rectangles.Add(program, program_rectangle);
                    totalPrograms++;
                }
                program_rectangles[program].Update();
            }
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
            Vector2 title_size = arial_16px_regular.MeasureString("Select a program");
            Vector2 title_location = new Vector2(popup_window.Area.X + (popup_window.Area.Width - title_size.X)/2, popup_window.Area.Y + 10);
            sprite_batch.DrawString(arial_16px_regular, "Select a program", title_location, Color.White);
            int currentProgram = 0;
            foreach (KeyValuePair<Haxxit.Programs.ProgramFactory, DrawableRectangle> pair in program_rectangles)
            {
                if (currentProgram >= currentScrollLevel && currentProgram < currentScrollLevel + 4)
                {
                    pair.Value.Draw(sprite_batch);
                    Vector2 name_size = arial_16px_regular.MeasureString(pair.Key.TypeName);
                    Vector2 name_location = new Vector2(pair.Value.Area.X + 2, pair.Value.Area.Y + 2);
                    sprite_batch.DrawString(arial_16px_regular, pair.Key.TypeName, name_location, Color.White);
                    string info = "Moves: " + pair.Key.Moves.ToString() + "   Size: " + pair.Key.Size.ToString()
                        + "   Spawn Weight: " + pair.Key.SpawnWeight.ToString() + "\nCommands: ";
                    bool first = true;
                    foreach (Haxxit.Commands.Command command in pair.Key.Commands)
                    {
                        if (!first)
                            info += ", ";
                        else
                            first = false;
                        info += command.Name;
                        //info += " (Range: " + command.Range.ToString() + "; " + command.Description + ")";
                    }
                    Vector2 move_size = arial_10px_regular.MeasureString(info);
                    Vector2 move_location = new Vector2(pair.Value.Area.X + 2, pair.Value.Area.Y + 24);
                    sprite_batch.DrawString(arial_10px_regular, info, move_location, Color.White);
                }
                currentProgram++;
            }
            if (currentScrollLevel > 0)
            {
                sprite_batch.DrawString(arial_16px_regular, "...", new Vector2(popup_window.Area.X + 10, popup_window.Area.Y + popup_window.Area.Height - 30), Color.White);
            }
            if (currentScrollLevel < totalPrograms - 4)
            {
                sprite_batch.DrawString(arial_16px_regular, "...", new Vector2(popup_window.Area.X + 10, popup_window.Area.Y + 20), Color.White);
            }
        }
    }
}
