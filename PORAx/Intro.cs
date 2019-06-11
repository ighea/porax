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
    /// <summary>
    /// Logic to show fade in/out of the PunyWare logo in the game's startup before main menu.
    /// </summary>
    class Intro
    {
        private Texture2D texture;

        public bool Finished { get; set; }

        private Vector2 position;

        private enum Stages {Start, FadeIn, Still, FadeOut, End }

        private Stages stage = Stages.Start;

        private Timer timerFadeIn;
        private Timer timerFadeOut;
        private Timer timerStill;

        private Game game;

        public Intro(Game game, string textureLogo)
        {
            this.game = game;
            this.texture = game.Content.Load<Texture2D>(textureLogo);
            this.Finished = false;

            timerFadeIn = new Timer(1.0f, false);
            timerFadeOut = new Timer(1.0f, false);
            timerStill = new Timer(2.0f, false);

            int x = Randomizer.GetInstance().Random.Next(0, 2) * game.GraphicsDevice.Viewport.Width;
            int y = Randomizer.GetInstance().Random.Next(0, 2) * game.GraphicsDevice.Viewport.Height;
            this.position = new Vector2(x, y);
        }


        public void Update(GameTime gameTime)
        {
            float elapsedTime = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // Move the texture to the center
            int width = this.game.GraphicsDevice.Viewport.Width;
            int height = this.game.GraphicsDevice.Viewport.Height;

            float dX = width / 2 - position.X;
            float dY = height / 2 - position.Y;

            position.X += elapsedTime * dX;
            position.Y += elapsedTime * dY;

            // StageSelector
            switch (stage)
            {
                case Stages.Start:
                    Fader.GetInstance().Add(Fader.Fade.In, 1.0f);
                    stage++;
                    break;
                case Stages.FadeIn:
                    if (timerFadeIn.GetIsTriggered(elapsedTime))
                    {
                        stage++;
                    }
                    break;
                case Stages.Still:
                    if (timerStill.GetIsTriggered(elapsedTime))
                    {
                        Fader.GetInstance().Add(Fader.Fade.Out, 1.0f);
                        stage++;
                    }
                    break;
                case Stages.FadeOut:
                    if (timerFadeOut.GetIsTriggered(elapsedTime))
                    {
                        stage++;
                    }
                    break;
                case Stages.End:
                    this.Finished = true;
                    break;
            }

            
        
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.texture != null)
            {
                spriteBatch.Draw(texture, this.position - new Vector2(texture.Width/2, texture.Height/2), Color.White);
            }
        
        }

    }
}
