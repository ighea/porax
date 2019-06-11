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
    public class Mapper
    {
        private SpriteFont spriteFont20;

        private Video cutsceneVideoFile;
        private VideoPlayer videoPlayer;

        private Texture2D texturePauseMenu;
        private SpriteFont SpriteFont20;

        private MapperData mapperData;
        private Game game;
        private Map map;
        private int currentStage;

        private bool stagesCompleted;
        private bool stagesCompletedFadeInStarted;

        private bool quitted;

        private bool cutsceneSkipAttempt;
        private Timer cutsceneSkipAttemptTimer;

        private float totalGameTime;
        private bool totalGameTimeUpdated;

        private bool gameCompleted;
        
        public bool GameCompleted
        {
            get
            {
                return this.gameCompleted;
            }
        }

        public bool Quitted
        {
            get 
            {
                return this.quitted;
            }
        }

        Timer stageChangerTimer;

        Timer gameCompletionTimer;

        Timer stageEndFadeOut;

        public Mapper(Game game, MapperData data)
        {
            this.currentStage = 0;
            this.game = game;
            this.mapperData = data;
            this.map = null;
            this.quitted = false;
            this.stagesCompleted = false;
            this.stagesCompletedFadeInStarted = false;

            spriteFont20 = game.Content.Load<SpriteFont>("SpriteFont20");

            texturePauseMenu = game.Content.Load<Texture2D>("Pausemenu");
            SpriteFont20 = game.Content.Load<SpriteFont>("SpriteFont20");

            stageChangerTimer = new Timer(data.stageEndDelay, false);

            this.videoPlayer = new VideoPlayer();
            this.cutsceneVideoFile = null;

            this.cutsceneSkipAttempt = false;
            this.cutsceneSkipAttemptTimer = new Timer(3, false);

            this.gameCompletionTimer = new Timer(60, false);

            this.totalGameTimeUpdated = false;
            this.gameCompleted = false;

            Console.WriteLine("Mapper created.");
        }

        ~Mapper()
        {
            // Mapper's destructor
            Console.WriteLine("Mapper's destructor called.");
        }

        public void Reset()
        {
            this.gameCompletionTimer.Reset();
            this.totalGameTimeUpdated = false;
            this.totalGameTime = 0.0f;
            this.cutsceneSkipAttempt = false;
            this.videoPlayer.Stop();
            this.map = null;
            this.cutsceneVideoFile = null;
            this.stagesCompleted = false;
            this.stagesCompletedFadeInStarted = false;
            this.gameCompleted = false;
            this.quitted = false;
            this.currentStage = 0;
            stageChangerTimer.Reset();
            // Run the garbage collectors to cleanup now.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadOneState = GamePad.GetState(PlayerIndex.One);
            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // All game levels completed and cutscene videos played. Show total game time and exit the game stage changer.
            if (this.stagesCompleted)
            {
                // Add Fade In and show the finishing aspects.
                if (!this.stagesCompletedFadeInStarted)
                {
                    this.stagesCompletedFadeInStarted = true;
                    Fader.GetInstance().Add(Fader.Fade.In, 1);
                }

                // Finish the whole game and return to credits/main menu after
                // space is pressed or timer triggered.
                if (this.gameCompletionTimer.GetIsTriggered(elapsedTime) || 
                    Input.GetInstance().IsPressed(Keys.Space))
                {
                    this.gameCompleted = true;
                }
                return;
            }


            // If both map and cutsceneVideoFile are null load the currentStage to get things running
            if (this.map == null && this.cutsceneVideoFile == null)
            {
                if (this.currentStage < this.mapperData.stages.Length)
                { // Uncompleted stages left, load next:
                    LoadStage(false);
                }
                else
                { // All stages completed, set bool.
                    this.stagesCompleted = true;
                }
            }
            else
            {
                // MAP RELATED
                if (this.map != null)
                {
                    // Allows the game to exit
                    if (!map.Player.Alive && (keyboardState.IsKeyDown(Keys.Q) || (gamePadOneState.IsButtonDown(Buttons.Back) && gamePadOneState.IsButtonUp(Buttons.Back))))
                    {
                        this.quitted = true;
                    }
                    
                    // Reset the current stage when showing the death screen and player presses Y-button/key
                    if (!map.Player.Alive && (keyboardState.IsKeyDown(Keys.Y) || gamePadOneState.IsButtonDown(Buttons.Y)))
                    {
                        LoadStage(false);
                    }

                    // Pausing and resuming the gameplay
                    if (map.Player.Alive)
                    {
                        if (Input.GetInstance().IsPressed(Keys.P) || Input.GetInstance().IsPressed(Buttons.Start))
                        {
                            if (map.Paused)
                            {
                                map.Resume();
                            }
                            else
                            {
                                map.Pause();
                            }
                        }
                    }

                    // If the map is completed / end condition(s) reached load next stage.
                    if (this.map.Completed)
                    {
                        if (this.totalGameTimeUpdated == false)
                        {
                            this.totalGameTime += this.map.CompletionTime;
                            this.totalGameTimeUpdated = true;
                            this.stageEndFadeOut = new Timer(stageChangerTimer.TriggerTime - 1.0f, false);
                        }
                        this.map.Player.Enabled = false;

                        if (this.stageEndFadeOut.GetIsTriggered(elapsedTime))
                        {
                            Fader.GetInstance().Add(Fader.Fade.Out, 1);
                        }

                        if (stageChangerTimer.GetIsTriggered(elapsedTime))
                        {

                            stageChangerTimer.Reset();
                            this.LoadStage(true);

                        }
                    }


                    // Map update - the game logics do their thingies!
                    if (this.map != null)
                    {
                        this.map.Update(gameTime);
                    }
                } // MAP RELATED ENDS

                // CUTSCENE VIDEO PLAYBACK RELATED STARTS
                if (this.cutsceneVideoFile != null)
                {
                    if (this.videoPlayer.State == MediaState.Stopped)
                    {
                        Console.WriteLine("Video playback stopped, load next stage.");
                        this.cutsceneVideoFile = null;
                        this.LoadStage(true);
                    }
                    if (this.videoPlayer.State == MediaState.Playing)
                    {
                        if (Input.GetInstance().IsPressed(Keys.Space) ||
                            Input.GetInstance().IsPressed(Buttons.A))
                        {
                            if (this.cutsceneSkipAttempt)
                            {
                                // Skipping the current video and moving to next stage.
                                this.cutsceneSkipAttempt = false;
                                this.videoPlayer.Stop(); // Stoping the video will move us forward in the stage changer.
                                this.cutsceneSkipAttemptTimer.Reset();
                            }
                            else
                            {
                                // First skip attempt shows only the text for skipping.
                                this.cutsceneSkipAttempt = true;
                                this.cutsceneSkipAttemptTimer.Reset();
                            }
                        }
                        if (this.cutsceneSkipAttemptTimer.GetIsTriggered(elapsedTime) && this.cutsceneSkipAttempt)
                        { // Hide the skipping text if the skip-key is not pressed during the time period.
                            this.cutsceneSkipAttempt = false;
                            this.cutsceneSkipAttemptTimer.Reset();
                        }
                    }
                }
                // CUTSCENE VIDEO PLAYBACK RELATED ENDS

            }



            // Reset the current stage
            if (keyboardState.IsKeyDown(Keys.R) || gamePadOneState.IsButtonDown(Buttons.BigButton))
            {
                LoadStage(false);
            }

        }

        /// <summary>
        /// Loads the next game stage
        /// </summary>
        /// <param name="next">True to load next stage, false to repeat the current.</param>
        private void LoadStage(bool next)
        {
            if (next)
            {
                this.currentStage++;
            }

            if (this.cutsceneVideoFile != null)
            {
                this.videoPlayer.Stop();
            }

            this.map = null;
            this.cutsceneVideoFile = null;
            this.cutsceneSkipAttempt = false;

            // Run the garbage collectors to cleanup now.
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (this.currentStage >= this.mapperData.stages.Length)
            {
                Console.WriteLine("End of stages reached!");
                this.stagesCompleted = true;
                return;
            }

            // Load map if the currentStage is a map else load video file
            if (this.mapperData.stages[this.currentStage].isMap == true)
            {
                Console.WriteLine("Load map #{0}: " + this.mapperData.stages[this.currentStage].xmlDescriptor, this.currentStage);

                this.map = new Map(game, XMLHandler.LoadXML<MapData>(this.mapperData.stages[this.currentStage].xmlDescriptor));
                // Update map once with zero to have all animations set and ready.
                this.map.Update(new GameTime());
            }
            else
            {
                Console.WriteLine("Play video #{0}: " + this.mapperData.stages[this.currentStage].xmlDescriptor, this.currentStage);

                this.cutsceneVideoFile = this.game.Content.Load<Video>(this.mapperData.stages[this.currentStage].xmlDescriptor);
                this.videoPlayer.Play(this.cutsceneVideoFile);
            }

            // New stage, set totalGameTimeUpdated flag to not set
            this.totalGameTimeUpdated = false;
            
            this.game.ResetElapsedTime();

            // Set effect to Fade In
            Fader.GetInstance().Add(Fader.Fade.In, 1);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // If map is active draw game related things.
            if (map != null)
            {
                map.Draw(spriteBatch);

                if (this.map.Paused)
                {
                    spriteBatch.Draw(texturePauseMenu, Vector2.Zero, Color.White);
                }

                if (this.map.Completed)
                {
                    String text = "  Victory! \nTime: " + this.map.CompletionTime.ToString("0.00") + " s\nTotal time: " + this.totalGameTime.ToString("0.00") + " s";
                    spriteBatch.DrawString(SpriteFont20, text, new Vector2(350, 280), Color.Blue);
                }

            }

            // Draw full viewport video playback if video is playing.
            if (this.videoPlayer.State == MediaState.Playing &&
                this.cutsceneVideoFile != null)
            {
                int width = spriteBatch.GraphicsDevice.Viewport.Width;
                int height = spriteBatch.GraphicsDevice.Viewport.Height;

                Rectangle videoRectange = new Rectangle(0, 0, width, height);
                spriteBatch.Draw(this.videoPlayer.GetTexture(), videoRectange, Color.White);

                if (this.cutsceneSkipAttempt)
                {
                    string text = "";
                    if (GamePad.GetState(PlayerIndex.One).IsConnected)
                    {
                        text = "Press A to skip";
                    }
                    else
                    {
                        text = "Press space to skip";
                    }
                    Vector2 vString = spriteFont20.MeasureString(text);
                    Vector2 textPos = new Vector2(spriteBatch.GraphicsDevice.Viewport.Width - vString.X - 5, spriteBatch.GraphicsDevice.Viewport.Height - vString.Y - 5);
                    
                    spriteBatch.DrawString(spriteFont20, text, textPos, Color.Gray);
                }

            }

            // All stages completed, draw completion screen showing game time
            if (this.stagesCompleted)
            {
                spriteBatch.GraphicsDevice.Clear(Color.Black);
                String congratsText = "  Congratulations!\n\nWorld was saved in " + this.totalGameTime.ToString("0.00") + " seconds!\n\n   Good job!";
                spriteBatch.DrawString(spriteFont20, congratsText,
                    new Vector2(300, 300), Color.PowderBlue);
            }

        }


    }
}
