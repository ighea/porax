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

    class Placeable: Sprite
    {

        protected float gravity;
        protected bool moveable;
        protected bool drillable;
        protected bool destroyable;
        protected bool endCondition;

        protected int health;
        protected int damage;


        protected bool incollidable;

        protected Vector2 velocity;

        protected Rectangle areaBorders;

        protected Dialog dialog = null;
        
        protected bool dialogPlayed = false;

        protected char symbol;

        public SoundEffect soundEffectDestroyed;

        public Placeable(Game game, Vector2 position, Rectangle areaBorders, PlaceableData data, char symbol)
            : base(game, position)
        {
            this.Enabled = false;

            List<Texture2D> textureList = new List<Texture2D>();

            foreach (string texture in data.sprites)
            {
                textureList.Add(game.Content.Load<Texture2D>(texture));    
            }

            this.animation = new Animation(textureList, data.animSpeed);

            this.animation.LayerDepth = data.layerDepth;

            this.symbol = symbol;
            this.moveable = data.moveable;
            this.gravity = data.gravity;
            this.drillable = data.drillable;
            this.incollidable = data.incollidable;
            this.areaBorders = areaBorders;
            this.destroyable = data.destroyable;
            this.endCondition = data.endCondition;
            this.damage = data.damage;
            this.health = data.health;
//            this.placeableData = data;

            this.velocity = Vector2.Zero;

            // data.dialog is a string so test against string "null" instead of null.
            if (data.dialog != "null")
            {
                this.dialog = new Dialog(game, XMLHandler.LoadXML<DialogData>(data.dialog));
            }

            // Load destroyed sound
            if (data.destroyedSound != "null")
            {
                this.soundEffectDestroyed = game.Content.Load<SoundEffect>(@data.destroyedSound);
            }
            else
            {
                this.soundEffectDestroyed = null;
            }

            game.Components.Add(this);
        }


        // Destructor
        ~Placeable()
        {
            game.Components.Remove(this);
        }


        public void Destroy()
        {
            this.PlayDestroyedSoundEffect();
            this.Dispose();
        }

        public void PlayDestroyedSoundEffect()
        {
            this.PlayDestroyedSoundEffect(0.75f, 0f, 0f);
        }

        public void PlayDestroyedSoundEffect(float volume, float pitch, float pan)
        {
            if (this.soundEffectDestroyed != null)
            {
                this.soundEffectDestroyed.Play(volume, pitch, pan);
            }
        }

        /// <summary>
        /// Play the dialog assigned to this palceable, if any.
        /// </summary>
        public void PlayDialog()
        {
            if (this.dialog != null && this.dialogPlayed == false)
            {
                this.dialogPlayed = true;
                DialogEngine.GetInstance().AddDialog(this.dialog, this.symbol);
            }
        }

        public int Damage
        {
            get
            {
                return damage;
            }
        }

        public int Health
        {
            get 
            {
                return health;
            }
        }

        public bool Moveable
        {
            get
            {
                return moveable;
            }
        }

        public bool Incollidable
        {
            get
            {
                return incollidable;
            }
        }

        public bool EndCondition
        {
            get
            {
                return endCondition;
            }
        }

        public bool Drillable
        {
            get
            {
                return drillable;
            }
        }

        public Vector2 Velocity
        {
            get 
            {
                return this.velocity;
            }
            set
            {
                this.velocity = value;
            }
        }

        public void SetBordersRectangle(Rectangle areaBorders)
        {
            this.areaBorders = areaBorders;
        }

        public override void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // add gravity only if it is needed, placeable is not on the ground.
            if (this.gravity > 0)
            {
                for (float y = timeElapsed * this.gravity; y > 0.0f; y = y - 1f)
                {
                    if (!GetWillCollideWithOtherGameComponents(new Vector2(0, y)))
                    {
                        this.velocity += new Vector2(0, y);
                        break;
                    }
                }
            }
            if (this.gravity < 0)
            {
                for (float y = timeElapsed * this.gravity; y < 0.0f; y = y + 1f)
                {
                    if (!GetWillCollideWithOtherGameComponents(new Vector2(0, y)))
                    {
                        this.velocity += new Vector2(0, y);
                        break;
                    }
                }
            }

            // Check for vertical movement first, so placeables will fall to their respecting empty spaces below
            if (this.velocity.Y > 0.0f)
            {
                for (float y = this.velocity.Y; y > 0.0f; y = y - 1.0f)
                {
                    if (!GetWillCollideWithOtherGameComponents(new Vector2(0, y)))
                    {
                        this.position.Y += y;
                        //UpdateBoundingBox();
                        break;
                    }
                }
            }
            if (this.velocity.Y < 0.0f)
            {
                for (float y = this.velocity.Y; y < 0.0f; y = y + 1.0f)
                {
                    if (!GetWillCollideWithOtherGameComponents(new Vector2(0, y)))
                    {
                        this.position.Y += y;
                        //UpdateBoundingBox();
                        break;
                    }
                }
            }

            // And then the horizontal collisions
            if (this.velocity.X > 0.0f)
            {
                for (float x = this.velocity.X; x > 0.0f; x = x - 1f)
                {
                    if (!GetWillCollideWithOtherGameComponents(new Vector2(x, 0)))
                    {
                        this.position.X += x;
                        //UpdateBoundingBox();
                        break;
                    }
                }
            }
            if (this.velocity.X < 0.0f)
            {
                for (float x = this.velocity.X; x < 0.0f; x = x + 1f)
                {
                    if (!GetWillCollideWithOtherGameComponents(new Vector2(x, 0)))
                    {
                        this.position.X += x;
                        //UpdateBoundingBox();
                        break;
                    }
                }
            }

            //Friction
            if (this.velocity.X != 0 || this.velocity.Y != 0)
            {
                this.velocity = new Vector2(this.velocity.X * timeElapsed * 0.95f, this.velocity.Y * timeElapsed * 0.95f);
            }

            base.Update(gameTime);
        }

        public bool GetWillCollideWithOtherGameComponents(Vector2 positionShift)
        {
            GameComponentCollection components = game.Components;
            Rectangle bouncer;
            
            // See if we are going to go outside the game area's borders, not intersecting with the area borders
            bouncer = CreateCollisionRectangle(positionShift);
            bouncer.Inflate(-this.animation.Width + 1, -this.animation.Height + 1);
            if (!bouncer.Intersects(this.areaBorders))
            {
                return true;
            }

            // Collisions with game components
            bouncer = CreateCollisionRectangle(positionShift);

            foreach (GameComponent component in components)
            {
                if (!component.Equals(this))
                {
                    if (component.GetType().Equals(typeof(Placeable)))
                    {
                        Placeable placeable = (Placeable)component;
                        // if the object we are colliding with is not incollidable
                        // or the object colliding is incollidable
                        if (!placeable.Incollidable || this.Incollidable)
                        {
                            if (bouncer.Intersects(placeable.bouncingBox))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public Placeable GetCollidingPlaceable(Vector2 positionShift)
        {
            GameComponentCollection components = game.Components;

            foreach (GameComponent component in components)
            {
                if (!component.Equals(this))
                {
                    if (component.GetType().Equals(typeof(Placeable)))
                    {
                        Placeable placeable = (Placeable)component;
                        Rectangle bouncer = CreateCollisionRectangle(positionShift);
                        if (bouncer.Intersects(placeable.bouncingBox))
                        {
                            return placeable;
                        }
                    }
                }
            }

            return null;
        }

        public Placeable GetCollidingPlaceable(Rectangle bouncer)
        {
            GameComponentCollection components = game.Components;

            foreach (GameComponent component in components)
            {
                if (!component.Equals(this))
                {
                    if (bouncer.Intersects(((Placeable)component).bouncingBox))
                    {
                        return ((Placeable)component);
                    }
                }
            }

            return null;
        }

    }

}
