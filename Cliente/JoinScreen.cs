using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Client
{
    class JoinScreen : Screen
    {
        private MainGame Game { get; set; }
        private Boton BtnBack { get; set; }
        private Boton BtnStart { get; set; }
        private Texture2D BtnSelected { get; set; }
        private Texture2D BtnDefault { get; set; }
        private Texture2D InputDefault { get; set; }
        private Texture2D InputSelected { get; set; }
        private TextBox TxtInputRoom { get; set; }
        private TextBox TxtInputName { get; set; }
        private TextBox Focused { get; set; }
        private bool RoomError { get; set; }
        private bool NameError { get; set; }
        private bool ClickJoin { get; set; }
        private string ErrorRoomMsg { get; set; }
        private string ErrorNameMsg { get; set; }
        private List<TextBox> inputs = new List<TextBox>();

        private string RoomMessage { get; set; }
        private string NameMessage { get; set; }
        private SpriteFont DefaultFont { get; set; }
        private SpriteFont InputFont { get; set; }
        private SpriteFont ErrorFont { get; set; }
        private int ScreenWidth { get; set; }
        private int ScreenHeight { get; set; }
        public JoinScreen(MainGame game)
        {
            Game = game;
        }
        public void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin();
            BtnBack.draw(Game);
            Game.SpriteBatch.DrawString(
                DefaultFont, 
                RoomMessage, 
                new Vector2(ScreenWidth / 2 - DefaultFont.MeasureString(RoomMessage).X / 2, ScreenHeight /10), 
                Color.Black
            );
            TxtInputRoom.draw(Game);
            Game.SpriteBatch.DrawString(
                DefaultFont, 
                NameMessage, 
                new Vector2(ScreenWidth / 2 - DefaultFont.MeasureString(NameMessage).X / 2, ScreenHeight/2-DefaultFont.MeasureString(NameMessage).Y), 
                Color.Black
            );
            if (NameError)
            {
                Game.SpriteBatch.DrawString(ErrorFont, ErrorNameMsg, new Vector2(ScreenWidth / 2 - ErrorFont.MeasureString(ErrorNameMsg).X/2, ScreenHeight * 15/20), Color.Red);
            }
            if (RoomError)
            {
                Game.SpriteBatch.DrawString(ErrorFont, ErrorRoomMsg, new Vector2(ScreenWidth / 2 - ErrorFont.MeasureString(ErrorRoomMsg).X / 2, ScreenHeight * 7/20), Color.Red);
            }
            TxtInputName.draw(Game);
            BtnStart.draw(Game);
            Game.SpriteBatch.End();
        }

        public void Initialize()
        {
            ScreenWidth = Game.Graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = Game.Graphics.GraphicsDevice.Viewport.Height;
            RoomError = false;
            NameError = false;
            ClickJoin = false;
            ErrorRoomMsg = "Invalid room.";
            ErrorNameMsg = "This name is too short or is already in use.";
            RoomMessage = "Type room's number:";
            NameMessage = "Type your nick:";
        }

        public void LoadContent()
        {
            ErrorFont = Game.Content.Load<SpriteFont>("Fuentes/Error");
            BtnSelected = Game.Content.Load<Texture2D>("Sprites/btnSelected");
            BtnDefault = Game.Content.Load<Texture2D>("Sprites/btnDefault");
            BtnStart = new Boton(
                ScreenWidth / 2 - ScreenWidth / 8,
                ScreenHeight * 17/20,
                Game.Content.Load<Texture2D>("Sprites/btnDefault"),
                ScreenWidth / 4,
                ErrorFont,
                "Join room"
            );
            BtnBack = new Boton(
                0, 
                0,
                Game.Content.Load<Texture2D>("Sprites/btnBack"),
                ScreenWidth / 12
            );
            InputDefault = Game.Content.Load<Texture2D>("Sprites/textBox");
            InputSelected = Game.Content.Load<Texture2D>("Sprites/textBoxSelected");
            DefaultFont = Game.Content.Load<SpriteFont>("Fuentes/File");
            InputFont = Game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            TxtInputRoom = new TextBox(
                ScreenWidth / 2 - ScreenWidth * 3 / 8, 
                ScreenHeight * 7 / 20 - InputDefault.Height, 
                InputSelected, 
                InputDefault, 
                ScreenWidth * 3 / 4, 
                InputFont, 
                true
            );
            Focused = TxtInputRoom;
            TxtInputName = new TextBox(
                ScreenWidth / 2 - ScreenWidth * 3 / 8, 
                ScreenHeight * 15 / 20 - InputDefault.Height, 
                InputSelected, 
                InputDefault, 
                ScreenWidth * 3 / 4, 
                InputFont, 
                false
            );
            inputs.Add(TxtInputName);
            inputs.Add(TxtInputRoom);
            
        }

        public Screen Update(GameTime gameTime)
        {
            if (BtnStart.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                BtnStart.Img = BtnSelected;
            }
            else if(BtnStart.Img != BtnDefault)
            {
                BtnStart.Img = BtnDefault;
            }
            return this;
        }
        public string checkName(string newName, TextBox input)
        {
            //Digamos que sirve para los números
            if (newName.Length > 1)
            {
                newName = newName.Substring(newName.Length - 1);
            }
            if (InputFont.MeasureString(input.Text + newName).X < ScreenWidth * 3 / 4)
            {
                return input.Text + newName;
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
        public Screen goNext()
        {
            NameError = false;
            RoomError = false;
            if (TxtInputName.Text.Length >= 3 && TxtInputRoom.Text.Length > 0)
            {
                //Mandar al servidor y comprobar que existe dicha sala pero dentro de ella no dicho nombre
                try
                {
                    Server servidor = new Server();
                    servidor.sendData("join");
                    servidor.sendData(TxtInputRoom.Text);
                    servidor.sendData(TxtInputName.Text);
                    switch (servidor.getData())
                    {
                        case "true":
                            Game.effects[MainGame.eSounds.click].Play();
                            return new WaitingRoomScreen(Game, Convert.ToInt32(TxtInputRoom.Text), TxtInputName.Text, servidor, false);
                        case "errorNombre":
                            RoomError = false;
                            NameError = true;
                            break;
                        case "errorSala":
                            NameError = false;
                            RoomError = true;
                            break;
                    }
                    servidor.closeServer();
                }catch(SocketException)
                {
                    RoomError = true;
                    ErrorRoomMsg = "Can't connect with server";
                    NameError = false;
                }catch (InvalidOperationException){
                    RoomError = true;
                    ErrorRoomMsg = "Can't connect with server";
                    NameError = false;
                }
            }
            if (TxtInputName.Text.Length < 3)
            {
                NameError = true;
            }
            if (TxtInputRoom.Text.Length <= 0)
            {
                RoomError = true;
            }
            ClickJoin = false;
            return this;
        }
        public Screen Click()
        {
            if (BtnBack.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                Game.effects[MainGame.eSounds.click].Play();
                return new StartScreen(Game);
            }
            if (BtnStart.isHover(Mouse.GetState().X, Mouse.GetState().Y) && !ClickJoin)
            {
                ClickJoin = true;
                return goNext();
            }
            if (TxtInputName.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                changeFocus(TxtInputName);
            }
            if (TxtInputRoom.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                changeFocus(TxtInputRoom);
            }
            foreach (TextBox input in inputs)
            {
                if (input.Focus)
                {
                    Focused = input;
                }
            }
            return this;
        }

        public Screen KeyboardAction(Keys key)
        {
            if ((key >= Keys.A && key <= Keys.Z) ||
                        (key >= Keys.D0 && key <= Keys.D9) ||
                        (key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                if (Focused != TxtInputRoom || !(key >= Keys.A && key <= Keys.Z))
                {
                    Focused.Text = checkName(key.ToString(), Focused);
                }
            }
            else if (key == Keys.Space)
            {
                Focused.Text = checkName(" ", Focused);
            }
            else if (key == Keys.Tab)
            {
                if (Focused == TxtInputName)
                {
                    changeFocus(TxtInputRoom);
                }
                else
                {
                    changeFocus(TxtInputName);
                }
                foreach (TextBox input in inputs)
                {
                    if (input.Focus)
                    {
                        Focused = input;
                    }
                }
            }
            else if (key == Keys.Enter) {
                return goNext();
            }
            if (key == Keys.Back && Focused.Text.Length >= 1)
            {
                Focused.Text = Focused.Text.Remove(Focused.Text.Length - 1);
            }
            return this;
        }

        public void onExiting(object sender, EventArgs args)
        {

        }
    }
}
