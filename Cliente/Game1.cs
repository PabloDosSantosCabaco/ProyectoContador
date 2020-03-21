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

        List<Carta> cartas = new List<Carta>();
        List<Texture2D> cartas_img = new List<Texture2D>();
        Vector2 escala;
        Vector2 posicion;
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
            cartas.Add(new Carta(Carta.eTipo.Numero, 7, true));
            cartas.Add(new Carta(Carta.eTipo.Numero, 5, true));
            this.IsMouseVisible = true;
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
            string nombre = "";
            foreach(Carta card in cartas)
            {
                switch (card.getTipo())
                {
                    case Carta.eTipo.Numero:
                        nombre = card.getValor().ToString();
                        break;
                    case Carta.eTipo.Sentido:
                        break;
                    case Carta.eTipo.Efecto:
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
            posicion = new Vector2(0, graphics.GraphicsDevice.Viewport.Height - this.Content.Load<Texture2D>("3").Height);
            foreach (Texture2D img in cartas_img)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(img, posicion,Color.White);
                spriteBatch.End();
                posicion.X += this.Content.Load<Texture2D>("3").Width;
            }
            base.Draw(gameTime);
        }
    }
}
