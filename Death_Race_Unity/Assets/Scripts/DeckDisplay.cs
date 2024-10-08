using UnityEngine;
using UnityEngine.UI;

public class DeckDisplay : MonoBehaviour
{
    private void Start()
    {
        MazoAPI.Instance.OnDeckUpdated += UpdateDisplay;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (MazoAPI.Instance != null)
        {
            MazoAPI.Instance.OnDeckUpdated -= UpdateDisplay;
        }
    }

    private void UpdateDisplay()
    {
        // Assign card images and set up buttons
        for (int i = 0; i < MazoAPI.Instance.currentDeck.cartas.Count; i++)
        {
            if (i >= 8) break;  // Ensure there are not more cards than buttons

            string buttonName = "Carta" + (i + 1);
            Button cardButton = GameObject.Find(buttonName)?.GetComponent<Button>();
            if (cardButton != null)
            {
                int cardId = MazoAPI.Instance.currentDeck.cartas[i].id_carta;
                string cardImage = cardId.ToString();
                Sprite cardSprite = Resources.Load<Sprite>(cardImage);

                if (cardSprite != null)
                {
                    cardButton.image.sprite = cardSprite;
                }
                else
                {
                    Debug.LogError("Failed to load sprite for card ID: " + cardImage);
                }

                // Update the button mapping and set click listener
                CardSwap.Instance.UpdateButtonCardMapping(buttonName, cardId);
                cardButton.onClick.RemoveAllListeners();
                cardButton.onClick.AddListener(() => CardSwap.Instance.SetSelectedCardId(buttonName));
            }
            else
            {
                Debug.LogError("Failed to find button: " + buttonName);
            }
        }

        // Assign car image to Coche button
        AssignCarImage();
    }

    private void AssignCarImage()
    {
        string carName = MazoAPI.Instance.currentDeck.nombre_coche;
        Button carButton = GameObject.Find("Coche")?.GetComponent<Button>();
        if (carButton != null)
        {
            Sprite carSprite = Resources.Load<Sprite>(carName);
            if (carSprite != null)
            {
                carButton.image.sprite = carSprite;
            }
            else
            {
                Debug.LogError("Failed to load sprite for car: " + carName);
            }
        }
        else
        {
            Debug.LogError("Failed to find button: Coche");
        }
    }
}
