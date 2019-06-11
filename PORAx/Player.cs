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
    public enum Direction { Right, Left, Up, Down, Center }

    public enum AnimationStates { Normal, Broken }

    class Player: Placeable
    {
        private bool alive;
        private bool atEndCondition;

        private float acceleration;
        private Sprite spriteDrillFront;
        private Sprite spriteDrillBack;
        private Direction direction;

        private bool drilling;
        private Direction drillingDirection;
        private float currentDrillRotatingSpeed;
        private float minDrillRotatingSpeedToDrill;

        private bool jumping;
        private float jumpTimer;

        private Animation[] animations;

        public bool AtEndCondition
        {
            get
            {
                return this.atEndCondition;
            }
        }

        public bool Alive
        {
            get
            {
                return this.alive;
            }
        }

        public Player(Game game, Vector2 position, Rectangle areaBorders, PlaceableData data, char symbol)
            : base(game, position, areaBorders, data, symbol)
        {
            this.acceleration = data.acceleration;
            this.direction = Direction.Left;
            this.drilling = false;
            this.drillingDirection = Direction.Center;
            this.jumping = false;
            this.jumpTimer = 0.0f;
            this.alive = true;
            this.atEndCondition = false;
            this.currentDrillRotatingSpeed = 0.0f;
            this.minDrillRotatingSpeedToDrill = 0.25f;

            this.LoadAndSetupAnimations();

            this.LoadDrillData();
        }

        private void LoadAndSetupAnimations()
        {
            this.animations = new Animation[Enum.GetNames(typeof(AnimationStates)).Length];
            this.animations[(int)AnimationStates.Normal] = this.animation;

            List<Texture2D> textureList = new List<Texture2D>();
            textureList.Add(this.game.Content.Load<Texture2D>(@"Pora\broken1"));
            textureList.Add(this.game.Content.Load<Texture2D>(@"Pora\broken2"));
            Animation anim = new Animation(textureList, 0.1f);
            this.animations[(int)AnimationStates.Broken] = anim;
        }

        private void LoadDrillData()
        {
            List<Texture2D> listDrillFront = new List<Texture2D>();
            List<Texture2D> listDrillBack = new List<Texture2D>();

            for (int i = 1; i < 4; i++)
            {
                listDrillFront.Add(game.Content.Load<Texture2D>(@"Drill\front" + i));
                listDrillBack.Add(game.Content.Load<Texture2D>(@"Drill\back" + i));
            }

            spriteDrillBack = new Sprite(game, this.position, new Animation(listDrillBack, 0.1f));
            spriteDrillFront = new Sprite(game, this.position, new Animation(listDrillFront, 0.1f));

        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float timeElapsed = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadOneState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDown(Keys.Left) || gamePadOneState.ThumbSticks.Left.X < 0)
            {
                this.velocity -= new Vector2(timeElapsed * this.acceleration, 0);
                this.direction = Direction.Left;
                this.drillingDirection = Direction.Left;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || gamePadOneState.ThumbSticks.Left.X > 0)
            {
                this.velocity += new Vector2(timeElapsed * this.acceleration, 0);
                this.direction = Direction.Right;
                this.drillingDirection = Direction.Right;
            }

            // Play the moving animation if player presses left or right movement buttons
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.Right) ||
                (gamePadOneState.ThumbSticks.Left.X != 0))
            {
                this.animation.Playing = true;
            }
            else
            {
                this.animation.Playing = false;
            }

            // Drilling and playing the animations for both drills
            if (keyboardState.IsKeyDown(Keys.Space) || gamePadOneState.IsButtonDown(Buttons.RightTrigger))
            {
                this.drilling = true;
                this.spriteDrillBack.Animation.Playing = true;
                this.spriteDrillFront.Animation.Playing = true;
            }
            else
            {
                this.drilling = false;
                this.spriteDrillBack.Animation.Playing = false;
                this.spriteDrillFront.Animation.Playing = false;
            }

            //JUMPING
            if (keyboardState.IsKeyDown(Keys.LeftAlt) || gamePadOneState.IsButtonDown(Buttons.LeftTrigger))
            {
                if (this.jumping == false)
                {
                    if (GetWillCollideWithOtherGameComponents(new Vector2(0, 1)))
                    { // Player is on another object or on the "ground" so we can jump
                        this.jumping = true;
                    }
                }
            }

            if (keyboardState.IsKeyDown(Keys.Up) || gamePadOneState.ThumbSticks.Left.Y > 0.1f)
            {
                this.drillingDirection = Direction.Up;
            }
            else if (keyboardState.IsKeyDown(Keys.Down) || gamePadOneState.ThumbSticks.Left.Y < -0.1f)
            {
                this.drillingDirection = Direction.Down;
            }
            else if (keyboardState.IsKeyDown(Keys.Left) || gamePadOneState.ThumbSticks.Left.X < 0)
            {
                this.drillingDirection = Direction.Left;
            }
            else if (keyboardState.IsKeyDown(Keys.Right) || gamePadOneState.ThumbSticks.Left.X > 0)
            {
                this.drillingDirection = Direction.Right;
            }

            ApplyMovementVelocity(this.velocity);

            UpdateDrillPositions(gameTime);

            ApplySpriteEffects(gameTime);

            DoDrilling(gameTime);

            DoJumping(gameTime, timeElapsed);

            CheckForFallingAndDamagingPlaceables(gameTime);

            CheckIfPlayerIsAtEndCondition();

            ApplyAnimationChanges();    
        }

        private void ApplyAnimationChanges()
        {
            int count = Enum.GetNames(typeof(AnimationStates)).Length;

            if (this.health == 2)
            {
                this.animation = this.animations[(int)AnimationStates.Normal];
            }
            else
            {
                this.animation = this.animations[(int)AnimationStates.Broken];
            }

        }

        private void CheckIfPlayerIsAtEndCondition()
        {
            Placeable placeable = GetCollidingPlaceable(Vector2.Zero);
            if (placeable != null)
            {
                if (placeable.EndCondition)
                {
                    this.atEndCondition = true;
                }
            }

        }


        private void CheckForFallingAndDamagingPlaceables(GameTime gameTime)
        {
            Placeable placeable = this.GetCollidingPlaceable(new Vector2(0, -2));
            if (placeable != null)
            {
                if (placeable.Damage > 0)
                {
                    Console.WriteLine("1 Teh ultimate collision!!!! " + placeable.Position.Y);
                    // Item is falling
                    if (placeable.Velocity.Y > 0)
                    {
                        Console.WriteLine("0 Teh ultimate collision!!!!" + placeable.Position.Y);
                        this.health -= placeable.Damage;
                        EffectEngine.GetInstance().AddEffect(game, placeable.Position, placeable.Animation.Texture, 1.0f);
                        placeable.Destroy();
                    } // Item horribly dangeroud and incollidable
                    else if (placeable.Incollidable)
                    {
                        this.health -= placeable.Damage;
                        EffectEngine.GetInstance().AddEffect(game, placeable.Position, placeable.Animation.Texture, 1.0f);
                        placeable.Destroy();
                    }
                    // See if player died
                    if (this.health <= 0)
                    {
                        this.alive = false;
                        
                        EffectEngine.GetInstance().AddEffect(game, this.Position, this.Animation.Texture, 1.0f);
                        EffectEngine.GetInstance().AddEffect(game, this.spriteDrillFront.Position, this.spriteDrillFront.Animation.Texture, 1.0f);
                        this.Visible = false;
                        this.spriteDrillBack.Visible = false;
                        this.spriteDrillFront.Visible = false;
                        this.PlayDestroyedSoundEffect();
                    }
                }
                placeable.PlayDialog();
            }
        }

        private void DoJumping(GameTime gameTime, float timeElapsed)
        {
            if (this.jumping)
            {
                jumpTimer += timeElapsed;
                if (jumpTimer >= 0.5f)
                {
                    this.jumping = false;
                    jumpTimer = 0.0f;
                }
                else if (GetWillCollideWithOtherGameComponents(new Vector2(0, -1)))
                {
                    this.jumping = false;
                }
                else
                {
                    this.velocity -= new Vector2(0, timeElapsed * 250.0f);

                }
            }
        }

        private void DoDrilling(GameTime gameTime)
        {
            if (this.drilling)
            {
                Rectangle collider = new Rectangle();
                int xShift = 0;
                int yShift = 0;
                switch (this.drillingDirection)
                {
                    case Direction.Left:
                        xShift = -50;
                        break;
                    case Direction.Right:
                        xShift = 50;
                        break;
                    case Direction.Down:
                        yShift = 30;
                        break;
                    case Direction.Up:
                        yShift = -50;
                        break;
                }

                this.currentDrillRotatingSpeed += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                if (this.currentDrillRotatingSpeed >= this.minDrillRotatingSpeedToDrill)
                {
                    collider = new Rectangle((int)this.position.X + xShift, (int)this.position.Y + yShift, 10, 10);

                    Placeable placeable = GetCollidingPlaceable(collider);

                    if (placeable != null)
                    {// Found an placeable
                        // See if it is drillable
                        if (placeable.Drillable)
                        {
                            EffectEngine.GetInstance().AddEffect(game, placeable.Position, placeable.Animation.Texture, 1.0f);
                            placeable.Destroy();
                        }
                    }
                }
            }
            else
            {
                this.currentDrillRotatingSpeed = 0.0f;
            }
        }

        private void ApplySpriteEffects(GameTime gameTime)
        {
            if (this.direction == Direction.Left)
            {
                this.spriteEffect = SpriteEffects.None;
            }
            if (this.direction == Direction.Right)
            {
                this.spriteEffect = SpriteEffects.FlipHorizontally;
            }

            this.spriteDrillBack.SpriteEffect = this.spriteEffect;
            this.spriteDrillFront.SpriteEffect = this.spriteEffect;

        }
        
        public void UpdateDrillPositions(GameTime gameTime)
        {
            Vector2 positionChange = Vector2.Zero;
            
            this.spriteDrillBack.Update(gameTime);
            this.spriteDrillFront.Update(gameTime);

            if (direction == Direction.Left)
            {
                positionChange = new Vector2(-28, 7.0f);
            }
            if (direction == Direction.Right)
            {
                positionChange = new Vector2(28, 7.0f);
            }

            // drill sprite position fixes and rotations.
            if (drillingDirection == Direction.Down)
            {
                if (direction == Direction.Right)
                {
                    positionChange = new Vector2(5.0f, 24.0f);
                    this.spriteDrillBack.Angle = -MathHelper.Pi - MathHelper.PiOver4 / 2;
                    this.spriteDrillFront.Angle = -MathHelper.Pi - MathHelper.PiOver4 / 2;
                }
                if (direction == Direction.Left)
                {
                    positionChange = new Vector2(-5.0f, 24.0f);
                    this.spriteDrillBack.Angle = MathHelper.PiOver4/2;
                    this.spriteDrillFront.Angle = MathHelper.PiOver4/2;
                }
            }
            else if (drillingDirection == Direction.Up)
            {
                if (direction == Direction.Right)
                {
                    positionChange = new Vector2(15.0f, -22.0f);
                    this.spriteDrillBack.Angle = MathHelper.PiOver4/4;
                    this.spriteDrillFront.Angle = MathHelper.PiOver4/4;
                }
                if (direction == Direction.Left)
                {
                    positionChange = new Vector2(-15.0f, -22.0f);
                    this.spriteDrillBack.Angle = -MathHelper.PiOver4 / 4 + MathHelper.Pi;
                    this.spriteDrillFront.Angle = -MathHelper.PiOver4 / 4 + MathHelper.Pi;
                }
            }
            else
            { // Going to horizontal direction, set angle to 90 degree
                this.spriteDrillBack.Angle = MathHelper.PiOver2;
                this.spriteDrillFront.Angle = MathHelper.PiOver2;
            }

            this.spriteDrillBack.Position = this.position + positionChange - new Vector2(0, 3);
            this.spriteDrillFront.Position = this.position + positionChange; ;
        }

        public void ApplyMovementVelocity(Vector2 positionShift)
        {
            GameComponentCollection components = game.Components;

            foreach (GameComponent component in components)
            {
                if (!component.Equals(this))
                {
                    if (component.GetType().Equals(typeof(Placeable)))
                    {
                        // See if the item is not penetrable
                        if (!((Placeable)component).Incollidable)
                        {
                            //See if it is moveable
                            if (((Placeable)component).Moveable)
                            {
                                // Pushing objects
                                Rectangle bouncer = CreateCollisionRectangle(positionShift);
                                if (bouncer.Intersects(((Placeable)component).BoundingBox))
                                {
                                    if (this.position.Y > ((Placeable)component).Position.Y - bouncer.Height / 2 &&
                                        this.position.Y < ((Placeable)component).Position.Y + bouncer.Height / 2)
                                    {
                                        ((Placeable)component).Velocity += new Vector2(this.velocity.X, 0);
                                    }
                                }
                                // Moving objects by rolling on them
                                bouncer = CreateCollisionRectangle(new Vector2(0, 1));
                                if (bouncer.Intersects(((Placeable)component).BoundingBox))
                                {
                                    if (this.position.Y <= ((Placeable)component).Position.Y)
                                    {
                                        ((Placeable)component).Velocity -= new Vector2(this.velocity.X, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteDrillBack.Draw(spriteBatch);
            base.Draw(spriteBatch);
            spriteDrillFront.Draw(spriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            spriteDrillBack.Draw(spriteBatch, offset);
            base.Draw(spriteBatch, offset);
            spriteDrillFront.Draw(spriteBatch, offset);
        }

    }

}
