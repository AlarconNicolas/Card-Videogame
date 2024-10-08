using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CardsAPI : MonoBehaviour{
    // Clase para adaptarse a la estructura del JSON
    [System.Serializable]
    public class Card{
        public int id_carta;
        public string nombre;
        public string tipo;
        public int costo;
        public int puntos_dano;
        public int puntos_defensa;
        public int puntos_velocidad;
        public string efecto;
        public string descripcion;
    }

    [System.Serializable]
    public class CardArray{
        public Card[] cards;
    }

    // URL de tu endpoint de API
    private string apiUrl = "http://localhost:3000/api/cards";

    // Lista para almacenar todas las cartas
    public List<Card> Cards = new List<Card>();

    public static CardsAPI Instance;

    void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }
    // Iniciar la solicitud a la API al comienzo
    void Start(){
        StartCoroutine(GetCardsCoroutine());
    }

    // Coroutina para realizar la solicitud GET al endpoint de cartas
    IEnumerator GetCardsCoroutine(){
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl)){
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success){
                Debug.LogError("Error al obtener las cartas: " + www.error);
            }
            else{
                ProcessCardsResponse(www.downloadHandler.text);
            }
        }
    }

    // Procesar la respuesta de la API y almacenar las cartas
    private void ProcessCardsResponse(string jsonResponse){
        string adjustedJson = "{\"cards\":" + jsonResponse + "}";
        CardArray cardArray = JsonUtility.FromJson<CardArray>(adjustedJson);
        Cards.Clear(); 
        Cards = new List<Card>();

        foreach(var card in cardArray.cards){
            Cards.Add(card);
        }
    }

    // Método para obtener una carta por su ID
    public Card GetCardById(int id){
        return Cards.Find(card => card.id_carta == id);
    }
}