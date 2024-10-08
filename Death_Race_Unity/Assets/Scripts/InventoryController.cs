using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    public InventoryAPI inventoryAPI; // Referencia al script InventoryAPI
    private Dictionary<string, Button> cartaButtons = new Dictionary<string, Button>(); // Diccionario de botones de cartas (nombre del botón como clave)
    private Dictionary<string, Button> cocheButtons = new Dictionary<string, Button>(); // Diccionario de botones de coches (nombre del botón como clave)

    void Start()
    {
        // Buscar el objeto InventoryAPI en tiempo de ejecución
        inventoryAPI = FindObjectOfType<InventoryAPI>();
        if (inventoryAPI == null)
        {
            Debug.LogError("¡El objeto InventoryAPI no se encontró en la escena!");
            return;
        }

        // Obtener todos los botones de cartas y coches en la escena
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons)
        {
            if (button.name.StartsWith("Invent"))
            {
                cartaButtons.Add(button.name, button);
            }
            else if (button.name.StartsWith("Coche") && !char.IsDigit(button.name[5])) // Verificar si es un botón individual de coche
            {
                cocheButtons.Add(button.name, button);
            }
        }

        // Esperar a que InventoryAPI obtenga los datos del jugador
        StartCoroutine(WaitForInventoryData());
    }

    System.Collections.IEnumerator WaitForInventoryData()
    {
        // Esperar hasta que InventoryAPI tenga los datos del jugador
        while (inventoryAPI.coleccionJugador == null)
        {
            Debug.Log("Esperando datos del inventario...");
            yield return null;
        }

        Debug.Log("Datos del inventario recibidos correctamente.");

        // Una vez que los datos estén disponibles, deshabilitar los botones no correspondientes
        DisableUnavailableCards();
        DisableUnavailableCoches();
    }

    private void DisableUnavailableCards()
    {
        // Obtener IDs de cartas disponibles
        HashSet<int> cartasDisponibles = new HashSet<int>();
        string[] ids = inventoryAPI.coleccionJugador.cartas_ids.Split(',');
        foreach (string id in ids)
        {
            if (int.TryParse(id, out int cardId))
            {
                cartasDisponibles.Add(cardId);
            }
        }

        // Deshabilitar botones de cartas no disponibles
        foreach (KeyValuePair<string, Button> pair in cartaButtons)
        {
            int cartaIndex = int.Parse(pair.Key.Replace("Invent", ""));
            if (!cartasDisponibles.Contains(cartaIndex))
            {
                pair.Value.interactable = false;
                pair.Value.GetComponent<Image>().color = Color.gray; // Cambia el color a gris para indicar desactivación
                Debug.Log("Carta desactivada: " + pair.Key);
            }
        }
    }

     private void DisableUnavailableCoches()
    {
        // Obtener nombres de coches disponibles
        HashSet<string> cochesDisponibles = new HashSet<string>(inventoryAPI.coleccionJugador.coches_nombres.Split(','));

        // Deshabilitar botones de coches no disponibles
        foreach (KeyValuePair<string, Button> pair in cocheButtons)
        {
            if (!cochesDisponibles.Contains(pair.Key.Substring(0, pair.Key.Length - 1))) // Quita el último carácter del nombre del botón
            {
                pair.Value.interactable = false;
                pair.Value.GetComponent<Image>().color = Color.gray; // Cambia el color a gris para indicar desactivación
                Debug.Log("Coche desactivado: " + pair.Key);
            }
        }
    }
}
