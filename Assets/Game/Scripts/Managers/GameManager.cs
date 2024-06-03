using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    // Global Variables
    public static readonly float GLOBAL_KNOCKBACK_MULTIPLIER = 0.25f;
    
    public readonly int PlayerMaxHealth = 100;
    public readonly int PlayerMaxStamina = 100;

    public Player playerRef;
    private int playerHealth = 100;
    private int playerStamina = 100;
    [Tooltip("Amount of Stamina increase per second (max: 100)")]
    public int staminaIncreaseRate = 15;

    public Canvas screenCanvas;

    public int PlayerHealth
    {
        get { return playerHealth; }
        set
        {
            playerHealth = value;
            OnPlayerHealthChange?.Invoke(playerHealth);
        }
    }
    public int PlayerStamina
    {
        get { return playerStamina; }
        set
        {
            playerStamina = value;
            OnPlayerStaminaChange?.Invoke(playerStamina);
        }
    }

    public event Action<int> OnPlayerHealthChange;
    public event Action<int> OnPlayerStaminaChange;

    private void Awake()
    {
        if (screenCanvas == null)
        {
            GameObject canvasObject = new GameObject("ScreenCanvas");
            canvasObject.transform.SetParent(null, false);
            screenCanvas = canvasObject.AddComponent<Canvas>();
            screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<GraphicRaycaster>();
        }
    }

    private void Start()
    {
        // Constantly increase stamina
        StartCoroutine(IncreaseStamina());
    }

    IEnumerator IncreaseStamina()
    {
        while (gameObject.activeSelf)
        {
            if (PlayerStamina < PlayerMaxStamina)
            {
                PlayerStamina += staminaIncreaseRate;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void OnDestroy()
    {
        Instance.OnPlayerHealthChange = null;
        Instance.OnPlayerStaminaChange = null;
    }
}
