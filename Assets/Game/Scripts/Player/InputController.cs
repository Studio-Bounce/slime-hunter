using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.InputSystem;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

[RequireComponent(typeof(PlayerController), typeof(WeaponController))]
public class InputController : MonoBehaviour
{
    private PlayerController _playerController;
    private WeaponController _weaponController;

    private PlayerInputActions _inputActions;
    private PlayerInputActions.PlayerActions _playerActions;
    private PlayerInputActions.UIActions _UIActions;

    public Vector2 movement = Vector2.zero;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _playerActions = _inputActions.Player;
        _UIActions = _inputActions.UI;
    }

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _weaponController = GetComponent<WeaponController>();
        SetupPlayerControls();
        SetupUIControls();
    }

    private void OnDestroy()
    {
        DisablePlayerControls();
        DisableUIControls();
    }

    private void SetupPlayerControls()
    {
        _playerActions.Move.performed += TrackMovement;
        _playerActions.Move.canceled += StopMovement;
        _playerActions.Dash.performed += _playerController.Dash;
        _playerActions.Rotate.performed += _playerController.RotateCamera;
        _playerActions.Attack.performed += _weaponController.Attack;
        _playerActions.CycleWeapon.performed += _weaponController.CycleWeapon;
    }

    private void SetupUIControls()
    {
        _UIActions.Pause.performed += Pause;
    }

    // Temp, may have global UI controls?
    private void Pause(InputContext context)
    {
        UIManager.Instance.SetPauseMenu(true);
        Time.timeScale = 0;
    }

    private void TrackMovement(InputContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    private void StopMovement(InputContext context)
    {
        movement = Vector2.zero;
    }

    private void DisablePlayerControls()
    {
        _playerActions.Move.performed -= TrackMovement;
        _playerActions.Move.canceled -= StopMovement;
        _playerActions.Dash.performed -= _playerController.Dash;
        _playerActions.Rotate.performed -= _playerController.RotateCamera;
        _playerActions.Attack.performed -= _weaponController.Attack;
        _playerActions.CycleWeapon.performed -= _weaponController.CycleWeapon;
    }

    private void DisableUIControls()
    {
        _UIActions.Pause.performed -= Pause;
    }
}
