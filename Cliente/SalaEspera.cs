using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    class SalaEspera : Pantalla
    {
        Game1 juego;

        Texture2D btnEmpezar;
        string sala = "Numero de sala:";
        List<string> jugadores = new List<string>();
        SpriteFont fuente;
        public int AnchoPantalla { get; set; }
        public int AltoPantalla { get; set; }
        int numSala;
        public SalaEspera(Game1 juego,int numSala)
        {
            this.juego = juego;
            this.numSala = numSala;
        }
        public void Draw(GameTime gameTime)
        {
            juego.spriteBatch.Begin();
            juego.spriteBatch.DrawString(fuente, sala+" "+numSala, new Vector2(AnchoPantalla / 2 - fuente.MeasureString(sala + " " + numSala).X/2, AltoPantalla / 8), Color.Black);
            int fila = 1;
            for (int i=0; i<jugadores.Count; i++)
            {
                juego.spriteBatch.DrawString(fuente, jugadores[i],
                    new Vector2(
                        (i % 2) == 0 ? (float)AnchoPantalla / 4 - fuente.MeasureString(jugadores[i]).X/2 : (float)AnchoPantalla*3 / 4 - fuente.MeasureString(jugadores[i]).X/2, 
                        (float)AltoPantalla *2/ 5 +AnchoPantalla*fila/15), 
                    Color.Black);
                if ((i+1) % 2 == 0)
                {
                    fila++;
                }
            }
            juego.spriteBatch.End();
        }

        public void Initialize()
        {
            AnchoPantalla = juego.graphics.GraphicsDevice.Viewport.Width;
            AltoPantalla = juego.graphics.GraphicsDevice.Viewport.Height;
        }

        public void LoadContent()
        {
            fuente = juego.Content.Load<SpriteFont>("Fuentes/Intro");
        }

        public Pantalla Update(GameTime gameTime)
        {
            return this;
        }
    }
}
