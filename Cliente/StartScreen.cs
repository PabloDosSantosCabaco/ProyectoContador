using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Client
{
    class StartScreen : Screen
    {
        private Boton BtnJoin { get; set; }
        private Boton BtnCreate { get; set; }
        private int ScreenWidth { get; set; }
        private int ScreenHeight { get; set; }
        private string MsgError { get; set; }
        private MainGame Game { get; set; }
        private SpriteFont Font { get; set; }
        private SpriteFont ErrorFont { get; set; }
        private Texture2D BtnSelected { get; set; }
        private Texture2D BtnDefault { get; set; }
        private List<Boton> btns = new List<Boton>();
        private Boton BtnFocused { get; set; }

        public StartScreen(MainGame game):this(game,null)
        {
        }
        public StartScreen(MainGame game, string msg)
        {
            Game = game;
            MsgError = msg;
        }
        public void Initialize()
        {
            ScreenWidth = Game.Graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = Game.Graphics.GraphicsDevice.Viewport.Height;
            BtnFocused = null;
        }
        public void LoadContent()
        {
            Font = Game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            ErrorFont = Game.Content.Load<SpriteFont>("Fuentes/Error");
            BtnSelected = Game.Content.Load<Texture2D>("Sprites/btnSelected");
            BtnDefault = Game.Content.Load<Texture2D>("Sprites/btnDefault");
            BtnJoin = new Boton(
                ScreenWidth / 2 - ScreenWidth / 6,
                ScreenHeight / 2,
                Game.Content.Load<Texture2D>("Sprites/btnDefault"),
                ScreenWidth / 3,
                ErrorFont,
                "Join room"
            );
            BtnCreate = new Boton(
                ScreenWidth / 2 - ScreenWidth / 6,
                ScreenHeight * 3 / 4,
                Game.Content.Load<Texture2D>("Sprites/btnDefault"),
                ScreenWidth / 3,
                ErrorFont,
                "Create room"
            );
            btns.Add(BtnCreate);
            btns.Add(BtnJoin);
        }
        public Screen Update(GameTime gameTime)
        {
            foreach(Boton btn in btns)
            {
                if (btn.isHover(Mouse.GetState().X, Mouse.GetState().Y)){
                    clearButtons();
                    btn.Img = BtnSelected;
                    BtnFocused = btn;
                }
            }
            return this;
        }
        public void clearButtons()
        {
            foreach(Boton btn in btns)
            {
                btn.Img = BtnDefault;
            }
            BtnFocused = null;
        }
        public void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin();
            if (MsgError != null)
            {
                Game.SpriteBatch.DrawString(
                    ErrorFont,
                    MsgError,
                    new Vector2(ScreenWidth/2- ErrorFont.MeasureString(MsgError).X/2,ScreenHeight*2/5),
                    Color.Red);
            }
            Game.SpriteBatch.DrawString(
                Font, 
                "Contador", 
                new Vector2(ScreenWidth / 2 - Font.MeasureString("Contador").X / 2, ScreenHeight / 4-Font.MeasureString("Contador").Y/2), 
                Color.Black
                );
            BtnJoin.draw(Game);
            BtnCreate.draw(Game);
            Game.SpriteBatch.End();
        }
        public Screen Click()
        {
            if (BtnJoin.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                Game.effects[MainGame.eSounds.click].Play();
                return new JoinScreen(Game);
            }
            if (BtnCreate.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                Game.effects[MainGame.eSounds.click].Play();
                return new CreateScreen(Game);
            }
            return this;
        }
        public Screen KeyboardAction(Keys key)
        {
            if (key == Keys.Tab)
            {
                if (BtnFocused != null)
                {
                    changeFocus();
                }
                else
                {
                    BtnFocused = btns[0];
                    btns[0].Img = BtnSelected;
                }
            }
            else if (key == Keys.Enter)
            {
                if (BtnFocused!=null)
                {
                    if (BtnFocused == BtnCreate)
                    {
                        return new CreateScreen(Game);
                    }else if (BtnFocused == BtnJoin)
                    {
                        return new JoinScreen(Game);
                    }
                }
            }
            return this;
        }
        public void changeFocus()
        {
            foreach (Boton btn in btns)
            {
                if (btn == BtnFocused)
                {
                    clearButtons();
                    btn.Img = BtnDefault;
                    BtnFocused = btns.IndexOf(btn) >= btns.Count-1 ? btns[0] : btns[btns.IndexOf(btn) + 1];
                    btns[btns.IndexOf(BtnFocused)].Img = BtnSelected;
                    return;
                }

            }
        }
        public void onExiting(object sender, EventArgs args)
        {

        }
    }
}
