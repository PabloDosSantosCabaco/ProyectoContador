using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Cliente
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        Pantalla p;
        string textoIntroducido = "";
        bool mouseClick;
        Dictionary<Keys, bool> keys;
        public enum eSonidos
        {
            click,
            play,
            overCount,
            changeWay,
            forCards,
            newPlayer,
            playerLeave
        }
        public Dictionary<eSonidos, SoundEffect> efectos;

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
            //Volvemos visible el puntero
            Window.TextInput += IntroducirTexto;
            this.IsMouseVisible = true;
            p = new PantallaInicio(this);
            p.Initialize();
            mouseClick = false;
            keys = new Dictionary<Keys, bool>();
            efectos = new Dictionary<eSonidos, SoundEffect>();
            //Valores marcadores
            base.Initialize();
        }
        private void IntroducirTexto(object sender, TextInputEventArgs args)
        {
            var pressedKey = args.Key;
            var character = args.Character;
            textoIntroducido += character;
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
            efectos.Add(eSonidos.click, this.Content.Load<SoundEffect>("Sounds/selectCard"));
            efectos.Add(eSonidos.play, this.Content.Load<SoundEffect>("Sounds/play"));
            efectos.Add(eSonidos.overCount, this.Content.Load<SoundEffect>("Sounds/overCount"));
            efectos.Add(eSonidos.changeWay, this.Content.Load<SoundEffect>("Sounds/changeWay"));
            efectos.Add(eSonidos.forCards, this.Content.Load<SoundEffect>("Sounds/for"));
            efectos.Add(eSonidos.newPlayer, this.Content.Load<SoundEffect>("Sounds/newPlayer"));
            efectos.Add(eSonidos.playerLeave, this.Content.Load<SoundEffect>("Sounds/playerLeave"));
            p.LoadContent();
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
            bool clickIntervention = false;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                mouseClick = true;
            }
            if (mouseClick && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                mouseClick = false;
                if (this.IsActive) { 
                    Pantalla pAux = p;
                    p = p.Click();
                    if (p != pAux)
                    {
                        p.Initialize();
                        p.LoadContent();
                        clickIntervention = true;
                    }
                }
            }
            //Teclado
            for (int i = 0; i < Keyboard.GetState().GetPressedKeys().Length; i++)
            {
                if (!keys.ContainsKey(Keyboard.GetState().GetPressedKeys()[i]) || !keys[Keyboard.GetState().GetPressedKeys()[i]])
                {
                    if (!clickIntervention)
                    {
                        p.KeyboardAction(Keyboard.GetState().GetPressedKeys()[i]);
                    }
                }
                keys[Keyboard.GetState().GetPressedKeys()[i]] = true;
            }
            foreach (var tecla in keys.Keys.ToList())
            {
                if (!Keyboard.GetState().GetPressedKeys().Contains(tecla) && keys[tecla])
                {
                    keys[tecla] = false;
                }
            }

            if (!clickIntervention)
            {
                Pantalla pAux = p;
                p = p.Update(gameTime);
                if (p != pAux)
                {
                    p.Initialize();
                    p.LoadContent();
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            // TODO: Add your drawing code here
            p.Draw(gameTime);
            base.Draw(gameTime);
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
            Console.WriteLine("Llamando onExiting");
            p.onExiting(sender,args);
            Exit();
            base.OnExiting(sender, args);
        }
    }
}
