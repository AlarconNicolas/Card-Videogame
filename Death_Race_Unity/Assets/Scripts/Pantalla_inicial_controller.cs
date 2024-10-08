/*
Controlador de la pantalla principal, un comando unico llevar al menu principal
19/03/2024 - Nicolas Alarcon Panopoulou
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class Pantalla_inicial_controller : MonoBehaviour
{
    public void Login(){
        SceneManager.LoadScene("Login");
    }

    public void LoadMenuPrincipal(){
        SceneManager.LoadScene("Menu_principal");
    }
}
