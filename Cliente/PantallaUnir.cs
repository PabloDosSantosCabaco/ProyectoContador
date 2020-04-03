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
            //Comprobación de introducción de texto
            for (int i = 0; i < Keyboard.GetState().GetPressedKeys().Length; i++)
            {
                if (!keys.ContainsKey(Keyboard.GetState().GetPressedKeys()[i]) || !keys[Keyboard.GetState().GetPressedKeys()[i]])
                {
                    if ((Keyboard.GetState().GetPressedKeys()[i] >= Keys.A && Keyboard.GetState().GetPressedKeys()[i] <= Keys.Z) ||
                        (Keyboard.GetState().GetPressedKeys()[i] >= Keys.D0 && Keyboard.GetState().GetPressedKeys()[i] <= Keys.D9) ||
                        (Keyboard.GetState().GetPressedKeys()[i] >= Keys.NumPad0 && Keyboard.GetState().GetPressedKeys()[i] <= Keys.NumPad9))
                    {
                        if (focused != txtInputRoom || !(Keyboard.GetState().GetPressedKeys()[i] >= Keys.A && Keyboard.GetState().GetPressedKeys()[i] <= Keys.Z))
                        {
                            focused.Text = compruebaNombre(Keyboard.GetState().GetPressedKeys()[i].ToString(), focused);
                        }
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
                if(btnStart.click(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    errorNombre = false;
                    errorSala = false;
                    if(txtInputName.Text.Length>=3 && txtInputRoom.Text.Length > 0)
                    {
                        //Mandar al servidor y comprobar que existe dicha sala pero dentro de ella no dicho nombre
                        Servidor servidor = new Servidor();
                        servidor.enviarDatos("join");
                        servidor.enviarDatos(txtInputRoom.Text);
                        servidor.enviarDatos(txtInputName.Text);
                        switch (servidor.recibirDatos())
                        {
                            case "true":
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
                    }
                    if(txtInputName.Text.Length < 3)
                    {
                        errorNombre = true;
                    }
                    if(txtInputRoom.Text.Length<=0){
                        errorSala = true;
                    }
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
