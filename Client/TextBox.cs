using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client
{
    class TextBox : Boton
    {
        public bool Focus { get; set; }
        public Texture2D Selected { get; set; }
        public Texture2D NotSelected { get; set; }
        public TextBox(float x,float y,Texture2D selected,Texture2D notSelected,float width,SpriteFont font,bool focus):base(x,y,notSelected, width)
        {
            Selected = selected;
            NotSelected = notSelected;
            Focus = focus;
            Font = font;
            Text = "";
        }
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
