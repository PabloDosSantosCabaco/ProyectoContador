using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Cliente
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int maxCartas = 8;
        float offSetX;
        float columna;

        List<Carta> cartas = new List<Carta>();
        List<Texture2D> cartas_img = new List<Texture2D>();
        Texture2D cartaEjemplo;
        Vector2 escala;
        Vector2 posicion;
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
            cartas.Add(new Carta(Carta.eTipo.Numero, 3, true));
            cartas.Add(new Carta(Carta.eTipo.Numero, 4, true));
            cartas.Add(new Carta(Carta.eTipo.Numero, 5, true));
            cartas.Add(new Carta(Carta.eTipo.Numero, 6, true));
            cartas.Add(new Carta(Carta.eTipo.Numero, 7, true));
            cartas.Add(new Carta(Carta.eTipo.Sentido, 0, true));
            cartas.Add(new Carta(Carta.eTipo.Sentido, 0, false));
            cartas.Add(new Carta(Carta.eTipo.Efecto, 0, true));
            //Volvemos visible el puntero
            this.IsMouseVisible = true;
            //Permitimos ver el cursor en la centana
            Window.AllowUserResizing = true;
            altoPantalla = graphics.GraphicsDevice.Viewport.Height;
            anchoPantalla = graphics.GraphicsDevice.Viewport.Width;
            columna = anchoPantalla / maxCartas;
            anchoCarta = columna * 8 / 10;

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
            cartaEjemplo = this.Content.Load<Texture2D>("3");
            escala = new Vector2(anchoCarta / (float)cartaEjemplo.Width, anchoCarta / (float)cartaEjemplo.Width);
            altoCarta = escala.Y * cartaEjemplo.Height;
            string nombre = "";
            foreach(Carta card in cartas)
            {
                switch (card.getTipo())
                {
                    case Carta.eTipo.Numero:
                        nombre = card.getValor().ToString();
                        break;
                    case Carta.eTipo.Sentido:
                        if (card.getSentido())
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
            if(graphics.GraphicsDevice.Viewport.Width!=anchoPantalla || graphics.GraphicsDevice.Viewport.Height != altoPantalla)
            {
                altoPantalla = graphics.GraphicsDevice.Viewport.Height;
                anchoPantalla = graphics.GraphicsDevice.Viewport.Width;
                columna = anchoPantalla / maxCartas;
                anchoCarta = columna * 8 / 10;
                escala = new Vector2(anchoCarta / (float)cartaEjemplo.Width, anchoCarta / (float)cartaEjemplo.Width);
                altoCarta = escala.Y * cartaEjemplo.Height;
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
            posicion = new Vector2(columna/10, altoPantalla/2 + altoCarta/2);
            spriteBatch.Begin();
            //Dibujado de las cartas
            foreach (Texture2D img in cartas_img)
            {
                //Dibujamos la carta
                spriteBatch.Draw(img,position:posicion,scale:escala);
                //Actualizamos la posicion x de las cartas
                posicion.X += anchoCarta+columna-anchoCarta;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
