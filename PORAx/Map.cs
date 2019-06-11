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

    class Map
    {

        private Game game;

        private bool completed;

        private bool isGamePaused;

        private bool endConditionReached;

        private Player player;

        List<Placeable> placeables;

        private Texture2D background;

        private Texture2D textureGameOver;

        private Camera camera;

        private Rectangle areaBorders;

        private Timer endingTimer;
        
        private Timer deadTimer;

        private SoundEffect soundEffectMusic;
        private SoundEffectInstance soundEffectInstanceMusic;

        public Player Player
        {
            get 
            {
                return player;
            }
        }

        public bool Completed
        {
            get 
            {
                return this.completed;
            }
        }

        public bool Paused
        {
            get
            {
                return this.isGamePaused;
            }
        }

        private float completionTime = 0.0f;
        public float CompletionTime
        {
            get
            {
                return this.completionTime;
            }
        }

        public Map(Game game, MapData mapData)
        {
            this.game = game;
            this.completed = false;
            this.isGamePaused = false;
            this.endConditionReached = false;

            camera = new Camera(((PORAGame)game).Graphics);

            int offset = mapData.backgroundOffset;
            int width = 0;
            int height = 0;

            this.completionTime = 0.0f;
            
            placeables = new List<Placeable>();

            background = game.Content.Load<Texture2D>(mapData.background);

            for (int y = mapData.Tiles.Length -1; y >= 0 ; y--)
            {
                if (y + 1 > height)
                {
                    height = y + 1;
                }
                for (int x = 0; x < mapData.Tiles[y].Length; x++)
                {
                    if (x + 1 > width)
                    {
                        width = x + 1;
                    }
                    Vector2 position = new Vector2(x * 64 + 32, y * 64 + 32 + offset);

                    foreach (tileProperty item in mapData.tileInfo)
                    {
                        if (mapData.Tiles[y][x] == item.symbol)
                        {
                            PlaceableData placeableData = XMLHandler.LoadXML<PlaceableData>(item.name);
                            if (placeableData.player == true)
                            {
                                player = new Player(game, position, areaBorders, placeableData, item.symbol);
                            }
                            else
                            {
                                placeables.Add(
                                        new Placeable(
                                            game,
                                            position,
                                            areaBorders,
                                            placeableData,
                                            item.symbol
                                            )
                                    );
                            }
                        }
                    }

                }
            }


            // Apply area borders to all items and enable game components
            areaBorders = new Rectangle(0, 0, width * 64, height * 64 + offset);
            foreach(Placeable item in placeables)
            {
                item.SetBordersRectangle(areaBorders);
                item.Enabled = true;
            }
            player.SetBordersRectangle(areaBorders);
            player.Enabled = true;

            // Create the ending delay timer
            endingTimer = new Timer(mapData.endDelay, false);

            // Create timer for death's after effects
            deadTimer = new Timer(1.0f, false);

            // Create dialog and add it to the DialogEngine
            DialogEngine.GetInstance().AddDialog(new Dialog(game, XMLHandler.LoadXML<DialogData>(mapData.dialog)));

            // Load Game Over dialog
            textureGameOver = game.Content.Load<Texture2D>("gameover");
        
            //Load background music
            if (mapData.music != "null")
            {
                this.soundEffectMusic = game.Content.Load<SoundEffect>(@mapData.music);
                this.soundEffectInstanceMusic = this.soundEffectMusic.CreateInstance();
            }
            else 
            {
                this.soundEffectInstanceMusic = null;
                this.soundEffectMusic = null;
            }
            


        }

        ~Map()
        {
            foreach (Placeable item in placeables)
            {
                item.Dispose();
            }
            placeables.Clear();
            if (this.player != null)
            {
                player.Dispose();
                player = null;
            }
            EffectEngine.GetInstance().Clear();
            DialogEngine.GetInstance().Clear();
        }

        public void Pause()
        {
            this.isGamePaused = true;
            foreach (Placeable item in placeables)
            {
                item.Enabled = false;
            }
            if (this.player != null)
            {
                player.Enabled = false;
            }
        
        }

        public void Resume()
        {
            this.isGamePaused = false;
            foreach (Placeable item in placeables)
            {
                item.Enabled = true;
            }
            if (this.player != null)
            {
                player.Enabled = true;
            }
       
        }

        public void Update(GameTime gameTime)
        {
            if (!this.isGamePaused)
            {

                float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

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

                // Increase level completion time as long as play is alive and not reached end conditions
                if (this.player.Alive && !this.endConditionReached)
                {
                    this.completionTime = this.completionTime + elapsedTime;
                }


                // Player is at end condition. This makes sure that the end condition reached status stays active even if player has left the end condition location
                if (player.AtEndCondition)
                {
                    Console.WriteLine("End condition reached!");
                    this.endConditionReached = true;
                }

                if (this.endConditionReached)
                {
                    // Disable player's movements
                    // Wait until all dialog is finished before exiting
                    if (DialogEngine.GetInstance().AllDialogPlayed() && endingTimer.GetIsTriggered(elapsedTime))
                    {
                        Console.WriteLine("Map really ended.");
                        this.completed = true;
                        player.Enabled = false;
                    }
                }

                if (!player.Alive)
                {
                    player.Enabled = false;
                    // Player is dead, update deadTimer to be used to enable drawing the retry graphics
                    deadTimer.GetIsTriggered(elapsedTime);
                }

                // Do updates for effects
                EffectEngine.GetInstance().Update(gameTime);

                // Update dialog
                if (player.Alive)
                {
                    DialogEngine.GetInstance().Update(gameTime);
                }
            }
            else 
            {
                // If the game is paused, pause the background music playback too.
                if (this.soundEffectInstanceMusic != null)
                {
                    if (this.soundEffectInstanceMusic.State == SoundState.Playing)
                    {
                        this.soundEffectInstanceMusic.Pause();
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //if (!this.isGamePaused)
            {

                Vector2 offset = Vector2.Zero;
                if (this.player != null)
                {
                    offset = camera.GetUpdatedOffset(this.player.Position, this.areaBorders);
                }

                spriteBatch.Draw(background, Vector2.Zero + offset, Color.White);
                //spriteBatch.Draw(background, new Vector2(background.Width, 0) + offset, Color.White);

                foreach (Placeable item in game.Components)
                {
                    if (item != null)
                    {
                        item.Draw(spriteBatch, offset);
                    }
                }
                if (this.player != null)
                {
                    player.Draw(spriteBatch, offset);
                }

                // Draw effects
                EffectEngine.GetInstance().Draw(spriteBatch, offset);

                // Draw all in the dialog
                if (player.Alive)
                {
                    DialogEngine.GetInstance().Draw(spriteBatch);
                }

                // Draw death's after thingies

                if (!player.Alive && deadTimer.Triggered)
                {
                    spriteBatch.Draw(textureGameOver, Vector2.Zero, Color.White);
                }

            }
        }

    }

}
