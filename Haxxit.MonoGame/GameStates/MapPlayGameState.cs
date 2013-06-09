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
    class ButtonHover
    {
        DrawableRectangle button_hover, tooltip_box;
        string tooltip_text;
        Point tooltip_text_position;
        SpriteFont tooltip_font;
        public ButtonHover(DrawableRectangle rectangle, Texture2D white_pixel, SpriteFont tooltip_font, string tooltip_text)
        {
            button_hover = new DrawableRectangle(rectangle.texture, rectangle.Area, Color.White * 0.5f);
            this.tooltip_font = tooltip_font;
            this.tooltip_text = tooltip_text;
            Vector2 tooltip_text_size = tooltip_font.MeasureString(tooltip_text);
            Rectangle tooltip_rect = new Rectangle(0, button_hover.Area.Y, (int)Math.Ceiling(tooltip_text_size.X), (int)Math.Ceiling(tooltip_text_size.Y));
            tooltip_rect = tooltip_rect.LeftAlignOn(button_hover.Area).OffsetBy(0, -1 * tooltip_rect.Height - 6);
            tooltip_text_position = tooltip_rect.GetPosition();
            tooltip_rect = tooltip_rect.BufferBy(8);
            tooltip_box = new DrawableRectangle(white_pixel, tooltip_rect, Color.White, 2, Color.Black);
        }
        public void Draw(SpriteBatch sprite_batch)
        {
            button_hover.Draw(sprite_batch);
            if (tooltip_text != "")
            {
                tooltip_box.Draw(sprite_batch);
                sprite_batch.DrawString(tooltip_font, tooltip_text, tooltip_text_position.ToVector2(), Color.Black);
            }
        }
    }

    public class MapPlayGameState : HaxxitGameState
    {
        public MapDisplayGameState display_map_state;
        public DrawableRectangle turn_done_button, undo_button, leave_map_button;
        ButtonHover button_hover;

        Texture2D rectangle_texture, rounded_rect_back, undo_button_texture,
            turn_done_texture, leave_map_texture;
        SpriteFont arial_16px_regular, arial_12px_regular;
        Dictionary<Haxxit.Maps.Point, DrawableRectangle> head_nodes;
        bool is_ai_turn, first_update;

        public MapPlayGameState(MapDisplayGameState background_state)
        {
            display_map_state = background_state;
        }

        public override void NewMediator(SimplePubSub.IMediator mediator)
        {
            display_map_state.Mediator = mediator;
        }

        public override void Init()
        {
            head_nodes = new Dictionary<Haxxit.Maps.Point, DrawableRectangle>();
            is_ai_turn = false;
            first_update = true;
        }

        public void OnProgramClick(DrawableRectangle rectangle)
        {
            Haxxit.Maps.Point haxxit_location = display_map_state.XnaPointToHaxxitPoint(rectangle.Area.Center);
            if (display_map_state.Map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(haxxit_location)
                && !display_map_state.Map.GetNode<Haxxit.Maps.ProgramHeadNode>(haxxit_location).Program.AlreadyRanCommand())
            {
                MapMovementGameState new_state = new MapMovementGameState(this, haxxit_location);
                _mediator_manager.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
            }
        }

        public void OnTurnDoneClick(DrawableRectangle rectangle)
        {
            _mediator_manager.Notify("haxxit.map.turn_done", this, new EventArgs());
        }

        public void OnUndoClick(DrawableRectangle rectangle)
        {
            _mediator_manager.Notify("haxxit.undo_stack.trigger", this, new EventArgs());
        }

        public void OnLeaveMapClick(DrawableRectangle rectangle)
        {
            // This shouldn't be how it operates. I should be able to pop the play and display states off the stack,
            // but it just restarts the map if I do that. Instead, I have to create a new overworld state, clear the
            // stack, then push the new state onto the stack in order to avoid the stack eventually overflowing.
            HaxxitGameState new_state = new ServerOverworldState();
            _mediator_manager.Notify("haxxit.engine.state.clear_change", this, new ChangeStateEventArgs(new_state));
        }

        public void OnButtonInside(DrawableRectangle rectangle)
        {
            //button_hover = new DrawableRectangle(rectangle.texture, rectangle.Area, Color.White * 0.5f);
            string tooltip = "";
            if (rectangle == turn_done_button)
                tooltip = "Ends your turn.";
            else if (rectangle == undo_button)
                tooltip = "Undo previous action.";
            else if (rectangle == leave_map_button)
                tooltip = "Leave (exit) the current map.";
            button_hover = new ButtonHover(rectangle, rectangle_texture, arial_12px_regular, tooltip);
        }

        public void OnButtonOutside(DrawableRectangle rectangle)
        {
            MouseState mouse_state = Mouse.GetState();
            if(!undo_button.Area.Contains(mouse_state.X, mouse_state.Y)
               && !leave_map_button.Area.Contains(mouse_state.X, mouse_state.Y)
               && !turn_done_button.Area.Contains(mouse_state.X, mouse_state.Y))
                button_hover = null;
        }

        public DrawableRectangle DrawProgramHead(Haxxit.Maps.Point p, Haxxit.Maps.ProgramHeadNode head_node)
        {
            DrawableRectangle retval;
            if (head_node.Program.AlreadyRanCommand())
                retval = new DrawableRectangle(rounded_rect_back, display_map_state.HaxxitPointToXnaRectangle(p), Color.Black * (2.0f / 3.0f));
            else if (!head_node.Program.Moves.CanMove())
                retval = new DrawableRectangle(rounded_rect_back, display_map_state.HaxxitPointToXnaRectangle(p), Color.Black * (1.0f / 3.0f));
            else
                retval = new DrawableRectangle(rounded_rect_back, display_map_state.HaxxitPointToXnaRectangle(p), Color.Transparent);
            retval.OnMouseLeftClick += OnProgramClick;
            return retval;
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            rounded_rect_back = content.Load<Texture2D>("Map-Square-Background");
            undo_button_texture = content.Load<Texture2D>("Map-Button-Undo");
            turn_done_texture = content.Load<Texture2D>("Map-Button-TurnDone");
            leave_map_texture = content.Load<Texture2D>("Map-Button-LeaveMap");
            arial_16px_regular = content.Load<SpriteFont>("Arial-16px-Regular");
            arial_12px_regular = content.Load<SpriteFont>("Arial-12px-Regular");

            int button_size = 32;
            int buffer_size = 6;
            //turn_done_button = new DrawableRectangle(rectangle_texture, new Rectangle(690, 440, 100, 30), Color.Green);
            turn_done_button = new DrawableRectangle(turn_done_texture,
                new Rectangle(788 - button_size, 468 - button_size, button_size, button_size), Color.Green);
            turn_done_button.OnMouseLeftClick += OnTurnDoneClick;
            turn_done_button.OnMouseInside += OnButtonInside;
            turn_done_button.OnMouseOutside += OnButtonOutside;
            //undo_button = new DrawableRectangle(rectangle_texture, new Rectangle(580, 440, 100, 30), Color.Orange);
            undo_button = new DrawableRectangle(undo_button_texture,
                new Rectangle(788 - buffer_size - button_size * 2, 468 - button_size, button_size, button_size), Color.Yellow);
            undo_button.OnMouseLeftClick += OnUndoClick;
            undo_button.OnMouseInside += OnButtonInside;
            undo_button.OnMouseOutside += OnButtonOutside;
            //leave_map_button = new DrawableRectangle(rectangle_texture, new Rectangle(675, 400, 115, 30), Color.Red);
            leave_map_button = new DrawableRectangle(leave_map_texture,
                new Rectangle(788 - buffer_size * 2 - button_size * 3, 468 - button_size, button_size, button_size), Color.Red);
            leave_map_button.OnMouseLeftClick += OnLeaveMapClick;
            leave_map_button.OnMouseInside += OnButtonInside;
            leave_map_button.OnMouseOutside += OnButtonOutside;
            button_hover = null;

            Haxxit.Maps.Map map = display_map_state.Map;
            foreach (Haxxit.Maps.Point p in map.Low.IterateOverRange(map.High))
            {
                if (map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
                {
                    head_nodes[p] = DrawProgramHead(p, map.GetNode<Haxxit.Maps.ProgramHeadNode>(p));
                }
            }
        }

        private void MapHackedListener(string channel, object sender, EventArgs args)
        {
            Haxxit.Maps.HackedEventArgs event_args = args as Haxxit.Maps.HackedEventArgs;
            //WinGameState new_state = new WinGameState(event_args.EarnedSilicoins, this);
            WinGameState new_state = new WinGameState(display_map_state.Map.EarnedSilicoins, event_args.WinningPlayer, this);
            _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
        }

        private void SilicoinNodeListener(string channel, object sender, EventArgs args)
        {
            SilicoinEventArgs event_args = (SilicoinEventArgs)args;
            TempDialogGameState new_state = new TempDialogGameState(this, "Added " + event_args.Silicoins.ToString() + " silicoins.", 500);
            // This breaks everything for some reason.
            //_mediator_manager.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
        }

        private void MapChangedListener(string channel, object sender, EventArgs args)
        {
            Haxxit.Maps.MapChangedEventArgs event_args = (Haxxit.Maps.MapChangedEventArgs)args;
            foreach (Haxxit.Maps.Point p in event_args.ChangedNodes)
            {
                if (head_nodes.ContainsKey(p) && !display_map_state.Map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
                    head_nodes.Remove(p);
                else if (display_map_state.Map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
                {
                    head_nodes[p] = DrawProgramHead(p, display_map_state.Map.GetNode<Haxxit.Maps.ProgramHeadNode>(p));
                }
            }
        }

        private void TurnDoneListener(string channel, object sender, EventArgs args)
        {
            _mediator_manager.Notify("haxxit.undo_stack.clear", this, new EventArgs());
            TempDialogGameState new_state;
            // This does not work since SimplePubSub does not guarentee any ordering of subscribers.
            //if (display_map_state.Map.CurrentPlayer.GetType() == typeof(PlayerAI))
            if (!is_ai_turn)
            {
                new_state = new TempDialogGameState(this, "S.A.N.T.A.'s turn", 1000);
                is_ai_turn = true;
            }
            else
            {
                new_state = new TempDialogGameState(this, "Your turn", 1000);
                is_ai_turn = false;
            }
            foreach (DrawableRectangle rectangle in head_nodes.Values)
            {
                rectangle.FillColor = Color.Transparent;
            }
            _mediator_manager.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
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

            _mediator_manager.Subscribe("haxxit.map.hacked", MapHackedListener);
            _mediator_manager.Subscribe("haxxit.map.nodes.changed", MapChangedListener);
            _mediator_manager.Subscribe("haxxit.map.turn_done", TurnDoneListener);
            _mediator_manager.Subscribe("haxxit.map.silicoins.add", SilicoinNodeListener);
        }

        public override void Update()
        {
            // To change a scene, just call one of these:
            // Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.pop", this, new EventArgs());

            if (first_update)
            {
                _mediator_manager.Notify("haxxit.map.hacked.check", this, new EventArgs());
                first_update = false;
            }

            if (display_map_state.Map.CurrentPlayer.GetType() != typeof(Haxxit.MonoGame.PlayerAI))
            {
                foreach (DrawableRectangle head_node in head_nodes.Values)
                {
                    head_node.Update();
                }

                turn_done_button.Update();
                undo_button.Update();
                leave_map_button.Update();
            }
            else
            {
                ((Haxxit.MonoGame.PlayerAI)display_map_state.Map.CurrentPlayer).HandleAITurn(display_map_state.Map, this);
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            display_map_state.Draw(sprite_batch);

            turn_done_button.Draw(sprite_batch);
            /*Vector2 turn_done_size = arial_16px_regular.MeasureString("Turn Done");
            Vector2 turn_done_position = new Vector2(turn_done_button.Area.X + (turn_done_button.Area.Width - turn_done_size.X) / 2,
                turn_done_button.Area.Y + (turn_done_button.Area.Height - turn_done_size.Y) / 2);
            sprite_batch.DrawString(arial_16px_regular, "Turn Done", turn_done_position, Color.White);*/

            undo_button.Draw(sprite_batch);
            /*Vector2 undo_text_size = arial_16px_regular.MeasureString("Undo");
            Vector2 undo_text_position = new Vector2(undo_button.Area.X + (undo_button.Area.Width - undo_text_size.X) / 2,
                undo_button.Area.Y + (undo_button.Area.Height - undo_text_size.Y) / 2);
            sprite_batch.DrawString(arial_16px_regular, "Undo", undo_text_position, Color.White);*/

            leave_map_button.Draw(sprite_batch);
            /*Vector2 leave_map_text_size = arial_16px_regular.MeasureString("Leave Map");
            Vector2 leave_map_text_position = new Vector2(leave_map_button.Area.X + (leave_map_button.Area.Width - leave_map_text_size.X) / 2,
                leave_map_button.Area.Y + (leave_map_button.Area.Height - leave_map_text_size.Y) / 2);
            sprite_batch.DrawString(arial_16px_regular, "Leave Map", leave_map_text_position, Color.White);*/

            if (button_hover != null)
                button_hover.Draw(sprite_batch);

            foreach (DrawableRectangle head_node in head_nodes.Values)
            {
                head_node.Draw(sprite_batch);
            }
        }
    }
}
