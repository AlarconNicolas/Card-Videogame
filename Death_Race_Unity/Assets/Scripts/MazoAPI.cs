using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class MazoAPI : MonoBehaviour
{
    public static MazoAPI Instance;

    [System.Serializable]
    public class Deck
    {
        public int id_mazo;
        public string nombre;
        public string nombre_coche;
        public List<Card> cartas;
    }

    [System.Serializable]
    public class Card
    {
        public int id_carta;
        public string nombre_carta;
    }

    public Deck currentDeck;
    public event Action OnDeckUpdated;

    void Awake()
    {
        Debug.Log("MazoAPI Awake");
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (PlayerData.Instance != null)
        {
            FetchMazoDetails(PlayerData.Instance.PlayerId);
            PlayerData.Instance.OnPlayerIdChanged += OnPlayerIdChangedHandler;
            CardSwap.Instance.OnCardsSwapped += UpdateDeck;
            CarsSwap.Instance.OnCarsSwapped += UpdateDeck;
        }
        else
        {
            Debug.LogError("PlayerData no está disponible.");
        }
    }

    void OnDestroy()
    {
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.OnPlayerIdChanged -= OnPlayerIdChangedHandler;
            CardSwap.Instance.OnCardsSwapped -= UpdateDeck;
            CarsSwap.Instance.OnCarsSwapped -= UpdateDeck;
        }
    }

    private void UpdateDeck()
    {
        if (PlayerData.Instance != null)
        {
            Debug.Log("Updating deck details...");
            ClearCurrentDeck();
            FetchMazoDetails(PlayerData.Instance.PlayerId);
        }
    }

    private void ClearCurrentDeck()
    {
        Debug.Log("Clearing current deck...");
        if (currentDeck != null)
        {
            currentDeck.cartas.Clear();
            currentDeck.nombre = string.Empty;
            currentDeck.nombre_coche = string.Empty;
        }
        else
        {
            currentDeck = new Deck();
        }
    }

    private void OnPlayerIdChangedHandler(int newPlayerId)
    {
        Debug.Log($"Player ID changed to {newPlayerId}, fetching deck details...");
        FetchMazoDetails(newPlayerId);
    }

    public void FetchMazoDetails(int playerId)
    {
        Debug.Log($"Fetching deck details for Player ID: {playerId}");
        StartCoroutine(GetMazoCoroutine($"http://localhost:3000/api/mazo/{playerId}"));
    }

    IEnumerator GetMazoCoroutine(string uri)
    {
        Debug.Log($"Sending request to {uri}");
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error while fetching deck details: " + www.error);
            }
            else
            {
                Debug.Log("Response received: " + www.downloadHandler.text);
                try
                {
                    ClearCurrentDeck();
                    currentDeck = JsonUtility.FromJson<Deck>(www.downloadHandler.text);
                    Debug.Log("Deck successfully updated.");
                    OnDeckUpdated?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error deserializing deck details: " + ex.Message);
                }
            }
        }
    }
}
