using System;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;



namespace PORAx
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

            using (PORAGame game = new PORAGame())
            {
                game.Run();
            }
        }

    }
}

