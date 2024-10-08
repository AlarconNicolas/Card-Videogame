SET NAMES utf8mb4;
SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL';
SET @old_autocommit=@@autocommit;

USE death_race;

SET AUTOCOMMIT = 1;

INSERT INTO carta (nombre, tipo, costo, puntos_dano, puntos_defensa, puntos_velocidad, efecto, descripcion) VALUES
('Speed', 'Velocidad', 2, 0, 0, 1, 'Jugador', 'El vehículo avanza 1 casilla adicional'),
('HP', 'Defensa', 2, 0, 10, 0, 'Jugador', 'Suma 10pts de vida al vehículo'),
('Crash', 'Ataque', 2, 10, 0, 0, 'Enemigo', 'El enemigo pierde 10pts de vida'),
('Boom', 'Ataque', 4, 30, 0, 0, 'Enemigo', 'El enemigo pierde 30pts de vida'),
('Angel', 'Defensa', 4, 0, 30, 0, 'Jugador', 'Suma 30pts de vida al vehículo'),
('Shield', 'Defensa', 3, 0, 20, 0, 'Jugador', 'Aguanta 20pts de daño de un ataque rival'),
('Dome', 'Defensa', 4, 0, 30, 0, 'Jugador', 'Aguanta 30pts de daño de un ataque rival'),
('Flash', 'Velocidad', 3, 0, 0, 3, 'Jugador', 'El vehículo avanza 3 casillas adicionales'),
('Zap', 'Ataque', 3, 0, 0, 0, 'Enemigo', 'Paraliza al enemigo por un turno'),
('Repair', 'Defensa', 3, 0, 15, 0, 'Jugador', 'Repara 15pts de vida al vehículo'),
('Turbo', 'Velocidad', 5, 0, 0, 4, 'Vehículo', 'Permite avanzar 4 casillas adicionales'),
('Sneak', 'Ataque', 4, 20, 0, 0, 'Enemigo', 'Inflige 20pts de vida si el enemigo no tiene cartas de defensa activas'),
('Boost', 'Velocidad', 2, 0, 0, 0, 'Jugador', '+1 el límite de energía por un turno'),
('Stealth', 'Defensa', 4, 0, 0, 0, 'Vehículo', 'Evita el próximo ataque enemigo'),
('Overload', 'Ataque', 4, 20, 0, 0, 'Enemigo', 'Duplica el daño del próximo ataque pero recibe 10pts de daño'),
('Nitro', 'Velocidad', 5, 0, 0, 4, 'Vehículo', 'Avanza 4 casillas adicionales, ignorando obstáculos'),
('EMP', 'Ataque', 4, 0, 0, 0, 'Enemigo', 'Anula la próxima carta de velocidad o ataque del enemigo'),
('Quick Fix', 'Defensa', 2, 0, 5, 0, 'Vehículo', 'Recupera 5pts de vida inmediato y otros 5pts en el siguiente turno'),
('Power Drain', 'Ataque', 2, 0, 0, 0, 'Enemigo', 'Reduce el límite de energía de -1 al enemigo'),
('Accelerate', 'Velocidad', 2, 0, 0, 0, 'Jugador', 'Aumenta la velocidad por dos turnos, avanzando 1 casilla adicional'),
('Firewall', 'Defensa', 3, 0, 0, 0, 'Vehículo', 'Protege contra las cartas de ataque por 1 turno'),
('Sabotage', 'Ataque', 5, 0, 0, 0, 'Enemigo', 'El enemigo pierde su carta de mayor costo de energía'),
('Surge', 'Velocidad', 1, 0, 0, 0, 'Jugador', '+1 de energía extra para el próximo turno'),
('Throttle', 'Velocidad', 5, 0, 0, 0, 'Jugador', 'Avanza inmediatamente a la casilla más cercana atrás del líder'),
('Light Shield', 'Defensa', 1, 0, 10, 0, 'Vehículo', 'Reduce el daño del próximo ataque en 10pts'),
('Jab', 'Ataque', 1, 5, 0, 0, 'Enemigo', 'Inflige 5pts de daño al enemigo'),
('Small Boost', 'Velocidad', 1, 0, 0, 1, 'Jugador', 'El vehículo avanza 1 casilla adicional'),
('Minor Repair', 'Defensa', 1, 0, 5, 0, 'Vehículo', 'Suma 5pts de vida al vehículo'),
('Energy Saver', 'Velocidad', 2, 0, 0, 0, 'Jugador', 'Reduce el costo de -1 energía de la próxima carta jugada'),
('Weak Spot', 'Ataque', 2, 10, 0, 0, 'Jugador', 'El próximo ataque que realices inflige 10pts de daño extra');

