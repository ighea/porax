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

    public class Fader
    {
        public enum Fade {In, Out}

        private Fade currentFadeEffect;

        private bool fadeEffectFinished;

        private float fadeTime;

        private float counter;

        private Texture2D texture;

        private Color fadeColor;

        private Fader()
        {
            this.currentFadeEffect = Fade.In;
            this.fadeEffectFinished = true;
            this.counter = 0;
        }

        private static Fader instance = null;

        public static Fader GetInstance()
        {
            if (instance == null)
            {
                instance = new Fader();
            }
            return instance;
        }


        public void Add(Fade fade, float fadeTime)
        {
            this.currentFadeEffect = fade;
            this.fadeEffectFinished = false;
            this.fadeTime = fadeTime;
            if (fade == Fade.In)
            {
                // From dark to picture
                this.counter = 255.0f;
            }
            else
            {
                // From picture to dark
                this.counter = 0.0f;
            }
        }

        public void Update(float elapsedTime)
        {
            if (!this.fadeEffectFinished)
            {
                if (this.currentFadeEffect == Fade.In)
                {
                    this.counter -= elapsedTime * (256.0f / this.fadeTime);
                    if (this.counter <= 1)
                    {
                        this.fadeEffectFinished = true;
                        this.counter = 0.0f;
                    }
                }
                else
                {
                    this.counter += elapsedTime * (256.0f / this.fadeTime);
                    if (this.counter >= 254)
                    {
                        this.fadeEffectFinished = true;
                        this.counter = 255;
                    }

                }
            }
        }

        public bool IsFadeFinished()
        {
            return this.fadeEffectFinished;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //if (!this.fadeEffectFinished)
            if (this.counter != 0)
            {
                if (texture == null)
                {
                    texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    Color[] color = new Color[1];
                    color[0] = Color.Black;
                    texture.SetData<Color>(color);
                    fadeColor = Color.Black;
                }
                fadeColor.A = (byte)this.counter;
                spriteBatch.Draw(texture, new Rectangle(0,0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), fadeColor);
                //Console.WriteLine("Fade: " + this.currentFadeEffect.ToString() + " color alpha: "+fadeColor.A);
            }

        }

    }
}
