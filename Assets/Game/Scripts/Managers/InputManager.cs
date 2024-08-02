using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private InputSpriteSettings _spriteSettings;

    private PlayerController _playerController;
    private WeaponController _weaponController;
    private SpellController _spellController;

    private PlayerInputActions _inputActions;
    private PlayerInputActions.PlayerActions _playerActions;
    private PlayerInputActions.UIActions _UIActions;

    private Vector2 _movement = Vector2.zero;

    public float inputQueueDelay = .3f;
    private Dictionary<Func<InputContext, bool>, InputContext> QueuedInputMap = new Dictionary<Func<InputContext, bool>, InputContext>();

    // Need to store the function to properly add and removed callbacks to inputs
    Action<InputContext> attackQueuedAction;
    Action<InputContext> dashQueuedAction;
    Action<InputContext> spell1Action;
    Action<InputContext> spell2Action;

    public Vector2 Movement { get { return _movement; } }

    private void Awake()
    {
        // Setup Input Sprite Settings
        if (_spriteSettings != null)
            _spriteSettings.InitializeDictionaries();
        // Enable InputActions
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _playerActions = _inputActions.Player;
        _UIActions = _inputActions.UI;
        // Assign actions
        attackQueuedAction = e => QueueInput(_weaponController.Attack, e);
        dashQueuedAction = e => QueueInput(_playerController.Dash, e);
        spell1Action = e => _spellController.ChangeSpell(0);
        spell2Action = e => _spellController.ChangeSpell(1);
        // Enables/disables inputs based on game state
        GameManager.Instance.OnGameStateChange += UpdateInputAvailability;
        GameManager.Instance.OnPlayerRefChange += GetControllers;
    }

    private void Start()
    {
        GetControllers(GameManager.Instance.PlayerRef);
        _AddUIControls();
        TogglePauseControl(false);
    }

    public Sprite StringActionToSprite(string actionName)
    {
        // Naive solution: Returns gamepad controls as long as one is plugged in
        if (Gamepad.current != null)
        {
            return _spriteSettings.gamepadSpriteMap.TryGetValue(actionName, out var sprite) ? sprite : null;

        }
        else
        {
            return _spriteSettings.keyboardSpriteMap.TryGetValue(actionName, out var sprite) ? sprite : null;
        }
    }

    public InputAction StringToAction(string actionName)
    {
        return _inputActions.FindAction(actionName);
    }

    public void UpdateInputAvailability(GameState state)
    {
        switch (state)
        {
            case GameState.MAIN_MENU:
                TogglePauseControl(false);
                TogglePlayerControls(false);
                break;

            case GameState.PAUSED:
                TogglePlayerControls(false);
                break;

            case GameState.GAMEPLAY:
                ToggleUIControls(true);
                TogglePlayerControls(true);
                break;

            case GameState.GAME_OVER:
                TogglePlayerControls(false);
                break;
        }
    }

    private void GetControllers(Player player)
    {
        if (player == null) return;
        _playerController = player.GetComponent<PlayerController>();
        _weaponController = player.GetComponent<WeaponController>();
        _spellController = player.GetComponent<SpellController>();
        _AddPlayerControls();
    }

    public void TogglePlayerControls(bool active)
    {
        if (active) _playerActions.Enable(); else _playerActions.Disable();
    }

    public void TogglePlayerMovement(bool active)
    {
        if (active) _playerActions.Move.Enable(); else _playerActions.Move.Disable();
    }

    public void TogglePlayerMeleeControls(bool active)
    {
        if (active) _playerActions.Attack.Enable(); else _playerActions.Attack.Disable();
        if (active) _playerActions.SpecialAttack.Enable(); else _playerActions.SpecialAttack.Disable();
        if (active) _playerActions.CycleWeapon.Enable(); else _playerActions.CycleWeapon.Disable();
    }

    public void TogglePlayerSpellControls(bool active)
    {
        if (active) _playerActions.Spell1.Enable(); else _playerActions.Spell1.Disable();
        if (active) _playerActions.Spell2.Enable(); else _playerActions.Spell2.Disable();
        if (active) _playerActions.CastSpell.Enable(); else _playerActions.CastSpell.Disable();
    }

    public void TogglePauseControl(bool active)
    {
        if (active) _UIActions.Pause.Enable(); else _UIActions.Pause.Disable();
    }

    public void ToggleUIControls(bool active)
    {
        if (active) _UIActions.Enable(); else _UIActions.Disable();
    }

    private void OnDestroy()
    {
        _RemovePlayerControls();
        _RemoveUIControls();
    }

    // Give leniency to player input when timing is important
    // Callback should return bool to check if the input had succeeded
    private void QueueInput(Func<InputContext, bool> inputCallback, InputContext e)
    {
        if (!QueuedInputMap.ContainsKey(inputCallback))
        {
            StartCoroutine(QueueInputCoroutine(inputCallback, e));
        }
    }

    IEnumerator QueueInputCoroutine(Func<InputContext, bool> inputCallback, InputContext e)
    {
        QueuedInputMap.Add(inputCallback, e);
        float timer = 0;
        while (timer < inputQueueDelay)
        {
            if (inputCallback(e))
            {
                timer = inputQueueDelay;
            } else
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
        QueuedInputMap.Remove(inputCallback);
    }

    private void _AddPlayerControls()
    {
        // Player
        _playerActions.Move.performed += TrackMovement;
        _playerActions.Move.canceled += StopMovement;
        _playerActions.Dash.performed += dashQueuedAction;
        _playerActions.Rotate.performed += _playerController.RotateCamera;
        // Weapon
        _playerActions.Attack.performed += attackQueuedAction;
        _playerActions.SpecialAttack.performed += _weaponController.SpecialAttack;
        _playerActions.CycleWeapon.performed += _weaponController.CycleWeapon;

        // Spells
        _playerActions.Spell1.performed += spell1Action;
        _playerActions.Spell2.performed += spell2Action;

        _playerActions.CastSpell.started += _spellController.AimSpell;
        _playerActions.Attack.performed += _spellController.CancelSpell;
        _playerActions.CastSpell.canceled += _spellController.CastSpell;
    }

    private void _AddUIControls()
    {
        _UIActions.Pause.performed += Pause;
        _UIActions.SkipDialogue.performed += DialogueManager.Instance.SkipDialogue;
    }

    private void _RemovePlayerControls()
    {
        _playerActions.Move.performed -= TrackMovement;
        _playerActions.Move.canceled -= StopMovement;
        _playerActions.Dash.performed -= dashQueuedAction;
        if (_playerController) _playerActions.Rotate.performed -= _playerController.RotateCamera;
        _playerActions.Attack.performed -= attackQueuedAction;
        if (_weaponController) _playerActions.SpecialAttack.performed -= _weaponController.SpecialAttack;
        if (_weaponController) _playerActions.CycleWeapon.performed -= _weaponController.CycleWeapon;

        _playerActions.Spell1.performed -= spell1Action;
        _playerActions.Spell2.performed -= spell2Action;

        if (_spellController) _playerActions.CastSpell.started -= _spellController.AimSpell;
        if (_spellController) _playerActions.Attack.performed -= _spellController.CancelSpell;
        if (_spellController) _playerActions.CastSpell.canceled -= _spellController.CastSpell;
    }

    private void _RemoveUIControls()
    {
        _UIActions.Pause.performed -= Pause;
        _UIActions.SkipDialogue.performed -= DialogueManager.Instance.SkipDialogue;
    }

    private void Pause(InputContext context)
    {
        if (GameManager.Instance.GameState == GameState.PAUSED)
        {
            GameManager.Instance.GameState = GameState.GAMEPLAY;
            CameraManager.Instance.SmoothSetBlur(0.0f, 1.0f);
            UIManager.Instance.SetPauseMenu(false);
        }
        else
        {
            GameManager.Instance.GameState = GameState.PAUSED;
            CameraManager.Instance.SmoothSetBlur(10.0f, 1.0f);
            UIManager.Instance.SetPauseMenu(true);
        }
    }

    private void TrackMovement(InputContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    // It can be used to force move player when movement input actions are disabled
    // To be used only for tutorial, NO WHERE ELSE
    public void MovementOverride(Vector2 moveDelta)
    {
        if (_playerActions.Move.enabled)
        {
            Debug.LogError("Overriding movement with input actions enabled!");
            return;
        }

        _movement = moveDelta;
    }

    private void StopMovement(InputContext context)
    {
        _movement = Vector2.zero;
    }
}
