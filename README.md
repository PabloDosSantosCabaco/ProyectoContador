# ProyectoContador
## 19/03/2020
#### Nuevo
* Creación de los proyectos **Cliente** y **ServidorContador**.
* Creación de clases **Sala** y **Cliente** en **ServidorContador**.
* Desarrollo de gestión de clientes: creación y unión a salas.
* Ampliación de las clases **Sala** y **Cliente^**. Gestión de la clase sala mediante hilos.
***
## 20/03/2020
#### Nuevo
* Redacción del anteproyecto.
***
## 21/03/2020
#### Nuevo
* Pruebas de dibujado con MonoGame.
* Creación de la clase **Carta**.
* Creación de sprites de la baraja de cartas.
#### Modificaciones
* Corrección del diagrama de Gantt.
***
## 22/03/2020
#### Nuevo
* Conseguida disposición del contenido  de la mesa de cartas de cada usuario.
* Redimensión del contenido en función de las dimensiones de la ventana.
***
## 23/03/2020
#### Nuevo
* Dibujado del valor, sentido, turno y marcadores de la partida.
* Gestión mediante hilos de las diferentes partidas.
* Creación de fuentes para escribir en MonoGame.
#### Avances
* Los datos como nº de jugadores por partida, turno, valor de mesa y cartas de cada jugador se pasan como único objeto de clase **PaqueteTurno**, que es enviado a cada jugador después de cada turno.
* Existe duplicada la clase **Carta** en ambos proyectos ya que es necesaria en ambos. Se plantea la creación de una librería de clases en caso de necesitar más clases compartidas. Para una sola tan simple, se ha dejado duplicada.
***
## 25/03/2020
#### Nuevo
* Creación de la clase **Partida** que contendrá los datos de una partida, como el *valor de mesa*, *sentido* del contador, *jugadores* de la sala, *turno* del jugador actual y *límite* que alcanza el contador.
* Duplicado de la clase **PaqueteTurno**. Aún no se ha creado la biblioteca de clases ya que no está claro como se intercambiarán datos entre el cliente y el servidor, si como objetos o propiedad a propiedad.
* Programada modificación de barajas de jugadores en función de qué carta haya sido jugada en un turno. La aplicación cuenta con una base ya funcional de cómo será el juego.
* Las cartas se generan de forma aleatoria con un porcetanje de probabilidad en cuanto al **tipo** de carta.
* Creación de una función en la clase **Cliente** que envíe la carta jugada al servidor.
#### Modificaciones
* La lista de clientes de la clase **Sala** pasa a ser un *Diccionario* para mayor facilidad a la hora de encontrar jugadores e información sobre ellos.
* **Carta Efecto** modificada. Ya no suma tantas cartas como jugadores existan en ese momento. Haciendo cálculos, la estadística apunta a que el juego se extenderá mucho en caso de que se sumen tantas cartas a los jugadores, por lo que se reducen a 2.
***
## 27/03/2020
#### Nuevo
* Creación de la clase **Boton** en el proyecto **Cliente** para la interacción con las cartas por parte del jugador.
* La resolución de posibles errores son anotados en github para resolverlos al finalizar el proyecto.
* Conseguido el funcionamiento básico del servidor. Permite partidas simples por consola.
***
## 28/03/2020
#### Avances
* Colocación de los elementos visuales en el cliente y recepción del con click en los botones necesarios.
***
## 29/03/2020
#### Nuevo
* Creación de la función **actualizarDatos()** en el juego del proyecto **Cliente** que servirá para intercambiar información entre cada turno de la partida.
* Primera prueba de juego entre cliente y servidor a través de interfaz gráfica con éxito.
#### Avances
* Comprobación de clicks sobre botones mejorada, comprueba si hace un *press* y un *release*.
* Comprobación de si es el turno del cliente para poder gestionar los clicks.
* Básicamente, hoy ha sido enlazar la comunicación cliente-servidor.
***
## 31/03/2020
#### Nuevo
* Creación de las siguientes clases en el proyecto **Cliente**:
    * PantallaCrear
    * PantallaInicio
    * PantallaUnir
    * SalaEspera
    * Partida
