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
    public class NewUserMapGameState : HaxxitGameState
    {
        public MapDisplayGameState display_map_state;
        public DrawableRectangle turn_done_button, undo_button;

        Texture2D rectangle_texture;
        SpriteFont arial_16px_regular, arial_12px_regular;
        Dictionary<Haxxit.Maps.Point, DrawableRectangle> head_nodes;

        public NewUserMapGameState(MapDisplayGameState background_state)
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

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            arial_16px_regular = content.Load<SpriteFont>("Arial-16px-Regular");
            arial_12px_regular = content.Load<SpriteFont>("Arial-12px-Regular");

            turn_done_button = new DrawableRectangle(rectangle_texture, new Rectangle(690, 440, 100, 30), Color.Green);
            turn_done_button.OnMouseLeftClick += OnTurnDoneClick;
            undo_button = new DrawableRectangle(rectangle_texture, new Rectangle(580, 440, 100, 30), Color.Orange);
            undo_button.OnMouseLeftClick += OnUndoClick;

            Haxxit.Maps.Map map = display_map_state.Map;
            foreach (Haxxit.Maps.Point p in map.Low.IterateOverRange(map.High))
            {
                if (map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p) 
                    && map.GetNode<Haxxit.Maps.ProgramHeadNode>(p).Player == map.CurrentPlayer)
                {
                    head_nodes[p] =
                        new DrawableRectangle(rectangle_texture, display_map_state.HaxxitPointToXnaRectangle(p), Color.Transparent);
                    head_nodes[p].OnMouseLeftClick += OnProgramClick;
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

        private void MapChangedListener(string channel, object sender, EventArgs args)
        {
            Haxxit.Maps.MapChangedEventArgs event_args = (Haxxit.Maps.MapChangedEventArgs)args;
            foreach (Haxxit.Maps.Point p in event_args.ChangedNodes)
            {
                if (head_nodes.ContainsKey(p) && !display_map_state.Map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
                    head_nodes.Remove(p);
                else if (!head_nodes.ContainsKey(p) && display_map_state.Map.NodeIsType<Haxxit.Maps.ProgramHeadNode>(p))
                {
                    head_nodes[p] =
                        new DrawableRectangle(rectangle_texture, display_map_state.HaxxitPointToXnaRectangle(p), Color.Transparent);
                    head_nodes[p].OnMouseLeftClick += OnProgramClick;
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

            _mediator_manager.Subscribe("haxxit.map.hacked", MapHackedListener);
            _mediator_manager.Subscribe("haxxit.map.nodes.changed", MapChangedListener);
        }

        public override void Update()
        {
            // To change a scene, just call one of these:
            // Mediator.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new OtherGameState()));
            // Mediator.Notify("haxxit.engine.state.pop", this, new EventArgs());

            if (display_map_state.Map.CurrentPlayer.GetType() != typeof(Haxxit.MonoGame.PlayerAI))
            {
                foreach (DrawableRectangle head_node in head_nodes.Values)
                {
                    head_node.Update();
                }

                turn_done_button.Update();
                undo_button.Update();
            }
            else
            {
                ((Haxxit.MonoGame.PlayerAI)display_map_state.Map.CurrentPlayer).HandleAITurn(display_map_state.Map);
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            display_map_state.Draw(sprite_batch);

            turn_done_button.Draw(sprite_batch);
            Vector2 turn_done_size = arial_16px_regular.MeasureString("Turn Done");
            Vector2 turn_done_position = new Vector2(turn_done_button.Area.X + (turn_done_button.Area.Width - turn_done_size.X) / 2,
                turn_done_button.Area.Y + (turn_done_button.Area.Height - turn_done_size.Y) / 2);
            sprite_batch.DrawString(arial_16px_regular, "Turn Done", turn_done_position, Color.White);

            undo_button.Draw(sprite_batch);
            Vector2 undo_text_size = arial_16px_regular.MeasureString("Undo");
            Vector2 undo_text_position = new Vector2(undo_button.Area.X + (undo_button.Area.Width - undo_text_size.X) / 2,
                undo_button.Area.Y + (undo_button.Area.Height - undo_text_size.Y) / 2);
            sprite_batch.DrawString(arial_16px_regular, "Undo", undo_text_position, Color.White);

            foreach (DrawableRectangle head_node in head_nodes.Values)
            {
                head_node.Draw(sprite_batch);
            }

            /*List<string> bottom_status = new List<string>();
            int spawn_weight_sum = 0;
            foreach (Haxxit.Maps.Point p in user_map_state.Map.Low.IterateOverRange(user_map_state.Map.High))
            {
                if (user_map_state.Map.NodeIsType<Haxxit.Maps.SpawnNode>(p)
                    && user_map_state.Map.GetNode<Haxxit.Maps.SpawnNode>(p).program != null)
                {
                    spawn_weight_sum += user_map_state.Map.GetNode<Haxxit.Maps.SpawnNode>(p).program.SpawnWeight;
                }
            }
            bottom_status.Add("Spawn Points remaining: " + (user_map_state.Map.TotalSpawnWeights - spawn_weight_sum).ToString());
            for (int i = 0; i < bottom_status.Count; i++)
            {
                Vector2 bottom_status_position = new Vector2(10, 450 - 18 * (bottom_status.Count - i - 1));
                sprite_batch.DrawString(arial_12px_regular, bottom_status[i], bottom_status_position, Color.White);
            }*/
        }
    }
}
