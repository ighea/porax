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

    public enum GameState {Intro, Mainmenu, Start, Info, Credits, Quit }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PORAGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont spriteFont20;

        bool quitFadeOutSet = false;

        public SpriteBatch SpriteBatch
        {
            get 
            {
                return spriteBatch;
            }
        }

        public GraphicsDeviceManager Graphics
        {
            get
            {
                return graphics;
            }
        }


        private Intro gameIntro;
        private GameState currentGameState;
        private Mapper mapper;
        private Menu mainMenu;
        private Credits credits;
        private Credits info;

        public PORAGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            // Set the starting game state.
            this.currentGameState = GameState.Intro;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            
            this.mainMenu = new Menu(this, XMLHandler.LoadXML<MenuData>(@"Menus\Mainmenu"));

            this.gameIntro = new Intro(this, "punyware");

            this.spriteFont20 = Content.Load<SpriteFont>("SpriteFont20");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            Input.GetInstance().Update();
            Fader.GetInstance().Update(elapsedTime);

            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadOneState = GamePad.GetState(PlayerIndex.One);

            // Menu music playback handling:
            // Play music in mainmenu, credits and info
            // pause for start or gameplay
            switch (this.currentGameState)
            {
                case GameState.Mainmenu:
                case GameState.Info:
                case GameState.Credits:
                    this.mainMenu.PlayMusic();
                break;
                case GameState.Start:
                    this.mainMenu.PauseMusic();
                break;
            }

            // Allows the game to exit
            if (Input.GetInstance().IsPressed(Buttons.Back) ||
                (Input.GetInstance().IsPressed(Keys.Escape)))
            {
                //if (this.currentGameState != GameState.Start)
                {
                    // If not in the MainMenu go there else goto Quit.
                    if (this.currentGameState != GameState.Mainmenu && this.currentGameState != GameState.Quit)
                    {
                        this.currentGameState = GameState.Mainmenu;
                        if (this.mapper != null)
                        {
                            Fader.GetInstance().Add(Fader.Fade.In, 1.0f);
                            this.mapper.Reset();
                            this.mapper = null;
                        }
                    }
                    else
                    {
                        this.currentGameState = GameState.Quit;
                    }
                }
            }

            switch (this.currentGameState)
            {
                case GameState.Intro:
                    this.gameIntro.Update(gameTime);
                    if (this.gameIntro.Finished)
                    {
                        Fader.GetInstance().Add(Fader.Fade.In, 1.0f);
                        this.gameIntro = null; // Shown it, not needed anymore
                        this.currentGameState = GameState.Mainmenu; // and goto main menu.
                    }
                    break;

                case GameState.Mainmenu:
                    this.currentGameState = this.mainMenu.Update(gameTime, this.currentGameState);
                    break;

                case GameState.Start:

                    // Update StageChanger.
                    if (this.mapper == null)
                    {
                        this.mapper = new Mapper(this, XMLHandler.LoadXML<MapperData>(@"stages.xml"));
                    }
                    else
                    {
                        this.mapper.Update(gameTime);


                        if (this.mapper.GameCompleted)
                        {   // All game stages completed. Game finished. Last videos and cutscenes played. Goto Credits.
                            this.mapper.Reset();
                            this.currentGameState = GameState.Credits;
                        }

                        if (this.mapper.Quitted)
                        {   // Quitted from the gameplay or Map. goto mainmenu.
                            this.mapper.Reset();
                            this.currentGameState = GameState.Mainmenu;
                        }
                    }

                    break;

                case GameState.Info:
                    // Info or instructions goes here:
                    if (this.info == null)
                    {
                        this.info = new Credits(this, "info");
                    }
                    if (this.info.Update(gameTime))
                    {
                        this.currentGameState = GameState.Mainmenu;
                        this.info = null;
                    }
                    break;

                case GameState.Credits:
                    // Credits goes here:
                    if (credits == null)
                    {
                        credits = new Credits(this, "credits");
                    }
                    if (this.credits.Update(gameTime))
                    {
                        this.currentGameState = GameState.Mainmenu;
                        credits = null;
                    }
                    break;

                case GameState.Quit:
                    // Exiting the game.
                    if (!this.quitFadeOutSet)
                    {
                        this.quitFadeOutSet = true;
                        Fader.GetInstance().Add(Fader.Fade.Out, 1.0f);
                    }
                    if (Fader.GetInstance().IsFadeFinished())
                    {
                        this.Exit();
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            //Show loading text if nothing is being drawn "abowe" it == always draw.
            string loadingText = "Loading...";
            Vector2 textDimensionVector = spriteFont20.MeasureString(loadingText);
            Vector2 viewportDimensionVector = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            spriteBatch.DrawString(
                spriteFont20, 
                loadingText, 
                viewportDimensionVector - 2 * textDimensionVector, 
                Color.Gainsboro
                );

            if (this.mapper != null)
            {
                if (!this.mapper.GameCompleted || !this.mapper.Quitted)
                {
                    this.mapper.Draw(spriteBatch);
                }
            }
            if (this.mainMenu != null)
            {
                if (this.currentGameState == GameState.Mainmenu ||this.currentGameState == GameState.Quit)
                {
                    this.mainMenu.Draw(spriteBatch);
                }
            }

            if (this.gameIntro != null && GameState.Intro == this.currentGameState)
            {
                this.gameIntro.Draw(spriteBatch);
            }

            if (this.credits != null && GameState.Credits == this.currentGameState)
            {
                this.credits.Draw(spriteBatch);
            }

            if (this.info != null && GameState.Info == this.currentGameState)
            {
                this.info.Draw(spriteBatch);
            }

            Fader.GetInstance().Draw(spriteBatch);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
