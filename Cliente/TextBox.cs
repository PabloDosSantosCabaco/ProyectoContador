using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Cliente
{
    class TextBox : Boton
    {
        public string Text { get; set; }
        public bool Focus { get; set; }
        public Texture2D Selected { get; set; }
        public Texture2D NotSelected { get; set; }
        public SpriteFont Font { get; set; }
        public TextBox(float x,float y,Texture2D selected,Texture2D notSelected,float width,SpriteFont font,bool focus):base(x,y,notSelected, width)
        {
            Selected = selected;
            NotSelected = notSelected;
            Focus = focus;
            Font = font;
            Text = "";
        }
        public override void draw(Game1 game)
        {
            this.Img = Focus ? Selected : NotSelected;
            base.draw(game);
            game.spriteBatch.DrawString(
                Font,
                Text,
                new Vector2(this.X + this.Width / 2 - Font.MeasureString(Text).X / 2, this.Y),
                Color.Black
            );
        }
    }
}
