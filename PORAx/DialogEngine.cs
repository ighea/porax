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
    public class DialogEngine
    {

        private static DialogEngine instance = null;

        private List<Dialog> dialogList;

        private List<char> dialogSymbolList;

        /// <summary>
        /// Gets the state is the DialogEngine finished it's jobs.
        /// </summary>
        /// <returns>true if all dialogues are played</returns>
        public bool AllDialogPlayed()
        {
            if (this.dialogList.Count > 0)
            {
                return false;
            }
            return true;
        }

        public static DialogEngine GetInstance()
        {
            if (instance == null)
            {
                instance = new DialogEngine();
            }
            return instance;
        }
    
        private DialogEngine()
        {
            this.dialogList = new List<Dialog>(2);
            this.dialogSymbolList = new List<char>(10);
        }

        public void AddDialog(Dialog dialog, char symbol)
        {
            if (this.dialogSymbolList.Contains<char>(symbol))
            {
                return;
            }
            this.dialogSymbolList.Add(symbol);
            AddDialog(dialog);
        }

        public void AddDialog(Dialog dialog)
        {
            this.dialogList.Add(dialog);
        }

        public void Clear()
        {
            this.dialogList.Clear();
            this.dialogSymbolList.Clear();
        }

        public void Update(GameTime gameTime)
        {
            
            for (int i = dialogList.Count - 1; i >= 0; i--)
            {
                if (dialogList[i].Finished)
                {
                    dialogList.Remove(dialogList[i]);
                }
                else 
                {
                    dialogList[i].Update(gameTime);
                    break; // This makes sure that the latest added dialog is played first and then the possible out going dialogs are finished after it.
                }
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Dialog dialog in dialogList)
            {
                dialog.Draw(spriteBatch);
            }
        }

        
    }
}
