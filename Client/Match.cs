using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Client
{
    class Match : Screen
    {
        private MainGame Game { get; set; }
        private Server Server { get; set; }

        private Thread ConnectThread;
        private string Name { get; set; }
        private bool Playing { get; set; }
        private int Rank { get; set; }
        private bool ServerDown { get; set; }

        //Imagenes cartas
        private Dictionary<string, Texture2D> ImgCards;
        //Cartas jugador
        private List<Boton> BtnCards;
        private Boton BtnSelectedCard { get; set; }
        //Boton jugar y botón pasar
        private Boton BtnPlay { get; set; }
        private Boton BtnPass { get; set; }
        private Boton BtnNextCard { get; set; }
        private Boton BtnPreviousCard { get; set; }
        private int SelectedCard { get; set; }
        private bool ServerWaiting { get; set; }
        //Definen el ancho y alto de la ventana
        private float ScreenWidth { get; set; }
        private float ScreenHeight { get; set; }
        //Define el ancho que tendrá cada columna contenedora de una carta
        private float Column { get; set; }

        //Vector que define la posición del valor de mesa
        private SpriteFont DefaultFont { get; set; }
        private SpriteFont FontValue { get; set; }

        //Maximo numero de cartas que se muestran a la vez
        private int MaxCards { get; set; }
        private Data ActualData { get; set; }

        //Texture2D que nos permite tener siempre acceso a una carta
        private Texture2D ExampleCard { get; set; }
        //Vectores que definen la posicion y reescalado de las cartas del jugador
        private Vector2 CardPosition;
        public Match(MainGame game,Server server,string name,int players)
        {
            Game = game;
            Server = server;
            Name = name;
            Playing = true;
            ServerDown = false;
            BtnCards = new List<Boton>();
            ImgCards = new Dictionary<string, Texture2D>();
            ActualData = null;
        }
        public void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin();
            if (ActualData != null)
            {
                drawTable();
                drawPlayers();
                drawTurn();
                drawButtons();
                if (SelectedCard != -1)
                {
                    BtnSelectedCard.draw(Game);
                }
                drawCards();
            }
            Game.SpriteBatch.End();
        }   
        public void Initialize()
        {
            SelectedCard = -1;
            MaxCards = 8;
            Rank = 0;
            //Permitimos ver el cursor en la centana
            ScreenHeight = Game.Graphics.GraphicsDevice.Viewport.Height;
            ScreenWidth = Game.Graphics.GraphicsDevice.Viewport.Width;
            Column = ScreenWidth / MaxCards;
            ServerWaiting = true;
        }
        public void LoadContent()
        {
            for (int i = 3; i < 8; i++)
            {
                ImgCards.Add(i.ToString(), Game.Content.Load<Texture2D>("Sprites/" + i));
            }
            ImgCards.Add("bucle", Game.Content.Load<Texture2D>("Sprites/bucle"));
            ImgCards.Add("minus", Game.Content.Load<Texture2D>("Sprites/minus"));
            ImgCards.Add("plus", Game.Content.Load<Texture2D>("Sprites/plus"));
            ImgCards.Add("selectedCard", Game.Content.Load<Texture2D>("Sprites/selectedCard"));
            //Cargamos la fuente del juego
            DefaultFont = Game.Content.Load<SpriteFont>("Fuentes/Fuente");
            FontValue = Game.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            ExampleCard = ImgCards["3"];
            CardPosition = new Vector2(Column / 10, ScreenHeight * 7 / 8 - ExampleCard.Height / 2);
            BtnPlay = new Boton(ScreenWidth / 2 - ScreenWidth / 8, ScreenHeight / 2, Game.Content.Load<Texture2D>("Sprites/btnJugar"), ScreenWidth / 8);
            BtnPass = new Boton(ScreenWidth / 2, ScreenHeight / 2, Game.Content.Load<Texture2D>("Sprites/btnPasar"), ScreenWidth / 8);
            BtnNextCard = new Boton(ScreenWidth - ScreenWidth / 15, ScreenHeight - ScreenWidth / 15, Game.Content.Load<Texture2D>("Sprites/btnNextCard"), ScreenWidth / 15);
            BtnPreviousCard = new Boton(0, ScreenHeight - ScreenWidth / 15, Game.Content.Load<Texture2D>("Sprites/btnPreviousCard"), ScreenWidth / 15);
            BtnSelectedCard = new Boton(0, 0, ImgCards["selectedCard"], Column);
            ConnectThread = new Thread(() => refreshData());
            ConnectThread.Start();
        }
        public Screen Update(GameTime gameTime)
        {
            if (!Playing)
            {
                Server.closeServer();
                ConnectThread.Join();
                return new EndGame(Game, Rank);
            }
            if (ServerDown)
            {
                Server.closeServer();
                ConnectThread.Join();
                return new StartScreen(Game, "Server's connection lost");
            }
            //Si la ventana del juego cambia sus dimensiones, se adaptan los tamaños de los objetos
            if (Game.Graphics.GraphicsDevice.Viewport.Width != ScreenWidth || Game.Graphics.GraphicsDevice.Viewport.Height != ScreenHeight)
            {
                ScreenHeight = Game.Graphics.GraphicsDevice.Viewport.Height;
                ScreenWidth = Game.Graphics.GraphicsDevice.Viewport.Width;
                Column = ScreenWidth / MaxCards;
            }
            centerCards();
            return this;
        }
        public void centerCards()
        {
            if (BtnCards.Count <= MaxCards)
            {
                CardPosition.X = Column / 10;
            }
        }
        public void refreshData()
        {
            try
            {
                while (Playing)
                {
                    string aux = Server.getData();
                    if (aux == "finPartida")
                    {
                        Rank = Convert.ToInt32(Server.getData());
                        Playing = false;
                        return;
                    }
                    int numCards = Convert.ToInt32(aux);
                    List<Card> auxList = new List<Card>();
                    for (int i = 0; i < numCards; i++)
                    {
                        auxList.Add(
                            new Card(
                                (Card.eType)Enum.Parse(typeof(Card.eType),Server.getData()),
                                Convert.ToInt32(Server.getData()),
                                Convert.ToBoolean(Server.getData())
                                )
                            );
                    }
                    int auxValor = Convert.ToInt32(Server.getData());
                    bool auxSentido = Convert.ToBoolean(Server.getData());
                    string auxTurno = Server.getData();
                    Dictionary<string, int> auxDic = new Dictionary<string, int>();
                    int numJugadores = Convert.ToInt32(Server.getData());
                    for (int i = 0; i < numJugadores; i++)
                    {
                        auxDic.Add(Server.getData(), Convert.ToInt32(Server.getData()));
                    }
                    ActualData = new Data(auxList, auxValor, auxSentido, auxTurno, auxDic);
                    refreshDeck();
                    ServerWaiting = true;
                }
            }catch(IOException)
            {
                ServerDown = true;
            }
        }
        public void refreshDeck()
        {
            BtnCards.Clear();
            foreach (Card card in ActualData.Cards)
            {
                string name = "";
                switch (card.Type)
                {
                    case Card.eType.Number:
                        name = card.Value.ToString();
                        break;
                    case Card.eType.Way:
                        if (card.Way)
                        {
                            name = "plus";
                        }
                        else
                        {
                            name = "minus";
                        }
                        break;
                    case Card.eType.Effect:
                        name = "bucle";
                        break;
                }
                BtnCards.Add(new Boton(0, 0, ImgCards[name], Column * 8 / 10));
            }
            if (BtnCards.Count <= MaxCards)
            {
                CardPosition.X = Column / 10;
            }
            foreach (Boton btn in BtnCards)
            {
                btn.X = CardPosition.X;
                btn.Y = CardPosition.Y;
                CardPosition.X += Column;
            }
            CardPosition.X -= Column * BtnCards.Count;
        }
        public void moveCards(bool advance)
        {
            if (advance)
            {
                if (BtnCards[BtnCards.Count - 1].X > ScreenWidth)
                {
                    foreach(Boton card in BtnCards)
                    {
                        card.X -= Column;
                    }
                    BtnSelectedCard.X -= Column;
                    CardPosition.X -= Column;
                }
            }
            else
            {
                if (BtnCards[0].X < 0)
                {
                    foreach (Boton card in BtnCards)
                    {
                        card.X += Column;
                    }
                    BtnSelectedCard.X += Column;
                    CardPosition.X += Column;
                }
            }
        }
        public void drawButtons()
        {
            Game.SpriteBatch.Draw(BtnPlay.Img, position: new Vector2(BtnPlay.X, BtnPlay.Y), scale: BtnPlay.Scale);
            Game.SpriteBatch.Draw(BtnPass.Img, position: new Vector2(BtnPass.X, BtnPass.Y), scale: BtnPass.Scale);
            if (BtnCards[0].X < 0)
            {
                Game.SpriteBatch.Draw(BtnPreviousCard.Img, position: new Vector2(BtnPreviousCard.X, BtnPreviousCard.Y), scale: BtnPreviousCard.Scale);
            }
            if (BtnCards[BtnCards.Count - 1].X > ScreenWidth)
            {
                Game.SpriteBatch.Draw(BtnNextCard.Img, position: new Vector2(BtnNextCard.X, BtnNextCard.Y), scale: BtnNextCard.Scale);
            }
        }
        public void drawCards()
        {
            //Dibujado de las cartas
            foreach (Boton btn in BtnCards)
            {
                //Dibujamos la carta
                Game.SpriteBatch.Draw(btn.Img, position: new Vector2(btn.X, btn.Y), scale: btn.Scale);
            }
        }
        public void drawTable()
        {
            //Dibujamos el valor y sentido de mesa
            Game.SpriteBatch.DrawString(
                FontValue,
                ActualData.TableValue.ToString(),
                new Vector2(ScreenWidth / 2, ScreenHeight / 4) - FontValue.MeasureString(ActualData.TableValue.ToString()) / 2,
                Color.White
                );
            string wayName = ActualData.Way ? "Adding +" : "Substracting -";
            Game.SpriteBatch.DrawString(
                FontValue,
                wayName,
                new Vector2(ScreenWidth/2- FontValue.MeasureString(wayName).X / 2,BtnPlay.Y - FontValue.MeasureString(wayName).Y),
                Color.White);
        }
        public void drawTurn()
        {
            Vector2 turnPosition = new Vector2(ScreenWidth - DefaultFont.MeasureString("Turn: " + ActualData.Turn).X, 0);
            Game.SpriteBatch.DrawString(DefaultFont, "Turn: " + ActualData.Turn, turnPosition, Color.White);
        }
        public void drawPlayers()
        {
            Vector2 playersPosition = new Vector2(Column / 10, Column / 10);
            foreach (var jugador in ActualData.Players)
            {
                Game.SpriteBatch.DrawString(DefaultFont, jugador.Key + ": " + jugador.Value, playersPosition, Color.White);
                playersPosition.Y += DefaultFont.MeasureString(jugador.Key + ": " + jugador.Value).Y;
            }
        }
        public void sendCard()
        {
            ServerWaiting = false;
            switch (ActualData.Cards[SelectedCard].Type)
            {
                case Card.eType.Number:
                    if (ActualData.Way && ActualData.TableValue + ActualData.Cards[SelectedCard].Value >= 10 ||
                    !ActualData.Way && ActualData.TableValue - ActualData.Cards[SelectedCard].Value <= -10)
                    {
                        Game.effects[MainGame.eSounds.overCount].Play();
                    }
                    else
                    {
                        Game.effects[MainGame.eSounds.play].Play();
                    }
                    break;
                case Card.eType.Way:
                    Game.effects[MainGame.eSounds.changeWay].Play();
                    break;
                case Card.eType.Effect:
                    Game.effects[MainGame.eSounds.forCards].Play();
                    break;
            }
            Server.sendData("jugar");
            Server.sendData(ActualData.Cards[SelectedCard].Type.ToString());
            Server.sendData(ActualData.Cards[SelectedCard].Value.ToString());
            Server.sendData(ActualData.Cards[SelectedCard].Way.ToString());
            SelectedCard = -1;
        }
        public void skipTurn()
        {
            ServerWaiting = false;
            Game.effects[MainGame.eSounds.overCount].Play();
            Server.sendData("pasar");
            SelectedCard = -1;
        }
        public Screen Click()
        {
            if (ActualData.Turn == Name)
            {
                if (BtnPass.isHover(Mouse.GetState().X, Mouse.GetState().Y) && ServerWaiting)
                {
                    skipTurn();
                }
                if (BtnPlay.isHover(Mouse.GetState().X, Mouse.GetState().Y) && SelectedCard != -1 && ServerWaiting)
                {
                    sendCard();
                }
            }
            if (BtnNextCard.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                moveCards(true);
            }
            if (BtnPreviousCard.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                moveCards(false);
            }
            foreach (Boton btn in BtnCards)
            {
                if (btn.isHover(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    Game.effects[MainGame.eSounds.click].Play();
                    SelectedCard = BtnCards.IndexOf(btn);
                    BtnSelectedCard.X = btn.X-Column/10;
                    BtnSelectedCard.Y = (btn.Y+btn.Height/2)-BtnSelectedCard.Height/2;
                }
            }
            return this;
        }
        public Screen KeyboardAction(Keys key)
        {
            switch (key)
            {
                case Keys.Tab:
                case Keys.Right:
                    if (SelectedCard != -1)
                    {
                        moveFocus(true);
                    }
                    else
                    {
                        changeFocus(0);
                    }
                    break;
                case Keys.Left:
                    if (SelectedCard != -1)
                    {
                        moveFocus(false);
                    }
                    else
                    {
                        changeFocus(0);
                    }
                    break;
                case Keys.Enter:
                    if(ActualData.Turn == Name && SelectedCard != -1 && ServerWaiting)
                    {
                        sendCard();
                    }
                    break;
                case Keys.Space:
                    if (ActualData.Turn == Name && ServerWaiting)
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
            foreach(Boton card in BtnCards)
            {
                if (card == BtnCards[SelectedCard])
                {
                    if (right)
                    {
                        SelectedCard = SelectedCard + 1 >= BtnCards.Count ? 0 : SelectedCard + 1;
                    }
                    else
                    {
                        SelectedCard = SelectedCard - 1 < 0 ? BtnCards.Count - 1 : SelectedCard - 1;
                    }
                    while (BtnCards[SelectedCard].X < 0)
                    {
                        moveCards(false);
                    }
                    while (BtnCards[SelectedCard].X > ScreenWidth)
                    {
                        moveCards(true);
                    }
                    break;
                }
            }
            changeFocus(SelectedCard);
        }
        public void changeFocus(int card)
        {
            SelectedCard = card;
            BtnSelectedCard.X = BtnCards[SelectedCard].X - Column / 10;
            BtnSelectedCard.Y = (BtnCards[SelectedCard].Y + BtnCards[SelectedCard].Height / 2) - BtnSelectedCard.Height / 2;
        }
        public void onExiting(object sender, EventArgs args)
        {
            Server.closeServer();
            ConnectThread.Join();
        }
    }
}
