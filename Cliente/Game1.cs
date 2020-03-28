using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Cliente
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TcpClient client;
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
        Vector2 escalaBotones;
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
        Vector2 escalaCartas;
        Vector2 posicionCartas;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            /*for (int i = 31416; i < 31420; i++)
            {
                try
                {
                    client = new TcpClient("127.0.0.1", 31416);
                    conectado = true;
                    break;
                }
                catch (SocketException ex)
                {

                }
            }*/
            conectado = true;
            cartaSeleccionada = -1;
            datos = new PaqueteTurno(new List<Carta>(), 0,true, "1", new Dictionary<string, int>());
            datos.Jugadores.Add("Jugador 1", 5);
            datos.Jugadores.Add("Jugador 2", 3);
            datos.Cartas.Add(new Carta(Carta.eTipo.Numero, 3, true));
            datos.Cartas.Add(new Carta(Carta.eTipo.Numero, 4, true));
            datos.Cartas.Add(new Carta(Carta.eTipo.Numero, 5, true));
            //datos.Cartas.Add(new Carta(Carta.eTipo.Numero, 6, true));
            datos.Cartas.Add(new Carta(Carta.eTipo.Numero, 7, true));
            //datos.Cartas.Add(new Carta(Carta.eTipo.Sentido, 0, true));
            datos.Cartas.Add(new Carta(Carta.eTipo.Sentido, 0, false));
            datos.Cartas.Add(new Carta(Carta.eTipo.Efecto, 0, true));
            

            //Al empezar el jugador está jugando
            terminar = false;
            //Volvemos visible el puntero
            this.IsMouseVisible = true;
            //Permitimos ver el cursor en la centana
            //Window.AllowUserResizing = true;
            altoPantalla = graphics.GraphicsDevice.Viewport.Height;
            anchoPantalla = graphics.GraphicsDevice.Viewport.Width;
            columna = anchoPantalla / maxCartas;
            //Valores marcadores
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            if (conectado)
            {
                for(int i=3; i<8; i++)
                {
                    cartas.Add(i.ToString(), this.Content.Load<Texture2D>("Sprites/"+i));
                }
                cartas.Add("bucle", this.Content.Load<Texture2D>("Sprites/bucle"));
                cartas.Add("minus", this.Content.Load<Texture2D>("Sprites/minus"));
                cartas.Add("plus", this.Content.Load<Texture2D>("Sprites/plus"));
                //Cargamos la fuente del juego
                fuente = this.Content.Load<SpriteFont>("Fuentes/Fuente");
                fuenteValor = this.Content.Load<SpriteFont>("Fuentes/FuenteValor");
                cartaEjemplo = cartas["3"];
                //Establecemos el reescalado de las cartas
                escalaCartas = new Vector2(columna*8/10 / (float)cartaEjemplo.Width, columna * 8 / 10 / (float)cartaEjemplo.Width);

                btnJugarImg = this.Content.Load<Texture2D>("Sprites/btnJugar");
                btnPasarImg = this.Content.Load<Texture2D>("Sprites/btnPasar");
                escalaBotones = new Vector2(anchoPantalla / 8 / (float)btnJugarImg.Width, anchoPantalla / 8 / (float)btnJugarImg.Width);
                actualizarBaraja();
                jugar = new Boton(0, 0, btnJugarImg, anchoPantalla / 8, escalaBotones.Y * btnJugarImg.Height);
                pasar = new Boton(0, 0, btnPasarImg, anchoPantalla / 8, escalaBotones.Y * btnPasarImg.Height);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (conectado)
            {
                //Si la ventana del juego cambia sus dimensiones, se adaptan los tamaños de los objetos
                if (graphics.GraphicsDevice.Viewport.Width != anchoPantalla || graphics.GraphicsDevice.Viewport.Height != altoPantalla)
                {
                    altoPantalla = graphics.GraphicsDevice.Viewport.Height;
                    anchoPantalla = graphics.GraphicsDevice.Viewport.Width;
                    columna = anchoPantalla / maxCartas;
                    //Establecemos el reescalado de las cartas
                    escalaCartas = new Vector2(columna * 8 / 10 / (float)cartaEjemplo.Width, columna * 8 / 10 / (float)cartaEjemplo.Width);
                }
                //Para cada carta del jugador, se carga su correspondiente imagen
                
                foreach (Boton btn in cartasBtn)
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        if (btn.click(Mouse.GetState().X, Mouse.GetState().Y))
                        {
                            cartaSeleccionada = cartasBtn.IndexOf(btn);
                            btn.Imagen = this.Content.Load<Texture2D>("Sprites/3");
                        }
                        //Si el jugador le da a jugar y ha preseleccionado carta, la juega
                        if(jugar.click(Mouse.GetState().X, Mouse.GetState().Y) && cartaSeleccionada!=-1)
                        {

                        }
                        //Si el jugador le da a pasar, no juega carta y suma 1 a su baraja
                        if(pasar.click(Mouse.GetState().X, Mouse.GetState().Y) && cartaSeleccionada != -1)
                        {

                        }
                    }
                }
                
            }
            base.Update(gameTime);
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
                cartasBtn.Add(new Boton(0, 0, cartas[nombre], columna * 8 / 10, escalaCartas.Y * cartaEjemplo.Height));
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            if (conectado)
            {
                spriteBatch.Begin();
                dibujarMesa();
                dibujarMarcadores();
                dibujarTurno();
                dibujarBotones();
                dibujarCartas();
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }
        public void dibujarBotones()
        {
            spriteBatch.Draw(jugar.Imagen, position: new Vector2(anchoPantalla / 2 - jugar.Ancho, altoPantalla / 2), scale: escalaBotones);
            spriteBatch.Draw(pasar.Imagen, position: new Vector2(anchoPantalla / 2, altoPantalla / 2), scale: escalaBotones);
        }
        public void dibujarCartas()
        {
            //Estableceemos donde empezarán a dibujarse 
            posicionCartas = new Vector2(columna / 10, altoPantalla / 2 + columna*8/10 / 2);
            //Dibujado de las cartas
            foreach (Boton btn in cartasBtn)
            {
                //Dibujamos la carta
                spriteBatch.Draw(btn.Imagen, position: posicionCartas, scale: escalaCartas);
                btn.X = posicionCartas.X;
                btn.Y = posicionCartas.Y;
                //Actualizamos la posicion x de las cartas
                posicionCartas.X += columna;
            }
        }
        public void dibujarMesa()
        {
            //Dibujamos el valor y sentido de mesa
            spriteBatch.DrawString(
                fuenteValor,
                datos.ValorMesa.ToString(),
                new Vector2(anchoPantalla / 2, altoPantalla / 4) - fuenteValor.MeasureString(datos.ValorMesa.ToString()) / 2,
                Color.White
                );
            spriteBatch.DrawString(
                fuenteValor,
                datos.Sentido?"+":"-",
                new Vector2(anchoPantalla / 2, altoPantalla / 4) - fuenteValor.MeasureString(datos.ValorMesa.ToString()) / 2 - fuenteValor.MeasureString(datos.Sentido.ToString()),
                Color.White);
        }
        public void dibujarTurno()
        {
            Vector2 posicionTurno = new Vector2(anchoPantalla - fuente.MeasureString("Turno: " + datos.Turno).X, 0);
            spriteBatch.DrawString(fuente, "Turno: " + datos.Turno, posicionTurno, Color.White);
        }
        public void dibujarMarcadores()
        {
            Vector2 posicionMarcador = new Vector2(columna / 10, columna / 10);
            foreach (var jugador in datos.Jugadores)
            {
                spriteBatch.DrawString(fuente, jugador.Key + ": " + jugador.Value, posicionMarcador, Color.White);
                posicionMarcador.Y += fuente.MeasureString(jugador.Key + ": " + jugador.Value).Y;
            }
        }
    }
}
