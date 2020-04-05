using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cliente
{
    class Partida : Pantalla
    {
        Game1 game;
        Servidor server;

        Thread hiloConectividad;
        string name;
        bool mouseClick;
        bool playing;
        int rank;
        int playersAtBegin;

        //Imagenes cartas
        Dictionary<string, Texture2D> cartas = new Dictionary<string, Texture2D>();
        //Cartas jugador
        List<Boton> cartasBtn = new List<Boton>();
        Boton btnSelectedCard;
        //Boton jugar y botón pasar
        Boton btnPlay;
        Boton btnPass;
        Texture2D imgBtnPlay;
        Texture2D imgBtnPass;
        int selectedCard;
        //Definen el ancho y alto de la ventana
        public float ScreenWidth { get; set; }
        public float ScreenHeight { get; set; }
        //Define el ancho que tendrá cada columna contenedora de una carta
        float column;

        //Vector que define la posición del valor de mesa
        SpriteFont font;
        SpriteFont fontValue;

        //Maximo numero de cartas que se muestran a la vez
        int maxCards = 8;
        PaqueteTurno data;

        //Determina si el jugador ha acabado
        bool finish;
        //Texture2D que nos permite tener siempre acceso a una carta
        Texture2D exampleCard;
        //Vectores que definen la posicion y reescalado de las cartas del jugador
        Vector2 cardPosition;
        public Partida(Game1 game,Servidor server,string name,int players)
        {
            this.game = game;
            this.server = server;
            this.name = name;
            playing = true;
            playersAtBegin = players;
        }

        public void Draw(GameTime gameTime)
        {
            game.spriteBatch.Begin();
            dibujarMesa();
            dibujarMarcadores();
            dibujarTurno();
            dibujarBotones();
            if (selectedCard != -1)
            {
                btnSelectedCard.draw(game);
            }
            dibujarCartas();
            game.spriteBatch.End();
        }   

        public void Initialize()
        {
            selectedCard = -1;
            actualizarDatos();
            hiloConectividad = new Thread(() => comprobarTurno());
            hiloConectividad.Start();
            //Al empezar el jugador está jugando
            finish = false;
            rank = 0;
            //Permitimos ver el cursor en la centana
            //Window.AllowUserResizing = true;
            ScreenHeight = game.graphics.GraphicsDevice.Viewport.Height;
            ScreenWidth = game.graphics.GraphicsDevice.Viewport.Width;
            column = ScreenWidth / maxCards;
            mouseClick = false;
        }

        public void LoadContent()
        {
            for (int i = 3; i < 8; i++)
            {
                cartas.Add(i.ToString(), game.Content.Load<Texture2D>("Sprites/" + i));
            }
            cartas.Add("bucle", game.Content.Load<Texture2D>("Sprites/bucle"));
            cartas.Add("minus", game.Content.Load<Texture2D>("Sprites/minus"));
            cartas.Add("plus", game.Content.Load<Texture2D>("Sprites/plus"));
            cartas.Add("selectedCard", game.Content.Load<Texture2D>("Sprites/selectedCard"));
            //Cargamos la fuente del juego
            font = game.Content.Load<SpriteFont>("Fuentes/Fuente");
            fontValue = game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            exampleCard = cartas["3"];

            imgBtnPlay = game.Content.Load<Texture2D>("Sprites/btnJugar");
            imgBtnPass = game.Content.Load<Texture2D>("Sprites/btnPasar");
            actualizarBaraja();
            btnPlay = new Boton(ScreenWidth / 2 - ScreenWidth / 8, ScreenHeight / 2, imgBtnPlay, ScreenWidth / 8);
            btnPass = new Boton(ScreenWidth / 2, ScreenHeight / 2, imgBtnPass, ScreenWidth / 8);
            btnSelectedCard = new Boton(0, 0, cartas["selectedCard"], column);
        }

        public Pantalla Update(GameTime gameTime)
        {
            if (!playing)
            {
                server.closeServer();
                return new FinPartida(game, rank);
            }
            //Si la ventana del juego cambia sus dimensiones, se adaptan los tamaños de los objetos
            if (game.graphics.GraphicsDevice.Viewport.Width != ScreenWidth || game.graphics.GraphicsDevice.Viewport.Height != ScreenHeight)
            {
                ScreenHeight = game.graphics.GraphicsDevice.Viewport.Height;
                ScreenWidth = game.graphics.GraphicsDevice.Viewport.Width;
                column = ScreenWidth / maxCards;
            }
            //Para cada carta del jugador, se carga su correspondiente imagen
            actualizarBaraja();
            return this;
        }
        public void comprobarTurno()
        {
            while (playing)
            {
                actualizarDatos();
            }
        }
        public void actualizarDatos()
        {
            string aux = server.recibirDatos();
            if (aux == "finPartida")
            {
                rank = Convert.ToInt32(server.recibirDatos());
                playing = false;
                return;
            }
            int numCartas = Convert.ToInt32(aux);
            List<Carta> auxLista = new List<Carta>();
            for (int i = 0; i < numCartas; i++)
            {
                auxLista.Add(
                    new Carta(
                        (Carta.eTipo)Enum.Parse(typeof(Carta.eTipo), 
                        server.recibirDatos()), 
                        Convert.ToInt32(server.recibirDatos()), 
                        Convert.ToBoolean(server.recibirDatos())
                        )
                    );
            }
            int auxValor = Convert.ToInt32(server.recibirDatos());
            bool auxSentido = Convert.ToBoolean(server.recibirDatos());
            string auxTurno = server.recibirDatos();
            Dictionary<string, int> auxDic = new Dictionary<string, int>();
            int numJugadores = Convert.ToInt32(server.recibirDatos());
            for (int i = 0; i < numJugadores; i++)
            {
                auxDic.Add(server.recibirDatos(), Convert.ToInt32(server.recibirDatos()));
            }
            data = new PaqueteTurno(auxLista, auxValor, auxSentido, auxTurno, auxDic);
        }
        public void actualizarBaraja()
        {
            cartasBtn.Clear();
            foreach (Carta card in data.Cartas)
            {
                string nombre = "";
                switch (card.Tipo)
                {
                    case Carta.eTipo.Numero:
                        nombre = card.Valor.ToString();
                        break;
                    case Carta.eTipo.Sentido:
                        if (card.Sentido)
                        {
                            nombre = "plus";
                        }
                        else
                        {
                            nombre = "minus";
                        }
                        break;
                    case Carta.eTipo.Efecto:
                        nombre = "bucle";
                        break;
                }
                cartasBtn.Add(new Boton(0, 0, cartas[nombre], column * 8 / 10));
                cardPosition = new Vector2(column / 10, ScreenHeight / 2 + column * 8 / 10 / 2);
                foreach (Boton btn in cartasBtn)
                {
                    btn.X = cardPosition.X;
                    btn.Y = cardPosition.Y;
                    cardPosition.X += column;
                }
            }
        }
        public void dibujarBotones()
        {
            game.spriteBatch.Draw(btnPlay.Img, position: new Vector2(ScreenWidth / 2 - btnPlay.Width, ScreenHeight / 2), scale: btnPlay.Scale);
            game.spriteBatch.Draw(btnPass.Img, position: new Vector2(ScreenWidth / 2, ScreenHeight / 2), scale: btnPass.Scale);
        }
        public void dibujarCartas()
        {
            //Estableceemos donde empezarán a dibujarse 
            cardPosition = new Vector2(column / 10, ScreenHeight / 2 + column * 8 / 10 / 2);
            //Dibujado de las cartas
            foreach (Boton btn in cartasBtn)
            {
                //Dibujamos la carta
                game.spriteBatch.Draw(btn.Img, position: new Vector2(btn.X, btn.Y), scale: btn.Scale);
            }
        }
        public void dibujarMesa()
        {
            //Dibujamos el valor y sentido de mesa
            game.spriteBatch.DrawString(
                fontValue,
                data.ValorMesa.ToString(),
                new Vector2(ScreenWidth / 2, ScreenHeight / 4) - fontValue.MeasureString(data.ValorMesa.ToString()) / 2,
                Color.White
                );
            game.spriteBatch.DrawString(
                fontValue,
                data.Sentido ? "+" : "-",
                new Vector2(ScreenWidth / 2, ScreenHeight / 4) - fontValue.MeasureString(data.ValorMesa.ToString()) / 2 - fontValue.MeasureString(data.Sentido.ToString()),
                Color.White);
        }
        public void dibujarTurno()
        {
            Vector2 posicionTurno = new Vector2(ScreenWidth - font.MeasureString("Turno: " + data.Turno).X, 0);
            game.spriteBatch.DrawString(font, "Turno: " + data.Turno, posicionTurno, Color.White);
        }
        public void dibujarMarcadores()
        {
            Vector2 posicionMarcador = new Vector2(column / 10, column / 10);
            foreach (var jugador in data.Jugadores)
            {
                game.spriteBatch.DrawString(font, jugador.Key + ": " + jugador.Value, posicionMarcador, Color.White);
                posicionMarcador.Y += font.MeasureString(jugador.Key + ": " + jugador.Value).Y;
            }
        }

        public Pantalla Click()
        {
            if (data.Turno == name)
            {
                if (btnPass.click(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    server.enviarDatos("pasar");
                }
                if (btnPlay.click(Mouse.GetState().X, Mouse.GetState().Y) && selectedCard != -1)
                {
                    server.enviarDatos("jugar");
                    server.enviarDatos(data.Cartas[selectedCard].Tipo.ToString());
                    server.enviarDatos(data.Cartas[selectedCard].Valor.ToString());
                    server.enviarDatos(data.Cartas[selectedCard].Sentido.ToString());
                    selectedCard = -1;
                }
            }
            foreach (Boton btn in cartasBtn)
            {
                if (btn.click(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    selectedCard = cartasBtn.IndexOf(btn);
                    btnSelectedCard.X = btn.X-column/10;
                    btnSelectedCard.Y = (btn.Y+btn.Height/2)-btnSelectedCard.Height/2;
                }
            }
            return this;
        }

        public void KeyboardAction(Keys key)
        {

        }
    }
}