INSERT INTO jugador (nombre, correo) VALUES
('Joseph', 'Joseph@tec.mx'),
('Emiliano', 'Emiliano@tec.mx'),
('Nicolas', 'Nicolas@tec.mx'),
('Isaac', 'Isaac@tec.mx'),
('Daniela', 'Daniela@tec.mx');

INSERT INTO contrasena (id_jugador, contrasena) VALUES
(1, 'Joseph'),
(2, 'Emiliano'),
(3, 'Nicolas'),
(4, 'Isaac'),
(5, 'Daniela');

INSERT INTO coche (nombre, vida, velocidad, efecto_ataque, efecto_velocidad, efecto_defensa) VALUES
#Por el momento solo tenemos tres coches por lo que solo se insertan tres coches
('CocheRojo', 100, 50, 1, 1, 1),
('CocheVerde', 90, 55, 1, 1, 1),
('CocheAmarillo', 120, 45, 1, 1, 1);

INSERT INTO escenario (id_escenario, nombre, distancia, descripcion) VALUES
#Solo hay dos pistas por el momento planeadas
(1, 'Pista1', 100, 'Pista original'),
(2, 'Pista2', 100, 'Tematica de desierto');

INSERT INTO victorias_jugador (id_jugador, total_victorias) VALUES
(1, 10),
(2, 30),
(3, 123),
(4, 45),
(5, 142);

INSERT INTO turnos_partida (id_jugador, turnos_total, partidas_jugadas) VALUES 
(1, 200, 20),
(2, 321, 40),
(3, 1234, 200),
(4, 432, 54),
(5, 1453, 200);

INSERT INTO coleccion_cartas (id_jugador, id_carta, id_coche, numero_cartas) VALUES
(1, 1, 1, 1),  (1, 2, 1, 1),  (1, 10, 1, 1),
(1, 11, 2, 1), (1, 12, 2, 1), (1, 13, 2, 1), (1, 14, 2, 1), (1, 15, 2, 1), (1, 16, 2, 1), (1, 17, 2, 1), (1, 18, 2, 1), (1, 19, 2, 1), (1, 20, 2, 1),
(1, 21, 3, 1), (1, 22, 3, 1), (1, 23, 3, 1), (1, 24, 3, 1), (1, 25, 3, 1), (1, 26, 3, 1), (1, 27, 3, 1), (1, 28, 3, 1), (1, 29, 3, 1), (1, 30, 3, 1),

(2, 1, 1, 1),  (2, 2, 1, 1),  (2, 3, 1, 1),  (2, 4, 1, 1),  (2, 5, 1, 1), (2, 6, 1, 1),  (2, 7, 1, 1),  (2, 8, 1, 1),  (2, 9, 1, 1),  (2, 10, 1, 1),
(2, 11, 2, 1), (2, 12, 2, 1), (2, 13, 2, 1), (2, 14, 2, 1), (2, 15, 2, 1), (2, 16, 2, 1), (2, 17, 2, 1), (2, 18, 2, 1), (2, 19, 2, 1), (2, 20, 2, 1),
(2, 21, 3, 1), (2, 22, 3, 1), (2, 23, 3, 1),

(3, 9, 1, 1),  (3, 10, 1, 1),
(3, 11, 2, 1), (3, 12, 2, 1), (3, 13, 2, 1), (3, 14, 2, 1), (3, 15, 2, 1), (3, 16, 2, 1), (3, 17, 2, 1), (3, 18, 2, 1), (3, 19, 2, 1), (3, 20, 2, 1),
(3, 21, 3, 1), (3, 22, 3, 1), (3, 23, 3, 1), (3, 24, 3, 1), (3, 25, 3, 1), (3, 26, 3, 1), (3, 27, 3, 1), (3, 28, 3, 1), (3, 29, 3, 1), (3, 30, 3, 1),

(4, 1, 1, 1),  (4, 2, 1, 1),  (4, 3, 1, 1),  (4, 4, 1, 1),  (4, 5, 1, 1), (4, 6, 1, 1),  (4, 7, 1, 1),  (4, 8, 1, 1),  (4, 9, 1, 1),  (4, 10, 1, 1), 
(4, 11, 2, 1), (4, 12, 2, 1), (4, 20, 2, 1),
(4, 21, 3, 1), (4, 22, 3, 1), (4, 23, 3, 1), (4, 24, 3, 1), (4, 25, 3, 1), (4, 26, 3, 1), (4, 27, 3, 1), (4, 28, 3, 1), (4, 29, 3, 1), (4, 30, 3, 1),

