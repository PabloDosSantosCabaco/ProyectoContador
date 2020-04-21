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
        private MainGame Game { get; set; }
        private Server Server { get; set; }

        private bool ClickStart { get; set; }
        private Boton BtnStart { get; set; }
        private Boton BtnBack { get; set; }
        private Texture2D BtnSelected { get; set; }
        private Texture2D BtnDefault { get; set; }
        private bool Host { get; set; }
        private bool WaitingRoomActive { get; set; }
        private string Name { get; set; }
        private string Room { get; set; }
        private List<string> Players = new List<string>();
        private SpriteFont Font { get; set; }
        private SpriteFont ErrorFont { get; set; }
        private int ScreenWidth { get; set; }
        private int ScreenHeight { get; set; }
        private int NumberRoom { get; set; }
        private bool ServerError { get; set; }
        private Thread getNewPlayersThread;
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

        public void Initialize()
        {
            ScreenWidth = Game.Graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = Game.Graphics.GraphicsDevice.Viewport.Height;
            Room = "Room's number:";
            ServerError = false;
            ClickStart = false;
        }

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

        public Screen KeyboardAction(Keys key)
        {
            if(Host && key == Keys.Enter && Players.Count >= 2)
            {
                Game.effects[MainGame.eSounds.click].Play();
                Server.sendData("empezar");
            }
            return this;
        }

        public void onExiting(object sender, EventArgs args)
        {
            Server.closeServer();
            getNewPlayersThread.Join();
        }
    }
}
