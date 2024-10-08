using System;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public int speedCardCount;
    public int defenseCardCount;
    public int attackCardCount;

    public enum AIAction { Speed, Defense, Attack }

    private System.Random random = new System.Random();

    // Cambiamos los parámetros para utilizar valores numéricos
    public AIAction DetermineAIAction(int healthLevel, int distanceToGoal, int cardStrength, int energyLevel)
    {
        AIAction chosenAction = AIAction.Attack; // Default action

        List<AIAction> possibleActions = new List<AIAction>();

        // Convierte los rangos numéricos en condiciones booleanas
        bool healthIsOk = healthLevel >= 1 && healthLevel <= 35;
        bool distanceIsOk = distanceToGoal >= 75 && distanceToGoal <= 99;
        bool cardStrengthIsOk = cardStrength >= 4 && cardStrength <= 5;
        bool energyLevelIsOk = energyLevel >= 7 && energyLevel <= 10;

        if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal >= 75 && distanceToGoal <= 99 && cardStrength >= 4 && cardStrength <= 5 && energyLevel >= 7 && energyLevel <= 10)
{
    chosenAction = AIAction.Speed;
}
else if (healthLevel > 35 && distanceToGoal >= 75 && distanceToGoal <= 99 && cardStrength >= 4 && cardStrength <= 5 && energyLevel >= 7 && energyLevel <= 10)
{
    chosenAction = AIAction.Speed;
}
else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal < 75 && cardStrength >= 4 && cardStrength <= 5 && energyLevel >= 7 && energyLevel <= 10)
{
    chosenAction = AIAction.Defense;
}
else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal < 75 && cardStrength < 4 && energyLevel >= 7 && energyLevel <= 10)
{
    chosenAction = AIAction.Attack;
}
else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal >= 75 && distanceToGoal <= 99 && cardStrength < 4 && energyLevel >= 7 && energyLevel <= 10)
{
    chosenAction = AIAction.Speed;
}
else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal < 75 && cardStrength < 4 && energyLevel < 7)
{
    chosenAction = AIAction.Defense;
}
else if (healthLevel > 35 && distanceToGoal < 75 && cardStrength < 4 && energyLevel >= 7 && energyLevel <= 10)
{
    chosenAction = AIAction.Attack;
}
else if (healthLevel > 35 && distanceToGoal >= 75 && distanceToGoal <= 99 && cardStrength >= 4 && cardStrength <= 5 && energyLevel < 7)
{
    chosenAction = AIAction.Speed;
}
else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal >= 75 && distanceToGoal <= 99 && cardStrength >= 4 && cardStrength <= 5 && energyLevel < 7)
{
    chosenAction = AIAction.Defense;
}
else if (healthLevel > 35 && distanceToGoal < 75 && cardStrength >= 4 && cardStrength <= 5 && energyLevel < 7)
{
    chosenAction = AIAction.Attack;
}
else if (healthLevel > 35 && distanceToGoal < 75 && cardStrength >= 4 && cardStrength <= 5 && energyLevel >= 7 && energyLevel <= 10)
{
    chosenAction = AIAction.Speed;
}
else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal < 75 && cardStrength >= 4 && cardStrength <= 5 && energyLevel < 7)
{
    chosenAction = AIAction.Defense;
}
else
{
    // Por defecto, si ninguna condición se cumple
    chosenAction = AIAction.Attack;
}

        possibleActions.Add(chosenAction);

        AddMultipleCardActions(possibleActions, chosenAction);

        int randomIndex = random.Next(possibleActions.Count);
        return possibleActions[randomIndex];
    }

    private void AddMultipleCardActions(List<AIAction> possibleActions, AIAction chosenAction)
    {
        // Implementación de ejemplo
        switch (chosenAction)
        {
            case AIAction.Speed:
                if (speedCardCount > 1) possibleActions.Add(AIAction.Speed);
                break;
            case AIAction.Defense:
                if (defenseCardCount > 1) possibleActions.Add(AIAction.Defense);
                break;
            case AIAction.Attack:
                if (attackCardCount > 1) possibleActions.Add(AIAction.Attack);
                break;
        }
    }
}
