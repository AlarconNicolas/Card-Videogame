using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DeckController : MonoBehaviour
{
    public static DeckController Instance { get; private set; }
    private Button[] carButtons;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Destruir duplicados
        }
    }

    void Start() {
        StartCoroutine(WaitForDeckToBeLoaded());
        MazoAPI.Instance.OnDeckUpdated += DisableItemsInInventory;

        // Subscribe to CarsSwap.OnCarsSwapped to update buttons
        if (CarsSwap.Instance != null) {
            CarsSwap.Instance.OnCarsSwapped += UpdateButtonInteractivity;
        }
    }

    private void OnDestroy()
    {
        // Asegúrate de desuscribirte cuando este objeto se destruya
        if (MazoAPI.Instance != null)
        {
            MazoAPI.Instance.OnDeckUpdated -= DisableItemsInInventory;
            CarsSwap.Instance.OnCarsSwapped -= UpdateButtonInteractivity;
        }
    }

    private IEnumerator WaitForDeckToBeLoaded()
    {
        // Esperamos que MazoAPI se inicialice y tenga un mazo cargado
        yield return new WaitUntil(() => MazoAPI.Instance != null && MazoAPI.Instance.currentDeck != null);

        // Llamamos a desactivar los ítems una vez que el mazo está cargado
        DisableItemsInInventory();
    }

    private void UpdateButtonInteractivity()
    {
        if (MazoAPI.Instance == null || MazoAPI.Instance.currentDeck == null)
        {
            Debug.LogError("MazoAPI is not initialized or currentDeck is null.");
            return;
        }

        // Update car buttons
        var deckCarName = MazoAPI.Instance.currentDeck.nombre_coche;

       carButtons = new Button[] { GameObject.Find("CocheVerde1").GetComponent<Button>(), 
                                GameObject.Find("CocheAmarillo1").GetComponent<Button>(),
                                GameObject.Find("CocheRojo1").GetComponent<Button>() };

        foreach (var button in carButtons)
        {
            if (button == null) continue;

            string carName = button.name.Replace("1", "");
            bool isInDeck = carName == deckCarName;
            bool isInInventory = CarsAPI.Instance.IsCarInInventory(carName);

            button.interactable = isInInventory && !isInDeck;
            button.GetComponent<Image>().color = button.interactable ? Color.white : Color.gray;
        }
    }

    public void DisableItemsInInventory()
    {
        if (MazoAPI.Instance != null && MazoAPI.Instance.currentDeck != null && MazoAPI.Instance.currentDeck.cartas != null)
        {
            // Obtener el conjunto de IDs de cartas en el inventario
            HashSet<int> inventoryCardIds = InventoryAPI.Instance.GetInventoryCardIds();
            // Obtener el conjunto de IDs de cartas en el mazo actual
            HashSet<int> deckCardIds = new HashSet<int>(MazoAPI.Instance.currentDeck.cartas.Select(c => c.id_carta));

            // Desactivar botones "Invent" de cartas no presentes en el inventario o ya presentes en el mazo
            Button[] buttons = FindObjectsOfType<Button>();
            foreach (Button button in buttons)
            {
                if (button.name.StartsWith("Invent"))
                {
                    string idStr = button.name.Replace("Invent", "");
                    if (int.TryParse(idStr, out int cardId))
                    {
                        bool isInInventory = inventoryCardIds.Contains(cardId);
                        bool isInDeck = deckCardIds.Contains(cardId);
                        button.interactable = isInInventory && !isInDeck;
                        var image = button.GetComponent<Image>();
                        if (image != null)
                        {
                            image.color = button.interactable ? Color.white : Color.gray;
                        }
                    }
                }
            }

            // Desactivar botón del coche si no está en el inventario o ya está en el mazo
            if (!string.IsNullOrEmpty(MazoAPI.Instance.currentDeck.nombre_coche))
            {
                DisableButton(MazoAPI.Instance.currentDeck.nombre_coche + "1", MazoAPI.Instance.currentDeck.nombre_coche, inventoryCardIds, deckCardIds);
            }
        }
        else
        {
            Debug.LogError("MazoAPI.Instance, currentDeck o cartas están nulos.");
        }
    }

    public void DisableButton(string buttonName, string itemName, HashSet<int> inventoryCardIds, HashSet<int> deckCardIds)
    {
        var buttonGameObject = GameObject.Find(buttonName);
        if (buttonGameObject != null)
        {
            var button = buttonGameObject.GetComponent<Button>();
            if (button != null)
            {
                // Verificar si el coche está en el inventario y no está en el mazo
                bool isInInventory = InventoryAPI.Instance.IsCarInInventory(itemName);
                bool isInDeck = MazoAPI.Instance.currentDeck.nombre_coche == itemName;

                button.interactable = isInInventory && !isInDeck;
                var image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.color = button.interactable ? Color.white : Color.gray;
                }
            }
            else
            {
                Debug.LogError($"No se encontró el componente Button en el GameObject '{buttonName}'.");
            }
        }
        else
        {
            Debug.LogError($"No se encontró el botón '{buttonName}'. Asegúrate de que esté bien escrito y activo en la escena.");
        }
    }

    // Método para recibir y procesar los datos del inventario del jugador
    public void ReceiveInventoryData(ColeccionJugador coleccionJugador)
    {
        Debug.Log($"Datos de inventario recibidos en DeckController: {coleccionJugador.cartas_ids}, {coleccionJugador.coches_nombres}");

        // Procesar cualquier lógica adicional que dependa de los datos del inventario
    }
}