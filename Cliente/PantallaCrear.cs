using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    class PantallaCrear : Pantalla
    {
        Game1 juego;
        Texture2D input;
        string intro = "Introduce tu nombre:";
        SpriteFont fuente;
        public int AnchoPantalla { get; set; }
        public int AltoPantalla { get; set; }
        public PantallaCrear(Game1 juego)
        {
            this.juego = juego;
        }
        public void Draw(GameTime gameTime)
        {
            juego.spriteBatch.Begin();
            juego.spriteBatch.DrawString(fuente, intro, new Vector2(AnchoPantalla / 2 - fuente.MeasureString(intro).X/2, AltoPantalla / 3), Color.Black);
            juego.spriteBatch.Draw(
                input, 
                position: new Vector2(AnchoPantalla / 2 - AnchoPantalla*3 / 8, AltoPantalla * 2 / 3),
                scale: new Vector2((float)AnchoPantalla * 3 / 4 / input.Width, (float)AnchoPantalla * 3 / 4 / input.Width)
                );
            juego.spriteBatch.End();
        }

        public void Initialize()
        {
            AnchoPantalla = juego.graphics.GraphicsDevice.Viewport.Width;
            AltoPantalla = juego.graphics.GraphicsDevice.Viewport.Height;
        }

        public void LoadContent()
        {
            input = juego.Content.Load<Texture2D>("Sprites/textBox");
            fuente = juego.Content.Load<SpriteFont>("Fuentes/FuenteValor");
        }

        public Pantalla Update(GameTime gameTime)
        {
            return new SalaEspera(juego,0);
        }
    }
}
