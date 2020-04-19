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
    class PantallaInicio : Pantalla
    {
        public Boton BtnJoin { get; set; }
        public Boton BtnCreate { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public string MsgError;
        public Game1 game;
        SpriteFont font;
        SpriteFont errorFont;
        Texture2D btnSelected;
        Texture2D btnDefault;
        List<Boton> btns = new List<Boton>();
        Boton btnFocused;

        public PantallaInicio(Game1 game):this(game,null)
        {
        }
        public PantallaInicio(Game1 game, string msg)
        {
            this.game = game;
            MsgError = msg;
        }
        public void Initialize()
        {
            ScreenWidth = game.graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = game.graphics.GraphicsDevice.Viewport.Height;
            btnFocused = null;
        }
        public void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            errorFont = game.Content.Load<SpriteFont>("Fuentes/Error");
            btnSelected = game.Content.Load<Texture2D>("Sprites/btnSelected");
            btnDefault = game.Content.Load<Texture2D>("Sprites/btnDefault");
            BtnJoin = new Boton(ScreenWidth / 2 - ScreenWidth / 6, ScreenHeight /2, game.Content.Load<Texture2D>("Sprites/btnDefault"), ScreenWidth / 3,errorFont,"Unirse a sala");
            BtnCreate = new Boton(ScreenWidth / 2 - ScreenWidth / 6, ScreenHeight * 3 / 4, game.Content.Load<Texture2D>("Sprites/btnDefault"), ScreenWidth / 3,errorFont,"Crear sala");
            btns.Add(BtnCreate);
            btns.Add(BtnJoin);
        }
        public Pantalla Update(GameTime gameTime)
        {
            foreach(Boton btn in btns)
            {
                if (btn.isHover(Mouse.GetState().X, Mouse.GetState().Y)){
                    clearButtons();
                    btn.Img = btnSelected;
                    btnFocused = btn;
                }
            }
            return this;
        }
        public void clearButtons()
        {
            foreach(Boton btn in btns)
            {
                btn.Img = btnDefault;
            }
            btnFocused = null;
        }
        public void Draw(GameTime gameTime)
        {
            game.spriteBatch.Begin();
            if (MsgError != null)
            {
                game.spriteBatch.DrawString(
                    errorFont,
                    MsgError,
                    new Vector2(ScreenWidth/2- errorFont.MeasureString(MsgError).X/2,ScreenHeight*2/5),
                    Color.Red);
            }
            game.spriteBatch.DrawString(
                font, 
                "Contador", 
                new Vector2(ScreenWidth / 2 - font.MeasureString("Contador").X / 2, ScreenHeight / 4-font.MeasureString("Contador").Y/2), 
                Color.Black
                );
            BtnJoin.draw(game);
            BtnCreate.draw(game);
            game.spriteBatch.End();
        }
        public Pantalla Click()
        {
            if (BtnJoin.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                game.efectos[Game1.eSonidos.click].Play();
                return new PantallaUnir(game);
            }
            if (BtnCreate.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                game.efectos[Game1.eSonidos.click].Play();
                return new PantallaCrear(game);
            }
            return this;
        }
        public Pantalla KeyboardAction(Keys key)
        {
            if (key == Keys.Tab)
            {
                if (btnFocused != null)
                {
                    changeFocus();
                }
                else
                {
                    btnFocused = btns[0];
                    btns[0].Img = btnSelected;
                }
            }
            else if (key == Keys.Enter)
            {
                if (btnFocused!=null)
                {
                    if (btnFocused == BtnCreate)
                    {
                        return new PantallaCrear(game);
                    }else if (btnFocused == BtnJoin)
                    {
                        return new PantallaUnir(game);
                    }
                }
            }
            return this;
        }
        public void changeFocus()
        {
            foreach (Boton btn in btns)
            {
                if (btn == btnFocused)
                {
                    clearButtons();
                    btn.Img = btnDefault;
                    btnFocused = btns.IndexOf(btn) >= btns.Count-1 ? btns[0] : btns[btns.IndexOf(btn) + 1];
                    btns[btns.IndexOf(btnFocused)].Img = btnSelected;
                    return;
                }

            }
        }
        public void onExiting(object sender, EventArgs args)
        {

        }
    }
}
