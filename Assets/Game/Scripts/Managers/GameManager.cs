using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public readonly int playerMaxHealth = 100;
    private int playerHealth = 100;
    public int PlayerHealth
    {
        get { return playerHealth; }
        set
        {
            playerHealth = value;
            OnPlayerHealthChange?.Invoke(playerHealth);
        }
    }

    public event Action<int> OnPlayerHealthChange;

    private void OnDestroy()
    {
        Instance.OnPlayerHealthChange = null;
    }
}