(5, 4, 1, 1),  (5, 5, 1, 1), (5, 6, 1, 1),  (5, 7, 1, 1),  (5, 8, 1, 1),  (5, 9, 1, 1),  (5, 10, 1, 1),
(5, 11, 2, 1), (5, 12, 2, 1), (5, 13, 2, 1), (5, 16, 2, 1), (5, 17, 2, 1), (5, 18, 2, 1), (5, 19, 2, 1), (5, 20, 2, 1),
(5, 21, 3, 1), (5, 22, 3, 1), (5, 23, 3, 1), (5, 24, 3, 1), (5, 25, 3, 1), (5, 26, 3, 1), (5, 27, 3, 1), (5, 28, 3, 1), (5, 29, 3, 1), (5, 30, 3, 1);

INSERT INTO mazo (id_jugador, nombre, id_coche) VALUES
(1, 'Mazo A Jugador 1', 1),
(2, 'Mazo A Jugador 2', 2),
(3, 'Mazo A Jugador 3', 3),
(4, 'Mazo A Jugador 4', 1),
(5, 'Mazo A Jugador 5', 2);

INSERT INTO mazo_cartas (id_mazo, id_carta) VALUES
(1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8),
(2, 1), (2, 2), (2, 3), (2, 4), (2, 5), (2, 6), (2, 7), (2, 8),
(3, 9), (3, 10), (3, 11), (3, 12), (3, 13), (3, 14), (3, 15), (3, 16),
(4, 9), (4, 10), (4, 11), (4, 12), (4, 13), (4, 14), (4, 15), (4, 16),
(5, 17), (5, 18), (5, 19), (5, 20), (5, 21), (5, 22), (5, 23), (5, 24);

INSERT INTO uso_cartas (id_jugador, id_carta, cantidad_usos) VALUES
(1, 1, 10), (1, 2, 20), (1, 3, 30),
(2, 1, 5), (2, 2, 15), (2, 3, 25),
(3, 1, 35), (3, 2, 45), (3, 3, 55),
(4, 1, 65), (4, 2, 75), (4, 3, 85),
(5, 1, 95), (5, 2, 5), (5, 3, 15),
(1, 4, 25), (1, 5, 35), (1, 6, 45),
(2, 4, 55), (2, 5, 65), (2, 6, 75),
(3, 4, 85), (3, 5, 95), (3, 6, 105),
(4, 4, 30), (4, 5, 40), (4, 6, 50),
(5, 4, 60), (5, 5, 70), (5, 6, 80);


/*INSERT INTO juego (id_jugador1, id_jugador2, id_escenario, id_ganador, duracion) VALUES
(1, 2, 1, 1, 30),
(2, 3, 2, 2, 25),
(3, 4, 3, 3, 28),
(4, 5, 4, 4, 32),
(5, 6, 5, 5, 27),
(6, 7, 6, 6, 30),
(7, 8, 7, 7, 26),
(8, 9, 8, 8, 29),
(9, 10, 9, 9, 31),
(10, 11, 10, 10, 28),
(11, 12, 11, 11, 29),
(12, 13, 12, 12, 32),
(13, 14, 13, 13, 27),
(14, 15, 14, 14, 30),
(15, 16, 15, 15, 26),
(16, 17, 16, 16, 29),
(17, 18, 17, 17, 31),
(18, 19, 18, 18, 28),
(19, 20, 19, 19, 30),
(20, 1, 20, 20, 25);*/

/*INSERT INTO efecto (id_efecto, nombre, efecto_enemigo, efecto_jugador, descripcion) VALUES
#El apartado de efectos no esta tan desarollado todavia pero se pueden añadir mas efectos en un futuro
(1, 'Ponchar llanta', 5, 0, 'El jugador enemigo no avanza este turno'),
(2, 'Curación total', 8, 8, 'Ambos coches reciben una reparación de daños de 10 puntos'),
(3, 'Super ataque', 0, 15, 'Durante este turno cada punto de daño vale por dos'),
(4, 'Super defensa', 7, 0, 'Durante este turno cada punto de daño de tu enemigo vale 0.5'),
(5, 'Super Velocidad', 0, 13, 'Durante este turno cada punto de velocidad vale por dos');