using UnityEngine;
using UnityEngine.UI;
using System.Collections;  // Esto proporciona el IEnumerator correcto para las corutinas

public class InventoryDisplay : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitForCardsToBeLoaded());
    }

    private IEnumerator WaitForCardsToBeLoaded()
    {
        // Esperamos a que la lista de cartas no sea null y tenga al menos una carta
        yield return new WaitUntil(() => CardsAPI.Instance != null && CardsAPI.Instance.Cards.Count > 0);
        DisplayCards();
        DisplayCars();
    }

    private void DisplayCards()
    {
        int maxCards = 30;
        for (int i = 0; i < maxCards && i < CardsAPI.Instance.Cards.Count; i++)
        {
            var cardButtonGameObject = GameObject.Find("Invent" + (i + 1));
            if (cardButtonGameObject == null)
            {
                Debug.LogError($"No se encontró el botón 'Invent{i + 1}'. Asegúrate de que esté bien escrito y activo en la escena.");
                continue;
            }

            var cardButton = cardButtonGameObject.GetComponent<Button>();
            if (cardButton == null)
            {
                Debug.LogError($"No se encontró el componente Button en el GameObject 'Invent{i + 1}'.");
                continue;
            }

            var card = CardsAPI.Instance.Cards[i];
            Sprite cardSprite = Resources.Load<Sprite>($"{card.id_carta}");
            if (cardSprite != null)
            {
                cardButton.image.sprite = cardSprite;
            }
            else
            {
                Debug.LogError($"No se pudo cargar el sprite para la carta con ID {card.id_carta}");
            }

            var textComponent = cardButton.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = card.nombre;
            }
        }
    }

    private void DisplayCars()
    {
        if (CarsAPI.Instance.Cars.Count == 0)
        {
            Debug.LogError("DisplayCars: No cars loaded.");
            return;
        }

        foreach (var car in CarsAPI.Instance.Cars)
        {
            string buttonName = car.nombre + "1";
            Debug.Log($"Attempting to assign {car.nombre} to {buttonName}.");
            AssignCarToButton(buttonName, car);
        }
    }

    private void AssignCarToButton(string buttonName, CarsAPI.Car car)
    {
        var button = GameObject.Find(buttonName)?.GetComponent<Button>();
        if (button != null)
        {
            var textComponent = button.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = car.nombre;
            }

            // Correctly load the sprite using the car's name
            Sprite carSprite = Resources.Load<Sprite>($"{car.nombre}");
            if (carSprite != null)
            {
                button.image.sprite = carSprite;
            }
            else
            {
                Debug.LogError($"No sprite found for car name '{car.nombre}'. Make sure it's correctly placed in the Resources folder and named appropriately.");
            }
        }
        else
        {
            Debug.LogError($"Button not found: {buttonName}. Make sure the button exists and is correctly named in the scene.");
        }
    }
}