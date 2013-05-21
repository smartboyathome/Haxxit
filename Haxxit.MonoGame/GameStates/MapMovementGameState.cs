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
    public class MapMovementGameState : HaxxitGameState
    {
        MapPlayGameState user_map_state;
        Texture2D rectangle_texture;
        SpriteFont arial_16px_regular, arial_12px_regular;
        List<DrawableRectangle> movement;
        List<Tuple<DrawableRectangle, string>> attacks;
        DrawableRectangle selected_border;
        Haxxit.Maps.Point selected_program;

        public MapMovementGameState(MapPlayGameState background_state, Haxxit.Maps.Point selected_program)
        {
            user_map_state = background_state;
            this.selected_program = selected_program;
        }

        public override void NewMediator(SimplePubSub.IMediator mediator)
        {
            user_map_state.Mediator = mediator;
        }

        public override void Init()
        {
            movement = new List<DrawableRectangle>();
            attacks = new List<Tuple<DrawableRectangle, string>>();
        }

        public void OnMoveClick(DrawableRectangle rectangle)
        {
            Haxxit.Maps.Map map = user_map_state.display_map_state.Map;
            Haxxit.Maps.Point haxxit_location = user_map_state.display_map_state.XnaPointToHaxxitPoint(rectangle.Area.Center);
            Haxxit.Maps.Point direction = haxxit_location - selected_program;
            if (map.CanMoveProgram(selected_program, direction))
            {
                _mediator_manager.Notify("haxxit.map.move", this, new Haxxit.Maps.MoveEventArgs(selected_program, direction));
                if (map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(haxxit_location)
                    && !map.GetNode<Haxxit.Maps.ProgramHeadNode>(haxxit_location).Program.AlreadyRanCommand())
                {
                    MapMovementGameState new_state = new MapMovementGameState(user_map_state, haxxit_location);
                    _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                }
                else
                {
                    _mediator_manager.Notify("haxxit.engine.state.pop", this, new EventArgs());
                }
            }
        }

        public void OnAttackClick(DrawableRectangle rectangle)
        {
            Haxxit.Maps.Map map = user_map_state.display_map_state.Map;
            foreach (Tuple<DrawableRectangle, string> attack in attacks)
            {
                if (attack.Item1 == rectangle)
                {
                    if (!map.GetNode<Haxxit.Maps.ProgramHeadNode>(selected_program).Program.HasCommand(attack.Item2))
                        break;
                    MapAttackGameState new_state = new MapAttackGameState(user_map_state, selected_program, attack.Item2);
                    _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
                }
            }
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            arial_16px_regular = content.Load<SpriteFont>("Arial-16px-Regular");
            arial_12px_regular = content.Load<SpriteFont>("Arial-12px-Regular");

            selected_border = new DrawableRectangle(rectangle_texture,
                user_map_state.display_map_state.HaxxitPointToXnaRectangle(selected_program), Color.Transparent, 2, Color.White);

            Haxxit.Maps.Map map = user_map_state.display_map_state.Map;
            foreach (Haxxit.Maps.Point p in selected_program.GetOrthologicalNeighbors())
            {
                Haxxit.Maps.Point direction = selected_program - p;
                if (map.CanMoveProgram(selected_program, direction))
                {
                    DrawableRectangle move =
                        new DrawableRectangle(rectangle_texture, user_map_state.display_map_state.HaxxitPointToXnaRectangle(p),
                            Color.White * 0.5f);
                    move.OnMouseLeftClick += OnMoveClick;
                    movement.Add(move);
                }
            }

            Haxxit.Maps.ProgramHeadNode selected_node = map.GetNode<Haxxit.Maps.ProgramHeadNode>(selected_program);
            if (!selected_node.Program.AlreadyRanCommand())
            {
                foreach (string command in selected_node.Program.GetAllCommands())
                {
                    Vector2 text_size = arial_16px_regular.MeasureString(command);
                    DrawableRectangle rectangle = new DrawableRectangle(rectangle_texture,
                            new Rectangle(760 - (int)Math.Floor(text_size.Y), 10 + ((int)Math.Floor(text_size.Y) + 15) * attacks.Count,
                                (int)Math.Floor(text_size.X) + 10, (int)Math.Floor(text_size.Y) + 10),
                            Color.Red);
                    rectangle.OnMouseLeftClick += OnAttackClick;
                    attacks.Add(new Tuple<DrawableRectangle, string>(rectangle, command));
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

        public override void Update()
        {
            // To change a scene, just call one of these:
            // Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.pop", this, new EventArgs());

            MouseState mouse_state = Mouse.GetState();
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y);
            bool within_others = false;

            foreach (DrawableRectangle move in movement)
            {
                if(!within_others && move.Area.Contains(mouse_position))
                    within_others = true;
                move.Update();
            }
            foreach (Tuple<DrawableRectangle, string> attack in attacks)
            {
                if (!within_others && attack.Item1.Area.Contains(mouse_position))
                    within_others = true;
                attack.Item1.Update();
            }
            user_map_state.turn_done_button.Update();
            user_map_state.undo_button.Update();
            
            if (mouse_state.LeftButton == ButtonState.Pressed && !within_others)
            {
                _mediator_manager.Notify("haxxit.engine.state.pop", this, new EventArgs());
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            user_map_state.Draw(sprite_batch);

            foreach (DrawableRectangle move in movement)
            {
                move.Draw(sprite_batch);
            }

            foreach (Tuple<DrawableRectangle, string> attack in attacks)
            {
                attack.Item1.Draw(sprite_batch);
                Vector2 command_size = arial_16px_regular.MeasureString(attack.Item2);
                sprite_batch.DrawString(arial_16px_regular, attack.Item2,
                    new Vector2(attack.Item1.Area.X + (attack.Item1.Area.Width - command_size.X) / 2,
                        attack.Item1.Area.Y + (attack.Item1.Area.Width - command_size.X) / 2), Color.White);
            }
        }
    }
}