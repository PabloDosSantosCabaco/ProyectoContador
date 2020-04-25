namespace Client
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    class Boton
    {
        public float X {get; set;}
        public float Y { get; set; }
        public Texture2D Img { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Boton(float x, float y, Texture2D image, float width)
        {
            Img = image;
            Width = width;
            Scale = new Vector2((float)width / image.Width, (float)width / image.Width);
            Height = image.Height * width / image.Width;
            X = x;
            Y = y;
        }
        public Boton(float x, float y, Texture2D image, float width,SpriteFont font,string texto)
        {
            Img = image;
            Width = width;
            Scale = new Vector2((float)width / image.Width, (float)width / image.Width);
            Height = image.Height * width / image.Width;
            X = x;
            Y = y;
            Font = font;
            Text = texto;
        }
        public virtual void draw(MainGame game)
        {
            game.SpriteBatch.Draw(this.Img, new Vector2(this.X, this.Y), scale: this.Scale);
            if(Font!=null)
            game.SpriteBatch.DrawString(Font,Text,new Vector2(this.X+Width/2-Font.MeasureString(Text).X/2,this.Y+Height/2-Font.MeasureString(Text).Y/2),Color.Black);
        }
        public bool isHover(float x, float y)
        {
            if(x>=X && x<=X+Width && y >= Y && y <= Y + Height)
            {
                return true;
            }
            return false;
        }
    }
}
