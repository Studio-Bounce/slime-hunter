using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

[RequireComponent(typeof(PlayerController), typeof(WeaponController), typeof(SpellController))]
public class InputController : MonoBehaviour
{
    private PlayerController _playerController;
    private WeaponController _weaponController;
    private SpellController _spellController;
    private Trail _trail;

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
    Action<InputContext> spell3Action;

    // FIXME: Used to stop player input at different times. Move InputController away from player
    bool enableInput = true;
    public bool EnableInput { get { return enableInput; } set { enableInput = value; } }
    public Vector2 Movement { get { return _movement; } }

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _playerActions = _inputActions.Player;
        _UIActions = _inputActions.UI;
        EnableInput = true;
    }

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _weaponController = GetComponent<WeaponController>();
        _spellController = GetComponent<SpellController>();
        _trail = GetComponent<Trail>();
        attackQueuedAction = e => QueueInput(_weaponController.Attack, e);
        dashQueuedAction = e => QueueInput(_playerController.Dash, e);

        spell1Action = e => _spellController.StartCast(0);
        spell2Action = e => _spellController.StartCast(1);
        spell3Action = e => _spellController.StartCast(2);

        SetupPlayerControls();
        SetupUIControls();
    }

    private void OnDestroy()
    {
        DisablePlayerControls();
        DisableUIControls();
    }

    // Give leniency to player input when timing is important
    // Callback should return bool to check if the input had succeeded
    private void QueueInput(Func<InputContext, bool> inputCallback, InputContext e)
    {
        if (!QueuedInputMap.ContainsKey(inputCallback) && EnableInput)
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

    private void SetupPlayerControls()
    {
        // Player
        _playerActions.Move.performed += TrackMovement;
        _playerActions.Move.canceled += StopMovement;
        _playerActions.Dash.performed += dashQueuedAction;
        _playerActions.Rotate.performed += _playerController.RotateCamera;
        // Weapon
        _playerActions.Attack.performed += attackQueuedAction;
        _playerActions.CycleWeapon.performed += _weaponController.CycleWeapon;
        // Spells
        _playerActions.Spell1.performed += spell1Action;
        _playerActions.Spell2.performed += spell2Action;
        _playerActions.Spell3.performed += spell3Action;

        _playerActions.CastSpell.performed += _spellController.Cast;
    }

    private void SetupUIControls()
    {
        _UIActions.Pause.performed += Pause;
    }

    private void Pause(InputContext context)
    {
        if (GameManager.Instance.GameState == GameStates.PAUSED)
        {
            GameManager.Instance.GameState = GameStates.GAMEPLAY;
            UIManager.Instance.SetPauseMenu(false);
            Time.timeScale = 1;
        }
        else
        {
            GameManager.Instance.GameState = GameStates.PAUSED;
            UIManager.Instance.SetPauseMenu(true);
            Time.timeScale = 0;
        }
    }

    private void TrackMovement(InputContext context)
    {
        if (EnableInput)
        {
            _movement = context.ReadValue<Vector2>();
        }
        else
        {
            _movement = Vector2.zero;
        }
    }

    private void StopMovement(InputContext context)
    {
        _movement = Vector2.zero;
    }

    private void DisablePlayerControls()
    {
        _playerActions.Move.performed -= TrackMovement;
        _playerActions.Move.canceled -= StopMovement;
        _playerActions.Dash.performed -= dashQueuedAction;
        _playerActions.Rotate.performed -= _playerController.RotateCamera;
        _playerActions.Attack.performed -= attackQueuedAction;
        _playerActions.CycleWeapon.performed -= _weaponController.CycleWeapon;

        //_playerActions.Spell1.performed -= _spellController.StartCast;
        //_playerActions.CastSpell.performed -= _spellController.Cast;
    }

    private void DisableUIControls()
    {
        _UIActions.Pause.performed -= Pause;
    }
}
