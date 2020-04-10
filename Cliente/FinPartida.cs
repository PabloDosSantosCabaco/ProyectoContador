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
    class FinPartida : Pantalla
    {
        int rank;
        Game1 game;
        Boton btnBack;
        SpriteFont font;
        bool mouseClick;
        public float ScreenWidth { get; set; }
        public float ScreenHeight { get; set; }
        public FinPartida(Game1 game,int rank)
        {
            this.game = game;
            this.rank = rank;
            mouseClick = false;
        }
        public void Draw(GameTime gameTime)
        {
            game.spriteBatch.Begin();
            game.spriteBatch.DrawString(font, "You are the number " + rank, new Vector2(ScreenWidth / 2 - font.MeasureString("You are the number " + rank).X / 2, ScreenWidth / 5), Color.Black);
            btnBack.draw(game);
            game.spriteBatch.End();
        }

        public void Initialize()
        {
            ScreenWidth = game.GraphicsDevice.Viewport.Width;
            ScreenHeight = game.GraphicsDevice.Viewport.Height;
        }

        public void LoadContent()
        {
            btnBack = new Boton(ScreenWidth / 2 - ScreenWidth / 4/2, ScreenHeight * 3 / 5, game.Content.Load<Texture2D>("Sprites/btnRepeat"),ScreenWidth/4);
            font = game.Content.Load<SpriteFont>("Fuentes/Fuente");
        }

        public Pantalla Update(GameTime gameTime)
        {
            return this;
        }

        public Pantalla Click()
        {
            if (btnBack.click(Mouse.GetState().X, Mouse.GetState().Y))
            {
                game.efectos[Game1.eSonidos.click].Play();
                return new PantallaInicio(game);
            }
            return this;
        }

        public void KeyboardAction(Keys key)
        {

        }

        public void onExiting(object sender, EventArgs args)
        {

        }
    }
}
