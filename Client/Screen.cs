using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Client
{
    interface Screen
    {
        /// <summary>
        /// Inicializa todas las propiedades y variables de la clase.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Carga el contenido necesario en memoria.
        /// </summary>
        void LoadContent();
        /// <summary>
        /// Se encarga del refresco de pantalla. Se realiza 60 veces por segundo.
        /// </summary>
        /// <param name="gameTime">Valor temporal interno.</param>
        /// <returns></returns>
        Screen Update(GameTime gameTime);
        /// <summary>
        /// Dibuja todos los elementos de la pantalla.
        /// </summary>
        /// <param name="gameTime">Valor temporal interno.</param>
        void Draw(GameTime gameTime);
        /// <summary>
        /// Gestiona los clicks del ratón del usuario.
        /// </summary>
        /// <returns>Devuelve un objeto tipo Screen según las acciones del usuario.</returns>
        Screen Click();
        /// <summary>
        /// Gestiona las entradas por teclado del usuario.
        /// </summary>
        /// <param name="key">Tecla pulsada por el usuario.</param>
        /// <returns>Devuelve un objeto tipo Screen en función de las acciones del usuario.</returns>
        Screen KeyboardAction(Keys key);
        /// <summary>
        /// Se ejecuta al cerrar la aplicación.
        /// Se encarga de cerrar posibles sockets abiertos, hilos y demás procesos que no han finalizado ni terminado de forma natural.
        /// </summary>
        void onExiting();
    }
}
