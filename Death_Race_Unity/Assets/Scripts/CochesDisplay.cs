using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CochesDisplay : MonoBehaviour
{
    public static CochesDisplay Instance;

    void Start()
    {
        // Iniciamos la carga de la información de coches
        StartCoroutine(WaitForCarsToBeLoaded());
        // Suscribirse al evento de actualización de coches de CarsAPI
        if (CarsAPI.Instance != null)
        {
            CarsAPI.Instance.OnCarsUpdated += DisplayCars;
        }
    }

    public IEnumerator WaitForCarsToBeLoaded() // Cambiado a public
    {
        // Esperamos a que la lista de coches no sea null y tenga al menos un coche
        yield return new WaitUntil(() => CarsAPI.Instance != null && CarsAPI.Instance.Cars.Count > 0);
        DisplayCars();
    }

    private void OnDestroy()
    {
        // Desuscribirse del evento cuando el objeto sea destruido
        if (CarsAPI.Instance != null)
        {
            CarsAPI.Instance.OnCarsUpdated -= DisplayCars;
        }
    }

    private void DisplayCars()
    {
        foreach (var car in CarsAPI.Instance.Cars)
        {
            // Usamos el nombre del coche para buscar el botón correspondiente
            string buttonName = car.nombre + "1";
            var carButtonGameObject = GameObject.Find(buttonName);
            if (carButtonGameObject == null)
            {
                Debug.LogError($"No se encontró el botón '{buttonName}'. Asegúrate de que esté bien escrito y activo en la escena.");
                continue;
            }

            var carButton = carButtonGameObject.GetComponent<Button>();
            if (carButton == null)
            {
                Debug.LogError($"No se encontró el componente Button en el GameObject '{buttonName}'.");
                continue;
            }

            // Cargamos el sprite usando el nombre del coche
            Sprite carSprite = Resources.Load<Sprite>(car.nombre);
            if (carSprite != null)
            {
                carButton.image.sprite = carSprite;
            }
            else
            {
                Debug.LogError($"No se pudo cargar el sprite para el coche con nombre {car.nombre}");
            }

            // Actualizamos el texto del botón con el nombre del coche, si es necesario
            var textComponent = carButton.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = car.nombre;
            }
        }
    }
}
