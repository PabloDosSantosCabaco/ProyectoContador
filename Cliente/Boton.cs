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
        public Boton(float x, float y,Texture2D imagen,float ancho,float alto)
        {
            X = x;
            Y = y;
            Imagen = imagen;
            Ancho = ancho;
            Alto = alto;
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
