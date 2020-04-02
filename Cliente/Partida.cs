using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cliente
{
    class Partida : Pantalla
    {

        TcpClient client;
        NetworkStream ns;
        StreamWriter sw;
        StreamReader sr;
        Thread hiloConectividad;
        string miNombre;
        bool ratonPresionado;
        bool conectado = false;

        //Imagenes cartas
        Dictionary<string, Texture2D> cartas = new Dictionary<string, Texture2D>();
        //Cartas jugador
        List<Boton> cartasBtn = new List<Boton>();
        //Boton jugar y botón pasar
        Boton jugar;
        Boton pasar;
        Texture2D btnJugarImg;
        Texture2D btnPasarImg;
        int cartaSeleccionada;
        //Definen el ancho y alto de la ventana
        float anchoPantalla;
        float altoPantalla;
        //Define el ancho que tendrá cada columna contenedora de una carta
        float columna;

        //Vector que define la posición del valor de mesa
        SpriteFont fuente;
        SpriteFont fuenteValor;

        //Maximo numero de cartas que se muestran a la vez
        int maxCartas = 8;
        PaqueteTurno datos;

        //Determina si el jugador ha acabado
        bool terminar;
        //Texture2D que nos permite tener siempre acceso a una carta
        Texture2D cartaEjemplo;
        //Vectores que definen la posicion y reescalado de las cartas del jugador
        Vector2 posicionCartas;
        Game1 juego;
        public Partida(Game1 juego)
        {
            this.juego = juego;
        }

        public void Draw(GameTime gameTime)
        {
            juego.spriteBatch.Begin();
            dibujarMesa();
            dibujarMarcadores();
            dibujarTurno();
            dibujarBotones();
            dibujarCartas();
            juego.spriteBatch.End();
        }   

        public void Initialize()
        {
            for (int i = 31416; i < 31420; i++)
            {
                try
                {
                    client = new TcpClient("127.0.0.1", i);
                    conectado = true;
                    ns = client.GetStream();
                    sr = new StreamReader(ns);
                    sw = new StreamWriter(ns);
                    sw.WriteLine("join");
                    sw.Flush();
                    sw.WriteLine("0");
                    sw.Flush();
                    miNombre = "Kenny";
                    sw.WriteLine(miNombre);
                    sw.Flush();
                    break;
                }
                catch (SocketException ex)
                {

                }
            }
            if (conectado)
            {
                cartaSeleccionada = -1;
                actualizarDatos();
                hiloConectividad = new Thread(() => comprobarTurno());
                hiloConectividad.Start();
                //Al empezar el jugador está jugando
                terminar = false;
                
                //Permitimos ver el cursor en la centana
                //Window.AllowUserResizing = true;
                altoPantalla = juego.graphics.GraphicsDevice.Viewport.Height;
                anchoPantalla = juego.graphics.GraphicsDevice.Viewport.Width;
                columna = anchoPantalla / maxCartas;
                ratonPresionado = false;
            }
            
        }

        public void LoadContent()
        {
            if (conectado)
            {
                for (int i = 3; i < 8; i++)
                {
                    cartas.Add(i.ToString(), juego.Content.Load<Texture2D>("Sprites/" + i));
                }
                cartas.Add("bucle", juego.Content.Load<Texture2D>("Sprites/bucle"));
                cartas.Add("minus", juego.Content.Load<Texture2D>("Sprites/minus"));
                cartas.Add("plus", juego.Content.Load<Texture2D>("Sprites/plus"));
                //Cargamos la fuente del juego
                fuente = juego.Content.Load<SpriteFont>("Fuentes/Fuente");
                fuenteValor = juego.Content.Load<SpriteFont>("Fuentes/FuenteValor");
                cartaEjemplo = cartas["3"];

                btnJugarImg = juego.Content.Load<Texture2D>("Sprites/btnJugar");
                btnPasarImg = juego.Content.Load<Texture2D>("Sprites/btnPasar");
                actualizarBaraja();
                jugar = new Boton(anchoPantalla / 2 - anchoPantalla / 8, altoPantalla / 2, btnJugarImg, anchoPantalla / 8);
                pasar = new Boton(anchoPantalla / 2, altoPantalla / 2, btnPasarImg, anchoPantalla / 8);
            }
        }

        public Pantalla Update(GameTime gameTime)
        {
            if (conectado)
            {
                //Si la ventana del juego cambia sus dimensiones, se adaptan los tamaños de los objetos
                if (juego.graphics.GraphicsDevice.Viewport.Width != anchoPantalla || juego.graphics.GraphicsDevice.Viewport.Height != altoPantalla)
                {
                    altoPantalla = juego.graphics.GraphicsDevice.Viewport.Height;
                    anchoPantalla = juego.graphics.GraphicsDevice.Viewport.Width;
                    columna = anchoPantalla / maxCartas;
                }
                //Para cada carta del jugador, se carga su correspondiente imagen
                actualizarBaraja();
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    ratonPresionado = true;
                }
                if (ratonPresionado && Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    if (datos.Turno == miNombre)
                    {
                        if (pasar.click(Mouse.GetState().X, Mouse.GetState().Y))
                        {
                            sw.WriteLine("pasar");
                            sw.Flush();
                        }
                        if (jugar.click(Mouse.GetState().X, Mouse.GetState().Y) && cartaSeleccionada != -1)
                        {
                            sw.WriteLine("jugar");
                            sw.WriteLine(datos.Cartas[cartaSeleccionada].Tipo.ToString());
                            sw.WriteLine(datos.Cartas[cartaSeleccionada].Valor.ToString());
                            sw.WriteLine(datos.Cartas[cartaSeleccionada].Sentido.ToString());
                            sw.Flush();
                            cartaSeleccionada = -1;
                        }
                    }
                    foreach (Boton btn in cartasBtn)
                    {
                        if (btn.click(Mouse.GetState().X, Mouse.GetState().Y))
                        {
                            cartaSeleccionada = cartasBtn.IndexOf(btn);
                        }
                    }
                    ratonPresionado = false;
                }
            }
            return this;
        }
        public void comprobarTurno()
        {
            while (conectado)
            {
                actualizarDatos();
            }
        }
        public void actualizarDatos()
        {
            string aux = sr.ReadLine();
            Console.WriteLine(aux);
            int numCartas = Convert.ToInt32(aux);
            List<Carta> auxLista = new List<Carta>();
            for (int i = 0; i < numCartas; i++)
            {
                auxLista.Add(new Carta((Carta.eTipo)Enum.Parse(typeof(Carta.eTipo), sr.ReadLine()), Convert.ToInt32(sr.ReadLine()), Convert.ToBoolean(sr.ReadLine())));
            }
            int auxValor = Convert.ToInt32(sr.ReadLine());
            bool auxSentido = Convert.ToBoolean(sr.ReadLine());
            string auxTurno = sr.ReadLine();
            Dictionary<string, int> auxDic = new Dictionary<string, int>();
            int numJugadores = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i < numJugadores; i++)
            {
                auxDic.Add(sr.ReadLine(), Convert.ToInt32(sr.ReadLine()));
            }
            datos = new PaqueteTurno(auxLista, auxValor, auxSentido, auxTurno, auxDic);
        }
        public void actualizarBaraja()
        {
            cartasBtn.Clear();
            foreach (Carta card in datos.Cartas)
            {
                string nombre = "";
                switch (card.Tipo)
                {
                    case Carta.eTipo.Numero:
                        nombre = card.Valor.ToString();
                        break;
                    case Carta.eTipo.Sentido:
                        if (card.Sentido)
                        {
                            nombre = "plus";
                        }
                        else
                        {
                            nombre = "minus";
                        }
                        break;
                    case Carta.eTipo.Efecto:
                        nombre = "bucle";
                        break;
                }
                cartasBtn.Add(new Boton(0, 0, cartas[nombre], columna * 8 / 10));
                posicionCartas = new Vector2(columna / 10, altoPantalla / 2 + columna * 8 / 10 / 2);
                foreach (Boton btn in cartasBtn)
                {
                    btn.X = posicionCartas.X;
                    btn.Y = posicionCartas.Y;
                    posicionCartas.X += columna;
                }
            }
        }
        public void dibujarBotones()
        {
            juego.spriteBatch.Draw(jugar.Img, position: new Vector2(anchoPantalla / 2 - jugar.Width, altoPantalla / 2), scale: jugar.Scale);
            juego.spriteBatch.Draw(pasar.Img, position: new Vector2(anchoPantalla / 2, altoPantalla / 2), scale: pasar.Scale);
        }
        public void dibujarCartas()
        {
            //Estableceemos donde empezarán a dibujarse 
            posicionCartas = new Vector2(columna / 10, altoPantalla / 2 + columna * 8 / 10 / 2);
            //Dibujado de las cartas
            foreach (Boton btn in cartasBtn)
            {
                //Dibujamos la carta
                juego.spriteBatch.Draw(btn.Img, position: new Vector2(btn.X, btn.Y), scale: btn.Scale);
            }
        }
        public void dibujarMesa()
        {
            //Dibujamos el valor y sentido de mesa
            juego.spriteBatch.DrawString(
                fuenteValor,
                datos.ValorMesa.ToString(),
                new Vector2(anchoPantalla / 2, altoPantalla / 4) - fuenteValor.MeasureString(datos.ValorMesa.ToString()) / 2,
                Color.White
                );
            juego.spriteBatch.DrawString(
                fuenteValor,
                datos.Sentido ? "+" : "-",
                new Vector2(anchoPantalla / 2, altoPantalla / 4) - fuenteValor.MeasureString(datos.ValorMesa.ToString()) / 2 - fuenteValor.MeasureString(datos.Sentido.ToString()),
                Color.White);
        }
        public void dibujarTurno()
        {
            Vector2 posicionTurno = new Vector2(anchoPantalla - fuente.MeasureString("Turno: " + datos.Turno).X, 0);
            juego.spriteBatch.DrawString(fuente, "Turno: " + datos.Turno, posicionTurno, Color.White);
        }
        public void dibujarMarcadores()
        {
            Vector2 posicionMarcador = new Vector2(columna / 10, columna / 10);
            foreach (var jugador in datos.Jugadores)
            {
                juego.spriteBatch.DrawString(fuente, jugador.Key + ": " + jugador.Value, posicionMarcador, Color.White);
                posicionMarcador.Y += fuente.MeasureString(jugador.Key + ": " + jugador.Value).Y;
            }
        }
    }
}
