using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PORAx
{
    public struct tileProperty
    {
        public char symbol;
        public string name;
    }

    public class MapData
    {
        public string background;
        public int backgroundOffset;
        public string music;
        public float endDelay;
        public string dialog;
        public tileProperty[] tileInfo;
        public string[] Tiles;
    }
}
