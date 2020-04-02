using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    class PantallaUnir : Pantalla
    {
        Game1 game;
        Texture2D imgBack;
        Boton btnBack;
        Texture2D input;
        Texture2D inputSelected;
        TextBox txtInputRoom;
        TextBox txtInputName;
        TextBox focused;
        bool mouseClick;
        List<TextBox> inputs = new List<TextBox>();
        Dictionary<Keys, bool> keys = new Dictionary<Keys, bool>();

        string intro = "Introduce tu nombre:";
        string roomIntro = "Introduce el numero de sala:";
        SpriteFont font;
        SpriteFont inputFont;
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public PantallaUnir(Game1 game)
        {
            this.game = game;
        }
        public void Draw(GameTime gameTime)
        {
            game.spriteBatch.Begin();
            btnBack.draw(game);
            game.spriteBatch.DrawString(
                font, 
                intro, 
                new Vector2(ScreenWidth / 2 - font.MeasureString(intro).X / 2, ScreenHeight /10), 
                Color.Black
            );
            txtInputRoom.draw(game);
            game.spriteBatch.DrawString(
                font, 
                roomIntro, 
                new Vector2(ScreenWidth / 2 - font.MeasureString(roomIntro).X / 2, ScreenHeight/2-font.MeasureString(roomIntro).Y), 
                Color.Black
            );
            txtInputName.draw(game);
            game.spriteBatch.End();
        }

        public void Initialize()
        {
            ScreenWidth = game.graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = game.graphics.GraphicsDevice.Viewport.Height;
            mouseClick = false;
        }

        public void LoadContent()
        {
            imgBack = game.Content.Load<Texture2D>("Sprites/btnBack");
            btnBack = new Boton(0, 0, imgBack, ScreenWidth / 12);
            input = game.Content.Load<Texture2D>("Sprites/textBox");
            inputSelected = game.Content.Load<Texture2D>("Sprites/textBoxSelected");
            font = game.Content.Load<SpriteFont>("Fuentes/Intro");
            inputFont = game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            txtInputRoom = new TextBox(ScreenWidth / 2 - ScreenWidth * 3 / 8, ScreenHeight * 4 / 10 - input.Height, inputSelected, input, ScreenWidth * 3 / 4, inputFont, true);
            focused = txtInputRoom;
            txtInputName = new TextBox(ScreenWidth / 2 - ScreenWidth * 3 / 8, ScreenHeight * 8 / 10 - input.Height, inputSelected, input, ScreenWidth * 3 / 4, inputFont, false);
            inputs.Add(txtInputName);
            inputs.Add(txtInputRoom);
        }

        public Pantalla Update(GameTime gameTime)
        {
            //Comprobación de introducción de texto
            for (int i = 0; i < Keyboard.GetState().GetPressedKeys().Length; i++)
            {
                if (!keys.ContainsKey(Keyboard.GetState().GetPressedKeys()[i]) || !keys[Keyboard.GetState().GetPressedKeys()[i]])
                {
                    if ((Keyboard.GetState().GetPressedKeys()[i] >= Keys.A && Keyboard.GetState().GetPressedKeys()[i] <= Keys.Z) ||
                        (Keyboard.GetState().GetPressedKeys()[i] >= Keys.D0 && Keyboard.GetState().GetPressedKeys()[i] <= Keys.D9) ||
                        (Keyboard.GetState().GetPressedKeys()[i] >= Keys.NumPad0 && Keyboard.GetState().GetPressedKeys()[i] <= Keys.NumPad9))
                    {
                        focused.Text = compruebaNombre(Keyboard.GetState().GetPressedKeys()[i].ToString(),focused);
                    }
                    else if (Keyboard.GetState().GetPressedKeys()[i] == Keys.Space)
                    {
                        focused.Text = compruebaNombre(" ",focused);
                    }
                    if (Keyboard.GetState().GetPressedKeys()[i] == Keys.Back && focused.Text.Length >= 1)
                    {
                        focused.Text = focused.Text.Remove(focused.Text.Length - 1);
                    }
                }
                keys[Keyboard.GetState().GetPressedKeys()[i]] = true;
            }
            foreach (var tecla in keys.Keys.ToList())
            {
                if (!Keyboard.GetState().GetPressedKeys().Contains(tecla) && keys[tecla])
                {
                    keys[tecla] = false;
                }
            }
            //Control ratón
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                mouseClick = true;
            }
            if (mouseClick && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (btnBack.click(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    return new PantallaInicio(game);
                }
                if (txtInputName.click(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    changeFocus(txtInputName);
                }
                if (txtInputRoom.click(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    changeFocus(txtInputRoom);
                }
                foreach(TextBox input in inputs)
                {
                    if (input.Focus)
                    {
                        focused = input;
                    }
                }
                mouseClick = false;
            }
            return this;
        }
        public string compruebaNombre(string nuevo, TextBox input)
        {
            if (inputFont.MeasureString(input.Text + nuevo).X < ScreenWidth * 3 / 4)
            {
                //Digamos que sirve para los números
                if (nuevo.Length > 1)
                {
                    nuevo = nuevo.Substring(nuevo.Length - 1);
                }
                return input.Text + nuevo;
            }
            return input.Text;
        }
        public void changeFocus(TextBox newFocus)
        {
            foreach(TextBox input in inputs)
            {
                input.Focus = input == newFocus ? true : false;
            }
        }
    }
}
