using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client
{
    class TextBox : Boton
    {
        /// <summary>
        /// Indica si la caja tiene el focus en ella.
        /// </summary>
        public bool Focus { get; set; }
        /// <summary>
        /// Imagen que indica que la caja mantiene el focus en ella.
        /// </summary>
        public Texture2D Selected { get; set; }
        /// <summary>
        /// Imagen por defecto de la caja de texto.
        /// </summary>
        public Texture2D NotSelected { get; set; }
        /// <summary>
        /// Constructor de TextBox.
        /// </summary>
        /// <param name="x">Coordenada X.</param>
        /// <param name="y">Coordenada Y.</param>
        /// <param name="selected">Imagen seleccionada.</param>
        /// <param name="notSelected">Imagen para no seleccionada.</param>
        /// <param name="width">Ancho del Boton.</param>
        /// <param name="font">Fuente del texto.</param>
        /// <param name="focus">Indica si tiene el foco en la pantalla.</param>
        public TextBox(float x,float y,Texture2D selected,Texture2D notSelected,float width,SpriteFont font,bool focus):base(x,y,notSelected, width)
        {
            Selected = selected;
            NotSelected = notSelected;
            Focus = focus;
            Font = font;
            Text = "";
        }
        /// <summary>
        /// Dibuja la caja de texto en pantalla.
        /// </summary>
        /// <param name="game">Base de la aplicación.</param>
        public override void draw(MainGame game)
        {
            this.Img = Focus ? Selected : NotSelected;
            base.draw(game);
            game.SpriteBatch.DrawString(
                Font,
                Text,
                new Vector2(this.X + this.Width / 2 - Font.MeasureString(Text).X / 2, this.Y),
                Color.Black
            );
        }
    }
}
