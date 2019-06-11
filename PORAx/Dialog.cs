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
    public class Dialog
    {
        private Game game;
        private DialogData dialogData;
        private Texture2D backgroundTexture;
        private Color color;

        private List<Animation> faceAnimationsList = new List<Animation>();
        private List<String> faceNames = new List<String>();

        private int currentDialog = 0;
        private int dialogCount = 0;

        private string currentDialogString = "";

        private SpriteFont spriteFont20;

        private Timer timer;

        private bool running;

        public bool Finished
        {
            get 
            {
                return !this.running;
            }
        }

        public Dialog(Game game, DialogData dialogData)
        {
            this.game = game;
            this.dialogData = dialogData;
            this.color = Color.White;
            this.running = true;

            // Load font
            spriteFont20 = game.Content.Load<SpriteFont>("SpriteFont20");

            // Load backgroundTexture
            backgroundTexture = game.Content.Load<Texture2D>(dialogData.background);

            // Load and create the face animations
            for (int i = 0; i < dialogData.dialogs.Length; i++)
            {
                FaceData faceData = XMLHandler.LoadXML<FaceData>(dialogData.dialogs[i].faceAnimation);
                List<Texture2D> textureList = new List<Texture2D>();
                for (int j = 0; j < faceData.textures.Length; j++)
                {
                    textureList.Add(game.Content.Load<Texture2D>(faceData.textures[j]));
                }
                Animation anim = new Animation(textureList, faceData.frameChangeSpeed);
                faceAnimationsList.Add(anim);
            }

            // Number of dialogs
            this.dialogCount = dialogData.dialogs.Length;

            // Timer for changing the dialog
            timer = new Timer(dialogData.dialogs[0].textDisplayTime, true);


            MakeDialogTextFit();
        }

        /// <summary>
        /// This method measures the width space available for the text from the backgroundTexture's width minus 155 pixels and splits the text to several lines if necessary.
        /// </summary>
        private void MakeDialogTextFit()
        {
            int textMaxWidth = this.backgroundTexture.Width - 150 - 5;
            string text = this.dialogData.dialogs[this.currentDialog].text;
            int width = (int)this.spriteFont20.MeasureString(text).X;
            
            if (width > textMaxWidth)
            {
                string[] words = text.Split(' ');
                text = "";
                int counter = 0;
                do
                {
                    text = text + words[counter] + " ";
                    if (counter + 1 < words.Length)
                    {
                        if ((int)this.spriteFont20.MeasureString(text + words[counter + 1]).X > textMaxWidth)
                        {
                            text = text + "\n";
                        }
                    }
                    else
                    {
                        break;
                    }
                    counter++;
                }
                while(width > textMaxWidth);
            }
            
            this.currentDialogString = text;
        }

        public void Update(GameTime gameTime)
        {
            if (this.running)
            {
                float elapsedTime = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                this.faceAnimationsList[this.currentDialog].Update(gameTime);

                if (timer.GetIsTriggered(elapsedTime))
                {
                    if (this.currentDialog < this.dialogCount - 1)
                    {
                        this.currentDialog++;
                        MakeDialogTextFit();
                        timer.SetTriggerTime(this.dialogData.dialogs[this.currentDialog].textDisplayTime);
                    }
                    else
                    {
                        this.running = false;
                    }
                
                }
            
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.running)
            {

                // Background
                spriteBatch.Draw(backgroundTexture, Vector2.Zero, this.color);

                // Face
                Vector2 facePosition = new Vector2(80, 75);
                this.faceAnimationsList[this.currentDialog].Draw(spriteBatch, facePosition, MathHelper.PiOver2, SpriteEffects.None);

                // Text
                Vector2 textPosition = new Vector2(150, 20);
                spriteBatch.DrawString(this.spriteFont20,
                    this.currentDialogString,
                    textPosition,
                    color);

            }

        }

    }
}
