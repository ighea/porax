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
    class Credits
    {
        private Texture2D background;

        private Timer exitTimer;

        private bool exiting;

        public Credits(Game game, string backgroundImage)
        {
            this.background = game.Content.Load<Texture2D>(backgroundImage);
            Fader.GetInstance().Add(Fader.Fade.In, 1.0f);
            this.exiting = false;
            this.exitTimer = new Timer(1.0f, false);
        }

        /// <summary>
        /// Takes input to determine if the credits should be exited.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns>true if credits should be exited, false otherwise.</returns>
        public bool Update(GameTime gameTime)
        {
            if ((this.exiting == false) && (Input.GetInstance().IsPressed(Keys.Space) || Input.GetInstance().IsPressed(Keys.Enter) || Input.GetInstance().IsPressed(Buttons.A)))
            {
                this.exiting = true;
                Fader.GetInstance().Add(Fader.Fade.Out, 1.0f);
            }

            if (this.exiting)
            {
                if (this.exitTimer.GetIsTriggered(gameTime.ElapsedGameTime.Milliseconds / 1000.0f))
                {
                    // Add Fade in to be playd so we won't end up with black screen and
                    // this will also provide nice "return to main menu effect".
                    Fader.GetInstance().Add(Fader.Fade.In, 1.0f);
                    // Return "true" to exit the Credits.
                    return true;
                }
            }

            
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                background,
                new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height),
                Color.White
                );

        }

    }
}
