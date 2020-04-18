﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
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
        bool playing;
        int rank;
        bool serverDown;

        //Imagenes cartas
        Dictionary<string, Texture2D> cartas;
        //Cartas jugador
        List<Boton> cartasBtn;
        Boton btnSelectedCard;
        //Boton jugar y botón pasar
        Boton btnPlay;
        Boton btnPass;
        Boton btnNextCard;
        Boton btnPreviousCard;
        int selectedCard;
        bool serverWaiting;
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
            serverDown = false;
            cartasBtn = new List<Boton>();
            cartas = new Dictionary<string, Texture2D>();
            data = null;
        }

        public void Draw(GameTime gameTime)
        {
            game.spriteBatch.Begin();
            if (data != null)
            {
                dibujarMesa();
                dibujarMarcadores();
                dibujarTurno();
                dibujarBotones();
                if (selectedCard != -1)
                {
                    btnSelectedCard.draw(game);
                }
                dibujarCartas();
            }
            game.spriteBatch.End();
        }   

        public void Initialize()
        {
            selectedCard = -1;
            //actualizarDatos();
            //Al empezar el jugador está jugando
            rank = 0;
            //Permitimos ver el cursor en la centana
            //Window.AllowUserResizing = true;
            ScreenHeight = game.graphics.GraphicsDevice.Viewport.Height;
            ScreenWidth = game.graphics.GraphicsDevice.Viewport.Width;
            column = ScreenWidth / maxCards;
            serverWaiting = true;
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
            cardPosition = new Vector2(column / 10, ScreenHeight * 7 / 8 - exampleCard.Height / 2);
            btnPlay = new Boton(ScreenWidth / 2 - ScreenWidth / 8, ScreenHeight / 2, game.Content.Load<Texture2D>("Sprites/btnJugar"), ScreenWidth / 8);
            btnPass = new Boton(ScreenWidth / 2, ScreenHeight / 2, game.Content.Load<Texture2D>("Sprites/btnPasar"), ScreenWidth / 8);
            btnNextCard = new Boton(ScreenWidth - ScreenWidth / 15, ScreenHeight - ScreenWidth / 15, game.Content.Load<Texture2D>("Sprites/btnNextCard"), ScreenWidth / 15);
            btnPreviousCard = new Boton(0, ScreenHeight - ScreenWidth / 15, game.Content.Load<Texture2D>("Sprites/btnPreviousCard"), ScreenWidth / 15);
            btnSelectedCard = new Boton(0, 0, cartas["selectedCard"], column);
            hiloConectividad = new Thread(() => actualizarDatos());
            hiloConectividad.Start();
        }

        public Pantalla Update(GameTime gameTime)
        {
            if (!playing)
            {
                server.closeServer();
                hiloConectividad.Join();
                return new FinPartida(game, rank);
            }
            if (serverDown)
            {
                server.closeServer();
                hiloConectividad.Join();
                return new PantallaInicio(game, "Se ha perdido la conexion con el servidor");
            }
            //Si la ventana del juego cambia sus dimensiones, se adaptan los tamaños de los objetos
            if (game.graphics.GraphicsDevice.Viewport.Width != ScreenWidth || game.graphics.GraphicsDevice.Viewport.Height != ScreenHeight)
            {
                ScreenHeight = game.graphics.GraphicsDevice.Viewport.Height;
                ScreenWidth = game.graphics.GraphicsDevice.Viewport.Width;
                column = ScreenWidth / maxCards;
            }
            centerCards();
            return this;
        }
        public void centerCards()
        {
            if (cartasBtn.Count <= maxCards)
            {
                cardPosition.X = column / 10;
            }
        }
        public void actualizarDatos()
        {
            try
            {
                while (playing)
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
                    actualizarBaraja();
                    serverWaiting = true;
                }
            }catch(IOException)
            {
                serverDown = true;
            }
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
            }
            if (cartasBtn.Count <= maxCards)
            {
                cardPosition.X = column / 10;
            }
            foreach (Boton btn in cartasBtn)
            {
                btn.X = cardPosition.X;
                btn.Y = cardPosition.Y;
                cardPosition.X += column;
            }
            cardPosition.X -= column * cartasBtn.Count;
        }
        public void moveCards(bool avanzar)
        {
            if (avanzar)
            {
                if (cartasBtn[cartasBtn.Count - 1].X > ScreenWidth)
                {
                    foreach(Boton card in cartasBtn)
                    {
                        card.X -= column;
                    }
                    btnSelectedCard.X -= column;
                    cardPosition.X -= column;
                }
            }
            else
            {
                if (cartasBtn[0].X < 0)
                {
                    foreach (Boton card in cartasBtn)
                    {
                        card.X += column;
                    }
                    btnSelectedCard.X += column;
                    cardPosition.X += column;
                }
            }
        }
        public void dibujarBotones()
        {
            game.spriteBatch.Draw(btnPlay.Img, position: new Vector2(btnPlay.X, btnPlay.Y), scale: btnPlay.Scale);
            game.spriteBatch.Draw(btnPass.Img, position: new Vector2(btnPass.X, btnPass.Y), scale: btnPass.Scale);
            if (cartasBtn[0].X < 0)
            {
                game.spriteBatch.Draw(btnPreviousCard.Img, position: new Vector2(btnPreviousCard.X, btnPreviousCard.Y), scale: btnPreviousCard.Scale);
            }
            if (cartasBtn[cartasBtn.Count - 1].X > ScreenWidth)
            {
                game.spriteBatch.Draw(btnNextCard.Img, position: new Vector2(btnNextCard.X, btnNextCard.Y), scale: btnNextCard.Scale);
            }
        }
        public void dibujarCartas()
        {
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
        public void sendCard()
        {
            serverWaiting = false;
            switch (data.Cartas[selectedCard].Tipo)
            {
                case Carta.eTipo.Numero:
                    if (data.Sentido && data.ValorMesa + data.Cartas[selectedCard].Valor >= 10 ||
                    !data.Sentido && data.ValorMesa - data.Cartas[selectedCard].Valor <= -10)
                    {
                        game.efectos[Game1.eSonidos.overCount].Play();
                    }
                    else
                    {
                        game.efectos[Game1.eSonidos.play].Play();
                    }
                    break;
                case Carta.eTipo.Sentido:
                    game.efectos[Game1.eSonidos.changeWay].Play();
                    break;
                case Carta.eTipo.Efecto:
                    game.efectos[Game1.eSonidos.forCards].Play();
                    break;
            }
            server.enviarDatos("jugar");
            server.enviarDatos(data.Cartas[selectedCard].Tipo.ToString());
            server.enviarDatos(data.Cartas[selectedCard].Valor.ToString());
            server.enviarDatos(data.Cartas[selectedCard].Sentido.ToString());
            selectedCard = -1;
        }
        public void skipTurn()
        {
            serverWaiting = false;
            game.efectos[Game1.eSonidos.overCount].Play();
            server.enviarDatos("pasar");
            selectedCard = -1;
        }
        public Pantalla Click()
        {
            if (data.Turno == name)
            {
                if (btnPass.isHover(Mouse.GetState().X, Mouse.GetState().Y) && serverWaiting)
                {
                    skipTurn();
                }
                if (btnPlay.isHover(Mouse.GetState().X, Mouse.GetState().Y) && selectedCard != -1 && serverWaiting)
                {
                    sendCard();
                }
            }
            if (btnNextCard.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                moveCards(true);
            }
            if (btnPreviousCard.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                moveCards(false);
            }
            foreach (Boton btn in cartasBtn)
            {
                if (btn.isHover(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    game.efectos[Game1.eSonidos.click].Play();
                    selectedCard = cartasBtn.IndexOf(btn);
                    btnSelectedCard.X = btn.X-column/10;
                    btnSelectedCard.Y = (btn.Y+btn.Height/2)-btnSelectedCard.Height/2;
                }
            }
            return this;
        }

        public Pantalla KeyboardAction(Keys key)
        {
            switch (key)
            {
                case Keys.Tab:
                case Keys.Right:
                    if (selectedCard != -1)
                    {
                        moveFocus(true);
                    }
                    else
                    {
                        changeFocus(0);
                    }
                    break;
                case Keys.Left:
                    if (selectedCard != -1)
                    {
                        moveFocus(false);
                    }
                    else
                    {
                        changeFocus(0);
                    }
                    break;
                case Keys.Enter:
                    if(data.Turno == name && selectedCard != -1 && serverWaiting)
                    {
                        sendCard();
                    }
                    break;
                case Keys.Space:
                    if (data.Turno == name && serverWaiting)
                    {
                        skipTurn();
                    }
                    break;
                default:
                    break;
            }
            return this;
        }
        public void moveFocus(bool right)
        {
            foreach(Boton card in cartasBtn)
            {
                if (card == cartasBtn[selectedCard])
                {
                    if (right)
                    {
                        selectedCard = selectedCard + 1 >= cartasBtn.Count ? 0 : selectedCard + 1;
                    }
                    else
                    {
                        selectedCard = selectedCard - 1 < 0 ? cartasBtn.Count - 1 : selectedCard - 1;
                    }
                    while (cartasBtn[selectedCard].X < 0)
                    {
                        moveCards(false);
                    }
                    while (cartasBtn[selectedCard].X > ScreenWidth)
                    {
                        moveCards(true);
                    }
                    break;
                }
            }
            changeFocus(selectedCard);
        }
        public void changeFocus(int card)
        {
            selectedCard = card;
            btnSelectedCard.X = cartasBtn[selectedCard].X - column / 10;
            btnSelectedCard.Y = (cartasBtn[selectedCard].Y + cartasBtn[selectedCard].Height / 2) - btnSelectedCard.Height / 2;
        }
        public void onExiting(object sender, EventArgs args)
        {
            server.closeServer();
            hiloConectividad.Join();
        }
    }
}
