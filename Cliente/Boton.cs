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
        public Texture2D Imagen { get; set; }
        public float Ancho { get; set; }
        public float Alto { get; set; }
        public Vector2 Escala { get; set; }
        public Boton(float x, float y,Texture2D imagen,float ancho)
        {
            Imagen = imagen;
            Ancho = ancho;
            Escala = new Vector2((float)ancho / imagen.Width, (float)ancho / imagen.Width);
            Alto = imagen.Height * ancho / imagen.Width;
            X = x;
            Y = y;
        }
        public bool click(float x, float y)
        {
            if(x>=X && x<=X+Ancho && y >= Y && y <= Y + Alto)
            {
                return true;
            }
            return false;
        }
    }
}
