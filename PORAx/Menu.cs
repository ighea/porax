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
    public class Menu
    {
        private Game game;

        private MenuData menuData;

        KeyboardState oldKeyboardState;
        GamePadState oldGamePadOneState;

        private List<Animation> animations;

        private int currentMenuItem;

        private SoundEffect soundEffectMusic;
        private SoundEffectInstance soundEffectInstanceMusic;

        public Menu(Game game, MenuData menuData)
        {
            this.game = game;
            this.menuData = menuData;

            this.currentMenuItem = 0;

            // Load menu animations
            animations = new List<Animation>(4);
            foreach (MenuItem item in menuData.items)
            {
                List<Texture2D> textureList = new List<Texture2D>(3);
                foreach (string textureName in item.sprites)
                {
                    textureList.Add(this.game.Content.Load<Texture2D>(textureName));
                }
                animations.Add(new Animation(textureList, item.changeRate));
            }

            //Load background music
            if (menuData.backgroundMusic != "null")
            {
                this.soundEffectMusic = game.Content.Load<SoundEffect>(@menuData.backgroundMusic);
                this.soundEffectInstanceMusic = this.soundEffectMusic.CreateInstance();
            }
            else
            {
                this.soundEffectInstanceMusic = null;
                this.soundEffectMusic = null;
            }
            

        }

        public void PlayMusic()
        {
            // Play background music
            if (this.soundEffectInstanceMusic != null)
            {
                if (this.soundEffectInstanceMusic.State == SoundState.Stopped)
                {
                    this.soundEffectInstanceMusic.Play();
                }
                else if (this.soundEffectInstanceMusic.State == SoundState.Paused)
                {
                    this.soundEffectInstanceMusic.Resume();
                }
            }
        }

        public void PauseMusic()
        {
            if (this.soundEffectInstanceMusic != null)
            {
                if (this.soundEffectInstanceMusic.State == SoundState.Playing)
                {
                    this.soundEffectInstanceMusic.Pause();
                }
            }
        }

        public GameState Update(GameTime gameTime, GameState menuItem)
        {
            GameState selectedMenuItem = menuItem;

            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadOneState = GamePad.GetState(PlayerIndex.One);

            if ( (Input.GetInstance().IsPressed(Keys.Down) ||
                Input.GetInstance().IsPressed(Keys.Right))
                ||
                (gamePadOneState.ThumbSticks.Left.X > 0.1f && oldGamePadOneState.ThumbSticks.Left.X < 0.1f)
                )
            {
                this.currentMenuItem++;
                if (this.currentMenuItem > this.menuData.items.Length - 1)
                {
                    this.currentMenuItem = 0;
                }
                Console.WriteLine("Menu down!");
            }
            if ( (Input.GetInstance().IsPressed(Keys.Up) ||
                Input.GetInstance().IsPressed(Keys.Left))
                ||
                (gamePadOneState.ThumbSticks.Left.X < -0.1f && oldGamePadOneState.ThumbSticks.Left.X > -0.1f)
                )
            {
                this.currentMenuItem--;
                if (this.currentMenuItem < 0)
                {
                    this.currentMenuItem = this.menuData.items.Length - 1;
                }
                Console.WriteLine("Menu up!");
            }
            if ( Input.GetInstance().IsPressed(Keys.Enter) ||
                Input.GetInstance().IsPressed(Keys.Space)
                ||
                Input.GetInstance().IsPressed(Buttons.A))
            {
                Console.WriteLine("Current selected menuitem: " + this.menuData.items[this.currentMenuItem].gameState.ToString() );
                selectedMenuItem = this.menuData.items[this.currentMenuItem].gameState;
            }

            this.animations[this.currentMenuItem].Update(gameTime);

            oldKeyboardState = keyboardState;
            oldGamePadOneState = gamePadOneState;
            
            return selectedMenuItem;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.animations[this.currentMenuItem] != null)
            {
                Rectangle rect = new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height);
                this.animations[this.currentMenuItem].Draw(spriteBatch, rect, Color.White);
            }
        }
    }
}
