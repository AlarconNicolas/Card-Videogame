using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;  // Importación necesaria para UnityWebRequest
using System;

public class CardSwap : MonoBehaviour
{
    public static CardSwap Instance;
    public event Action OnCardsSwapped;
    public int selectedCardId = 0;
    public int selectedInventoryId = 0;

    private string apiUrl = "http://localhost:3000/api/decks/{DeckId}/cards/swap";
    private Dictionary<string, int> buttonToCardIdMap = new Dictionary<string, int>();

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
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            if (button.name.StartsWith("Carta"))
            {
                button.onClick.AddListener(() => SetSelectedCardId(button.name));
            }
            else if (button.name.StartsWith("Invent"))
            {
                button.onClick.AddListener(() => SetSelectedInventoryId(button.name));
            }
        }
    }

    public void SetSelectedCardId(string buttonName)
    {
        if (buttonToCardIdMap.TryGetValue(buttonName, out int cardId))
        {
            // Toggle the selection of the card ID
            if (selectedCardId == cardId)
            {
                selectedCardId = 0;
            }
            else
            {
                selectedCardId = cardId;
            }
            TrySwapCards();
        }
        else
        {
            Debug.LogError("No card ID found for button: " + buttonName);
        }
    }

    public void SetSelectedInventoryId(string buttonName)
    {
        string idStr = buttonName.Replace("Invent", "");
        if (int.TryParse(idStr, out int id))
        {
            // Toggle the selection of the inventory ID
            if (selectedInventoryId == id)
            {
                selectedInventoryId = 0;
            }
            else
            {
                selectedInventoryId = id;
            }
            TrySwapCards();
        }
    }

    private void TrySwapCards()
    {
        if (selectedCardId != 0 && selectedInventoryId != 0)
        {
            int deckId = PlayerData.Instance.PlayerId; // Asumiendo que PlayerData almacena el ID del jugador/deck
            StartCoroutine(SwapCards(deckId, selectedInventoryId));
        }
    }

    private IEnumerator SwapCards(int deckId, int cardToAdd)
    {
        string json = CreateJsonSwapData(selectedCardId, cardToAdd);
        string url = apiUrl.Replace("{DeckId}", deckId.ToString());

        UnityWebRequest request = UnityWebRequest.Put(url, json);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Error en la solicitud: " + request.error);
        }
        else
        {
            Debug.Log("Respuesta del servidor: " + request.downloadHandler.text);
            OnCardsSwapped?.Invoke(); // Disparar el evento cuando el intercambio es exitoso
        }

        selectedCardId = 0; // Reset IDs después de completar la solicitud
        selectedInventoryId = 0;
    }

    private string CreateJsonSwapData(int cardToRemove, int cardToAdd)
    {
        return $"{{\"cardToRemove\": {cardToRemove}, \"cardToAdd\": {cardToAdd}}}";
    }

    public void UpdateButtonCardMapping(string buttonName, int cardId)
    {
        if (buttonToCardIdMap.ContainsKey(buttonName))
        {
            buttonToCardIdMap[buttonName] = cardId;
        }
        else
        {
            buttonToCardIdMap.Add(buttonName, cardId);
        }
    }
}
