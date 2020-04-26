using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net.Sockets;

namespace Client
{
    class CreateScreen : Screen
    {
        /// <summary>
        /// Referencia la base del juego del que parten todas las pantallas.
        /// </summary>
        private MainGame Game { get; set; }
        /// <summary>
        /// Boton de regreso a la pantalla anterior.
        /// </summary>
        private Boton BtnBack { get; set; }
        /// <summary>
        /// Boton que permite crear la sala prepartida.
        /// </summary>
        private Boton BtnStart { get; set; }
        /// <summary>
        /// Imagen que indica que el Boton está seleccionado.
        /// </summary>
        private Texture2D BtnSelected { get; set; }
        /// <summary>
        /// Imagen de Boton por defecto.
        /// </summary>
        private Texture2D BtnDefault { get; set; }
        /// <summary>
        /// Caja de texto que permite introducir el nombre.
        /// </summary>
        private TextBox BtnInput { get; set; }
        /// <summary>
        /// Mensaje que indica la introducción de texto.
        /// </summary>
        private string Intro { get; set; }
        /// <summary>
        /// Mensaje de error ante posibles datos incorrectos.
        /// </summary>
        private string ErrorMsg { get; set; }
        /// <summary>
        /// Fuente por defecto.
        /// </summary>
        private SpriteFont Font { get; set; }
        /// <summary>
        /// Fuente para mensajes de error.
        /// </summary>
        private SpriteFont ErrorFont { get; set; }
        /// <summary>
        /// Indica si se ha hecho click en el Boton BtnStart.
        /// </summary>
        private bool ClickCreate { get; set; }
        /// <summary>
        /// Indica el ancho de la pantalla.
        /// </summary>
        private int ScreenWidth { get; set; }
        /// <summary>
        /// Indica el alto de la pantalla.
        /// </summary>
        private int ScreenHeight { get; set; }
        /// <summary>
        /// Indica si ha ocurrido un error.
        /// </summary>
        private bool ErrorOcurrered { get; set; }
        /// <summary>
        /// Constructor de CreateScreen.
        /// </summary>
        /// <param name="game">Base de la aplicación.</param>
        public CreateScreen(MainGame game)
        {
            Game = game;
        }
        /// <summary>
        /// Dibuja todos los elementos de la pantalla.
        /// </summary>
        /// <param name="gameTime">Valor temporal interno.</param>
        public void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin();
            BtnBack.draw(Game);
            Game.SpriteBatch.DrawString(
                Font, 
                Intro, 
                new Vector2(ScreenWidth / 2 - Font.MeasureString(Intro).X / 2, ScreenHeight / 4), 
                Color.Black
            );
            BtnInput.draw(Game);
            BtnStart.draw(Game);
            if (ErrorOcurrered)
            {
                Game.SpriteBatch.DrawString(
                    ErrorFont,
                    ErrorMsg, 
                    new Vector2(ScreenWidth/2-ErrorFont.MeasureString(ErrorMsg).X/2, ScreenHeight * 7/10), 
                    Color.Red
                );

            }
            Game.SpriteBatch.End();
        }
        /// <summary>
        /// Inicializa todas las propiedades y variables de la clase.
        /// </summary>
        public void Initialize()
        {
            ScreenWidth = Game.Graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = Game.Graphics.GraphicsDevice.Viewport.Height;
            Intro = "Type your nick:";
            ErrorOcurrered = false;
            ClickCreate = false;
            ErrorMsg = "Type a name with 3 characters or more";
        }
        /// <summary>
        /// Carga el contenido necesario en memoria.
        /// </summary>
        public void LoadContent()
        {
            BtnBack = new Boton(0,0, Game.Content.Load<Texture2D>("Sprites/btnBack"), ScreenWidth / 12);
            BtnSelected = Game.Content.Load<Texture2D>("Sprites/btnSelected");
            BtnDefault = Game.Content.Load<Texture2D>("Sprites/btnDefault");
            Font = Game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            BtnInput = new TextBox(
                ScreenWidth / 2 - ScreenWidth * 3 / 8,
                ScreenHeight / 2,
                Game.Content.Load<Texture2D>("Sprites/textBoxSelected"),
                Game.Content.Load<Texture2D>("Sprites/textBoxSelected"),
                ScreenWidth * 3 / 4,
                Font,
                true
            );
            ErrorFont = Game.Content.Load<SpriteFont>("Fuentes/Error");
            BtnStart = new Boton(
                ScreenWidth / 2 - ScreenWidth / 3 / 2, 
                ScreenHeight * 3 / 4, 
                BtnDefault, 
                ScreenWidth / 3,
                ErrorFont,
                "Create room"
            );
        }
        /// <summary>
        /// Se encarga del refresco de pantalla. Se realiza 60 veces por segundo.
        /// </summary>
        /// <param name="gameTime">Valor temporal interno.</param>
        /// <returns></returns>
        public Screen Update(GameTime gameTime)
        {
            if (BtnStart.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                BtnStart.Img = BtnSelected;
            }
            else if (BtnStart.Img != BtnDefault)
            {
                BtnStart.Img = BtnDefault;
            }
            return this;
        }
        /// <summary>
        /// Comprueba si el nombre pasado por parámetro es válido teniendo en cuenta su longitud.
        /// </summary>
        /// <param name="newName">Nuevo nombre.</param>
        /// <returns>Devuelve el nombre nuevo si es válido, sino el antiguo.</returns>
        public string checkName(string newName)
        {
            //Digamos que sirve para los números
            if (newName.Length > 1)
            {
                newName = newName.Substring(newName.Length - 1);
            }
            if (Font.MeasureString(BtnInput.Text + newName).X < ScreenWidth * 3 / 4)
            {
                return BtnInput.Text + newName;
            }
            return BtnInput.Text;
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
            if (BtnStart.isHover(Mouse.GetState().X, Mouse.GetState().Y) && !ClickCreate)
            {
                ClickCreate = true;
                return goNext();
            }
            return this;
        }
        /// <summary>
        /// Comprueba si es posible avanzar a la siguiente pantalla según el orden lógico de vida del programa.
        /// </summary>
        /// <returns>Devuelve la siguiente o la actual en caso de error.</returns>
        public Screen goNext()
        {
            if (BtnInput.Text.Trim().Length < 3)
            {
                ErrorOcurrered = true;
            }
            else
            {
                //Pedir numero sala al servidor
                Game.effects[MainGame.eSounds.click].Play();
                try
                {
                    Server servidor = new Server();
                    return new WaitingRoomScreen(Game, servidor.getSala(BtnInput.Text), BtnInput.Text, servidor, true);
                }
                catch (SocketException) {
                    ErrorOcurrered = true;
                    ErrorMsg = "Can't connect with server";
                }
            }
            ClickCreate = false;
            return this;
        }
        /// <summary>
        /// Gestiona las entradas por teclado del usuario.
        /// </summary>
        /// <param name="key">Tecla pulsada por el usuario.</param>
        /// <returns>Devuelve un objeto tipo Screen en función de las acciones del usuario.</returns>
        public Screen KeyboardAction(Keys key)
        {
            if (key >= Keys.A && key <= Keys.Z ||
                (key >= Keys.D0 && key <= Keys.D9) ||
                (key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                BtnInput.Text = checkName(key.ToString());
            }
            else if (key == Keys.Space)
            {
                BtnInput.Text = checkName(" ");
            }else if(key == Keys.Enter)
            {
                return goNext();
            }
            if (key == Keys.Back && BtnInput.Text.Length >= 1)
            {
                BtnInput.Text = BtnInput.Text.Remove(BtnInput.Text.Length - 1);
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
