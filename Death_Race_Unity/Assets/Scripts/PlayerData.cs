using UnityEngine;
using System;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    [SerializeField]
    private int playerId;
    public int PlayerId { get { return playerId; } private set { playerId = value; OnPlayerIdChanged?.Invoke(playerId); } }

    // Define el evento para notificar cambios en el ID del jugador
    public event Action<int> OnPlayerIdChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerId(int id)
    {
        PlayerId = id;
    }
}