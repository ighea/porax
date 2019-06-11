using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace PORAx
{
    public class Effect
    {
        private Game game;
        private float counter;
        private float lifetime;
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private Color color;
        private float fcolor;

        public bool remove = false;

        public Effect(Game game, Vector2 position, Texture2D texture, float lifetime)
        {
            this.game = game;
            this.texture = texture;
            this.lifetime = lifetime;
            this.counter = 0.0f;
            this.position = position;
            color = Color.White;
            fcolor = color.A = 255;

            int x = Randomizer.GetInstance().Random.Next(-100, 100);
            int y = Randomizer.GetInstance().Random.Next(-100, 100);
            this.velocity = new Vector2(x, y);
        }

        ~Effect()
        {
            this.texture = null;
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = gameTime.ElapsedGameTime.Milliseconds / 1000f;

            fcolor += (counter/lifetime)*256.0f;
            color.A = (byte)fcolor;
            if (counter >= lifetime)
            {
                this.remove = true;
            }
            else
            {
                counter += elapsedTime;
            }

            this.position += new Vector2(this.velocity.X * elapsedTime, this.velocity.Y * elapsedTime);
            this.velocity += new Vector2(0, 10.0f);
        
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            spriteBatch.Draw(this.texture, new Rectangle((int)(position.X + offset.X), (int)(position.Y + offset.Y), 5,5), color);
        }
    }
}
