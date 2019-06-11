using System;
using System.Collections.Generic;
using System.Linq;

namespace PORAx
{
    public struct Dialogue
    {
        public string faceAnimation;
        public string text;
        public float textDisplayTime;
    }

    public class DialogData
    {
        public string background;
        public Dialogue[] dialogs;
    }
}
