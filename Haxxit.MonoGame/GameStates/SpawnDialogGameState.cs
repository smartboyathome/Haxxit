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
        Texture2D rectangle_texture, scrollUpTexture, scrollDownTexture, scrollGripTexture;
        Texture2D selectTexture, cancelTexture;
        DrawableRectangle overlay, popup_window, scrollUpButton, scrollDownButton, scrollBar, scrollGrip, aboveGrip, belowGrip, commandInfoBox;
        DrawableRectangle selectProgramButton, cancelButton;
        SpriteFont arial_16px_regular, arial_14px_regular, arial_12px_regular, arial_10px_regular;
        HaxxitGameState background_state;
        Haxxit.Maps.Map map;
        Haxxit.Programs.ProgramFactory selectedProgram;
        DrawableRectangle lastSelectedRectangle;
        Haxxit.Maps.Point spawn_point;
        List<Tuple<Haxxit.Programs.ProgramFactory, DrawableRectangle>> program_rectangles;
        int totalPrograms;
        int currentScrollLevel;
        int total_spawn_points;
        int interval;
        int lastScrollWheelValue;
        int[] scrollGripPositions;
        bool mouseIsDraggingGrip;
        BoolWrapper spawnDialogStatus;
        ButtonHover buttonHover;

        public SpawnDialogGameState(HaxxitGameState background_state, Haxxit.Maps.Map map, Haxxit.Maps.Point spawn_point, BoolWrapper newSpawnDialogStatus) :
            base()
        {
            this.background_state = background_state;
            this.map = map;
            this.spawn_point = spawn_point;
            spawnDialogStatus = newSpawnDialogStatus;
            spawnDialogStatus.Status = true;
            selectedProgram = null;
            lastSelectedRectangle = null;
        }

        public override void Init()
        {
            totalPrograms = 0;
            currentScrollLevel = 0;
            program_rectangles = new List<Tuple<Haxxit.Programs.ProgramFactory, DrawableRectangle>>();
            total_spawn_points = 0;

            foreach (Haxxit.Maps.Point p in map.Low.IterateOverRange(map.High))
            {
                if (p != spawn_point && map.NodeIsType<Haxxit.Maps.SpawnNode>(p) && map.GetNode<Haxxit.Maps.SpawnNode>(p).program != null)
                {
                    total_spawn_points += map.GetNode<Haxxit.Maps.SpawnNode>(p).program.SpawnWeight;
                }
            }
            lastScrollWheelValue = int.MinValue;
            mouseIsDraggingGrip = false;
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            selectTexture = content.Load<Texture2D>("Map-Button-TurnDone");
            cancelTexture = content.Load<Texture2D>("Map-Button-LeaveMap");
            overlay = new DrawableRectangle(rectangle_texture, new Rectangle(0, 0, 800, 480), Color.Black * 0.5f);
            popup_window = new DrawableRectangle(rectangle_texture, new Rectangle(350, 20, 400, 350), Color.DarkBlue);
            commandInfoBox = new DrawableRectangle(rectangle_texture, new Rectangle(10, 385, 650, 60), Color.DarkBlue);
            cancelButton = new DrawableRectangle(cancelTexture, new Rectangle(740, 400, 32, 32), Color.Red);
            cancelButton.OnMouseLeftClick += OnCancelClick;
            cancelButton.OnMouseInside += OnButtonInside;
            cancelButton.OnMouseOutside += OnButtonOutside;
            buttonHover = null;

            arial_16px_regular = content.Load<SpriteFont>("Arial-16px-Regular");
            arial_14px_regular = content.Load<SpriteFont>("Arial-14px-Regular");
            arial_12px_regular = content.Load<SpriteFont>("Arial-12px-Regular");
            arial_10px_regular = content.Load<SpriteFont>("Arial-10px-Regular");

            List<Haxxit.Programs.ProgramFactory> programs = new List<Haxxit.Programs.ProgramFactory>();
            programs.Add(null); // For "None" option
            foreach (Haxxit.Programs.ProgramFactory program in programs.Concat(map.CurrentPlayer.GetPrograms()))
            {
                Rectangle program_location = new Rectangle(popup_window.Area.X + 10,
                    popup_window.Area.Y + program_rectangles.Count * 72 + 48, popup_window.Area.Width - 20, 60);
                DrawableRectangle program_rectangle = new DrawableRectangle(rectangle_texture, program_location, Color.White * 0.2f);
                program_rectangle.OnMouseLeftClick += OnProgramSelect;
                program_rectangles.Add(new Tuple<Haxxit.Programs.ProgramFactory, DrawableRectangle>(program, program_rectangle));
                totalPrograms++;
            }

            // If there are enough programs that we'll need to support scrolling...
            if (totalPrograms > 4)
            {
                scrollUpTexture = content.Load<Texture2D>("ArrowUp");
                scrollUpButton = new DrawableRectangle(rectangle_texture, new Rectangle(750, 20, 50, 50), Color.White);
                scrollUpButton.OnMouseLeftClick += OnScrollUp;

                scrollDownTexture = content.Load<Texture2D>("ArrowDown");
                scrollDownButton = new DrawableRectangle(rectangle_texture, new Rectangle(750, 320, 50, 50), Color.White);
                scrollDownButton.OnMouseLeftClick += OnScrollDown;

                scrollGripTexture = content.Load<Texture2D>("ScrollGrip");
                scrollBar = new DrawableRectangle(rectangle_texture, new Rectangle(760, 70, 30, 250), Color.DimGray);
                scrollGrip = new DrawableRectangle(rectangle_texture, new Rectangle(760, 70, 30, 50), Color.White);
                aboveGrip = new DrawableRectangle(rectangle_texture, new Rectangle(760, 70, 30, 0), Color.Red);
                aboveGrip.OnMouseLeftClick += OnScrollUp;
                belowGrip = new DrawableRectangle(rectangle_texture, new Rectangle(760, 120, 30, 200), Color.Green);
                belowGrip.OnMouseLeftClick += OnScrollDown;

                // Determine scroll grip positions for future use
                scrollGripPositions = new int[totalPrograms - 3];
                int startingPosition = scrollGrip.Area.Location.Y;
                int endingPosition = startingPosition + scrollBar.Area.Height - scrollGrip.Area.Height;
                interval = (endingPosition - startingPosition) / (totalPrograms - 4);
                for (int i = 0; i < totalPrograms - 3; i++)
                {
                    scrollGripPositions[i] = startingPosition + interval * i;
                }
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
                Rectangle scrollGripPosition = scrollGrip.Area;
                scrollGripPosition.Y = scrollGripPositions[currentScrollLevel];
                scrollGrip.Area = scrollGripPosition;

                Rectangle modifyAboveRectangle = aboveGrip.Area;
                modifyAboveRectangle.Height = scrollGrip.Area.Y - scrollBar.Area.Y;
                aboveGrip.Area = modifyAboveRectangle;

                Rectangle modifyBelowRectangle = belowGrip.Area;
                modifyBelowRectangle.Y = scrollGrip.Area.Y + scrollGrip.Area.Height;
                modifyBelowRectangle.Height = scrollBar.Area.Y + scrollBar.Area.Height - (scrollGrip.Area.Y + scrollGrip.Area.Height);
                belowGrip.Area = modifyBelowRectangle;
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
                Rectangle scrollGripPosition = scrollGrip.Area;
                scrollGripPosition.Y = scrollGripPositions[currentScrollLevel];
                scrollGrip.Area = scrollGripPosition;

                Rectangle modifyAboveRectangle = aboveGrip.Area;
                modifyAboveRectangle.Height = scrollGrip.Area.Y - scrollBar.Area.Y;
                aboveGrip.Area = modifyAboveRectangle;

                Rectangle modifyBelowRectangle = belowGrip.Area;
                modifyBelowRectangle.Y = scrollGrip.Area.Y + scrollGrip.Area.Height;
                modifyBelowRectangle.Height = scrollBar.Area.Y + scrollBar.Area.Height - (scrollGrip.Area.Y + scrollGrip.Area.Height);
                belowGrip.Area = modifyBelowRectangle;
            }
        }

        public void OnSelectClick(DrawableRectangle rectangle)
        {
            map.SpawnProgram(selectedProgram, spawn_point);
            spawnDialogStatus.Status = false;
            _mediator_manager.Notify("haxxit.engine.state.pop", this, new EventArgs());
        }

        public void OnCancelClick(DrawableRectangle rectangle)
        {
            spawnDialogStatus.Status = false;
            _mediator_manager.Notify("haxxit.engine.state.pop", this, new EventArgs());
        }

        public void OnButtonInside(DrawableRectangle rectangle)
        {
            string tooltip = "";
            if (rectangle == selectProgramButton)
                tooltip = "Select this program for spawning.";
            else if (rectangle == cancelButton)
                tooltip = "Leave (exit) the spawn program dialog.";
            buttonHover = new ButtonHover(rectangle, rectangle_texture, arial_12px_regular, tooltip);
        }

        public void OnButtonOutside(DrawableRectangle rectangle)
        {
            MouseState mouse_state = Mouse.GetState();
            if(selectProgramButton == null)
            {
                if(!cancelButton.Area.Contains(mouse_state.X, mouse_state.Y))
                {
                    buttonHover = null;
                }                
            }
            else if (!selectProgramButton.Area.Contains(mouse_state.X, mouse_state.Y)
                     && !cancelButton.Area.Contains(mouse_state.X, mouse_state.Y))
            {
                buttonHover = null;
            }
        }

        private void OnProgramSelect(DrawableRectangle rectangle)
        {
            foreach (Tuple<Haxxit.Programs.ProgramFactory, DrawableRectangle> tuple in program_rectangles)
            {
                if (Object.ReferenceEquals(tuple.Item2, rectangle))
                {
                    selectedProgram = tuple.Item1;
                    if (lastSelectedRectangle != null)
                    {
                        lastSelectedRectangle.FillColor = Color.White * 0.2f;
                    }
                    tuple.Item2.FillColor = Color.Yellow * 0.5f;
                    lastSelectedRectangle = tuple.Item2;
                    break;
                }
            }
            if (selectProgramButton == null)
            {
                selectProgramButton = new DrawableRectangle(selectTexture, new Rectangle(700, 400, 32, 32), Color.Green);
                selectProgramButton.OnMouseLeftClick += OnSelectClick;
                selectProgramButton.OnMouseInside += OnButtonInside;
                selectProgramButton.OnMouseOutside += OnButtonOutside;
            }
        }

        public override void Update()
        {
            // To change a scene, just call one of these:
            // Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.pop", this, new EventArgs());

            MouseState mouse_state = Mouse.GetState(); // Gets the mouse state object
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y); // creates a point for the mouse's position

            // If there are enough programs that we need to support scrolling...
            if (totalPrograms > 4)
            {
                // Track whether the user is dragging the scrollGrip or not
                if (mouse_state.LeftButton == ButtonState.Released)
                {
                    mouseIsDraggingGrip = false;
                }
                else if (scrollGrip.Area.Contains(mouse_position) && mouse_state.LeftButton == ButtonState.Pressed)
                {
                    mouseIsDraggingGrip = true;
                }

                // Scroll the list according to the mouse and grip positions
                if (mouseIsDraggingGrip)
                {
                    int intervals = (mouse_position.Y - scrollGrip.Area.Y) / interval;
                    if (intervals < 0)
                    {
                        for (int i = 0; i > intervals; i--)
                        {
                            OnScrollUp(scrollGrip);
                        }
                    }
                    else if (intervals > 0)
                    {
                        for (int i = 0; i < intervals; i++)
                        {
                            OnScrollDown(scrollGrip);
                        }
                    }
                }

                // Scroll the list using the mousewheel
                if (mouse_state.ScrollWheelValue > lastScrollWheelValue)
                {
                    OnScrollUp(scrollGrip);
                    lastScrollWheelValue = mouse_state.ScrollWheelValue;
                }
                else if (mouse_state.ScrollWheelValue < lastScrollWheelValue)
                {
                    OnScrollDown(scrollGrip);
                    lastScrollWheelValue = mouse_state.ScrollWheelValue;
                }

                // Scroll the list by up and down buttons
                scrollUpButton.Update();
                scrollDownButton.Update();

                // Scroll the list by clicking on the void space between the grip and the tops and bottoms of the scroll bar
                aboveGrip.Update();
                belowGrip.Update();
            }

            int currentProgram = 0;
            foreach (Tuple<Haxxit.Programs.ProgramFactory, DrawableRectangle> tuple in program_rectangles)
            {
                if (currentProgram >= currentScrollLevel && currentProgram < currentScrollLevel + 4)
                {
                    if (tuple.Item1 == null || tuple.Item1.SpawnWeight <= map.TotalSpawnWeights - total_spawn_points)
                    {
                        tuple.Item2.Update();
                    }
                }
                currentProgram++;
            }

            if (selectProgramButton != null)
            {
                selectProgramButton.Update();
            }
            cancelButton.Update();
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            background_state.Draw(sprite_batch);
            overlay.Draw(sprite_batch);
            popup_window.Draw(sprite_batch);
            if (selectProgramButton != null)
            {
                selectProgramButton.Draw(sprite_batch);
            }
            cancelButton.Draw(sprite_batch);

            // If there are enough programs that we need to support scrolling...
            if (totalPrograms > 4)
            {
                scrollBar.Draw(sprite_batch);
                //aboveGrip.Draw(sprite_batch); // Shows up red.  Useful for testing.
                //belowGrip.Draw(sprite_batch); // Shows up green.  Useful for testing.
                sprite_batch.Draw(scrollGripTexture, scrollGrip.Area, Color.White);
                sprite_batch.Draw(scrollUpTexture, scrollUpButton.Area, Color.White);
                sprite_batch.Draw(scrollDownTexture, scrollDownButton.Area, Color.White);
            }

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
            if (selectedProgram != null)
            {
                commandInfoBox.Draw(sprite_batch);
                sprite_batch.DrawString(arial_16px_regular, selectedProgram.TypeName, new Vector2(10, 360), Color.White);
                string commandInfo = "";
                foreach (Haxxit.Commands.Command command in selectedProgram.Commands)
                {
                    commandInfo += command.Name;
                    commandInfo += " (Range: " + command.Range.ToString() + "); " + command.Description + "";
                    commandInfo += "\n";
                }
                sprite_batch.DrawString(arial_12px_regular, commandInfo, new Vector2(12, 385), Color.White);
            }
            if (buttonHover != null)
            {
                buttonHover.Draw(sprite_batch);
            }
        }
    }
}
