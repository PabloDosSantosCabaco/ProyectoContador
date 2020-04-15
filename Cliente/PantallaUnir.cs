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
    class PantallaUnir : Pantalla
    {
        Game1 game;
        Boton btnBack;
        Texture2D input;
        Texture2D inputSelected;
        TextBox txtInputRoom;
        TextBox txtInputName;
        TextBox focused;
        Boton btnStart;
        bool errorSala;
        bool errorNombre;
        string errorRoomMsg;
        string errorNameMsg;
        bool mouseClick;
        List<TextBox> inputs = new List<TextBox>();
        Dictionary<Keys, bool> keys = new Dictionary<Keys, bool>();

        string intro = "Introduce el numero de sala:";
        string roomIntro = "Introduce tu nombre:";
        SpriteFont font;
        SpriteFont inputFont;
        SpriteFont errorFont;
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
            if (errorNombre)
            {
                game.spriteBatch.DrawString(errorFont, errorNameMsg, new Vector2(ScreenWidth / 2 - errorFont.MeasureString(errorNameMsg).X/2, ScreenHeight * 15/20), Color.Red);
            }
            if (errorSala)
            {
                game.spriteBatch.DrawString(errorFont, errorRoomMsg, new Vector2(ScreenWidth / 2 - errorFont.MeasureString(errorRoomMsg).X / 2, ScreenHeight * 7/20), Color.Red);
            }
            txtInputName.draw(game);
            btnStart.draw(game);
            game.spriteBatch.End();
        }

        public void Initialize()
        {
            ScreenWidth = game.graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = game.graphics.GraphicsDevice.Viewport.Height;
            mouseClick = false;
            errorSala = false;
            errorNombre = false;
            errorRoomMsg = "Invalid room.";
            errorNameMsg = "This name is too short or is already in use.";
        }

        public void LoadContent()
        {
            btnStart = new Boton(ScreenWidth/2-ScreenWidth/8,ScreenHeight*17/20, game.Content.Load<Texture2D>("Sprites/btnUnir"), ScreenWidth/ 4);
            btnBack = new Boton(0, 0, game.Content.Load<Texture2D>("Sprites/btnBack"), ScreenWidth / 12);
            input = game.Content.Load<Texture2D>("Sprites/textBox");
            inputSelected = game.Content.Load<Texture2D>("Sprites/textBoxSelected");
            font = game.Content.Load<SpriteFont>("Fuentes/Intro");
            inputFont = game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            txtInputRoom = new TextBox(ScreenWidth / 2 - ScreenWidth * 3 / 8, ScreenHeight * 7 / 20 - input.Height, inputSelected, input, ScreenWidth * 3 / 4, inputFont, true);
            focused = txtInputRoom;
            txtInputName = new TextBox(ScreenWidth / 2 - ScreenWidth * 3 / 8, ScreenHeight * 15 / 20 - input.Height, inputSelected, input, ScreenWidth * 3 / 4, inputFont, false);
            inputs.Add(txtInputName);
            inputs.Add(txtInputRoom);
            errorFont = game.Content.Load<SpriteFont>("Fuentes/Error");
            
        }

        public Pantalla Update(GameTime gameTime)
        {
            return this;
        }
        public string compruebaNombre(string nuevo, TextBox input)
        {
            //Digamos que sirve para los números
            if (nuevo.Length > 1)
            {
                nuevo = nuevo.Substring(nuevo.Length - 1);
            }
            if (inputFont.MeasureString(input.Text + nuevo).X < ScreenWidth * 3 / 4)
            {
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
        public Pantalla goNext()
        {
            errorNombre = false;
            errorSala = false;
            if (txtInputName.Text.Length >= 3 && txtInputRoom.Text.Length > 0)
            {
                //Mandar al servidor y comprobar que existe dicha sala pero dentro de ella no dicho nombre
                try
                {
                    Servidor servidor = new Servidor();
                    servidor.enviarDatos("join");
                    servidor.enviarDatos(txtInputRoom.Text);
                    servidor.enviarDatos(txtInputName.Text);
                    switch (servidor.recibirDatos())
                    {
                        case "true":
                            game.efectos[Game1.eSonidos.click].Play();
                            return new SalaEspera(game, Convert.ToInt32(txtInputRoom.Text), txtInputName.Text, servidor, false);
                        case "errorNombre":
                            errorSala = false;
                            errorNombre = true;
                            break;
                        case "errorSala":
                            errorNombre = false;
                            errorSala = true;
                            break;
                    }
                    servidor.closeServer();
                }catch(SocketException ex)
                {
                    errorSala = true;
                    errorRoomMsg = "No se ha podido conectar con el servidor";
                    errorNombre = false;
                }catch (InvalidOperationException ioex){
                    errorSala = true;
                    errorRoomMsg = "No se ha podido conectar con el servidor";
                    errorNombre = false;
                }
            }
            if (txtInputName.Text.Length < 3)
            {
                errorNombre = true;
            }
            if (txtInputRoom.Text.Length <= 0)
            {
                errorSala = true;
            }
            return this;
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
            if (txtInputName.click(Mouse.GetState().X, Mouse.GetState().Y))
            {
                changeFocus(txtInputName);
            }
            if (txtInputRoom.click(Mouse.GetState().X, Mouse.GetState().Y))
            {
                changeFocus(txtInputRoom);
            }
            foreach (TextBox input in inputs)
            {
                if (input.Focus)
                {
                    focused = input;
                }
            }
            return this;
        }

        public Pantalla KeyboardAction(Keys key)
        {
            if ((key >= Keys.A && key <= Keys.Z) ||
                        (key >= Keys.D0 && key <= Keys.D9) ||
                        (key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                if (focused != txtInputRoom || !(key >= Keys.A && key <= Keys.Z))
                {
                    focused.Text = compruebaNombre(key.ToString(), focused);
                }
            }
            else if (key == Keys.Space)
            {
                focused.Text = compruebaNombre(" ", focused);
            }
            else if (key == Keys.Tab)
            {
                if (focused == txtInputName)
                {
                    changeFocus(txtInputRoom);
                }
                else
                {
                    changeFocus(txtInputName);
                }
                foreach (TextBox input in inputs)
                {
                    if (input.Focus)
                    {
                        focused = input;
                    }
                }
            }
            else if (key == Keys.Enter) {
                return goNext();
            }
            if (key == Keys.Back && focused.Text.Length >= 1)
            {
                focused.Text = focused.Text.Remove(focused.Text.Length - 1);
            }
            return this;
        }

        public void onExiting(object sender, EventArgs args)
        {

        }
    }
}
