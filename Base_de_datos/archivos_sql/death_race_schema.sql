DROP SCHEMA IF EXISTS death_race;
CREATE SCHEMA death_race;
USE death_race;

CREATE TABLE carta (
    id_carta SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(45) NOT NULL,
    tipo VARCHAR(45) NOT NULL,
    costo TINYINT UNSIGNED NOT NULL,
    puntos_dano SMALLINT UNSIGNED NOT NULL,
    puntos_defensa SMALLINT UNSIGNED NOT NULL,
    puntos_velocidad SMALLINT UNSIGNED NOT NULL,
    efecto VARCHAR(100) NOT NULL,
    descripcion TEXT,
    PRIMARY KEY  (id_carta)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE jugador (
    id_jugador SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(45) NOT NULL,
    correo VARCHAR(45) NOT NULL,
    PRIMARY KEY  (id_jugador)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE contrasena (
	id_contrasena SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_jugador SMALLINT UNSIGNED NOT NULL,
    contrasena VARCHAR(45) NOT NULL,
    PRIMARY KEY (id_contrasena),
    CONSTRAINT fk_contrasena_jugador FOREIGN KEY (id_jugador) REFERENCES jugador (id_jugador)
) ENGINE=innoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE coche (
    id_coche SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(45) NOT NULL,
    vida SMALLINT NOT NULL,
    velocidad SMALLINT NOT NULL,
    efecto_ataque SMALLINT NOT NULL,
    efecto_velocidad SMALLINT NOT NULL,
    efecto_defensa SMALLINT NOT NULL,
    PRIMARY KEY  (id_coche)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE escenario (
    id_escenario SMALLINT UNSIGNED NOT NULL,
    nombre VARCHAR(45) NOT NULL,
    distancia SMALLINT UNSIGNED NOT NULL,
    descripcion TEXT,
	PRIMARY KEY  (id_escenario)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE victorias_jugador (
    id_victorias_jugador SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_jugador SMALLINT UNSIGNED NOT NULL,
    total_victorias INT UNSIGNED NOT NULL DEFAULT 0,
    PRIMARY KEY (id_victorias_jugador),
    CONSTRAINT fk_victorias_jugador FOREIGN KEY (id_jugador) REFERENCES jugador (id_jugador)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE uso_cartas (
    id_uso_cartas SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_jugador SMALLINT UNSIGNED NOT NULL,
    id_carta SMALLINT UNSIGNED NOT NULL,
    cantidad_usos INT UNSIGNED NOT NULL DEFAULT 0,
    PRIMARY KEY (id_uso_cartas),
    CONSTRAINT fk_uso_cartas_jugador FOREIGN KEY (id_jugador) REFERENCES jugador (id_jugador),
    CONSTRAINT fk_uso_cartas_carta FOREIGN KEY (id_carta) REFERENCES carta (id_carta)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE turnos_partida (
    id_jugador SMALLINT UNSIGNED NOT NULL,
    turnos_total INT UNSIGNED NOT NULL DEFAULT 0,
    partidas_jugadas INT UNSIGNED NOT NULL DEFAULT 0,
    PRIMARY KEY (id_jugador),
    CONSTRAINT fk_turnos_partida_jugador FOREIGN KEY (id_jugador) REFERENCES jugador (id_jugador) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE coleccion_cartas (
    id_coleccion_cartas SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_jugador SMALLINT UNSIGNED NOT NULL,
    id_carta SMALLINT UNSIGNED NOT NULL,
    id_coche SMALLINT UNSIGNED NOT NULL,
    numero_cartas SMALLINT NOT NULL,
    PRIMARY KEY  (id_coleccion_cartas),
    CONSTRAINT fk_coleccion_jugador FOREIGN KEY (id_jugador) REFERENCES jugador (id_jugador) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_coleccion_carta FOREIGN KEY (id_carta) REFERENCES carta (id_carta) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_coleccion_coche FOREIGN KEY (id_coche) REFERENCES coche (id_coche) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE mazo (
    id_mazo SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_jugador SMALLINT UNSIGNED NOT NULL,
    nombre VARCHAR(45) NOT NULL,
    id_coche SMALLINT UNSIGNED,  -- La columna nueva para almacenar el ID del coche asociado
    PRIMARY KEY  (id_mazo),
    CONSTRAINT fk_mazo_jugador FOREIGN KEY (id_jugador) REFERENCES jugador (id_jugador) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_mazo_coche FOREIGN KEY (id_coche) REFERENCES coche (id_coche) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE mazo_cartas (
    id_mazo SMALLINT UNSIGNED NOT NULL,
    id_carta SMALLINT UNSIGNED NOT NULL,
    PRIMARY KEY (id_mazo, id_carta),
    CONSTRAINT fk_mazo_cartas_mazo FOREIGN KEY (id_mazo) REFERENCES mazo (id_mazo) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_mazo_cartas_carta FOREIGN KEY (id_carta) REFERENCES carta (id_carta) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE inventario (
	id_inventario SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_carta SMALLINT UNSIGNED NOT NULL,
    id_jugador SMALLINT UNSIGNED NOT NULL,
    id_mazo SMALLINT UNSIGNED NOT NULL,
    id_coche SMALLINT UNSIGNED NOT NULL,
    id_coleccion_cartas SMALLINT UNSIGNED NOT NULL,
    PRIMARY KEY (id_inventario),
    KEY fk_id_carta_id_jugador_id_mazo_id_coche_id_coleccion_cartas (id_carta, id_jugador, id_mazo, id_coche, id_coleccion_cartas),
    CONSTRAINT fk_inventory_carta FOREIGN KEY (id_carta) REFERENCES carta (id_carta) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_inventory_jugador FOREIGN KEY (id_jugador) REFERENCES jugador (id_jugador) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_inventory_mazo FOREIGN KEY (id_mazo) REFERENCES mazo (id_mazo) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_inventory_coche FOREIGN KEY (id_coche) REFERENCES coche (id_coche) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_inventory_coleccion_cartas FOREIGN KEY (id_coleccion_cartas) REFERENCES coleccion_cartas (id_coleccion_cartas) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE juego (
	id_juego SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    id_jugador1 SMALLINT UNSIGNED NOT NULL,
    id_jugador2 SMALLINT UNSIGNED NOT NULL,
    id_escenario SMALLINT UNSIGNED NOT NULL,
    id_ganador SMALLINT UNSIGNED NOT NULL,
    duracion SMALLINT NOT NULL,
  PRIMARY KEY  (id_juego)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE efecto (
    id_efecto SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(45) NOT NULL,
    efecto_enemigo SMALLINT UNSIGNED NOT NULL,
    efecto_jugador SMALLINT UNSIGNED NOT NULL,
    descripcion TEXT,
  PRIMARY KEY  (id_efecto)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;