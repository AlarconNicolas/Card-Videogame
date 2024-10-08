using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Importante para trabajar con la UI.

public class AIUISymbol : MonoBehaviour
{
    public AI aiScript; // Referencia al script de AI en el oponente.
    public GameObject speedSymbol, defenseSymbol, attackSymbol; // Asigna estos en el Inspector.
    
    // Ejemplo de parámetros, deberías reemplazarlos con tus propios métodos o variables.
    private int healthLevel = 20;
    private int distanceToGoal = 80;
    private int cardStrength = 4;
    private int energyLevel = 8;

    private void Update()
    {
        // Oculta todos los símbolos primero
        speedSymbol.SetActive(false);
        defenseSymbol.SetActive(false);
        attackSymbol.SetActive(false);

        // Muestra el símbolo basado en la última acción determinada por la IA
        // Aquí pasas los parámetros actuales de tu juego, como el nivel de salud, distancia, etc.
        switch (aiScript.DetermineAIAction(healthLevel, distanceToGoal, cardStrength, energyLevel))
        {
            case AI.AIAction.Speed:
                speedSymbol.SetActive(true);
                break;
            case AI.AIAction.Defense:
                defenseSymbol.SetActive(true);
                break;
            case AI.AIAction.Attack:
                attackSymbol.SetActive(true);
                break;
        }
    }

    // Aquí podrías agregar métodos para actualizar los parámetros basados en tu lógica de juego
    // Por ejemplo, actualizar el nivel de salud del oponente, la distancia al objetivo, etc.
}
