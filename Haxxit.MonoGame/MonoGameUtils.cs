using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    /// <summary>
    /// Class for drawing primitives, taken then modified from
    /// http://stackoverflow.com/questions/13893959/how-to-draw-the-border-of-a-square
    /// </summary>
    
    public abstract class DrawablePrimitive
    {
        public Texture2D texture
        {
            get;
            protected set;
        }
        protected DrawablePrimitive(Texture2D white_pixel)
        {
            this.texture = white_pixel;
        }
        public abstract void Update();
        public abstract void Draw(SpriteBatch sprite_batch);
    }

    public enum DrawableMouseState
    {
        Outside,
        Inside,
        LeftClick,
        RightClick,
        MiddleClick
    }

    public class DrawableRectangle : DrawablePrimitive
    {
        public delegate void OnEventHandler(DrawableRectangle rectangle);
        public event OnEventHandler OnMouseLeftClick;
        public event OnEventHandler OnMouseMiddleClick;
        public event OnEventHandler OnMouseRightClick;
        public event OnEventHandler OnMouseInside;
        public event OnEventHandler OnMouseOutside;
        private List<DrawableMouseState> current_mouse_states;

        public Rectangle Area
        {
            get;
            set;
        }
        public Color FillColor
        {
            get;
            set;
        }
        public int BorderSize
        {
            get;
            set;
        }
        public Color BorderColor
        {
            get;
            set;
        }

        public DrawableRectangle(Texture2D white_pixel, Rectangle area, Color fill_color,
            int border_size, Color border_color) :
            base(white_pixel)
        {
            Area = area;
            FillColor = fill_color;
            BorderColor = border_color;
            BorderSize = border_size;
            current_mouse_states = new List<DrawableMouseState>();
            current_mouse_states.Add(DrawableMouseState.Outside);
        }

        public DrawableRectangle(Texture2D white_pixel, Rectangle area, Color fill_color) :
            this(white_pixel, area, fill_color, 0, Color.White)
        {

        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(texture, Area, FillColor);
            if (BorderSize > 0)
            {
                sprite_batch.Draw(texture, new Rectangle(Area.X, Area.Y, Area.Width, BorderSize), BorderColor);
                sprite_batch.Draw(texture, new Rectangle(Area.X, Area.Y, BorderSize, Area.Height), BorderColor);
                sprite_batch.Draw(texture, new Rectangle(Area.X + Area.Width - BorderSize, Area.Y, BorderSize, Area.Height), BorderColor);
                sprite_batch.Draw(texture, new Rectangle(Area.X, Area.Y + Area.Height - BorderSize, Area.Width, BorderSize), BorderColor);
            }
        }

        private bool CheckMouseUpdate(ButtonState button_state, DrawableMouseState drawable_mouse_state)
        {
            if (button_state == ButtonState.Pressed && !current_mouse_states.Contains(drawable_mouse_state))
            {
                current_mouse_states.Add(drawable_mouse_state);
            }
            else if (button_state == ButtonState.Released && current_mouse_states.Contains(drawable_mouse_state))
            {
                current_mouse_states.Remove(drawable_mouse_state);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            MouseState mouse_state = Mouse.GetState();
            Point mouse_location = new Point(mouse_state.X, mouse_state.Y);
            if (Area.Contains(mouse_location))
            {
                if (!current_mouse_states.Contains(DrawableMouseState.Inside))
                {
                    current_mouse_states.Clear();
                    current_mouse_states.Add(DrawableMouseState.Inside);
                    if(OnMouseInside != null)
                        OnMouseInside(this);
                }

                if (CheckMouseUpdate(mouse_state.LeftButton, DrawableMouseState.LeftClick) && OnMouseLeftClick != null)
                    OnMouseLeftClick(this);
                if (CheckMouseUpdate(mouse_state.RightButton, DrawableMouseState.RightClick) && OnMouseRightClick != null)
                    OnMouseRightClick(this);
                if (CheckMouseUpdate(mouse_state.MiddleButton, DrawableMouseState.MiddleClick) && OnMouseMiddleClick != null)
                    OnMouseMiddleClick(this);
            }
            else if (!current_mouse_states.Contains(DrawableMouseState.Outside))
            {
                current_mouse_states.Clear();
                current_mouse_states.Add(DrawableMouseState.Outside);
                if (OnMouseInside != null)
                    OnMouseOutside(this);
            }
        }
    }

    static public class PrimiviteDrawing
    {
        static public void DrawRectangle(Texture2D whitePixel, SpriteBatch batch, Rectangle area, int width, Color color)
        {
            batch.Draw(whitePixel, new Rectangle(area.X, area.Y, area.Width, width), color);
            batch.Draw(whitePixel, new Rectangle(area.X, area.Y, width, area.Height), color);
            batch.Draw(whitePixel, new Rectangle(area.X + area.Width - width, area.Y, width, area.Height), color);
            batch.Draw(whitePixel, new Rectangle(area.X, area.Y + area.Height - width, area.Width, width), color);
        }
        static public void DrawRectangle(Texture2D whitePixel, SpriteBatch batch, Rectangle area)
        {
            DrawRectangle(whitePixel, batch, area, 1, Color.White);
        }
        public static void DrawCircle(Texture2D whitePixel, SpriteBatch spritbatch, Vector2 center, float radius, Color color, int lineWidth = 2, int segments = 16)
        {
            Vector2[] vertex = new Vector2[segments];

            double increment = Math.PI * 2.0 / segments;
            double theta = 0.0;

            for (int i = 0; i < segments; i++)
            {
                vertex[i] = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                theta += increment;
            }

            DrawPolygon(whitePixel, spritbatch, vertex, segments, color, lineWidth);
        }
        public static void DrawPolygon(Texture2D whitePixel, SpriteBatch spriteBatch, Vector2[] vertex, int count, Color color, int lineWidth)
        {
            if (count > 0)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    DrawLineSegment(whitePixel, spriteBatch, vertex[i], vertex[i + 1], color, lineWidth);
                }
                DrawLineSegment(whitePixel, spriteBatch, vertex[count - 1], vertex[0], color, lineWidth);
            }
        }
        public static void DrawLineSegment(Texture2D whitePixel, SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, int lineWidth)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            spriteBatch.Draw(whitePixel, point1, null, color,
            angle, Vector2.Zero, new Vector2(length, lineWidth),
            SpriteEffects.None, 0f);
        }
    }
}
