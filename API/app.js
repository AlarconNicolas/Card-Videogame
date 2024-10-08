"use strict";

import express from "express";
import mysql from "mysql2/promise";
import fs from 'fs'

const app = express();
const port = 3000;

app.use(express.json());
app.use(express.static('./public'));

async function connectToDB(){
    console.log("connected to database")
    return await mysql.createConnection({
        host: "localhost",
        user: "Test",
        password: "",
        database: "death_race",
    });
} 

app.get('/', (req, res)=>
{
    fs.readFile('./public/html/user_charts.html', 'utf8', 
    (err, html) => {
        if(err)
        {
            res.status(500).send('There was an error: ' + err)
            return 
        }
        
        console.log("Sending page...")
        res.send(html)
        console.log("Page sent!")
    })
})

// Endpoint para conseguir todas las cartas
app.get("/api/cards", async (request, response) => {
    let connection = null;
    try {
        connection = await connectToDB();
        const [results, fields] = await connection.execute("SELECT * FROM carta");
        response.json(results);
    } catch (error) {
        console.error("Error al obtener las cartas: ", error);
        response.status(500).send("Error al obtener las cartas");
    } finally {
        if (connection) {
            await connection.end();
            console.log("connection closed")
        }
    }
});

// Endpoint para conseguir una carta específica por su ID
app.get("/api/cards/:id", async (request, response) => {
    let connection = null;
    try {
        const cardId = request.params.id;
        connection = await connectToDB();
        
        const [results, fields] = await connection.execute("SELECT * FROM carta WHERE id_carta = ?", [cardId]);
        
        if (results.length > 0) {
            response.json(results[0]);
        } else {
            response.status(404).send("Carta no encontrada");
        }
    } catch (error) {
        console.error("Error al obtener la carta: ", error);
        response.status(500).send("Error al obtener la carta");
    } finally {
        if (connection) {
            await connection.end();
            console.log("Connection closed");
        }
    }
});

// Endpoint para conseguir un jugador en base a su nombre
app.get("/api/players/:name", async (request, response) => {
    let connection = null;
    try {
        const { name } = request.params;
        connection = await connectToDB();

        const [results, fields] = await connection.execute('SELECT * FROM jugador WHERE nombre = ?', [name]);
        console.log(`${results.length} rows returned`);

        if (results.length > 0) {
            response.json({ found: true, player: results[0] });
        } else {
            response.json({ found: false });
        }
    } catch (error) {
        console.error("Error al obtener el jugador: ", error);
        response.status(500).json({ found: false, error: "Error al obtener el jugador" });
    } finally {
        if (connection) {
            await connection.end();
            console.log("connection closed");
        }
    }
});

