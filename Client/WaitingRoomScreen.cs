using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;

namespace Client
{
    class WaitingRoomScreen : Screen
    {
        /// <summary>
        /// Base de la aplicación.
        /// </summary>
        private MainGame Game { get; set; }
        /// <summary>
        /// Objeto Server que permite la conexión y comunicación con el servidor.
        /// </summary>
        private Server Server { get; set; }
        /// <summary>
        /// Indica si se ha pulsado el Boton que hay en pantalla.
        /// </summary>
        private bool ClickStart { get; set; }
        /// <summary>
        /// Boton que permite empezar la partida.
        /// </summary>
        private Boton BtnStart { get; set; }
        /// <summary>
        /// Boton que permite volver a la pantalla inicial abandonando ña sala.
        /// </summary>
        private Boton BtnBack { get; set; }

        /// <summary>
        /// Imagen que indica que el Boton está seleccionado.
        /// </summary>
        private Texture2D BtnSelected { get; set; }
        /// <summary>
        /// Imagen de Boton por defecto.
        /// </summary>
        private Texture2D BtnDefault { get; set; }
        /// <summary>
        /// Indica si el jugador es el host de la sala.
        /// </summary>
        private bool Host { get; set; }
        /// <summary>
        /// Indica si la sala de espera sigue como pantalla actual.
        /// </summary>
        private bool WaitingRoomActive { get; set; }
        /// <summary>
        /// Indica el nombre el jugador.
        /// </summary>
        private string Name { get; set; }
        /// <summary>
        /// Frase para indicar el número de la sala.
        /// </summary>
        private string Room { get; set; }
        /// <summary>
        /// Colección de nombres de los jugadores que entran a la sala.
        /// </summary>
        private List<string> Players = new List<string>();
        /// <summary>
        /// Fuente por defecto.
        /// </summary>
        private SpriteFont Font { get; set; }
        /// <summary>
        /// Fuente secundaria.
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
        /// Indica el número de la sala.
        /// </summary>
        private int NumberRoom { get; set; }
        /// <summary>
        /// Indica si ha sucedido un error con la conexión.
        /// </summary>
        private bool ServerError { get; set; }
        /// <summary>
        /// Hilo encargado de captar jugadores que quieran entrar en la sala.
        /// </summary>
        private Thread getNewPlayersThread;
        /// <summary>
        /// Constructor de la clase WaitingRoom.
        /// </summary>
        /// <param name="game">Base de la aplicación.</param>
        /// <param name="numberRoom">Numero de la sala.</param>
        /// <param name="name">Nombre del jugador.</param>
        /// <param name="server">Objeto Server que conecta con el servidor.</param>
        /// <param name="host">Indica si es o no el host de la sala.</param>
        public WaitingRoomScreen(MainGame game,int numberRoom,string name,Server server,bool host)
        {
            Game = game;
            NumberRoom = numberRoom;
            Name = name;
            Server = server;
            Host = host;
            WaitingRoomActive = true;
            Players.Add(name);
            getNewPlayersThread = new Thread(() => getNewPlayers());
            getNewPlayersThread.Start();
        }
        /// <summary>
        /// Recibe nuevos jugadores a la sala y actualiza la información.
        /// </summary>
        public void getNewPlayers()
        {
            try
            {
                while (WaitingRoomActive)
                {
                    string frase = Server.getData();
                    if (frase == "start")
                    {
                        WaitingRoomActive = false;
                    }
                    else if (frase == "players")
                    {
                        int playerNum = Convert.ToInt32(Server.getData());
                        List<string> auxPlayers = new List<string>();
                        for (int i = 0; i < playerNum; i++)
                        {
                            auxPlayers.Add(Server.getData());
                        }
                        string auxHost = Server.getData();
                        if (auxHost == Name)
                        {
                            Host = true;
                        }
                        if (Players.Count > auxPlayers.Count)
                        {
                            Game.effects[MainGame.eSounds.playerLeave].Play();
                        }
                        else if (Players.Count < auxPlayers.Count)
                        {
                            Game.effects[MainGame.eSounds.newPlayer].Play();
                        }
                        Players = auxPlayers;
                    }else if(frase == "error")
                    {
                        ClickStart = false;
                    }
                }
            }
            catch (IOException)
            {
                ServerError = true;
            }
        }
        /// <summary>
        /// Dibuja todos los elementos de la pantalla.
        /// </summary>
        /// <param name="gameTime">Valor temporal interno.</param>
        public void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin();
            BtnBack.draw(Game);
            Game.SpriteBatch.DrawString(Font, Room+" "+NumberRoom, new Vector2(ScreenWidth / 2 - Font.MeasureString(Room + " " + NumberRoom).X/2, ScreenHeight / 8), Color.Black);
            int row = 1;
            for (int i=0; i<Players.Count; i++)
            {
                Game.SpriteBatch.DrawString(Font, Players[i],
                    new Vector2(
                        (i % 2) == 0 ? (float)ScreenWidth / 4 - Font.MeasureString(Players[i]).X/2 : (float)ScreenWidth*3 / 4 - Font.MeasureString(Players[i]).X/2, 
                        (float)ScreenHeight *2/ 5 +ScreenWidth*row/15), 
                    Color.Black);
                if ((i+1) % 2 == 0)
                {
                    row++;
                }
            }
            if (Host)
            {
                BtnStart.draw(Game);
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
            Room = "Room's number:";
            ServerError = false;
            ClickStart = false;
        }
        /// <summary>
        /// Carga el contenido necesario en memoria.
        /// </summary>
        public void LoadContent()
        {
            BtnSelected = Game.Content.Load<Texture2D>("Sprites/btnSelected");
            BtnDefault = Game.Content.Load<Texture2D>("Sprites/btnDefault");
            ErrorFont = Game.Content.Load<SpriteFont>("Fuentes/Error");
            Font = Game.Content.Load<SpriteFont>("Fuentes/File");
            BtnStart = new Boton(
                ScreenWidth / 2 - ScreenWidth / 4 / 2, 
                ScreenHeight * 4 / 5, 
                BtnDefault, 
                ScreenWidth / 4,
                ErrorFont,
                "Start game"
            );
            BtnBack = new Boton(
                0, 
                0, 
                Game.Content.Load<Texture2D>("Sprites/btnBack"), 
                ScreenWidth / 12
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
            if (!WaitingRoomActive)
            {
                getNewPlayersThread.Join();
                return new Match(Game,Server,Name,Players.Count);
            }
            if (ServerError)
            {
                getNewPlayersThread.Join();
                return new StartScreen(Game,"Server's connection lost");
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
                Server.closeServer();
                getNewPlayersThread.Join();
                return new StartScreen(Game);
            }
            if (!ClickStart && Host && BtnStart.isHover(Mouse.GetState().X, Mouse.GetState().Y) && Players.Count >= 2)
            {
                ClickStart = true;
                Game.effects[MainGame.eSounds.click].Play();
                Server.sendData("empezar");
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
            if(Host && key == Keys.Enter && Players.Count >= 2)
            {
                Game.effects[MainGame.eSounds.click].Play();
                Server.sendData("empezar");
            }
            return this;
        }
        /// <summary>
        /// Se ejecuta al cerrar la aplicación.
        /// Se encarga de cerrar posibles sockets abiertos, hilos y demás procesos que no han finalizado ni terminado de forma natural.
        /// </summary>
        public void onExiting()
        {
            Server.closeServer();
            getNewPlayersThread.Join();
        }
    }
}
