using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cliente
{
    class SalaEspera : Pantalla
    {
        Game1 juego;
        Servidor servidor;

        Texture2D imgEmpezar;
        Boton btnEmpezar;
        bool host;
        bool salaEsperaActiva;
        string nombre;
        string sala = "Numero de sala:";
        List<string> jugadores = new List<string>();
        SpriteFont fuente;
        public int AnchoPantalla { get; set; }
        public int AltoPantalla { get; set; }
        int numSala;
        
        public SalaEspera(Game1 juego,int numSala,string nombre,Servidor servidor,bool host)
        {
            this.juego = juego;
            this.numSala = numSala;
            this.nombre = nombre;
            this.servidor = servidor;
            this.host = host;
            salaEsperaActiva = true;
            jugadores.Add(nombre);
            Thread hiloRecibirJugadores = new Thread(() => recibirJugadores());
            hiloRecibirJugadores.Start();
        }
        public void recibirJugadores()
        {
            while (salaEsperaActiva)
            {
                int numJug = Convert.ToInt32(servidor.recibirDatos());
                jugadores.Clear();
                for (int i = 0; i < numJug; i++)
                {
                    jugadores.Add(servidor.recibirDatos());
                }
            }
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
            if (host)
            {
                juego.spriteBatch.Draw(
                    btnEmpezar.Imagen,
                    position: new Vector2(btnEmpezar.X, btnEmpezar.Y),
                    scale: btnEmpezar.Escala
                    );
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
            if (host)
            {
                imgEmpezar = juego.Content.Load<Texture2D>("Sprites/btnEmpezar");
                btnEmpezar = new Boton(AnchoPantalla / 2 - AnchoPantalla / 4 / 2, AltoPantalla * 4 / 5, imgEmpezar, AnchoPantalla / 4);
            }
        }

        public Pantalla Update(GameTime gameTime)
        {
            
            return this;
        }
    }
}
