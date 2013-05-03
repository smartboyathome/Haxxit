using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    public class DefaultGameState : HaxxitGameState
    {
        Texture2D test_texture;

        public DefaultGameState() :
            base()
        {

        }

        public override void Init(GraphicsDeviceManager graphics, SpriteBatch sprite_batch)
        {
            test_texture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            test_texture.SetData(new Color[] { Color.White });
        }

        public override void SubscribeAll()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void Draw(GraphicsDeviceManager graphics, SpriteBatch sprite_batch)
        {
            sprite_batch.Begin();
            sprite_batch.Draw(test_texture, new Rectangle(10, 10, 10, 10), Color.Red);
            sprite_batch.End();
        }
    }
}
