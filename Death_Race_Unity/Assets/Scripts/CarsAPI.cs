using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CarsAPI : MonoBehaviour
{
    [System.Serializable]
    public class Car
    {
        public int id_coche;
        public string nombre;
        public int vida;
        public int velocidad;
        public int efecto_ataque;
        public int efecto_velocidad;
        public int efecto_defensa;
    }

    [System.Serializable]
    public class CarArray
    {
        public Car[] cars;
    }

    private string apiUrl = "http://localhost:3000/api/cars";
    public static CarsAPI Instance;
    public List<Car> Cars = new List<Car>();

    // Evento para notificar cuando la lista de coches se actualice
    public event System.Action OnCarsUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(GetCarsCoroutine());
    }

    public IEnumerator GetCarsCoroutine() // Cambiado a public
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al obtener los coches: " + www.error);
            }
            else
            {
                ProcessCarsResponse(www.downloadHandler.text);
                OnCarsUpdated?.Invoke();  // Notificar a cualquier suscriptor que los coches han sido actualizados
            }
        }
    }

    private void ProcessCarsResponse(string jsonResponse)
    {
        try
        {
            string adjustedJson = "{\"cars\":" + jsonResponse + "}";
            CarArray carArray = JsonUtility.FromJson<CarArray>(adjustedJson);

            Cars.Clear();

            foreach (Car car in carArray.cars)
            {
                Cars.Add(car);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error al procesar la respuesta de coches: " + ex.Message);
        }
    }

    public bool IsCarInInventory(string carName)
    {
        return Cars.Exists(car => car.nombre == carName);
    }

}
