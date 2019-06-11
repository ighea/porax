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
    public class Camera
    {
        private GraphicsDeviceManager graphics;

        public Camera(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }


        public Vector2 GetUpdatedOffset(Vector2 target, Rectangle area)
        {
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            /*viewport.X = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth / 10;
            viewport.Y = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight / 10;
            viewport.Width = (int)(graphics.GraphicsDevice.PresentationParameters.BackBufferWidth * 0.80f);
            viewport.Height = (int)(graphics.GraphicsDevice.PresentationParameters.BackBufferHeight * 0.80f);
            graphics.GraphicsDevice.Viewport = viewport;
            */
            Vector2 offset = new Vector2();
            if (target.X < area.Width - graphics.GraphicsDevice.Viewport.Width / 2 && target.X > graphics.GraphicsDevice.Viewport.Width / 2)
            {
                offset = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2) - target;
            }
            else if (target.X >= area.Width - graphics.GraphicsDevice.Viewport.Width / 2)
            {
                offset = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2) - new Vector2(area.Width - graphics.GraphicsDevice.Viewport.Width / 2, target.Y);
            }
            else if (target.X < graphics.GraphicsDevice.Viewport.Width / 2)
            {
                offset = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2) - new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, target.Y);
            }

            if (target.Y >= area.Height - graphics.GraphicsDevice.Viewport.Height / 2)
            {
                offset = new Vector2(offset.X, graphics.GraphicsDevice.Viewport.Height / 2) - new Vector2(0, area.Height - graphics.GraphicsDevice.Viewport.Height / 2);
            }
            else if (target.Y < graphics.GraphicsDevice.Viewport.Height / 2)
            {
                offset = new Vector2(offset.X, graphics.GraphicsDevice.Viewport.Height / 2) - new Vector2(0, graphics.GraphicsDevice.Viewport.Height / 2);
            }

            return offset;
        }

    }
}
