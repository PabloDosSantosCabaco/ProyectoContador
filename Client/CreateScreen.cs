using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net.Sockets;

namespace Client
{
    class CreateScreen : Screen
    {
        private MainGame Game { get; set; }
        private Boton BtnBack { get; set; }
        private Boton BtnStart { get; set; }
        private Texture2D BtnSelected { get; set; }
        private Texture2D BtnDefault { get; set; }
        private TextBox BtnInput { get; set; }
        private string Intro { get; set; }
        private string ErrorMsg { get; set; }
        private SpriteFont Font { get; set; }
        private SpriteFont ErrorFont { get; set; }
        private bool ClickCreate { get; set; }
        private int ScreenWidth { get; set; }
        private int ScreenHeight { get; set; }
        private bool ErrorOcurrered { get; set; }
        public CreateScreen(MainGame game)
        {
            Game = game;
        }
        public void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin();
            BtnBack.draw(Game);
            Game.SpriteBatch.DrawString(
                Font, 
                Intro, 
                new Vector2(ScreenWidth / 2 - Font.MeasureString(Intro).X / 2, ScreenHeight / 4), 
                Color.Black
            );
            BtnInput.draw(Game);
            BtnStart.draw(Game);
            if (ErrorOcurrered)
            {
                Game.SpriteBatch.DrawString(
                    ErrorFont,
                    ErrorMsg, 
                    new Vector2(ScreenWidth/2-ErrorFont.MeasureString(ErrorMsg).X/2, ScreenHeight * 7/10), 
                    Color.Red
                );

            }
            Game.SpriteBatch.End();
        }

        public void Initialize()
        {
            ScreenWidth = Game.Graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = Game.Graphics.GraphicsDevice.Viewport.Height;
            Intro = "Type your nick:";
            ErrorOcurrered = false;
            ClickCreate = false;
            ErrorMsg = "Type a name with 3 characters or more";
        }

        public void LoadContent()
        {
            BtnBack = new Boton(0,0, Game.Content.Load<Texture2D>("Sprites/btnBack"), ScreenWidth / 12);
            BtnSelected = Game.Content.Load<Texture2D>("Sprites/btnSelected");
            BtnDefault = Game.Content.Load<Texture2D>("Sprites/btnDefault");
            Font = Game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            BtnInput = new TextBox(
                ScreenWidth / 2 - ScreenWidth * 3 / 8,
                ScreenHeight / 2,
                Game.Content.Load<Texture2D>("Sprites/textBoxSelected"),
                Game.Content.Load<Texture2D>("Sprites/textBoxSelected"),
                ScreenWidth * 3 / 4,
                Font,
                true
            );
            ErrorFont = Game.Content.Load<SpriteFont>("Fuentes/Error");
            BtnStart = new Boton(
                ScreenWidth / 2 - ScreenWidth / 3 / 2, 
                ScreenHeight * 3 / 4, 
                BtnDefault, 
                ScreenWidth / 3,
                ErrorFont,
                "Create room"
            );
        }
        
        public Screen Update(GameTime gameTime)
        {
            if (BtnStart.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                BtnStart.Img = BtnSelected;
            }
            else if (BtnStart.Img != BtnDefault)
            {
                BtnStart.Img = BtnDefault;
            }
            return this;
        }
        public string checkName(string newName)
        {
            //Digamos que sirve para los números
            if (newName.Length > 1)
            {
                newName = newName.Substring(newName.Length - 1);
            }
            if (Font.MeasureString(BtnInput.Text + newName).X < ScreenWidth * 3 / 4)
            {
                return BtnInput.Text + newName;
            }
            return BtnInput.Text;
        }

        public Screen Click()
        {
            if (BtnBack.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                Game.effects[MainGame.eSounds.click].Play();
                return new StartScreen(Game);
            }
            if (BtnStart.isHover(Mouse.GetState().X, Mouse.GetState().Y) && !ClickCreate)
            {
                ClickCreate = true;
                return goNext();
            }
            return this;
        }
        public Screen goNext()
        {
            if (BtnInput.Text.Trim().Length < 3)
            {
                ErrorOcurrered = true;
            }
            else
            {
                //Pedir numero sala al servidor
                Game.effects[MainGame.eSounds.click].Play();
                try
                {
                    Server servidor = new Server();
                    return new WaitingRoomScreen(Game, servidor.getSala(BtnInput.Text), BtnInput.Text, servidor, true);
                }
                catch (SocketException) {
                    ErrorOcurrered = true;
                    ErrorMsg = "Can't connect with server";
                }
            }
            ClickCreate = false;
            return this;
        }
        public Screen KeyboardAction(Keys key)
        {
            if (key >= Keys.A && key <= Keys.Z ||
                (key >= Keys.D0 && key <= Keys.D9) ||
                (key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                BtnInput.Text = checkName(key.ToString());
            }
            else if (key == Keys.Space)
            {
                BtnInput.Text = checkName(" ");
            }else if(key == Keys.Enter)
            {
                return goNext();
            }
            if (key == Keys.Back && BtnInput.Text.Length >= 1)
            {
                BtnInput.Text = BtnInput.Text.Remove(BtnInput.Text.Length - 1);
            }
            return this;
        }

        public void onExiting(object sender, EventArgs args)
        {
        
        }
    }
}
