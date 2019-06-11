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
    public enum EffectType { Fade };

    public class EffectEngine
    {
        private static EffectEngine instance;

        private List<Effect> effectList;

        public List<Effect> EffectList
        {
            get
            {
                return effectList;
            }
        }

        private EffectEngine()
        {
            effectList = new List<Effect>(200);
        }

        static public EffectEngine GetInstance()
        {
            if (instance == null)
            {
                instance = new EffectEngine();
            }
            return instance;
        }

        
        public void Update(GameTime gameTime)
        {
            foreach(Effect effect in effectList)
            {
                effect.Update(gameTime);
            }

            for (int i = effectList.Count - 1 ; i >= 0; i--)
            {
                if (effectList[i].remove)
                {
                    effectList.RemoveAt(i);
                    //break;
                }
            }


        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            foreach (Effect effect in effectList)
            {
                effect.Draw(spriteBatch, offset);
            }
            //Console.WriteLine(effectList.Count);
        }

        public void Clear()
        {
            EffectList.Clear();
        }

        public void AddEffect(Game game, Vector2 position, Texture2D texture, float lifetime)
        {
            Color[] colors = new Color[texture.Height * texture.Width];
            texture.GetData<Color>(colors);

            for (int y = 0; y < texture.Height; y+=5)
            {
                for (int x = 0; x < texture.Width; x+=5)
                {
                    /*
                    Color[] color = new Color[1];
                    color[0] = colors[y * texture.Width + x];
                    if (color[0].A != 0)
                    {
                        
                        Texture2D texturePixel = new Texture2D(((PORAGame)game).Graphics.GraphicsDevice, 1, 1);
                        texturePixel.SetData<Color>(color);
                        Vector2 location = position + new Vector2(x - texture.Width/2, y - texture.Height/2);
                        effectList.Add(new Effect(game, location, texturePixel, lifetime));
                        
                    }*/
                    //Vector2 location = position + new Vector2(x - texture.Width / 2, y - texture.Height / 2);
                    Vector2 location = position + new Vector2(x - texture.Width / 2, y - texture.Height / 2);
                    effectList.Add(new Effect(game, location, texture, lifetime));
                }
            }

        }

    }
}
