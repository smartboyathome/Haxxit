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
    public class MapAttackGameState : HaxxitGameState
    {
        MapPlayGameState user_map_state;
        Texture2D rectangle_texture;
        SpriteFont arial_16px_regular, arial_12px_regular;
        List<DrawableRectangle> attack_nodes;
        DrawableRectangle selected_border;
        Haxxit.Maps.Point selected_program;
        string selected_attack;

        public MapAttackGameState(MapPlayGameState background_state, Haxxit.Maps.Point selected_program, string selected_attack)
        {
            user_map_state = background_state;
            this.selected_program = selected_program;
            this.selected_attack = selected_attack;
        }

        public override void NewMediator(SimplePubSub.IMediator mediator)
        {
            user_map_state.Mediator = mediator;
        }

        public override void Init()
        {
            attack_nodes = new List<DrawableRectangle>();
        }

        public void OnAttackNodeClick(DrawableRectangle rectangle)
        {
            Haxxit.Maps.Map map = user_map_state.display_map_state.Map;
            Haxxit.Maps.Point haxxit_location = user_map_state.display_map_state.XnaPointToHaxxitPoint(rectangle.Area.Center);
            _mediator_manager.Notify("haxxit.map.command", this,
                new Haxxit.Maps.CommandEventArgs(haxxit_location, selected_program, selected_attack));
            bool is_still_program_node = map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(selected_program);
            bool has_ran_command = map.GetNode<Haxxit.Maps.ProgramHeadNode>(selected_program).Program.AlreadyRanCommand();
            bool a = is_still_program_node && has_ran_command;
            if(map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(selected_program)
                && map.GetNode<Haxxit.Maps.ProgramHeadNode>(selected_program).Program.AlreadyRanCommand())
                _mediator_manager.Notify("haxxit.engine.state.pop", this, new EventArgs());
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
            Haxxit.Programs.Program program = map.GetNode<Haxxit.Maps.ProgramHeadNode>(selected_program).Program;
            foreach (Haxxit.Maps.Point p in selected_program.GetPointsWithinDistance(program.GetCommand(selected_attack).Range))
            {
                Rectangle attack_rectangle = user_map_state.display_map_state.HaxxitPointToXnaRectangle(p);
                if(attack_nodes.Count(x => x.Area == attack_rectangle) == 0)
                {
                    DrawableRectangle attack_node =
                        new DrawableRectangle(rectangle_texture, attack_rectangle, Color.White * 0.5f);
                    attack_node.OnMouseLeftClick += OnAttackNodeClick;
                    attack_nodes.Add(attack_node);
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

            foreach (DrawableRectangle attack_node in attack_nodes)
            {
                if(attack_node.Area.Contains(mouse_position))
                    within_others = true;
                attack_node.Update();
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

            foreach (DrawableRectangle attack_node in attack_nodes)
            {
                attack_node.Draw(sprite_batch);
            }
        }
    }
}