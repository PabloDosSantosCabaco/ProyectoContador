using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    class PantallaCrear : Pantalla
    {
        Game1 juego;
        Texture2D imgEmpezar;
        Boton btnEmpezar;
        Texture2D input;
        string intro;
        string nombre;
        string msnError;
        SpriteFont fuente;
        SpriteFont errorFuente;
        public int AnchoPantalla { get; set; }
        public int AltoPantalla { get; set; }
        Dictionary<Keys, bool> teclas = new Dictionary<Keys, bool>();
        bool error;
        bool ratonPresionado;
        public PantallaCrear(Game1 juego)
        {
            this.juego = juego;
        }
        public void Draw(GameTime gameTime)
        {
            juego.spriteBatch.Begin();
            
            juego.spriteBatch.DrawString(fuente, intro, new Vector2(AnchoPantalla / 2 - fuente.MeasureString(intro).X / 2, AltoPantalla / 4), Color.Black);
            juego.spriteBatch.Draw(
                input,
                position: new Vector2(AnchoPantalla / 2 - AnchoPantalla * 3 / 8, AltoPantalla/2),
                scale: new Vector2((float)AnchoPantalla * 3 / 4 / input.Width, (float)AnchoPantalla * 3 / 4 / input.Width)
                );
            juego.spriteBatch.Draw(
                btnEmpezar.Imagen,
                position: new Vector2(btnEmpezar.X,btnEmpezar.Y),
                scale: btnEmpezar.Escala
                );
            juego.spriteBatch.DrawString(fuente, nombre, new Vector2(AnchoPantalla / 2 - fuente.MeasureString(nombre).X / 2, AltoPantalla /2), Color.Black);
            if (error)
            {
                juego.spriteBatch.DrawString(errorFuente, msnError, new Vector2(AnchoPantalla/2-errorFuente.MeasureString(msnError).X/2, AltoPantalla * 7/10), Color.Red);

            }
            juego.spriteBatch.End();
        }

        public void Initialize()
        {
            AnchoPantalla = juego.graphics.GraphicsDevice.Viewport.Width;
            AltoPantalla = juego.graphics.GraphicsDevice.Viewport.Height;
            intro = "Introduce tu nombre:";
            nombre = "";
            error = false;
            ratonPresionado = false;
            msnError = "Introduce un nombre de al menos 3 caracteres";
        }

        public void LoadContent()
        {
            input = juego.Content.Load<Texture2D>("Sprites/textBox");
            fuente = juego.Content.Load<SpriteFont>("Fuentes/FuenteValor");
            imgEmpezar = juego.Content.Load<Texture2D>("Sprites/btnCrear");
            errorFuente = juego.Content.Load<SpriteFont>("Fuentes/Error");
            btnEmpezar = new Boton(AnchoPantalla / 2 - AnchoPantalla / 3 / 2, AltoPantalla * 3 / 4, imgEmpezar, AnchoPantalla / 3);
        }
        
        public Pantalla Update(GameTime gameTime)
        {
            //Comprobación de introducción de texto
            for (int i = 0; i < Keyboard.GetState().GetPressedKeys().Length; i++)
            {
                if (!teclas.ContainsKey(Keyboard.GetState().GetPressedKeys()[i]) || !teclas[Keyboard.GetState().GetPressedKeys()[i]])
                {
                    if (Keyboard.GetState().GetPressedKeys()[i] >= Keys.A && Keyboard.GetState().GetPressedKeys()[i] <= Keys.Z)
                    {
                        nombre = compruebaNombre(Keyboard.GetState().GetPressedKeys()[i].ToString());
                    }
                    else if (Keyboard.GetState().GetPressedKeys()[i] == Keys.Space)
                    {
                        nombre = compruebaNombre(" ");
                    }
                    if (Keyboard.GetState().GetPressedKeys()[i] == Keys.Back && nombre.Length >= 1)
                    {
                        nombre = nombre.Remove(nombre.Length - 1);
                    }
                }
                teclas[Keyboard.GetState().GetPressedKeys()[i]] = true;
            }
            foreach(var tecla in teclas.Keys.ToList())
            {
                if (!Keyboard.GetState().GetPressedKeys().Contains(tecla) && teclas[tecla])
                {
                    teclas[tecla] = false;
                }
            }
            //Comprobación de uso de ratón
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                ratonPresionado = true;
            }
            if (ratonPresionado && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (btnEmpezar.click(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    if (nombre.Trim().Length < 3)
                    {
                        error = true;
                    }
                    else
                    {
                        //Pedir numero sala al servidor
                        Servidor servidor = new Servidor();
                        return new SalaEspera(juego,servidor.getSala(nombre),nombre,servidor,true);
                    }  
                }
                ratonPresionado = false;
            }

            return this;
        }
        public string compruebaNombre(string nuevo)
        {
            if (fuente.MeasureString(nombre + nuevo).X < AnchoPantalla * 3 / 4)
            {
                return nombre + nuevo;
            }
            return nombre;
        }
    }
}
