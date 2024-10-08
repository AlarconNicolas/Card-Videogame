using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class StatsAPI : MonoBehaviour
{
    [SerializeField] private TMP_Text victoriesText;
    [SerializeField] private TMP_Text mostUsedCardText;
    [SerializeField] private TMP_Text leastUsedCardText;
    [SerializeField] private TMP_Text averageTurnsText;

    private void Start()
    {
        StartCoroutine(GetPlayerStatistics());
    }

    IEnumerator GetPlayerStatistics()
    {
        int playerId = PlayerData.Instance.PlayerId;
        string summaryUrl = $"http://localhost:3000/api/players/{playerId}/summary";

        using (UnityWebRequest summaryRequest = UnityWebRequest.Get(summaryUrl))
        {
            yield return summaryRequest.SendWebRequest();

            if (summaryRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error obtaining player stats: {summaryRequest.error}");
                yield break;
            }

            PlayerStatistics stats = JsonUtility.FromJson<PlayerStatistics>(summaryRequest.downloadHandler.text);

            // Obtiene el nombre de la carta más usada
            yield return StartCoroutine(GetCardName(stats.mostUsedCard, name => mostUsedCardText.text = $"{name}"));

            // Obtiene el nombre de la carta menos usada
            yield return StartCoroutine(GetCardName(stats.leastUsedCard, name => leastUsedCardText.text = $"{name}"));

            // Actualiza el resto de la UI
            victoriesText.text = $"{stats.victories}";
            averageTurnsText.text = $"{stats.averageTurns}";
        }
    }

    IEnumerator GetCardName(int cardId, Action<string> callback)
    {
        string cardUrl = $"http://localhost:3000/api/cards/{cardId}";

        using (UnityWebRequest cardRequest = UnityWebRequest.Get(cardUrl))
        {
            yield return cardRequest.SendWebRequest();

            if (cardRequest.result == UnityWebRequest.Result.Success)
            {
                Card card = JsonUtility.FromJson<Card>(cardRequest.downloadHandler.text);
                callback?.Invoke(card.nombre);
            }
            else
            {
                Debug.LogError($"Failed to get card with ID {cardId}: {cardRequest.error}");
                callback?.Invoke("Desconocida");
            }
        }
    }

    [System.Serializable]
    private class PlayerStatistics
    {
        public int victories;
        public int mostUsedCard;
        public int leastUsedCard;
        public string averageTurns;
    }

    [System.Serializable]
    private class Card
    {
        public string nombre;
    }
}