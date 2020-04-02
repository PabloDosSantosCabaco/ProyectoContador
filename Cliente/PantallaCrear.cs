using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    class PantallaCrear : Pantalla
    {
        Game1 game;
        Texture2D imgBack;
        Boton btnBack;
        Texture2D imgStart;
        Boton btnStart;
        Texture2D input;
        TextBox btnInput;
        string intro;
        string errorMsg;
        SpriteFont font;
        SpriteFont errorFont;
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        Dictionary<Keys, bool> keys = new Dictionary<Keys, bool>();
        bool error;
        bool mouseClick;
        public PantallaCrear(Game1 game)
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
                new Vector2(ScreenWidth / 2 - font.MeasureString(intro).X / 2, ScreenHeight / 4), 
                Color.Black
            );
            btnInput.draw(game);
            btnStart.draw(game);
            if (error)
            {
                game.spriteBatch.DrawString(
                    errorFont,
                    errorMsg, 
                    new Vector2(ScreenWidth/2-errorFont.MeasureString(errorMsg).X/2, ScreenHeight * 7/10), 
                    Color.Red
                );

            }
            
            game.spriteBatch.End();
        }

        public void Initialize()
        {
            ScreenWidth = game.graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = game.graphics.GraphicsDevice.Viewport.Height;
            intro = "Introduce tu nombre:";
            error = false;
            mouseClick = false;
            errorMsg = "Introduce un nombre de al menos 3 caracteres";
        }

        public void LoadContent()
        {
            imgBack = game.Content.Load<Texture2D>("Sprites/btnBack");
            btnBack = new Boton(0,0, imgBack, ScreenWidth / 12);
            input = game.Content.Load<Texture2D>("Sprites/textBoxSelected");
            Texture2D inputSelected = game.Content.Load<Texture2D>("Sprites/textBoxSelected");
            font = game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            btnInput = new TextBox(ScreenWidth / 2 - ScreenWidth * 3 / 8, ScreenHeight / 2, inputSelected,input, ScreenWidth * 3 / 4,font,true);
            imgStart = game.Content.Load<Texture2D>("Sprites/btnCrear");
            errorFont = game.Content.Load<SpriteFont>("Fuentes/Error");
            btnStart = new Boton(ScreenWidth / 2 - ScreenWidth / 3 / 2, ScreenHeight * 3 / 4, imgStart, ScreenWidth / 3);

        }
        
        public Pantalla Update(GameTime gameTime)
        {
            //Comprobación de introducción de texto
            for (int i = 0; i < Keyboard.GetState().GetPressedKeys().Length; i++)
            {
                if (!keys.ContainsKey(Keyboard.GetState().GetPressedKeys()[i]) || !keys[Keyboard.GetState().GetPressedKeys()[i]])
                {
                    if (Keyboard.GetState().GetPressedKeys()[i] >= Keys.A && Keyboard.GetState().GetPressedKeys()[i] <= Keys.Z)
                    {
                        btnInput.Text = compruebaNombre(Keyboard.GetState().GetPressedKeys()[i].ToString());
                    }
                    else if (Keyboard.GetState().GetPressedKeys()[i] == Keys.Space)
                    {
                        btnInput.Text = compruebaNombre(" ");
                    }
                    if (Keyboard.GetState().GetPressedKeys()[i] == Keys.Back && btnInput.Text.Length >= 1)
                    {
                        btnInput.Text = btnInput.Text.Remove(btnInput.Text.Length - 1);
                    }
                }
                keys[Keyboard.GetState().GetPressedKeys()[i]] = true;
            }
            foreach(var tecla in keys.Keys.ToList())
            {
                if (!Keyboard.GetState().GetPressedKeys().Contains(tecla) && keys[tecla])
                {
                    keys[tecla] = false;
                }
            }
            //Comprobación de uso de ratón
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                mouseClick = true;
            }
            if (mouseClick && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if(btnBack.click(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    return new PantallaInicio(game);
                }
                if (btnStart.click(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    if (btnInput.Text.Trim().Length < 3)
                    {
                        error = true;
                    }
                    else
                    {
                        //Pedir numero sala al servidor
                        Servidor servidor = new Servidor();
                        return new SalaEspera(game,servidor.getSala(btnInput.Text), btnInput.Text, servidor,true);
                    }  
                }
                mouseClick = false;
            }

            return this;
        }
        public string compruebaNombre(string nuevo)
        {
            if (font.MeasureString(btnInput.Text + nuevo).X < ScreenWidth * 3 / 4)
            {
                return btnInput.Text + nuevo;
            }
            return btnInput.Text;
        }
    }
}
