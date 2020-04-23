using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Client
{
    class EndGame : Screen
    {
        private int Rank { get; set; }
        private MainGame Game { get; set; }
        private Boton BtnBack { get; set; }
        private SpriteFont Font { get; set; }
        private SpriteFont ErrorFont { get; set; }
        private float ScreenWidth { get; set; }
        private float ScreenHeight { get; set; }
        private string RankMessage { get; set; }
        private Texture2D BtnSelected { get; set; }
        private Texture2D BtnDefault { get; set; }
        public EndGame(MainGame game,int rank)
        {
            this.Game = game;
            Rank = rank;
        }
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

        public void Initialize()
        {
            ScreenWidth = Game.GraphicsDevice.Viewport.Width;
            ScreenHeight = Game.GraphicsDevice.Viewport.Height;
            RankMessage = "You are the number " + Rank;
        }

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

        public Screen Click()
        {
            if (BtnBack.isHover(Mouse.GetState().X, Mouse.GetState().Y))
            {
                Game.effects[MainGame.eSounds.click].Play();
                return new StartScreen(Game);
            }
            return this;
        }

        public Screen KeyboardAction(Keys key)
        {
            if (key == Keys.Enter)
            {
                Game.effects[MainGame.eSounds.click].Play();
                return new StartScreen(Game);
            }
            return this;
        }

        public void onExiting(object sender, EventArgs args)
        {

        }
    }
}
