using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PORAx
{
    public struct MenuItem
    {
        public GameState gameState;
        public string[] sprites;
        public float changeRate;
    }


    public class MenuData
    {
        public string backgroundMusic;
        public MenuItem[] items;
    }
}
