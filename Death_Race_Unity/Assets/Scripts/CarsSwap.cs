using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;

public class CarsSwap : MonoBehaviour
{
    public static CarsSwap Instance;
    public string coloredCarName; // Nombre del coche seleccionado
    public bool isCarSelected = false; // Indica si un coche está seleccionado
    public Text responseText; // Elemento UI de texto para mostrar respuestas del servidor o errores
    public event Action OnCarsSwapped;

    [SerializeField] private string apiBaseUrl = "http://localhost:3000/api"; // Base URL para API

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // Asignar eventos a los botones de coches
        AssignButtonEvent("Coche", (button) => {
            isCarSelected = !isCarSelected; // Alternar selección de coche
            UpdateDisplayText("Car selection toggled: " + isCarSelected); // Actualizar texto de la UI
        });

        // Botones para coches de colores específicos
        AssignButtonEvent("CocheRojo1", ToggleCarSelection);
        AssignButtonEvent("CocheVerde1", ToggleCarSelection);
        AssignButtonEvent("CocheAmarillo1", ToggleCarSelection);
    }

    private void AssignButtonEvent(string buttonName, Action<Button> action)
    {
        var carButton = GameObject.Find(buttonName)?.GetComponent<Button>();
        if (carButton != null)
        {
            carButton.onClick.AddListener(() => action(carButton)); // Añadir listener
        }
        else
        {
            Debug.LogWarning($"Button not found or does not have a Button component: {buttonName}");
        }
    }

    private void ToggleCarSelection(Button button)
    {
        string newColoredCarName = button.name.Replace("1", ""); // Obtener el nombre del coche del botón

        // Verificar si el nuevo coche seleccionado es el mismo que el actualmente seleccionado
        if (newColoredCarName == coloredCarName)
        {
            // Si es el mismo coche, deseleccionarlo y establecer el nombre del coche en blanco
            coloredCarName = "";
            UpdateDisplayText("Car selection toggled off.");
        }
        else
        {
            // Si es un coche diferente al actualmente seleccionado, seleccionarlo y actualizar el nombre del coche
            coloredCarName = newColoredCarName;
            UpdateDisplayText("Car selected: " + coloredCarName);
            UpdateCarForDeck(coloredCarName); // Actualizar coche si está seleccionado
        }
    }

    private void UpdateDisplayText(string text)
    {
        Debug.Log(text); // Log a la consola
        if (responseText != null)
        {
            responseText.text = text; // Actualizar elemento de texto UI
        }
    }

    private void UpdateCarForDeck(string carName)
    {
        // Verificar si un coche está seleccionado y si el nombre del coche no está vacío
        if (isCarSelected && !string.IsNullOrEmpty(carName) || !string.IsNullOrEmpty(carName)  && isCarSelected)
        {
            StartCoroutine(SendCarUpdateRequest(carName)); // Solo enviar solicitud si un coche está seleccionado y el nombre es válido
        }
    }



    private IEnumerator SendCarUpdateRequest(string carName)
    {
        string playerId = PlayerData.Instance.PlayerId.ToString();
        string url = $"{apiBaseUrl}/decks/{playerId}/car";
        var request = new UnityWebRequest(url, "PUT");
        request.SetRequestHeader("Content-Type", "application/json");

        string jsonBody = "{\"carName\": \"" + carName + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al enviar la solicitud: " + request.error);
            UpdateDisplayText("Failed to update car: " + request.error);
        }
        else
        {
            Debug.Log("Solicitud enviada correctamente. Respuesta del servidor: " + request.downloadHandler.text);
            UpdateDisplayText("Car updated successfully!");
            
            // Reiniciar el selector de coches
            isCarSelected = false; // Esto pone en false la selección del coche tras la actualización
            
            // Disparar el evento de coches intercambiados
            OnCarsSwapped?.Invoke();

            // Actualizar la lista de coches y mostrarlos nuevamente en la interfaz de usuario
            if (CarsAPI.Instance != null)
            {
                yield return CarsAPI.Instance.StartCoroutine(CarsAPI.Instance.GetCarsCoroutine());
            }

            // Esperar a que los coches se carguen y mostrarlos nuevamente en la interfaz de usuario
            if (CochesDisplay.Instance != null)
            {
                yield return CochesDisplay.Instance.StartCoroutine(CochesDisplay.Instance.WaitForCarsToBeLoaded());
            }
        }
    }
}
