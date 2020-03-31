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
        public Boton btnUnir { get; set; }
        public Boton btnCrear { get; set; }
        public Texture2D ImgUnir { get; set; }
        public Texture2D ImgCrear { get; set; }
        public int AnchoPantalla { get; set; }
        public int AltoPantalla { get; set; }
        public Game1 juego;
        bool ratonPresionado = false;
        SpriteFont fuenteValor;

        public PantallaInicio(Game1 juego)
        {
            this.juego = juego;
        }

        public void Initialize()
        {
            AnchoPantalla = juego.graphics.GraphicsDevice.Viewport.Width;
            AltoPantalla = juego.graphics.GraphicsDevice.Viewport.Height;
        }

        public void LoadContent()
        {
            fuenteValor = juego.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            ImgUnir = juego.Content.Load<Texture2D>("Sprites/btnUnir");
            ImgCrear = juego.Content.Load<Texture2D>("Sprites/btnCrear");
            btnUnir = new Boton(AnchoPantalla / 2 - AnchoPantalla / 6, AltoPantalla /2, ImgUnir, AnchoPantalla / 3);
            btnCrear = new Boton(AnchoPantalla / 2 - AnchoPantalla / 6, AltoPantalla * 3 / 4, ImgCrear, AnchoPantalla / 3);
        }

        public Pantalla Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                ratonPresionado = true;
            }
            if (ratonPresionado && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (btnUnir.click(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    return new PantallaUnir(juego);
                }
                if (btnCrear.click(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    return new PantallaCrear(juego);
                }
                ratonPresionado = false;
            }
            return this;
        }

        public void Draw(GameTime gameTime)
        {
            juego.spriteBatch.Begin();
            juego.spriteBatch.DrawString(fuenteValor, "Contador", new Vector2(AnchoPantalla / 2 - fuenteValor.MeasureString("Contador").X / 2, AltoPantalla / 4-fuenteValor.MeasureString("Contador").Y/2), Color.Black);
            juego.spriteBatch.Draw(btnUnir.Imagen, position: new Vector2(btnUnir.X, btnUnir.Y), scale: btnUnir.Escala);
            juego.spriteBatch.Draw(btnCrear.Imagen, position: new Vector2(btnCrear.X, btnCrear.Y), scale: btnCrear.Escala);
            juego.spriteBatch.End();
        }
    }
}
