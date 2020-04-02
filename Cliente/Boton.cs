using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    class Boton
    {
        public float X {get; set;}
        public float Y { get; set; }
        public Texture2D Img { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2 Scale { get; set; }
        public Boton(float x, float y,Texture2D image,float width)
        {
            Img = image;
            Width = width;
            Scale = new Vector2((float)width / image.Width, (float)width / image.Width);
            Height = image.Height * width / image.Width;
            X = x;
            Y = y;
        }
        public virtual void draw(Game1 game)
        {
            game.spriteBatch.Draw(this.Img, new Vector2(this.X, this.Y), scale: this.Scale);
        }
        public bool click(float x, float y)
        {
            if(x>=X && x<=X+Width && y >= Y && y <= Y + Height)
            {
                return true;
            }
            return false;
        }
    }
}
