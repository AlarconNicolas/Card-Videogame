# Prototipo

El prototipo comienza en la escena Juego, sin embargo, el desarrollo de otras escenas como Login ya es avanzado. El script principal para correr el juego en su escena es GameController_DeathRace.cs, donde por ahora se encuentran todas las funciones y corutinas necesarias para obtener la información de la base de datos y la funcionalidad del juego. Para correr el juego, es necesario iniciar el servidor que se encuentra en la carpeta API del repositorio. También hay que correr los scripts de sql death_race_schema y death_race_data que se encuentran en la carpeta Base_de_datos. Para conectarse, el usuario y contraseña que se encuentran en el script de GameController deben estar dado de alta en MySQL Workbench.

Los controles del juego solo incluyen mover el cursor y hacer click sobre la carta que se desea jugar. 

Las funcionalidades ya terminadas son: contador de energía por turno, contador de tiempo restante de turno, selección y aplicación de efectos de las cartas del jugador, visualización de la actualización de posición de los coches por turno y visualización de la categoría de las cartas del rival.

Las funcionalidades que aún están en desarrollo son: la implementación correcta de la IA, el código y lógica de elección ya está listo en su mayoría pero falta implementar la elección únicamente de cartas que están en su mano durante el turno. Para finalizar la partida solo falta hacer la transición a una escena en específico, por el momento la hace al menu principal. Las condiciones de victoria ya funcionan. Falta implementar la barra de salud de los coches. 