* Creación de la interfaz **Pantalla**.
* Distribución genérica de los elementos de las pantallas inicio, crear, unirse y sala de espera para futuras pruebas.
* Introduzcción de la función `IntroducirTexto()` en **Game1** para probar la introducción de texto por teclado en MonoGame.
* Creación de los sprites de botones que forman la interfaz de los menús.
#### Modificaciones
* El constructor de la clase **Boton** ha sido modificado. Además, ahora guarda el vector **Escala** para poder dibujar con mayor sencillez.
* Traslado del código de **Game1** a su correspondiente clase **Partida**.
* Desde **Game1** se gestionará en todo momento un objeto de tipo **Pantalla** que se irá reinicializando en función.
***
## 01/04/2020
#### Nuevo
* Creación de la clase **Servidor** en el proyecto **Cliente**.
#### Modificaciones
* Cambios menores sobre dónde se inicializan algunas variables (*Constructor* vs `Initialize()`).
* Eliminación de líneas de depuración en el servidor y el cliente.
#### Avances
* Clase **PantallaCrear** finalizada y operativa. Permite la introducción de un nombre y la creación de una sala en la que esperar jugadores para iniciar la partida.
* Clase **SalaEspera** actualizable ante la llegada de nuevos jugadores. Muestra los nuevos clientes en la sala a tiempo real.
***
## 02/04/2020
#### Nuevo
* Creación clase **TextBox** que hereda de **Boton** para mayor facilidad a la hora manejar datos aportados por el usuario.
* Clase **Boton** cuenta con una función `draw(Game1 game)`que se encarga del dibujado de los objetos de tipo **Boton**.
#### Modificaciones
* Las cajas de texto, antes **Boton** y ahora **TextBox**, remarcan con un borde verde para indicar el focus sobre ellas.
* Uso de la función `draw(Game1 game)` en todos aquellos lugares donde se dibujaban botones manualmente.
* Comienzo de traducir todos los nombres de variables y funciones del español al inglés.
***
## 03/04/2020
#### Modificaciones
* El campo de introducción de sala al unirse solo acepta valores númericos.
* Cambios en el constructor de la clase **Partida** que permite suprimir numerosas variables innecesarias.
#### Avances
* Sala de espera funcional. Permite el comienzo de partidas.
* Las partidas son ya posibles de forma tosca y sencilla. No contempla la posibilidad de que un jugador termine la partida.
***
## 04/04/2020
#### Nuevo
* Creación de la clase **FinPartida** para cuando los jugadores acaban el juego.
* Implementación a la interfax **Pantalla** de las funciones `Click()` y `KeyboardAction(Keys key)` para mayor claridad en la estructuración del código de las clases hijas.
* Creación de sprites nuevos para botones.
#### Modificaciones
* Cambios en el envío de información en la clase **Program** del servidor para ajustarlo a los cambios en el protocolo.
* Supresión de la función `getJugadores()` en la clase **Servidor** del proyecto **Cliente** por no tener uso.
* Cambio de pantalla al finalizar la partida mostrando su posición entre el resto de jugadores.
***
## 05/04/2020
#### Nuevo
* Creación de un marco para señalar qué carta ha seleccionado el jugador.
***
## 06/04/2020
#### Modificaciones
* El servidor es capaz de gestionar múltiples clientes generando un hilo para cada uno de ellos.
* La función `crearSala(Cliente cliente)` del **Servidor** devuelve ahora un booleano permitiendo introducir de nuevo un comando para evitar la interrupción del programa ante un nombre inválido.
* Generado en **Game1** un enumerado para diferenciar todos los posibles efectos de sonido a utilizar en la aplicación. Se plantea concretar de esta forma los sprites, cargándolos y reuniéndolos todos en **Game1**.
* La función `Update(GameTime gameTime)` de **Game1** evita la comprobación de click o introducción de teclas si la pantalla del cliente se ha actualizando evitando así posibles errores de introducción de datos.
* Se evita la superposición de clicks ante pantallas superpuestas, error que había sido registrado. Esto daba lugar a errores también como la deselección de carta entre jugadores: cuando uno seleccionada una carta, al resto le cambiaba también la selección. Solucionado.
#### Avances
* Todas las pantallas cuentan con efectos de sonido al interactuar con la interfaz.
***
## 07/04/2020
#### Modificaciones
* Se carga en todos los usuarios el boton de empezar por si el host original se desconecta y uno debe relevar su puesto.
* Nueva función `refreshWaitingRoom(Sala sala)` en la clase **Cliente** para gestionar la llegada y salida de nuevos clientes a la sala.
* Creada una lista auxiliar con los nombres de los jugadores en la clase **Sala** para poder gestionar un orden de llegada.
#### Avances
* Gestionado en el servidor la desconexión de usuarios dentro de una sala de espera así como el relevo del host a otro jugador en caso de que el host original se desconecte.
***
## 09/04/2020
#### Modificaciones
* La clase **Sala** contiene la variable que guarda cuantos jugadores han entrado y han jugado durante toda la partida. Se va actualizando.
* El servidor gestiona la desconexión inesperada de clientes. Se han modificado los valores de los puertos y las direcciones de los Socket de clientes para lanzar el servidor online.
* Cuando al jugar una carta númerica, el valor sobrepasa el límite no se inicializa a 0, sino a la diferencia del limite menos el valor resultante.
#### Avances
* Supuestamente, la aplicación es funcional y la base está acabada.
***
## 11/04/2020
#### Modificaciones
* Nueva función en la interfaz **Pantalla** para gestionar el cierre de la aplicación por parte del usuario.
* Sincronización de todos los hilos del proyecto para evitar dejarlos en funcionamiento infinito.
* El servidor utiliza la clase **TcpListener** en lugar de la clase **Socket** empleada hasta el momento para mayor facilidad comunicandose con el **TcpClient** del cliente.
* Creado nuevo hilo durante la sala de espera para comprobar qué jugadores están o no conectados.
* La clase **Cliente** del servidor cuenta con la función `isConected()` para facilitar la comprobación de su estado actual.
* Eliminación de funciones a las que no se llaman que en un principio se pensaron tendrían utilidad (como `enviarPaquete(PaqueteTurno paquete)`).
#### Avances
* Controlada la creación y destrucción de salas, así como cuando dejan de aceptarse nuevos jugadores.
* **Sala Espera** se actualiza a tiempo real permitiendo ver qué jugadores han entrado en la sala y quienes se han ido.
***
## 11/04/2020
#### Nuevo
* En la clase **Partida** del **Cliente** existen dos botones que permiten ver toda la baraja aun cuando hay más de 8 cartas (número máximo de cartas mostradas por pantalla).
* Intercambio entre inputs de la clase **PantallaUnir** mediante la tecla tabulador. Próximamente implementaré la tecla Enter para simular el click en aceptar.
#### Modificaciones
* La función `actualizarBaraja()` en **Partida** se ejecuta solo cuando recibe nuevos datos y no en el `Update()` como se había hecho hasta ahora.
* Nuevas funciones encargadas de desplazarse por la baraja y de centrar las cartas cuando hay menos o tantas como las que se pueden mostrar por pantalla.
***
## 13/04/2020
#### Modificaciones
* La clase **Sala** pasa a tener en ella un objeto de tipo **Partida** para poder acceder desde un único objeto a toda la información de una sesión de juego.
* La función `KeyboardAction(Keys key)` interfaz **Pantalla** devuelve ahora un objeto tipo Pantalla que nos permite movernos entre escenas a través de acciones de teclado.
* Creación de otro hilo paralelo a la partida encargado de comprobar la desconexión de los jugadores e informar al resto actualizando sus datos sobre el juego.
#### Avances
* Controlada la desconexión de todo tipo de clientes en todo tipo de casos.
***
## 15/04/2020
#### Avances
* Controlada la caída del servidor en cualquier parte de la aplicación. El juego es completamente funcional, de ahora en adelante seguiré con la optimización del código, aumento de funcionalidad y/o nuevas reglas.
***
## 17/04/2020
#### Modificaciones
* Mediante booleanas, se controla que no se realice más de un click a los botones que se comunican con el servidor para evitar posibles problemas.
***
## 18/04/2020
#### Modificaciones
* Creación de la clase **GestionClientes**, **Juego** y **SalaEspera** en el Servidor para manejar con mayor facilidad las distintas etapas del código.
* La clase **Boton** del Cliente cuenta con un segundo constructor para poder introducir texto en su interior.
#### Avances
* Es posible navegar a lo largo de toda la aplicación mediante el uso único y exclusivo de teclado.
***
## 19/04/2020
#### Modificaciones
* Referencia al sentido de la mesa durante la partida modificada.
* Botón de regresar en **SalaEspera** de Cliente para poder salir de la sala.
* Reducción de código.
***
## 21/04/2020
#### Modificaciones
* Nombres de variables, propiedades, clases, funciones, etc, traducidos al inglés.
* Modificación de botones con imágenes estáticas a botones con imágenes dinámicas.