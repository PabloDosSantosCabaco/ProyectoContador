using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Client
{
    interface Screen
    {
        void Initialize();
        void LoadContent();
        Screen Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        Screen Click();
        Screen KeyboardAction(Keys key);
        void onExiting(object sender, EventArgs args);
    }
}
