using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Client
{
    class JoinScreen : Screen
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
        /// Imagen de caja de texto por defecto.
        /// </summary>
        private Texture2D InputDefault { get; set; }
        /// <summary>
        /// Imagen de caja de texto con el focus.
        /// </summary>
        private Texture2D InputSelected { get; set; }
        /// <summary>
        /// Caja de texto para indicar la sala.
        /// </summary>
        private TextBox TxtInputRoom { get; set; }
        /// <summary>
        /// Caja de texto para introducir el nombre del jugador.
        /// </summary>
        private TextBox TxtInputName { get; set; }
        /// <summary>
        /// Indica qué caja de texto recibe el focus.
        /// </summary>
        private TextBox Focused { get; set; }
        /// <summary>
        /// Indica si ha ocurrido un error con la sala deseada.
        /// </summary>
        private bool RoomError { get; set; }
        /// <summary>
        /// Indica si hay un error con el nombre introducido.
        /// </summary>
        private bool NameError { get; set; }
        /// <summary>
        /// Indica si se ha hecho click en el Boton de unirse.
        /// </summary>
        private bool ClickJoin { get; set; }
        /// <summary>
        /// Mensaje de error debido a un error con la sala deseada.
        /// </summary>
        private string ErrorRoomMsg { get; set; }
        /// <summary>
        /// Mensaje de error debido a un error con el nombre introducido.
        /// </summary>
        private string ErrorNameMsg { get; set; }
        /// <summary>
        /// Colección de cajas de texto de la pantalla.
        /// </summary>
        private List<TextBox> inputs = new List<TextBox>();
        /// <summary>
        /// Mensaje que indica cual es la caja de la sala.
        /// </summary>
        private string RoomMessage { get; set; }
        /// <summary>
        /// Mensaje que indica cual es la caja del nombre.
        /// </summary>
        private string NameMessage { get; set; }
        /// <summary>
        /// Fuente por defecto.
        /// </summary>
        private SpriteFont DefaultFont { get; set; }
        /// <summary>
        /// Fuente de las cajas de texto.
        /// </summary>
        private SpriteFont InputFont { get; set; }
        /// <summary>
        /// Fuente para mensajes de error.
        /// </summary>
        private SpriteFont ErrorFont { get; set; }
        /// <summary>
        /// Indica el ancho de la pantalla.
        /// </summary>
        private int ScreenWidth { get; set; }
        /// <summary>
        /// Indica el alto de la pantalla.
        /// </summary>
        private int ScreenHeight { get; set; }
        /// <summary>
        /// Constructor del objeto JoinScreen.
        /// </summary>
        /// <param name="game">Base de la aplicación.</param>
        public JoinScreen(MainGame game)
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
                DefaultFont, 
                RoomMessage, 
                new Vector2(ScreenWidth / 2 - DefaultFont.MeasureString(RoomMessage).X / 2, ScreenHeight /10), 
                Color.Black
            );
            TxtInputRoom.draw(Game);
            Game.SpriteBatch.DrawString(
                DefaultFont, 
                NameMessage, 
                new Vector2(ScreenWidth / 2 - DefaultFont.MeasureString(NameMessage).X / 2, ScreenHeight/2-DefaultFont.MeasureString(NameMessage).Y), 
                Color.Black
            );
            if (NameError)
            {
                Game.SpriteBatch.DrawString(ErrorFont, ErrorNameMsg, new Vector2(ScreenWidth / 2 - ErrorFont.MeasureString(ErrorNameMsg).X/2, ScreenHeight * 15/20), Color.Red);
            }
            if (RoomError)
            {
                Game.SpriteBatch.DrawString(ErrorFont, ErrorRoomMsg, new Vector2(ScreenWidth / 2 - ErrorFont.MeasureString(ErrorRoomMsg).X / 2, ScreenHeight * 7/20), Color.Red);
            }
            TxtInputName.draw(Game);
            BtnStart.draw(Game);
            Game.SpriteBatch.End();
        }
        /// <summary>
        /// Inicializa todas las propiedades y variables de la clase.
        /// </summary>
        public void Initialize()
        {
            ScreenWidth = Game.Graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = Game.Graphics.GraphicsDevice.Viewport.Height;
            RoomError = false;
            NameError = false;
            ClickJoin = false;
            ErrorRoomMsg = "Invalid room.";
            ErrorNameMsg = "This name is too short or is already in use.";
            RoomMessage = "Type room's number:";
            NameMessage = "Type your nick:";
        }
        /// <summary>
        /// Carga el contenido necesario en memoria.
        /// </summary>
        public void LoadContent()
        {
            ErrorFont = Game.Content.Load<SpriteFont>("Fuentes/Error");
            BtnSelected = Game.Content.Load<Texture2D>("Sprites/btnSelected");
            BtnDefault = Game.Content.Load<Texture2D>("Sprites/btnDefault");
            BtnStart = new Boton(
                ScreenWidth / 2 - ScreenWidth / 8,
                ScreenHeight * 17/20,
                Game.Content.Load<Texture2D>("Sprites/btnDefault"),
                ScreenWidth / 4,
                ErrorFont,
                "Join room"
            );
            BtnBack = new Boton(
                0, 
                0,
                Game.Content.Load<Texture2D>("Sprites/btnBack"),
                ScreenWidth / 12
            );
            InputDefault = Game.Content.Load<Texture2D>("Sprites/textBox");
            InputSelected = Game.Content.Load<Texture2D>("Sprites/textBoxSelected");
            DefaultFont = Game.Content.Load<SpriteFont>("Fuentes/File");
            InputFont = Game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            TxtInputRoom = new TextBox(
                ScreenWidth / 2 - ScreenWidth * 3 / 8, 
                ScreenHeight * 7 / 20 - InputDefault.Height, 
                InputSelected, 
                InputDefault, 
                ScreenWidth * 3 / 4, 
                InputFont, 
                true
            );
            Focused = TxtInputRoom;
            TxtInputName = new TextBox(
                ScreenWidth / 2 - ScreenWidth * 3 / 8, 
                ScreenHeight * 15 / 20 - InputDefault.Height, 
                InputSelected, 
                InputDefault, 
                ScreenWidth * 3 / 4, 
                InputFont, 
                false
            );
            inputs.Add(TxtInputName);
            inputs.Add(TxtInputRoom);
            
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
            else if(BtnStart.Img != BtnDefault)
            {
                BtnStart.Img = BtnDefault;
            }
            return this;
        }
        /// <summary>
        /// Comprueba si el nombre pasado por parámetro es válido teniendo en cuenta su longitud.
        /// </summary>
        /// <param name="newName">Nuevo nombre.</param>
        /// <param name="input">Caja de texto en la que se introduce.</param>
        /// <returns>El nuevo nombre si es válido, el antiguo si no.</returns>
        public string checkName(string newName, TextBox input)
        {
            //Digamos que sirve para los números
            if (newName.Length > 1)
            {
                newName = newName.Substring(newName.Length - 1);
            }
            if (InputFont.MeasureString(input.Text + newName).X < ScreenWidth * 3 / 4)
            {
                return input.Text + newName;
            }
            return input.Text;
        }
        /// <summary>
        /// Modifica el foco de las cajas de texto a la pasada por parámetro.
        /// </summary>
        /// <param name="newFocus">Caja de texto sobre la que hacer el foco.</param>
        public void changeFocus(TextBox newFocus)
        {
            foreach(TextBox input in inputs)
            {
                input.Focus = input == newFocus ? true : false;
            }
        }
        /// <summary>
        /// Comprueba si es posible avanzar a la siguiente pantalla según el orden lógico de vida del programa.
        /// </summary>
        /// <returns>Devuelve la siguiente o la actual en caso de error.</returns>
        public Screen goNext()
        {
            NameError = false;
            RoomError = false;
            if (TxtInputName.Text.Length >= 3 && TxtInputRoom.Text.Length > 0)
            {
                //Mandar al servidor y comprobar que existe dicha sala pero dentro de ella no dicho nombre
                try
                {
                    Server servidor = new Server();
                    servidor.sendData("join");
                    servidor.sendData(TxtInputRoom.Text);
                    servidor.sendData(TxtInputName.Text);
                    switch (servidor.getData())
                    {
                        case "true":
                            Game.effects[MainGame.eSounds.click].Play();
                            return new WaitingRoomScreen(Game, Convert.ToInt32(TxtInputRoom.Text), TxtInputName.Text, servidor, false);
                        case "errorNombre":
                            RoomError = false;
                            NameError = true;
                            break;
                        case "errorSala":
                            NameError = false;
                            RoomError = true;
                            break;
                    }
                    servidor.closeServer();
                }catch(SocketException)
                {
                    RoomError = true;
                    ErrorRoomMsg = "Can't connect with server";
                    NameError = false;
                }catch (InvalidOperationException){
                    RoomError = true;
                    ErrorRoomMsg = "Can't connect with server";
                    NameError = false;
                }
            }
            if (TxtInputName.Text.Length < 3)
            {
                NameError = true;
            }
            if (TxtInputRoom.Text.Length <= 0)
            {
                RoomError = true;
            }
            ClickJoin = false;
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
            if (BtnStart.isHover(Mouse.GetState().X, Mouse.GetState().Y) && !ClickJoin)
            {
                ClickJoin = true;
                return goNext();
            }
            if (TxtInputName.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                changeFocus(TxtInputName);
            }
            if (TxtInputRoom.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                changeFocus(TxtInputRoom);
            }
            foreach (TextBox input in inputs)
            {
                if (input.Focus)
                {
                    Focused = input;
                }
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
            if ((key >= Keys.A && key <= Keys.Z) ||
                        (key >= Keys.D0 && key <= Keys.D9) ||
                        (key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                if (Focused != TxtInputRoom || !(key >= Keys.A && key <= Keys.Z))
                {
                    Focused.Text = checkName(key.ToString(), Focused);
                }
            }
            else if (key == Keys.Space)
            {
                Focused.Text = checkName(" ", Focused);
            }
            else if (key == Keys.Tab)
            {
                if (Focused == TxtInputName)
                {
                    changeFocus(TxtInputRoom);
                }
                else
                {
                    changeFocus(TxtInputName);
                }
                foreach (TextBox input in inputs)
                {
                    if (input.Focus)
                    {
                        Focused = input;
                    }
                }
            }
            else if (key == Keys.Enter) {
                return goNext();
            }
            if (key == Keys.Back && Focused.Text.Length >= 1)
            {
                Focused.Text = Focused.Text.Remove(Focused.Text.Length - 1);
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
