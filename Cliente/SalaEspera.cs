using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        bool waitingRoomActive;
        string name;
        string room = "Numero de sala:";
        List<string> players = new List<string>();
        SpriteFont font;
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
                int playerNum = Convert.ToInt32(server.getData());
                players.Clear();
                for (int i = 0; i < playerNum; i++)
                {
                    players.Add(server.getData());
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
        }

        public void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("Fuentes/Intro");
            if (host)
            {
                imgStart = game.Content.Load<Texture2D>("Sprites/btnEmpezar");
                btnStart = new Boton(ScreenWidth / 2 - ScreenWidth / 4 / 2, ScreenHeight * 4 / 5, imgStart, ScreenWidth / 4);
            }
        }

        public Pantalla Update(GameTime gameTime)
        {
            
            return this;
        }
    }
}
