using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Define una clase para almacenar los datos recibidos del servidor
[System.Serializable]
public class ColeccionJugador
{
    public int id_jugador;
    public string cartas_ids;  // Lista de IDs de cartas como cadena
    public string coches_nombres;  // Nombres de los coches como cadena
}

public class InventoryAPI : MonoBehaviour
{
    public static InventoryAPI Instance { get; private set; }
    public ColeccionJugador coleccionJugador; // Objeto para almacenar la colección del jugador

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destruye cualquier duplicado que se cree.
        }
    }

    void Start()
    {
        // Suscribirse al evento OnPlayerIdChanged de PlayerData
        PlayerData.Instance.OnPlayerIdChanged += OnPlayerIdChanged;

        // Inicia la solicitud HTTP para obtener los datos del jugador
        if (PlayerData.Instance != null)
        {
            StartCoroutine(GetColeccionJugador(PlayerData.Instance.PlayerId));
        }
        else
        {
            Debug.LogError("PlayerData.Instance no está disponible.");
        }
    }

    // Método que se llama cada vez que cambia el ID del jugador en PlayerData
    void OnPlayerIdChanged(int newPlayerId)
    {
        StartCoroutine(GetColeccionJugador(newPlayerId));
    }

    IEnumerator GetColeccionJugador(int idJugador)
    {
        string url = $"http://localhost:3000/api/coleccion/{idJugador}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al conectarse al servidor: " + webRequest.error);
            }
            else if (webRequest.downloadHandler.text.Contains("Colección de cartas no encontrada"))
            {
                Debug.LogError("Colección de cartas no encontrada para el jugador: " + idJugador);
            }
            else
            {
                coleccionJugador = JsonUtility.FromJson<ColeccionJugador>(webRequest.downloadHandler.text);
            }
        }
    }

    // Asegúrate de desuscribirte del evento al destruir el objeto
    void OnDestroy()
    {
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.OnPlayerIdChanged -= OnPlayerIdChanged;
        }
    }

    // Método para obtener el conjunto de IDs de cartas en el inventario
    public HashSet<int> GetInventoryCardIds()
    {
        return new HashSet<int>(coleccionJugador.cartas_ids.Split(',').Select(int.Parse));
    }

    // Método para verificar si un coche específico está en el inventario
    public bool IsCarInInventory(string carName)
    {
        return coleccionJugador.coches_nombres.Split(',').Contains(carName);
    }
}
