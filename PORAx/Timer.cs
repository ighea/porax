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
    public class Timer
    {

        private float counter = 0.0f;
        private float triggerTime;
        private bool repeat;
        private bool triggered;

        public float TriggerTime
        {
            get
            {
                return this.triggerTime;
            }
        }

        public Timer(float triggerTime, bool repeat)
        {
            this.triggerTime = triggerTime;
            this.repeat = repeat;
            this.triggered = false;
        }

        public bool Triggered
        {
            get
            {
                return this.triggered;
            }
        }

        public void Reset()
        {
            this.counter = 0.0f;
            this.triggered = false;
        }

        public void SetTriggerTime(float triggerTime)
        {
            this.triggerTime = triggerTime;
        }

        public bool GetIsTriggered(float elapsedTimeInSeconds)
        {
            if (this.counter >= this.triggerTime && triggered == false)
            {
                this.triggered = true;
                if (this.repeat)
                {
                    this.Reset();
                }
                return true;
            }
            else
            {
                this.counter += elapsedTimeInSeconds;
            }

            return false;
        }

    }
}
