namespace Client
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    class Boton
    {
        /// <summary>
        /// Indica la coordenada X donde se posiciona el Boton.
        /// </summary>
        public float X {get; set;}
        /// <summary>
        /// Indica la coordenada Y donde se posiciona el Boton.
        /// </summary>
        public float Y { get; set; }
        /// <summary>
        /// Indica la imagen que representará visualmente al Boton.
        /// </summary>
        public Texture2D Img { get; set; }
        /// <summary>
        /// Indica la anchura del Boton.
        /// </summary>
        public float Width { get; set; }
        /// <summary>
        /// Indica la altura del Boton.
        /// </summary>
        public float Height { get; set; }
        /// <summary>
        /// Establece la escala a la que se redimensiona la imagen para adaptarse a las dimensiones del Boton.
        /// </summary>
        public Vector2 Scale { get; set; }
        /// <summary>
        /// Fuente con la que se rasteriza el posible texto del Boton.
        /// </summary>
        public SpriteFont Font { get; set; }
        /// <summary>
        /// Texto que se mostrará en el interior del Boton.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Constructor de un Boton sin texto, solo imagen.
        /// Se encarga también de inicializar el resto de propiedades.
        /// </summary>
        /// <param name="x">Coordenada X</param>
        /// <param name="y">Coordenada Y</param>
        /// <param name="image">Imagen del Boton</param>
        /// <param name="width">Ancho del Boton</param>
        public Boton(float x, float y, Texture2D image, float width)
        {
            Img = image;
            Width = width;
            Scale = new Vector2((float)width / image.Width, (float)width / image.Width);
            Height = image.Height * width / image.Width;
            X = x;
            Y = y;
        }
        /// <summary>
        /// Constructor de un Boton con imagen y texto.
        /// </summary>
        /// <param name="x">Coordenada X.</param>
        /// <param name="y">Coordenada Y.</param>
        /// <param name="image">Imagen del Boton.</param>
        /// <param name="width">Ancho del Boton.</param>
        /// <param name="font">Fuente del texto.</param>
        /// <param name="texto">Texto del Boton.</param>
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
        /// <summary>
        /// Se encarga de dibujar el Boton sobre la pantalla pasada como parámetro.
        /// </summary>
        /// <param name="game">Hace referecia al juego para obtener su pantalla.</param>
        public virtual void draw(MainGame game)
        {
            game.SpriteBatch.Draw(this.Img, new Vector2(this.X, this.Y), scale: this.Scale);
            if(Font!=null)
            game.SpriteBatch.DrawString(Font,Text,new Vector2(this.X+Width/2-Font.MeasureString(Text).X/2,this.Y+Height/2-Font.MeasureString(Text).Y/2),Color.Black);
        }
        /// <summary>
        /// Comprueba a partir de dos coordenadas dadas si se encuentran dentro del área del Boton o no.
        /// </summary>
        /// <param name="x">Coordeana X a comprobar</param>
        /// <param name="y">Coordenada Y a comprobar</param>
        /// <returns>True si está dentro</returns>
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
