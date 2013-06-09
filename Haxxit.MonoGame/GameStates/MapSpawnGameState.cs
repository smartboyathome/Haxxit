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
    public class MapSpawnGameState : HaxxitGameState
    {
        const string finished_text = "Finished";

        protected MapDisplayGameState display_map_state;
        Texture2D rectangle_texture, rounded_rect_border, finished_texture, leave_map_texture;
        ButtonHover button_hover;

        SpriteFont arial_16px_regular, arial_12px_regular;
        Vector2 finished_text_size;
        protected DrawableRectangle finished_button, leave_map_button;

        //List<DrawableRectangle> spawns;
        protected Dictionary<Haxxit.Maps.Point, DrawableRectangle> spawns;

        public MapSpawnGameState()
        {
        }

        public MapSpawnGameState(Haxxit.Maps.Map map)
        {
            display_map_state = new MapDisplayGameState(map);
        }

        public MapSpawnGameState(MapDisplayGameState background_state)
        {
            display_map_state = background_state;
        }

        public override void NewMediator(SimplePubSub.IMediator mediator)
        {
            display_map_state.Mediator = mediator;
        }

        public override void Init()
        {
            display_map_state.Init();
            //spawns = new List<DrawableRectangle>();
            spawns = new Dictionary<Haxxit.Maps.Point, DrawableRectangle>();
        }

        public void OnSpawnClick(DrawableRectangle rectangle)
        {
            Haxxit.Maps.Point haxxit_location = display_map_state.XnaPointToHaxxitPoint(rectangle.Area.Center);
            SpawnDialogGameState new_state = new SpawnDialogGameState(this, display_map_state.Map, haxxit_location);
            _mediator_manager.Notify("haxxit.engine.state.push", this, new ChangeStateEventArgs(new_state));
        }

        public virtual void OnFinishedClick(DrawableRectangle rectangle)
        {
            display_map_state.Map.FinishedSpawning();
            //UserMapGameState new_state = new UserMapGameState(user_map_state.Map);
            MapPlayGameState new_state = new MapPlayGameState(display_map_state);
            _mediator_manager.Notify("haxxit.engine.state.change", this, new ChangeStateEventArgs(new_state));
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
            if (rectangle == finished_button)
                tooltip = "Finished spawning programs.";
            else if (rectangle == leave_map_button)
                tooltip = "Leave (exit) the current map.";
            button_hover = new ButtonHover(rectangle, rectangle_texture, arial_12px_regular, tooltip);
        }

        public void OnButtonOutside(DrawableRectangle rectangle)
        {
            MouseState mouse_state = Mouse.GetState();
            if (!leave_map_button.Area.Contains(mouse_state.X, mouse_state.Y)
               && !finished_button.Area.Contains(mouse_state.X, mouse_state.Y))
                button_hover = null;
        }

        public override void LoadContent(GraphicsDevice graphics, SpriteBatch sprite_batch, ContentManager content)
        {
            display_map_state.LoadContent(graphics, sprite_batch, content);
            rectangle_texture = new Texture2D(graphics, 1, 1);
            rectangle_texture.SetData(new Color[] { Color.White });
            rounded_rect_border = content.Load<Texture2D>("Map-Square-Border");
            finished_texture = content.Load<Texture2D>("Map-Button-TurnDone");
            leave_map_texture = content.Load<Texture2D>("Map-Button-LeaveMap");
            arial_16px_regular = content.Load<SpriteFont>("Arial-16px-Regular");
            arial_12px_regular = content.Load<SpriteFont>("Arial-12px-Regular");

            int button_size = 32;
            int buffer_size = 6;
            finished_button = new DrawableRectangle(finished_texture,
                new Rectangle(788 - button_size, 468 - button_size, button_size, button_size), Color.Green);
            finished_button.OnMouseLeftClick += OnFinishedClick;
            finished_button.OnMouseInside += OnButtonInside;
            finished_button.OnMouseOutside += OnButtonOutside;
            leave_map_button = new DrawableRectangle(leave_map_texture,
                new Rectangle(788 - buffer_size - button_size * 2, 468 - button_size, button_size, button_size), Color.Red);
            leave_map_button.OnMouseLeftClick += OnLeaveMapClick;
            leave_map_button.OnMouseInside += OnButtonInside;
            leave_map_button.OnMouseOutside += OnButtonOutside;
            button_hover = null;

            /*finished_text_size = arial_16px_regular.MeasureString(finished_text);
            finished_button = new DrawableRectangle(
                rectangle_texture,
                new Rectangle(
                    800 - (int)Math.Floor(finished_text_size.X) - 15,
                    480 - (int)Math.Floor(finished_text_size.Y) - 15,
                    (int)Math.Floor(finished_text_size.X) + 10,
                    (int)Math.Floor(finished_text_size.Y) + 10
                ),
                Color.Green
            );
            finished_button.OnMouseLeftClick += OnFinishedClick;
            leave_map_button = new DrawableRectangle(rectangle_texture, new Rectangle(675, 400, 115, 30), Color.Red);
            leave_map_button.OnMouseLeftClick += OnLeaveMapClick;*/

            Haxxit.Maps.Map map = display_map_state.Map;
            foreach (Haxxit.Maps.Point p in map.Low.IterateOverRange(map.High))
            {
                if (map.NodeIsType<Haxxit.Maps.SpawnNode>(p)
                    && map.GetNode<Haxxit.Maps.SpawnNode>(p).Player == map.CurrentPlayer)
                {
                    DrawableRectangle spawn =
                        new DrawableRectangle(rounded_rect_border, display_map_state.HaxxitPointToXnaRectangle(p), Color.White);
                    spawn.OnMouseLeftClick += OnSpawnClick;
                    spawns.Add(p, spawn);
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

            _mediator_manager.Notify("haxxit.map.nodes.changed", this, new Haxxit.Maps.MapChangedEventArgs(spawns.Keys));

            foreach (DrawableRectangle spawn in spawns.Values)
            {
                spawn.Update();
            }

            finished_button.Update();
            leave_map_button.Update();
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            display_map_state.Draw(sprite_batch);

            finished_button.Draw(sprite_batch);
            /*Vector2 finished_text_position = new Vector2(finished_button.Area.X + (finished_button.Area.Width - finished_text_size.X) / 2,
                finished_button.Area.Y + (finished_button.Area.Height - finished_text_size.Y) / 2);
            sprite_batch.DrawString(arial_16px_regular, finished_text, finished_text_position, Color.White);*/

            leave_map_button.Draw(sprite_batch);
            /*Vector2 leave_map_text_size = arial_16px_regular.MeasureString("Leave Map");
            Vector2 leave_map_text_position = new Vector2(leave_map_button.Area.X + (leave_map_button.Area.Width - leave_map_text_size.X) / 2,
                leave_map_button.Area.Y + (leave_map_button.Area.Height - leave_map_text_size.Y) / 2);
            sprite_batch.DrawString(arial_16px_regular, "Leave Map", leave_map_text_position, Color.White);*/

            if (button_hover != null)
                button_hover.Draw(sprite_batch);

            foreach (DrawableRectangle spawn in spawns.Values)
            {
                spawn.Draw(sprite_batch);
            }

            List<string> bottom_status = new List<string>();
            int spawn_weight_sum = 0;
            foreach (Haxxit.Maps.Point p in display_map_state.Map.Low.IterateOverRange(display_map_state.Map.High))
            {
                if (display_map_state.Map.NodeIsType<Haxxit.Maps.SpawnNode>(p)
                    && display_map_state.Map.GetNode<Haxxit.Maps.SpawnNode>(p).program != null)
                {
                    spawn_weight_sum += display_map_state.Map.GetNode<Haxxit.Maps.SpawnNode>(p).program.SpawnWeight;
                }
            }
            bottom_status.Add("Spawn Points remaining: " + (display_map_state.Map.TotalSpawnWeights - spawn_weight_sum).ToString());
            for (int i = 0; i < bottom_status.Count; i++)
            {
                Vector2 bottom_status_position = new Vector2(10, 450 - 18 * (bottom_status.Count - i - 1));
                sprite_batch.DrawString(arial_12px_regular, bottom_status[i], bottom_status_position, Color.White);
            }
        }
    }
}