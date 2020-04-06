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
        public Texture2D ImgJoin { get; set; }
        public Texture2D ImgCreate { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public Game1 game;
        bool mouseClick = false;
        SpriteFont font;

        public PantallaInicio(Game1 game)
        {
            this.game = game;
        }

        public void Initialize()
        {
            ScreenWidth = game.graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = game.graphics.GraphicsDevice.Viewport.Height;
        }

        public void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            ImgJoin = game.Content.Load<Texture2D>("Sprites/btnUnir");
            ImgCreate = game.Content.Load<Texture2D>("Sprites/btnCrear");
            BtnJoin = new Boton(ScreenWidth / 2 - ScreenWidth / 6, ScreenHeight /2, ImgJoin, ScreenWidth / 3);
            BtnCreate = new Boton(ScreenWidth / 2 - ScreenWidth / 6, ScreenHeight * 3 / 4, ImgCreate, ScreenWidth / 3);
        }

        public Pantalla Update(GameTime gameTime)
        {
            return this;
        }

        public void Draw(GameTime gameTime)
        {
            game.spriteBatch.Begin();
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
            if (BtnJoin.click(Mouse.GetState().X, Mouse.GetState().Y))
            {
                game.efectos[Game1.eSonidos.click].Play();
                return new PantallaUnir(game);
            }
            if (BtnCreate.click(Mouse.GetState().X, Mouse.GetState().Y))
            {
                game.efectos[Game1.eSonidos.click].Play();
                return new PantallaCrear(game);
            }
            return this;
        }

        public void KeyboardAction(Keys key)
        {

        }
    }
}
