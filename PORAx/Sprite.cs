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
    class Sprite: Microsoft.Xna.Framework.GameComponent
    {
        protected Game game;
        protected Vector2 position;
        protected Animation animation;
        protected float angle;
        protected SpriteEffects spriteEffect;
        protected bool visible;
        
        protected Rectangle bouncingBox;

        public Sprite(Game game, Vector2 position, Animation animation)
            :base(game)
        {
            this.game = game;
            this.position = position;
            this.animation = animation;
            this.spriteEffect = SpriteEffects.None;
            this.angle = MathHelper.PiOver2;
            this.visible = true;
            UpdateBoundingBox();
        }

        public Sprite(Game game, Vector2 position)
            : base(game)
        {
            this.animation = null;
            this.game = game;
            this.position = position;
            this.spriteEffect = SpriteEffects.None;
            this.angle = MathHelper.PiOver2;
            this.visible = true;
        }

        public Animation Animation
        {
            get 
            {
                return animation;
            }
            set
            {
                this.animation = value;
                UpdateBoundingBox();
            }
        }

        public bool Visible
        {
            get 
            {
                return this.visible;
            }
            set
            {
                this.visible = value;
            }
        }

        public float Angle
        {
            get
            {
                return this.angle;
            }
            set
            {
                this.angle = value;
            }
        }

        public SpriteEffects SpriteEffect
        {
            get
            {
                return this.spriteEffect;
            }
            set
            {
                this.spriteEffect = value;
            }
        }

        public void UpdateBoundingBox()
        {
            this.bouncingBox = CreateCollisionRectangle(Vector2.Zero);
        }

        public Rectangle BoundingBox
        {
            get
            {
                return this.bouncingBox;
            }
        }

        public Vector2 Position
        {
            get 
            {
                return this.position;
            }
            set 
            {
                this.position = value;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (this.animation != null)
            {
                this.animation.Update(gameTime);
            }
            this.bouncingBox = CreateCollisionRectangle(Vector2.Zero);
        
            base.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (this.animation != null && this.visible)
            {
                this.animation.Draw(spriteBatch, this.position, this.angle, this.spriteEffect);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            if (this.animation != null && this.visible)
            {
                this.animation.Draw(spriteBatch, this.position + offset, this.angle, this.spriteEffect);
            }

        }


        protected Rectangle CreateCollisionRectangle(Vector2 shift)
        {
            int width = this.animation.Width;
            int height = this.animation.Height;
            Rectangle rect = new Rectangle();

            if (shift == Vector2.Zero)
            {
                rect = new Rectangle(
                    (int)this.position.X - width / 2,
                    (int)this.position.Y - height / 2,
                    width,
                    height
                    );
            }
            else
            {
                rect = new Rectangle(
                    (int)this.position.X - width / 2 + (int)(shift.X + (shift.X > 0 ? 1f : (shift.X < 0 ? -1f : 0))),
                    (int)this.position.Y - height / 2 + (int)(shift.Y + (shift.Y > 0 ? 1f : (shift.Y < 0 ? -1f : 0))),
                    width,
                    height
                    );
            }

            return rect;
        }


    }
}
