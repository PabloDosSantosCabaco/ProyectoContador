﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    interface Pantalla
    {
        void Initialize();
        void LoadContent();
        Pantalla Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        Pantalla Click();
        void KeyboardAction(Keys key);
    }
}
