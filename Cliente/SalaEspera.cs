using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cliente
{
    class SalaEspera : Pantalla
    {
        Game1 game;
        Servidor server;

        Texture2D imgStart;
        Boton btnStart;
        bool host;
        bool mouseClick;
        bool waitingRoomActive;
        string name;
        string room;
        List<string> players = new List<string>();
        SpriteFont font;
        SpriteFont errorFont;
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        int numberRoom;
        
        public SalaEspera(Game1 game,int numberRoom,string name,Servidor server,bool host)
        {
            this.game = game;
            this.numberRoom = numberRoom;
            this.name = name;
            this.server = server;
            this.host = host;
            waitingRoomActive = true;
            players.Add(name);
            Thread getNewPlayersThread = new Thread(() => getNewPlayers());
            getNewPlayersThread.Start();
        }
        public void getNewPlayers()
        {
            while (waitingRoomActive)
            {
                string frase = server.recibirDatos();
                if (frase == "start")
                {
                    waitingRoomActive = false;
                }
                else if(frase =="players")
                {
                    int playerNum = Convert.ToInt32(server.recibirDatos());
                    List<string> auxPlayers = new List<string>();
                    for (int i = 0; i < playerNum; i++)
                    {
                        auxPlayers.Add(server.getData());
                    }
                    string auxHost = server.recibirDatos();
                    if(auxHost == name)
                    {
                        host = true;
                    }
                    if (players.Count > auxPlayers.Count)
                    {
                        game.efectos[Game1.eSonidos.playerLeave].Play();
                    }
                    else if (players.Count < auxPlayers.Count)
                    {
                        game.efectos[Game1.eSonidos.newPlayer].Play();
                    }
                    players = auxPlayers;
                }
            }
        }
        public void Draw(GameTime gameTime)
        {
            game.spriteBatch.Begin();
            game.spriteBatch.DrawString(font, room+" "+numberRoom, new Vector2(ScreenWidth / 2 - font.MeasureString(room + " " + numberRoom).X/2, ScreenHeight / 8), Color.Black);
            int fila = 1;
            for (int i=0; i<players.Count; i++)
            {
                game.spriteBatch.DrawString(font, players[i],
                    new Vector2(
                        (i % 2) == 0 ? (float)ScreenWidth / 4 - font.MeasureString(players[i]).X/2 : (float)ScreenWidth*3 / 4 - font.MeasureString(players[i]).X/2, 
                        (float)ScreenHeight *2/ 5 +ScreenWidth*fila/15), 
                    Color.Black);
                if ((i+1) % 2 == 0)
                {
                    fila++;
                }
            }
            if (host)
            {
                btnStart.draw(game);
            }
            game.spriteBatch.End();
        }

        public void Initialize()
        {
            ScreenWidth = game.graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = game.graphics.GraphicsDevice.Viewport.Height;
            room = "Numero de sala:";
            mouseClick = false;
        }

        public void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("Fuentes/Intro");
            imgStart = game.Content.Load<Texture2D>("Sprites/btnEmpezar");
            btnStart = new Boton(ScreenWidth / 2 - ScreenWidth / 4 / 2, ScreenHeight * 4 / 5, imgStart, ScreenWidth / 4);
        }

        public Pantalla Update(GameTime gameTime)
        {
            if (!waitingRoomActive)
            {
                return new Partida(game,server,name,players.Count);
            }
            return this;
        }

        public Pantalla Click()
        {
            if (host && btnStart.click(Mouse.GetState().X, Mouse.GetState().Y) && players.Count >= 2)
            {
                game.efectos[Game1.eSonidos.click].Play();
                server.enviarDatos("empezar");
            }
            return this;
        }

        public void KeyboardAction(Keys key)
        {

        }
    }
}