// Enpoint para confimrar el id de un jugador en base a una contraseña
app.get("/api/password/:contrasena", async (request, response) => {
    let connection = null;
    try {
        const { contrasena } = request.params;
        if (!contrasena) {
            return response.status(400).json({ error: "Falta la contraseña en la solicitud" });
        }
        connection = await connectToDB();
        
        // Asumiendo que la tabla que contiene los nombres y correos de los jugadores se llama 'jugador'
        // y que 'contrasena' tiene una columna 'id_jugador' que es clave foránea a 'jugador'
        const query = `SELECT j.id_jugador, j.nombre, j.correo FROM jugador j JOIN contrasena c ON j.id_jugador = c.id_jugador WHERE c.contrasena = ?`;
        
        const [results] = await connection.execute(query, [contrasena]);
        
        if (results.length === 0) {
            return response.status(404).json({ error: "No se encontró ningún jugador con esa contraseña" });
        }

        // Asumiendo que la consulta devuelve los campos esperados
        const playerData = {
            id_jugador: results[0].id_jugador,
            nombre: results[0].nombre,
            correo: results[0].correo
        };
        response.status(200).json({ found: true, player: playerData });
    } catch (error) {
        console.error("Error al obtener el ID del jugador: ", error);
        response.status(500).json({ error: "Error al obtener el ID del jugador" });
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Endpoint para añadir un nuevo juego
app.post("/api/games", async (request, response) => {
    let connection = null;
    try {
        const { id_jugador1, id_jugador2, id_escenario, id_ganador, duracion } = request.body;
        connection = await connectToDB();
        const [results] = await connection.execute("INSERT INTO juego (id_jugador1, id_jugador2, id_escenario, id_ganador, duracion) VALUES (?, ?, ?, ?, ?)", [id_jugador1, id_jugador2, id_escenario, id_ganador, duracion]);
        
        response.status(201).send(`Juego creado con ID: ${results.insertId}`);
    } catch (error) {
        console.error("Error al crear el juego: ", error);
        response.status(500).send("Error al crear el juego");
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Endpoint para añadir una carta al inventario
app.post('/api/inventory/:id/card', async (req, res) => {
    let connection = null;
    try {
        const playerId = req.params.id;
        const { id_carta, id_coche, numero_cartas } = req.body;

        connection = await connectToDB();

        const [results] = await connection.execute(
            'INSERT INTO coleccion_cartas (id_jugador, id_carta, id_coche, numero_cartas) VALUES (?, ?, ?, ?)',
            [playerId, id_carta, id_coche, numero_cartas]
        );

        res.status(201).send(`Card added to collection: ${results.insertId}`);
    } catch (error) {
        console.error('Error inserting card into collection: ', error);
        res.status(500).send('Error inserting card into collection');
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Endpoint para actualizar las estadísticas de un juego
app.post("/api/games/:id/stats", async (request, response) => {
    let connection = null;
    try {
        const { id } = request.params; // ID del juego
        const { id_ganador, uso_cartas, numero_turnos } = request.body;
        
        connection = await connectToDB();
        await connection.beginTransaction();

        // Actualiza el total de victorias para el jugador ganador
        await connection.query("UPDATE victorias_jugador SET total_victorias = total_victorias + 1 WHERE id_jugador = ?", [id_ganador]);
        
        // Actualiza el uso de cartas
        for (const uso of uso_cartas) {
            await connection.query("UPDATE uso_cartas SET cantidad_usos = cantidad_usos + ? WHERE id_carta = ? AND id_jugador = ?", [uso.numero_usos, uso.id_carta, id_ganador]);
        }
        
        // Actualiza los turnos de la partida
        await connection.query("UPDATE turnos_partida SET partidas_jugadas = partidas_jugadas + 1, turnos_total = turnos_total + ? WHERE id_jugador = ?", [numero_turnos, id_ganador]);
        
        await connection.commit();

        response.status(200).send("Estadísticas actualizadas correctamente");
    } catch (error) {
        if (connection) {
            await connection.rollback();
        }
        console.error("Error al publicar estadísticas del juego: ", error);
        response.status(500).send("Error al publicar estadísticas del juego");
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Endpoint para obtener el resumen de las estadisticas de un jugador
app.get("/api/players/:id/summary", async (request, response) => {
    let connection = null;
    try {
        const { id } = request.params;
        connection = await connectToDB();

        // Número de victorias
        const [victories] = await connection.execute(
            "SELECT total_victorias FROM victorias_jugador WHERE id_jugador = ?",[id]
        );

        // Carta más usada
        const [mostUsedCard] = await connection.execute(
            "SELECT id_carta, MAX(cantidad_usos) as max_uses FROM uso_cartas WHERE id_jugador = ? GROUP BY id_carta ORDER BY max_uses DESC LIMIT 1",[id]
        );

        // Carta menos usada
        const [leastUsedCard] = await connection.execute(
            "SELECT id_carta, MIN(cantidad_usos) as min_uses FROM uso_cartas WHERE id_jugador = ? GROUP BY id_carta ORDER BY min_uses ASC LIMIT 1",[id]
        );

        // Promedio de turnos por partida
        const [averageTurns] = await connection.execute(
            "SELECT IFNULL(AVG(turnos_total / partidas_jugadas), 0) as avg_turns FROM turnos_partida WHERE id_jugador = ?",
            [id]
        );


        // Convertir explícitamente avg_turns a un número y aplicar toFixed
        let avgTurns = parseFloat(averageTurns[0]?.avg_turns);
        if (isNaN(avgTurns)) { // Verifica si avgTurns no es un número (NaN)
            avgTurns = 0; // Establece a 0 si el resultado no es un número
        }

        // Componer el objeto de respuesta con avgTurns ya validado y convertido
        const summary = {
            victories: victories[0]?.total_victorias || 0,
            mostUsedCard: mostUsedCard[0]?.id_carta || "N/A",
            leastUsedCard: leastUsedCard[0]?.id_carta || "N/A",
            averageTurns: avgTurns.toFixed(2) // Usar avgTurns validado
        };
        response.json(summary);
        } 
            catch (error) {
            console.error("Error al obtener el resumen del jugador: ", error);
            response.status(500).json({ error: "Error al obtener el resumen del jugador" });
        } 
        finally {
            if(connection) {
                await connection.end();
            }
        }
});

app.get("/api/summary/general", async (request, response) => {
    let connection = null;
    try {
        connection = await connectToDB();

        // Total de victorias por jugador, incluyendo el nombre del jugador
        const [allVictories] = await connection.execute(
            `SELECT j.nombre AS nombre_jugador, SUM(v.total_victorias) AS total_victorias
             FROM victorias_jugador v
             JOIN jugador j ON v.id_jugador = j.id_jugador
             GROUP BY v.id_jugador
             ORDER BY total_victorias DESC`
        );

        // Top 3 cartas más usadas incluyendo nombres de las cartas
        const [mostUsedCards] = await connection.execute(
            `SELECT c.nombre, SUM(u.cantidad_usos) AS total_usos
             FROM uso_cartas u
             JOIN carta c ON u.id_carta = c.id_carta
             GROUP BY u.id_carta
             ORDER BY total_usos DESC
             LIMIT 3`
        );

        // Top 3 cartas menos usadas incluyendo nombres de las cartas
        const [leastUsedCards] = await connection.execute(
            `SELECT c.nombre, SUM(u.cantidad_usos) AS total_usos
             FROM uso_cartas u
             JOIN carta c ON u.id_carta = c.id_carta
             GROUP BY u.id_carta
             ORDER BY total_usos ASC
             LIMIT 3`
        );

        // Promedio de turnos por partida de todos los jugadores
        const [averageTurns] = await connection.execute(
            "SELECT IFNULL(AVG(turnos_total / partidas_jugadas), 0) AS avg_turns FROM turnos_partida"
        );

        // Formatear el promedio de turnos a dos decimales
        let avgTurns = parseFloat(averageTurns[0]?.avg_turns);
        if (isNaN(avgTurns)) {
            avgTurns = 0;
        }

        const summary = {
            totalVictories: allVictories.map(v => ({ nombre_jugador: v.nombre_jugador, victorias: v.total_victorias })),
            topUsedCards: mostUsedCards.map(card => ({ nombre: card.nombre, usos: card.total_usos })),
            leastUsedCards: leastUsedCards.map(card => ({ nombre: card.nombre, usos: card.total_usos })),
            averageTurns: avgTurns.toFixed(2)
        };

        response.json(summary);
    } catch (error) {
        console.error("Error al obtener el resumen general de estadísticas: ", error);
        response.status(500).json({ error: "Error al obtener el resumen general de estadísticas" });
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});


// Endpoint para obtener todos los coches
app.get("/api/cars", async (request, response) => {
    let connection = null;
    try {
        connection = await connectToDB();
        const [results] = await connection.execute("SELECT * FROM coche");
        response.json(results);
    } catch (error) {
        console.error("Error al obtener los coches: ", error);
        response.status(500).send("Error al obtener los coches");
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Endpoint para obtener in inventario de cartas y coches de un jugador
app.get("/api/coleccion/:id", async (request, response) => {
    let connection = null;
    try {
        const { id } = request.params; // Obtenemos el ID del jugador desde los parámetros de la URL
        connection = await connectToDB();
        // Modificamos la consulta para incluir un JOIN con la tabla coche y obtener los nombres de los coches
        const query = `
            SELECT 
                cc.id_jugador,
                GROUP_CONCAT(DISTINCT cc.id_carta) AS cartas_ids,
                GROUP_CONCAT(DISTINCT co.nombre ORDER BY co.nombre) AS coches_nombres
            FROM coleccion_cartas cc
            JOIN coche co ON cc.id_coche = co.id_coche
            WHERE cc.id_jugador = ?
            GROUP BY cc.id_jugador;
        `;
        const [results] = await connection.execute(query, [id]);

        if (results.length > 0) {
            response.json(results[0]);
        } else {
            response.status(404).send("Colección de cartas no encontrada");
        }
    } catch (error) {
        console.error("Error al obtener la colección de cartas: ", error);
        response.status(500).send("Error al obtener la colección de cartas");
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Endpoint para obtener los detalles de un solo mazo, incluyendo cartas y el coche asociado
app.get("/api/mazo/:idMazo", async (request, response) => {
    let connection = null;
    try {
        // Obtiene el ID del mazo desde los parámetros de la ruta.
        const { idMazo } = request.params;

        // Conexión a la base de datos.
        connection = await connectToDB();

        // Primero, obtiene los detalles del mazo especificado, incluyendo el coche asociado.
        const [mazos] = await connection.execute(
            "SELECT m.id_mazo, m.nombre, c.nombre AS nombre_coche FROM mazo m LEFT JOIN coche c ON m.id_coche = c.id_coche WHERE m.id_mazo = ?",
            [idMazo]
        );

        // Verifica si se encontró el mazo.
        if (mazos.length === 0) {
            response.status(404).send("No se encontró el mazo especificado.");
            return;
        }

        // Al ser un único mazo, simplificamos la lógica tomando el primer elemento
        let mazo = mazos[0];

        // Obtiene las cartas asociadas al mazo, incluyendo el nombre de cada carta.
        const [cartas] = await connection.execute(
            "SELECT mc.id_carta, c.nombre AS nombre_carta FROM mazo_cartas mc JOIN carta c ON mc.id_carta = c.id_carta WHERE mc.id_mazo = ?",
            [idMazo]
        );

        // Asocia las cartas al mazo
        mazo.cartas = cartas.map(carta => ({ id_carta: carta.id_carta, nombre_carta: carta.nombre_carta }));

        response.json(mazo);
    } catch (error) {
        console.error("Error al obtener los detalles del mazo: ", error);
        response.status(500).send("Error al obtener los detalles del mazo.");
    } finally {
        if (connection) {
            // Cierra la conexión a la base de datos.
            await connection.end();
        }
    }
});

// Endpoint para cambiar una carta por otra de un deck en especifico gracias a los ids
app.put("/api/decks/:deckId/cards/swap", async (request, response) => {
    let connection = null;
    try {
        const { deckId } = request.params;  // ID del mazo a modificar
        const { cardToRemove, cardToAdd } = request.body;  // IDs de las cartas a intercambiar

        connection = await connectToDB();

        // Inicia una transacción para manejar las operaciones atómicamente
        await connection.beginTransaction();

        // Verificar que la carta a remover realmente esté en el mazo
        const checkRemoval = await connection.query("SELECT 1 FROM mazo_cartas WHERE id_mazo = ? AND id_carta = ?", [deckId, cardToRemove]);
        if (checkRemoval.length === 0) {
            throw new Error('La carta a remover no se encuentra en el mazo especificado.');
        }

        // Remover la carta antigua
        await connection.query("DELETE FROM mazo_cartas WHERE id_mazo = ? AND id_carta = ?", [deckId, cardToRemove]);

        // Añadir la nueva carta
        await connection.query("INSERT INTO mazo_cartas (id_mazo, id_carta) VALUES (?, ?)", [deckId, cardToAdd]);

        // Confirmar la transacción
        await connection.commit();

        response.send("Intercambio de cartas completado con éxito.");
    } catch (error) {
        // Revertir la transacción en caso de errores
        if (connection) {
            await connection.rollback();
        }
        console.error("Error durante el intercambio de cartas: ", error);
        response.status(500).send("Error durante el intercambio de cartas: " + error.message);
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Endpoint para cambiar el coche de un mazo en específico
app.put("/api/decks/:deckId/car", async (request, response) => {
    let connection = null;
    try {
        const { deckId } = request.params; // ID del mazo a modificar
        const { carName } = request.body; // Nombre del coche a asociar con el mazo

        connection = await connectToDB();

        // Inicia una transacción para manejar las operaciones atómicamente
        await connection.beginTransaction();

        // Obtener el ID del coche basado en el nombre
        const [carResults] = await connection.query("SELECT id_coche FROM coche WHERE nombre = ?", [carName]);
        if (carResults.length === 0) {
            throw new Error('El coche especificado no existe.');
        }
        const newCarId = carResults[0].id_coche; // Asumimos que el nombre del coche es único

        // Actualizar el coche del mazo
        const updateResult = await connection.query("UPDATE mazo SET id_coche = ? WHERE id_mazo = ?", [newCarId, deckId]);
        if (updateResult.affectedRows === 0) {
            throw new Error('No se pudo actualizar el coche del mazo o el mazo no existe.');
        }

        // Confirmar la transacción
        await connection.commit();

        response.send("Coche del mazo actualizado con éxito.");
    } catch (error) {
        // Revertir la transacción en caso de errores
        if (connection) {
            await connection.rollback();
        }
        console.error("Error durante la actualización del coche del mazo: ", error);
        response.status(500).send("Error durante la actualización del coche del mazo: " + error.message);
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Endpoint para obtener todos los efectos
app.get("/api/effects", async (request, response) => {
    let connection = null;
    try {
        connection = await connectToDB();
        const [results] = await connection.execute("SELECT * FROM efecto");
        response.json(results);
    } catch (error) {
        console.error("Error al obtener los efectos: ", error);
        response.status(500).send("Error al obtener los efectos");
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Endpoint para obtener todos los escenarios
app.get("/api/scenarios", async (request, response) => {
    let connection = null;
    try {
        connection = await connectToDB();
        const [results] = await connection.execute("SELECT * FROM escenario");
        response.json(results);
    } catch (error) {
        console.error("Error al obtener los escenarios: ", error);
        response.status(500).send("Error al obtener los escenarios");
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

app.listen(port, () => {
    console.log(`Server running on port ${port}`);
  });