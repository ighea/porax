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
    /// Class to handle the animations
    /// </summary>
    public class Animation
    {
        private int currentFrame;
        private int numberOfFrames;
        private Texture2D texture;
        private List<Texture2D> textureList;
        private float framesPerSecond;
        private float timer;
        private int frameWidth;
        private int frameHeight;
        private Rectangle sourceRectangle;
        private bool playing;
        private float scale;
        private float layerDepth;

        public Texture2D Texture
        {
            get 
            {
                if (this.texture != null)
                {
                    return texture;
                }
                else if (this.textureList[(int)this.currentFrame] != null)
                {
                    return this.textureList[(int)this.currentFrame];
                }
                return null;
            }
        }

        private void Initialize()
        {
            this.currentFrame = 0;
            this.textureList = null;
            this.texture = null;
            this.sourceRectangle = new Rectangle(0, 0, 0, 0);
            this.frameWidth = 0;
            this.frameHeight = 0;
            this.playing = true;
            this.layerDepth = 0.5f;
            this.scale = 1.0f;
        }

        /// <summary>
        /// Creates an Animation with multi-frame texture. Texture is split into nice sprites with it's width divided by frameCount.
        /// </summary>
        public Animation(Texture2D multiframeTexture, int frameCount, float framesPerSecond)
        {
            this.Initialize();
            if (frameCount < 1)
            {
                frameCount = 1;
            }
            this.texture = multiframeTexture;
            this.numberOfFrames = frameCount;
            this.framesPerSecond = framesPerSecond;
            this.frameHeight = this.texture.Height;
            this.frameWidth = this.texture.Width / frameCount;
        }

        /// <summary>
        /// Creates an Animation from list of textures.
        /// </summary>
        /// <param name="textureListSingleFrames">List of single sprite textures</param>
        /// <param name="framesPerSecond">Frames per second, how many frames will be displayed during one second</param>
        public Animation(List<Texture2D> textureListSingleFrames, float framesPerSecond)
        {
            this.Initialize();
            this.textureList = textureListSingleFrames;
            this.numberOfFrames = textureList.Count;
            this.framesPerSecond = framesPerSecond;
            this.frameHeight = this.textureList[0].Height;
            this.frameWidth = this.textureList[0].Width;
        }

        /// <summary>
        /// Sprite's height, read-only.
        /// </summary>        
        public int Height
        {
            get
            {
                return this.frameHeight;
            }
        }

        /// <summary>
        /// Sprite's width, read-only.
        /// </summary>        
        public int Width
        {
            get 
            {
                return this.frameWidth;
            }
        }

        public float Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                this.scale = value;
            }
        }

        public float LayerDepth
        {
            get
            {
                return this.layerDepth;
            }
            set
            {
                this.layerDepth = value;
            }
        }

        public bool Playing
        {
            get
            {
                return playing;
            }
            set
            {
                playing = value;
            }
        }

        /// <summary>
        /// Updates the shown frame to match current time line.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (this.playing)
            {
                // Time elapsed since last time update was called
                this.timer = this.timer + gameTime.ElapsedGameTime.Milliseconds;

                // If timer passes the set interval, increase the current frame by one.
                if (this.timer > (this.framesPerSecond * 1000)) // Converts the framesPerSecond to milliseconds.
                {
                    this.currentFrame++;
                    if (this.currentFrame > this.numberOfFrames - 1) // On last frame, go back to first
                    {
                        this.currentFrame = 0;
                    }
                    this.timer = 0.0f; //Reset the timer
                }
            }

            // Use the right sprite "mode"
            if (this.texture != null)
            {   // Texture with multiple sprites
                this.sourceRectangle = new Rectangle(this.currentFrame * this.frameWidth, 0, this.frameWidth, this.frameHeight);
            }
            else
            {   // Sprite list
                this.sourceRectangle = new Rectangle(0, 0, this.frameWidth, this.frameHeight);
            }
        }

        /// <summary>
        /// Resets the animation and sets the next frame to the first one.
        /// </summary>
        public void Reset()
        {
            this.currentFrame = 0;
        }

        /// <summary>
        /// Draws the current active frameSprite to given position. spritebatch.begin and end must be called before this method is used.
        /// </summary>
        /// <param name="spriteBatch">reference to the spriteBatch</param>
        /// <param name="position">center position of the sprite</param>
        /// <param name="angle">rotation angle in radians</param>
        /// <param name="spriteEffect">SpriteEffect to use</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float angle, SpriteEffects spriteEffect)
        {
            if (spriteBatch == null)
            {
                return;
            }

            // With horizontal sprite sheet
            if (this.texture != null)
            {
                spriteBatch.Draw(this.texture, position, this.sourceRectangle, Color.White, angle-(float)(Math.PI/2), new Vector2(this.frameWidth / 2, this.frameHeight / 2), this.scale, spriteEffect, this.layerDepth);
            }
            else if (this.textureList[this.currentFrame] != null)
            {// and if SpriteList is being used
                spriteBatch.Draw(this.textureList[this.currentFrame], position, this.sourceRectangle, Color.White, angle - (float)(Math.PI / 2), new Vector2(this.frameWidth / 2, this.frameHeight / 2), this.scale, spriteEffect, this.layerDepth);
            }
        }

        /// <summary>
        /// Draws the current active frameSprite to given position. spritebatch.begin and end must be called before this method is used.
        /// </summary>
        /// <param name="spriteBatch">reference to the spriteBatch</param>
        /// <param name="position">center position of the sprite</param>
        /// <param name="angle">rotation angle in radians</param>
        /// <param name="spriteEffect">SpriteEffect to use</param>
        /// <param name="color">tinting color</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float angle, SpriteEffects spriteEffect, Color color)
        {
            // With horizontal sprite sheet
            if (this.texture != null)
            {
                spriteBatch.Draw(this.texture, position, this.sourceRectangle, color, angle - (float)(Math.PI / 2), new Vector2(this.frameWidth / 2, this.frameHeight / 2), this.scale, spriteEffect, this.layerDepth);
            }
            else if (this.textureList[this.currentFrame] != null)
            {// and if SpriteList is being used
                spriteBatch.Draw(this.textureList[this.currentFrame], position, this.sourceRectangle, color, angle - (float)(Math.PI / 2), new Vector2(this.frameWidth / 2, this.frameHeight / 2), this.scale, spriteEffect, this.layerDepth);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            // With horizontal sprite sheet
            if (this.texture != null)
            {
                spriteBatch.Draw(this.texture, rectangle, this.sourceRectangle, color);
            }
            else if (this.textureList[this.currentFrame] != null)
            {// and if SpriteList is being used
                spriteBatch.Draw(this.textureList[this.currentFrame], rectangle, color);
            }
        }


    }
}
