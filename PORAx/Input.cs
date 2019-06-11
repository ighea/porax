using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace PORAx
{
    class Input
    {

        private GamePadState gamePadOneState;
        private KeyboardState keyboardState;

        private GamePadState oldGamePadOneState;
        private KeyboardState oldKeyboardState;

        private static Input instance;

        public static Input GetInstance()
        {
            if (instance == null)
            {
                instance = new Input();
            }
            return instance;
        }

        private Input()
        {
            oldGamePadOneState = GamePad.GetState(PlayerIndex.One);
            oldKeyboardState = Keyboard.GetState();
        }

        public void Update()
        {
            oldKeyboardState = keyboardState;
            oldGamePadOneState = gamePadOneState;

            keyboardState = Keyboard.GetState();
            gamePadOneState = GamePad.GetState(PlayerIndex.One);

        }

        public GamePadState GamePadOneState
        {
            get
            {
                return this.gamePadOneState;
            }
        }

        public KeyboardState KeyboardState
        {
            get
            {
                return this.keyboardState;
            }
        }

        public bool IsPressed(Keys key)
        {
            if (keyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyUp(key))
            {
                return true;
            }
            return false;
        }

        public bool IsPressed(Buttons button)
        {
            if (gamePadOneState.IsButtonDown(button) && oldGamePadOneState.IsButtonUp(button))
            {
                return true;
            }
            return false;
        }

    }
}
