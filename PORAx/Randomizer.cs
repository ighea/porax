using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PORAx
{
    public class Randomizer
    {
        private static Randomizer instance;

        private Random random;

        public static Randomizer GetInstance()
        {
            if (instance == null)
            {
                instance = new Randomizer();
            }
            return instance;
        }

        private Randomizer()
        {
            random = new Random((int)DateTime.Now.Ticks);
        }

        public Random Random
        {
            get
            {
                return random;
            }
        }

    }
}
