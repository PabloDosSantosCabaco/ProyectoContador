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

        //Valor y sentido de la mesa
        int valorMesa;
        string sentidoMesa;
        //Vector que define la posición del valor de mesa
        Vector2 posicionValor;
        SpriteFont fuente;
        SpriteFont fuenteValor;

        //Maximo numero de cartas que se muestran a la vez
        int maxCartas = 8;
        //Define el ancho que tendrá cada columna contenedora de una carta
        float columna;
        /******************Información general**********************************/
        PaqueteTurno datos;
        /***********************Información única******************************/
        
        //Determina si el jugador ha acabado
        bool terminar;
        //Colección de cartas del jugador
        //List<Carta> cartas = new List<Carta>();
        //Colección de imagenes de cartas del jugador
        List<Texture2D> cartas_img = new List<Texture2D>();
        //Texture2D que nos permite tener siempre acceso a una carta
        Texture2D cartaEjemplo;
        //Vectores que definen la posicion y reescalado de las cartas del jugador
        Vector2 escalaCartas;
        Vector2 posicionCartas;
        //Definen el ancho y alto de la ventana
        float anchoPantalla;
        float altoPantalla;
        //Definen el ancho y alto de las cartas
        float anchoCarta;
        float altoCarta;
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
            datos = new PaqueteTurno(new List<Carta>(), 0, 1, new Dictionary<string, int>());
            datos.Jugadores.Add("Jugador 1", 5);
            datos.Jugadores.Add("Jugador 2", 3);
            datos.Cartas.Add(new Carta(Carta.eTipo.Numero, 3, true));
            datos.Cartas.Add(new Carta(Carta.eTipo.Numero, 4, true));
            datos.Cartas.Add(new Carta(Carta.eTipo.Numero, 5, true));
            datos.Cartas.Add(new Carta(Carta.eTipo.Numero, 6, true));
            datos.Cartas.Add(new Carta(Carta.eTipo.Numero, 7, true));
            datos.Cartas.Add(new Carta(Carta.eTipo.Sentido, 0, true));
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
            anchoCarta = columna * 8 / 10;
            //Iniciamos los valores de la mesa
            valorMesa = 0;
            sentidoMesa = "+";
            posicionValor = new Vector2(anchoPantalla / 2, altoPantalla / 4);
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

            //Cargamos la fuente del juego
            fuente = this.Content.Load<SpriteFont>("Fuentes/Fuente");
            fuenteValor = this.Content.Load<SpriteFont>("Fuentes/FuenteValor");

            cartaEjemplo = this.Content.Load<Texture2D>("3");
            //Establecemos el reescalado de las cartas
            escalaCartas = new Vector2(anchoCarta / (float)cartaEjemplo.Width, anchoCarta / (float)cartaEjemplo.Width);
            altoCarta = escalaCartas.Y * cartaEjemplo.Height;
            //Para cada carta del jugador, se carga su correspondiente imagen
            foreach(Carta card in datos.Cartas)
            {
                string nombre = "Sprites/";
                switch (card.Tipo)
                {
                    case Carta.eTipo.Numero:
                        nombre += card.Valor.ToString();
                        break;
                    case Carta.eTipo.Sentido:
                        if (card.Sentido)
                        {
                            nombre += "plus";
                        }
                        else
                        {
                            nombre += "minus";
                        }
                        break;
                    case Carta.eTipo.Efecto:
                        nombre += "bucle";
                        break;
                }
                cartas_img.Add(this.Content.Load<Texture2D>(nombre));
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

            //Si la ventana del juego cambia sus dimensiones, se adaptan los tamaños de los objetos
            if (graphics.GraphicsDevice.Viewport.Width!=anchoPantalla || graphics.GraphicsDevice.Viewport.Height != altoPantalla)
            {
                altoPantalla = graphics.GraphicsDevice.Viewport.Height;
                anchoPantalla = graphics.GraphicsDevice.Viewport.Width;
                columna = anchoPantalla / maxCartas;
                anchoCarta = columna * 8 / 10;
                escalaCartas = new Vector2(anchoCarta / (float)cartaEjemplo.Width, anchoCarta / (float)cartaEjemplo.Width);
                altoCarta = escalaCartas.Y * cartaEjemplo.Height;
                posicionValor = new Vector2(anchoPantalla / 2, altoPantalla / 4);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //Estableceemos donde empezarán a dibujarse 
            posicionCartas = new Vector2(columna/10, altoPantalla/2 + altoCarta/2);
            spriteBatch.Begin();
            //Dibujamos el valor y sentido de mesa
            spriteBatch.DrawString(
                fuenteValor, 
                valorMesa.ToString(), 
                posicionValor- fuenteValor.MeasureString(valorMesa.ToString())/2, 
                Color.White
                );
            spriteBatch.DrawString(
                fuenteValor, 
                sentidoMesa.ToString(), 
                posicionValor - fuenteValor.MeasureString(valorMesa.ToString())/2 - fuenteValor.MeasureString(sentidoMesa.ToString()), 
                Color.White);
            //Dibujo marcadores
            Vector2 posicionMarcador = new Vector2(columna / 10, columna / 10);
            foreach (var jugador in datos.Jugadores)
            {
                spriteBatch.DrawString(fuente, jugador.Key + ": " + jugador.Value, posicionMarcador, Color.White);
                posicionMarcador.Y += fuente.MeasureString(jugador.Key + ": " + jugador.Value).Y;
            }
            Vector2 posicionTurno = new Vector2(anchoPantalla - fuente.MeasureString("Turno: " + datos.Turno).X, 0);
            spriteBatch.DrawString(fuente, "Turno: " + datos.Turno, posicionTurno, Color.White);
            //Dibujado de las cartas
            foreach (Texture2D img in cartas_img)
            {
                //Dibujamos la carta
                spriteBatch.Draw(img,position: posicionCartas, scale: escalaCartas);
                //Actualizamos la posicion x de las cartas
                posicionCartas.X += anchoCarta+columna-anchoCarta;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
