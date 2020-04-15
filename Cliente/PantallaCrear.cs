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
            return this;
        }
        public string compruebaNombre(string nuevo)
        {
            //Digamos que sirve para los números
            if (nuevo.Length > 1)
            {
                nuevo = nuevo.Substring(nuevo.Length - 1);
            }
            if (font.MeasureString(btnInput.Text + nuevo).X < ScreenWidth * 3 / 4)
            {
                return btnInput.Text + nuevo;
            }
            return btnInput.Text;
        }

        public Pantalla Click()
        {
            if (btnBack.click(Mouse.GetState().X, Mouse.GetState().Y))
            {
                game.efectos[Game1.eSonidos.click].Play();
                return new PantallaInicio(game);
            }
            if (btnStart.click(Mouse.GetState().X, Mouse.GetState().Y))
            {
                return goNext();
            }
            return this;
        }
        public Pantalla goNext()
        {
            if (btnInput.Text.Trim().Length < 3)
            {
                error = true;
            }
            else
            {
                //Pedir numero sala al servidor
                game.efectos[Game1.eSonidos.click].Play();
                try
                {
                    Servidor servidor = new Servidor();
                    return new SalaEspera(game, servidor.getSala(btnInput.Text), btnInput.Text, servidor, true);
                }
                catch (SocketException ex) {
                    error = true;
                    errorMsg = "No se ha podido conectar con el servidor";
                }
            }
            return this;
        }
        public Pantalla KeyboardAction(Keys key)
        {
            if (key >= Keys.A && key <= Keys.Z ||
                (key >= Keys.D0 && key <= Keys.D9) ||
                (key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                btnInput.Text = compruebaNombre(key.ToString());
            }
            else if (key == Keys.Space)
            {
                btnInput.Text = compruebaNombre(" ");
            }else if(key == Keys.Enter)
            {
                return goNext();
            }
            if (key == Keys.Back && btnInput.Text.Length >= 1)
            {
                btnInput.Text = btnInput.Text.Remove(btnInput.Text.Length - 1);
            }
            return this;
        }

        public void onExiting(object sender, EventArgs args)
        {
        
        }
    }
}
