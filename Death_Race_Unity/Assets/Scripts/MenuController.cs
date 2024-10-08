using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void LoadJuego(){
        SceneManager.LoadScene("Juego");
    }

    public void LoadMazo(){
        SceneManager.LoadScene("Creacion_mazo");
    }

    public void LoadEstadistica(){
        SceneManager.LoadScene("Estadisticas");
    }

    public void LoadSalir(){
        SceneManager.LoadScene("Login");
    }

}
