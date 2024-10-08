using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class JugadorIdAPI : MonoBehaviour
{
    [System.Serializable]
    public class PlayerResponse
    {
        public bool found;
        public Player player; 
    }

    [System.Serializable]
    public class Player
    {
        public int id_jugador;
        public string nombre;
        public string correo;
    }

    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public Button submitButton;
    public Text feedbackText;
    private string playerApiUrl = "http://localhost:3000/api/players/";
    private string passwordApiUrl = "http://localhost:3000/api/password/";
    private string realPassword = "";

    void Awake()
    {
        FindComponents();
        AddEventListeners();
    }

    private void FindComponents()
    {
        usernameInputField = GameObject.Find("InputFieldUsername (TMP)")?.GetComponent<TMP_InputField>();
        passwordInputField = GameObject.Find("InputFieldPassword (TMP)")?.GetComponent<TMP_InputField>();
        submitButton = GameObject.Find("Submit")?.GetComponent<Button>();
        feedbackText = GameObject.Find("FeedbackText")?.GetComponent<Text>();

        if (usernameInputField == null || passwordInputField == null || submitButton == null || feedbackText == null)
        {
            Debug.LogError("No se encontraron uno o más componentes requeridos.");
        }
    }

    private void AddEventListeners()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SubmitButtonClicked);
        }
        if (usernameInputField != null)
        {
            usernameInputField.onValueChanged.AddListener(delegate { ValidateInput(); });
        }
        if (passwordInputField != null)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
            passwordInputField.onValueChanged.AddListener(delegate(string input) {
                realPassword = input;
                ValidateInput();
            });
        }
    }

    public void ValidateInput()
    {
        bool isUsernameNotEmpty = !string.IsNullOrEmpty(usernameInputField?.text.Trim());
        bool isPasswordNotEmpty = !string.IsNullOrEmpty(realPassword.Trim());
        submitButton.interactable = isUsernameNotEmpty && isPasswordNotEmpty;
    }

    public void SubmitButtonClicked()
    {
        if (usernameInputField != null && passwordInputField != null)
        {
            StartCoroutine(ValidateCredentials(usernameInputField.text.Trim(), realPassword));
        }
    }

    IEnumerator ValidateCredentials(string username, string password)
    {
        int playerIdByUsername = 0;
        int playerIdByPassword = 0;

        yield return StartCoroutine(GetPlayerIdByUsername(username, result => {
            playerIdByUsername = result;
        }));

        if (playerIdByUsername == 0)
        {
            yield break;
        }

        yield return StartCoroutine(GetPlayerIdByPassword(password, result => {
            playerIdByPassword = result;
        }));

        if (playerIdByUsername != 0 && playerIdByUsername == playerIdByPassword)
        {
            if (PlayerData.Instance != null)
            {
                PlayerData.Instance.SetPlayerId(playerIdByUsername);
                SceneManager.LoadScene("Menu_principal");
            }
            else
            {
                Debug.LogError("PlayerData.Instance no está accesible.");
            }
        }
        else
        {
            DisplayErrorMessage("Los datos no coinciden o la contraseña es incorrecta.");
        }
    }

    IEnumerator GetPlayerIdByUsername(string username, System.Action<int> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(playerApiUrl + UnityWebRequest.EscapeURL(username));
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.Success)
        {
            PlayerResponse response = JsonUtility.FromJson<PlayerResponse>(request.downloadHandler.text);
            if (response.found)
            {
                callback.Invoke(response.player.id_jugador);
            }
            else
            {
                if (passwordInputField != null)
                {
                    passwordInputField.text = "";
                }
                if (usernameInputField != null)
                {
                    usernameInputField.text = "";
                }
                callback.Invoke(0);
            }
        }
        else
        {
            DisplayErrorMessage("Error de conexión: " + request.error);
            callback.Invoke(0);
        }
    }

    IEnumerator GetPlayerIdByPassword(string password, System.Action<int> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(passwordApiUrl + UnityWebRequest.EscapeURL(password));
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.Success)
        {
            PlayerResponse response = JsonUtility.FromJson<PlayerResponse>(request.downloadHandler.text);
            if (response.found)
            {
                callback.Invoke(response.player.id_jugador);
            }
        }
        else
        {
            if (passwordInputField != null)
            {
                passwordInputField.text = "";
            }
            if (usernameInputField != null)
            {
                usernameInputField.text = "";
            }
            callback.Invoke(0);
        }
    }

    void DisplayErrorMessage(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
        else
        {
            Debug.LogError("Error: " + message);
        }
    }

    void HideFeedbackText()
    {
        if (feedbackText != null)
        {
            feedbackText.enabled = false;
        }
    }

    void ShowFeedbackText()
    {
        if (feedbackText != null)
        {
            feedbackText.enabled = true;
        }
    }
}
