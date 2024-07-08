using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// All game state actions must use this!
public enum GameState
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

    // Scene Flow
    [Header("Entry Scene")]
    public bool forceCorrectEntry = true;
    public string entryPointSceneName = "Core";

    [Header("Menu Scene")]
    [SerializeField] string menuSceneName = "";
    public string MenuSceneName
    {
        get { return menuSceneName; }
    }

    [Header("Game Scene")]
    [SerializeField] string gameSceneName = "";
    public string GameSceneName
    {
        get { return gameSceneName; }
    }

    // Player Attributes
    private Player _playerRef;
    public readonly int PlayerMaxHealth = 100;
    public readonly int PlayerMaxStamina = 3;
    [Header("Player Attributes")]
    public int playerHealth = 100;
    public int playerStamina = 3;
    [Tooltip("Amount of Stamina increase per second")]
    public int staminaIncreaseValue = 1;
    public float staminaIncreaseInterval = 0.33f;
    public float cooldownLength = 1.5f;
    private float _staminaTimer = 0.0f;
    private bool _cooldown = false;

    // GameState
    private GameState _gameState = GameState.INVALID;

    // Events
    public event Action<GameState> OnGameStateChange = delegate { };
    public event Action<Player> OnPlayerRefChange = delegate { };
    public event Action<int> OnPlayerHealthChange = delegate { };
    public event Action<int> OnPlayerStaminaChange = delegate { };
    public event Action<int> OnPlayerUseStamina = delegate { };

    // Getter/Setters
    // C# Naming Convention https://softwareengineering.stackexchange.com/questions/212638/is-it-poor-practice-to-name-a-property-member-the-same-as-the-declaring-type-in
    public GameState GameState
    {
        get { 
            return _gameState; 
        }
        set {
            _gameState = value;
            OnGameStateChange?.Invoke(value);
        }
    }

    public Player PlayerRef 
    { 
        get 
        {
            if (_playerRef == null)
            {
                _playerRef = FindObjectOfType<Player>();
            }
            return _playerRef;
        }
        set 
        {
            _playerRef = value;
            OnPlayerRefChange?.Invoke(_playerRef);
        }
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

    private void Awake()
    {
        // Ensure that we start from core scene
        if (forceCorrectEntry && SceneManager.GetActiveScene().name != entryPointSceneName)
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

    public void ReturnStamina(int value)
    {
        PlayerStamina = Mathf.Min(PlayerMaxStamina, PlayerStamina + value);
        _staminaTimer = 0;
        _cooldown = true;
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
        GameState = GameState.GAME_OVER;
        // TODO: Show game over screen

        // Reload game data from the saved file
        PersistenceManager.Instance.LoadGame();
        PlayerHealth = PlayerMaxHealth;
        GameState = GameState.GAMEPLAY;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        OnPlayerRefChange = null;
        OnGameStateChange = null;
        OnPlayerHealthChange = null;
        OnPlayerStaminaChange = null;
    }
}
