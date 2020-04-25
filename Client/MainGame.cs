using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager Graphics { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        Screen Screen { get; set; }
        public bool MouseClick { get; set; }
        Dictionary<Keys, bool> keys;
        public enum eSounds
        {
            click,
            play,
            overCount,
            changeWay,
            forCards,
            newPlayer,
            playerLeave
        }
        public Dictionary<eSounds, SoundEffect> effects;

        public MainGame()
        {
            Graphics = new GraphicsDeviceManager(this);
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
            this.IsMouseVisible = true;
            Screen = new StartScreen(this);
            Screen.Initialize();
            MouseClick = false;
            keys = new Dictionary<Keys, bool>();
            effects = new Dictionary<eSounds, SoundEffect>();
            //Valores marcadores
            base.Initialize();
            this.Window.Title = "Contador";
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            effects.Add(eSounds.click, this.Content.Load<SoundEffect>("Sounds/selectCard"));
            effects.Add(eSounds.play, this.Content.Load<SoundEffect>("Sounds/play"));
            effects.Add(eSounds.overCount, this.Content.Load<SoundEffect>("Sounds/overCount"));
            effects.Add(eSounds.changeWay, this.Content.Load<SoundEffect>("Sounds/changeWay"));
            effects.Add(eSounds.forCards, this.Content.Load<SoundEffect>("Sounds/for"));
            effects.Add(eSounds.newPlayer, this.Content.Load<SoundEffect>("Sounds/newPlayer"));
            effects.Add(eSounds.playerLeave, this.Content.Load<SoundEffect>("Sounds/playerLeave"));
            Screen.LoadContent();
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
                MouseClick = true;
            }
            if (MouseClick && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                MouseClick = false;
                if (this.IsActive) { 
                    Screen pAux = Screen;
                    Screen = Screen.Click();
                    if (Screen != pAux)
                    {
                        Screen.Initialize();
                        Screen.LoadContent();
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
                        if (this.IsActive)
                        {
                            Screen screenAux = Screen;
                            Screen = Screen.KeyboardAction(Keyboard.GetState().GetPressedKeys()[i]);
                            if (Screen != screenAux)
                            {
                                Screen.Initialize();
                                Screen.LoadContent();
                            }
                        }
                        
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
                Screen screenAux = Screen;
                Screen = Screen.Update(gameTime);
                if (Screen != screenAux)
                {
                    Screen.Initialize();
                    Screen.LoadContent();
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
            Screen.Draw(gameTime);
            base.Draw(gameTime);
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
            Console.WriteLine("Llamando onExiting");
            Screen.onExiting(sender,args);
            Exit();
            base.OnExiting(sender, args);
        }
    }
}
