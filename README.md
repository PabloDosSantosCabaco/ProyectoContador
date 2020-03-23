# ProyectoContador
## 19/03/2020
#### Nuevo
* Creación de los proyectos Cliente y ServidorContador.
* Creación de clases Sala y Cliente en ServidorContador.
* Desarrollo de gestión de clientes: creación y unión a salas.
* Ampliación de las clases Sala y Cliente. Gestión de la clase sala mediante hilos.
## 20/03/2020
#### Nuevo
* Redacción del anteproyecto.
## 21/03/2020
#### Modificaciones
* Corrección del diagrama de Gantt.
#### Nuevo
* Pruebas de dibujado con MonoGame.
* Creación de la clase Carta.
* Creación de sprites de la baraja de cartas.
## 22/03/2020
#### Nuevo
* Conseguida disposición del contenido  de la mesa de cartas de cada usuario.
* Redimensión del contenido en función de las dimensiones de la ventana.
## 23/03/2020
#### Nuevo
* Dibujado del valor, sentido, turno y marcadores de la partida.
* Gestión mediante hilos de las diferentes partidas.
* Creación de fuentes para escribir en MonoGame.
#### Modificaciones
* Los datos como nº de jugadores por partida, turno, valor de mesa y cartas de cada jugador se pasan como único objeto de clase **PaqueteTurno**, que es enviado a cada jugador después de cada turno.
* Existe duplicada la clase **Carta** en ambos proyectos ya que es necesaria en ambos. Se plantea la creación de una librería de clases en caso de necesitar más clases compartidas. Para una sola tan simple, se ha dejado duplicada.