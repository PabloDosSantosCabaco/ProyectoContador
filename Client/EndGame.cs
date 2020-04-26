using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Client
{
    class EndGame : Screen
    {
        /// <summary>
        /// Puesto en el que queda el jugador.
        /// </summary>
        private int Rank { get; set; }
        /// <summary>
        /// Referencia la base del juego del que parten todas las pantallas.
        /// </summary>
        private MainGame Game { get; set; }
        /// <summary>
        /// Boton de regreso a la pantalla inicial.
        /// </summary>
        private Boton BtnBack { get; set; }
        /// <summary>
        /// Fuente por defecto.
        /// </summary>
        private SpriteFont Font { get; set; }
        //Fuente utilizada para indicar errores.
        private SpriteFont ErrorFont { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// Indica el ancho de la pantalla.
        /// </summary>
        private int ScreenWidth { get; set; }
        /// <summary>
        /// Indica el alto de la pantalla.
        /// </summary>
        private int ScreenHeight { get; set; }
        /// <summary>
        /// Mensaje de fin de partida.
        /// </summary>
        private string RankMessage { get; set; }
        /// <summary>
        /// Imagen que indica que el Boton está seleccionado.
        /// </summary>
        private Texture2D BtnSelected { get; set; }
        /// <summary>
        /// Imagen de Boton por defecto.
        /// </summary>
        private Texture2D BtnDefault { get; set; }
        /// <summary>
        /// Constructor del objeto EndGame.
        /// </summary>
        /// <param name="game">Base de la aplicación.</param>
        /// <param name="rank">Puesto del jugador en el ranking.</param>
        public EndGame(MainGame game,int rank)
        {
            this.Game = game;
            Rank = rank;
        }
        /// <summary>
        /// Dibuja todos los elementos de la pantalla.
        /// </summary>
        /// <param name="gameTime">Valor temporal interno.</param>
        public void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.DrawString(
                Font,
                RankMessage, 
                new Vector2(ScreenWidth / 2 - Font.MeasureString(RankMessage).X / 2, ScreenWidth / 5), 
                Color.Black
            );
            BtnBack.draw(Game);
            Game.SpriteBatch.End();
        }
        /// <summary>
        /// Inicializa todas las propiedades y variables de la clase.
        /// </summary>
        public void Initialize()
        {
            ScreenWidth = Game.GraphicsDevice.Viewport.Width;
            ScreenHeight = Game.GraphicsDevice.Viewport.Height;
            RankMessage = "You are the number " + Rank;
        }
        /// <summary>
        /// Carga el contenido necesario en memoria.
        /// </summary>
        public void LoadContent()
        {
            BtnSelected = Game.Content.Load<Texture2D>("Sprites/btnSelected");
            BtnDefault = Game.Content.Load<Texture2D>("Sprites/btnDefault");
            ErrorFont = Game.Content.Load<SpriteFont>("Fuentes/Error");
            BtnBack = new Boton(
                ScreenWidth / 2 - ScreenWidth / 4/2,
                ScreenHeight * 3 / 5,
                BtnSelected,
                ScreenWidth / 4,
                ErrorFont,
                "Volver"
            );

            Font = Game.Content.Load<SpriteFont>("Fuentes/Fuente");
        }
        /// <summary>
        /// Se encarga del refresco de pantalla. Se realiza 60 veces por segundo.
        /// </summary>
        /// <param name="gameTime">Valor temporal interno.</param>
        /// <returns></returns>
        public Screen Update(GameTime gameTime)
        {
            if (BtnBack.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                BtnBack.Img = BtnSelected;
            }else if(BtnBack.Img == BtnSelected)
            {
                BtnBack.Img = BtnDefault;
            }
            return this;
        }
        /// <summary>
        /// Gestiona los clicks del ratón del usuario.
        /// </summary>
        /// <returns>Devuelve un objeto tipo Screen según las acciones del usuario.</returns>
        public Screen Click()
        {
            if (BtnBack.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                Game.effects[MainGame.eSounds.click].Play();
                return new StartScreen(Game);
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
            if (key == Keys.Enter)
            {
                Game.effects[MainGame.eSounds.click].Play();
                return new StartScreen(Game);
            }
            return this;
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
