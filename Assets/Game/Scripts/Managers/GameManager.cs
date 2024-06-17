using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// All game state actions must use this!
public enum GameStates
{
    INVALID,
    MAIN_MENU,
    LOADING,
    PAUSED,
    GAMEPLAY,
    GAME_OVER
};

public class GameManager : Singleton<GameManager>
{
    // Global Variables
    public static readonly float GLOBAL_KNOCKBACK_MULTIPLIER = 0.25f;
    
    public readonly int PlayerMaxHealth = 100;
    public readonly int PlayerMaxStamina = 3;

    public string gameSceneName = "DemoLevel";
    public string entryPointSceneName = "Core";

    public Player playerRef;
    public int playerHealth = 100;
    public int playerStamina = 3;
    [Tooltip("Amount of Stamina increase per second (max: 100)")]
    public int staminaIncreaseValue = 1;
    public float staminaIncreaseInterval = 0.33f;
    public float cooldownLength = 1.5f;

    private float _staminaTimer = 0.0f;
    private bool _cooldown = false;

    public Canvas screenCanvas;

    GameStates _gameState = GameStates.INVALID;
    public GameStates GameState
    {
        get { return _gameState; }
        set { _gameState = value; }
    }

    public int PlayerHealth
    {
        get { return playerHealth; }
        set
        {
            playerHealth = value;
            OnPlayerHealthChange?.Invoke(playerHealth);
            if (playerHealth <= 0)
            {
                GameOver();
            }
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

    public bool IsFullStamina { get { return playerStamina == PlayerMaxStamina; } }

    public event Action<int> OnPlayerHealthChange;
    public event Action<int> OnPlayerStaminaChange;
    public event Action<int> OnPlayerUseStamina;

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

        // Ensure that we start from core scene
        if (SceneManager.GetActiveScene().name != entryPointSceneName)
        {
            SceneManager.LoadScene(entryPointSceneName);
        }
    }

    private void Update()
    {
        UpdateStamina();
    }

    public bool UseStamina(int value)
    {
        OnPlayerUseStamina.Invoke(PlayerStamina);
        if (PlayerStamina >= value)
        {
            PlayerStamina -= value;
            _staminaTimer = 0;
            _cooldown = true;
            return true;
        }
        return false;
    }

    private void UpdateStamina()
    {
        _staminaTimer += Time.deltaTime;
        if (_cooldown)
        {
            if (_staminaTimer > cooldownLength)
            {
                _cooldown = false;
            }
            return;
        }

        if (_staminaTimer > staminaIncreaseInterval)
        {
            _staminaTimer = 0.0f;
            if (PlayerStamina < PlayerMaxStamina)
            {
                PlayerStamina = Mathf.Min(PlayerStamina + staminaIncreaseValue, PlayerMaxStamina);
            }
        }
    }

    void GameOver()
    {
        GameState = GameStates.GAME_OVER;
        // TODO: Show game over screen

        // Reload game data from the saved file
        PersistenceManager.Instance.LoadGame();
        PlayerHealth = PlayerMaxHealth;
        GameState = GameStates.GAMEPLAY;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        OnPlayerHealthChange = null;
        OnPlayerStaminaChange = null;
    }
}
