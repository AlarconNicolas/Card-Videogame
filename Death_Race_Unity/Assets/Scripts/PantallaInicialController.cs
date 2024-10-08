using UnityEngine;
using UnityEngine.SceneManagement;

public class PantallaInicialController : MonoBehaviour
{
    public void Login(){
        SceneManager.LoadScene("Login");
    }

    public void LoadMenuPrincipal(){
        SceneManager.LoadScene("Menu_principal");
    }
}
