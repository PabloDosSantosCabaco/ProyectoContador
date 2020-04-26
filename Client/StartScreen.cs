using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Client
{
    class StartScreen : Screen
    {
        /// <summary>
        /// Boton que permite unirse a la sala.
        /// </summary>
        private Boton BtnJoin { get; set; }
        /// <summary>
        /// Boton que permite crear una sala.
        /// </summary>
        private Boton BtnCreate { get; set; }
        /// <summary>
        /// Indica el ancho de la pantalla.
        /// </summary>
        private int ScreenWidth { get; set; }
        /// <summary>
        /// Indica el alto de la pantalla.
        /// </summary>
        private int ScreenHeight { get; set; }
        /// <summary>
        /// Mensaje de error.
        /// </summary>
        private string MsgError { get; set; }
        /// <summary>
        /// Base de la aplicación.
        /// </summary>
        private MainGame Game { get; set; }
        /// <summary>
        /// Fuente por defecto.
        /// </summary>
        private SpriteFont Font { get; set; }
        /// <summary>
        /// Fuente usada para indicar errores.
        /// </summary>
        private SpriteFont ErrorFont { get; set; }
        /// <summary>
        /// Imagen que indica que el Boton está seleccionado.
        /// </summary>
        private Texture2D BtnSelected { get; set; }
        /// <summary>
        /// Imagen de Boton por defecto.
        /// </summary>
        private Texture2D BtnDefault { get; set; }
        /// <summary>
        /// Colección de botones de la pantalla inicial.
        /// </summary>
        private List<Boton> btns = new List<Boton>();
        /// <summary>
        /// Boton que indica qué Boton tiene el focus.
        /// </summary>
        private Boton BtnFocused { get; set; }
        /// <summary>
        /// Constructor de la clase StartScreen sin parámetros.
        /// </summary>
        /// <param name="game">Base de la aplicación.</param>
        public StartScreen(MainGame game):this(game,null)
        {
        }
        /// <summary>
        /// Constructor de la clase StartScreen al que se le indica qué error 
        /// ha sucedido durante la ejecución de la aplicación para mostrarlo en pantalla.
        /// </summary>
        /// <param name="game">Base de la aplicación.</param>
        /// <param name="msg">Mensaje de error.</param>
        public StartScreen(MainGame game, string msg)
        {
            Game = game;
            MsgError = msg;
        }
        /// <summary>
        /// Inicializa todas las propiedades y variables de la clase.
        /// </summary>
        public void Initialize()
        {
            ScreenWidth = Game.Graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = Game.Graphics.GraphicsDevice.Viewport.Height;
            BtnFocused = null;
        }
        /// <summary>
        /// Carga el contenido necesario en memoria.
        /// </summary>
        public void LoadContent()
        {
            Font = Game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            ErrorFont = Game.Content.Load<SpriteFont>("Fuentes/Error");
            BtnSelected = Game.Content.Load<Texture2D>("Sprites/btnSelected");
            BtnDefault = Game.Content.Load<Texture2D>("Sprites/btnDefault");
            BtnJoin = new Boton(
                ScreenWidth / 2 - ScreenWidth / 6,
                ScreenHeight / 2,
                Game.Content.Load<Texture2D>("Sprites/btnDefault"),
                ScreenWidth / 3,
                ErrorFont,
                "Join room"
            );
            BtnCreate = new Boton(
                ScreenWidth / 2 - ScreenWidth / 6,
                ScreenHeight * 3 / 4,
                Game.Content.Load<Texture2D>("Sprites/btnDefault"),
                ScreenWidth / 3,
                ErrorFont,
                "Create room"
            );
            btns.Add(BtnCreate);
            btns.Add(BtnJoin);
        }
        /// <summary>
        /// Se encarga del refresco de pantalla. Se realiza 60 veces por segundo.
        /// </summary>
        /// <param name="gameTime">Valor temporal interno.</param>
        /// <returns></returns>
        public Screen Update(GameTime gameTime)
        {
            foreach(Boton btn in btns)
            {
                if (btn.isHover(Mouse.GetState().X, Mouse.GetState().Y)){
                    clearButtons();
                    btn.Img = BtnSelected;
                    BtnFocused = btn;
                }
            }
            return this;
        }
        /// <summary>
        /// Borra el focus de todos los botones de la pantalla.
        /// </summary>
        public void clearButtons()
        {
            foreach(Boton btn in btns)
            {
                btn.Img = BtnDefault;
            }
            BtnFocused = null;
        }
        /// <summary>
        /// Dibuja todos los elementos de la pantalla.
        /// </summary>
        /// <param name="gameTime">Valor temporal interno.</param>
        public void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin();
            if (MsgError != null)
            {
                Game.SpriteBatch.DrawString(
                    ErrorFont,
                    MsgError,
                    new Vector2(ScreenWidth/2- ErrorFont.MeasureString(MsgError).X/2,ScreenHeight*2/5),
                    Color.Red);
            }
            Game.SpriteBatch.DrawString(
                Font, 
                "Contador", 
                new Vector2(ScreenWidth / 2 - Font.MeasureString("Contador").X / 2, ScreenHeight / 4-Font.MeasureString("Contador").Y/2), 
                Color.Black
                );
            BtnJoin.draw(Game);
            BtnCreate.draw(Game);
            Game.SpriteBatch.End();
        }
        /// <summary>
        /// Gestiona los clicks del ratón del usuario.
        /// </summary>
        /// <returns>Devuelve un objeto tipo Screen según las acciones del usuario.</returns>
        public Screen Click()
        {
            if (BtnJoin.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                Game.effects[MainGame.eSounds.click].Play();
                return new JoinScreen(Game);
            }
            if (BtnCreate.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                Game.effects[MainGame.eSounds.click].Play();
                return new CreateScreen(Game);
            }
            return this;
        }
        /// <summary>
        /// Gestiona las entradas por teclado del usuario.
        /// </summary>
        /// <param name="key">Tecla pulsada por el usuario.</param>
        /// <returns>Devuelve un objeto tipo Screen en función de las acciones del usuario.</returns>
        public Screen KeyboardAction(Keys key)
        {
            if (key == Keys.Tab)
            {
                if (BtnFocused != null)
                {
                    changeFocus();
                }
                else
                {
                    BtnFocused = btns[0];
                    btns[0].Img = BtnSelected;
                }
            }
            else if (key == Keys.Enter)
            {
                if (BtnFocused!=null)
                {
                    if (BtnFocused == BtnCreate)
                    {
                        return new CreateScreen(Game);
                    }else if (BtnFocused == BtnJoin)
                    {
                        return new JoinScreen(Game);
                    }
                }
            }
            return this;
        }
        /// <summary>
        /// Actualiza el focus de los botones.
        /// </summary>
        public void changeFocus()
        {
            foreach (Boton btn in btns)
            {
                if (btn == BtnFocused)
                {
                    clearButtons();
                    btn.Img = BtnDefault;
                    BtnFocused = btns.IndexOf(btn) >= btns.Count-1 ? btns[0] : btns[btns.IndexOf(btn) + 1];
                    btns[btns.IndexOf(BtnFocused)].Img = BtnSelected;
                    return;
                }

            }
        }
        /// <summary>
        /// Se ejecuta al cerrar la aplicación.
        /// Se encarga de cerrar posibles sockets abiertos, hilos y demás procesos que no han finalizado ni terminado de forma natural.
        /// </summary>
        public void onExiting()
        {

        }
    }
}
