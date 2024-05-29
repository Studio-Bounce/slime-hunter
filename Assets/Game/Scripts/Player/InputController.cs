using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public float inputQueueDelay = .3f;
    private Dictionary<Func<InputContext, bool>, InputContext> QueuedInputMap = new Dictionary<Func<InputContext, bool>, InputContext>();

    // Need to store the function to properly add and removed callbacks to inputs
    // WIP: Find a cleaner solution?
    Action<InputContext> attackQueuedAction;
    Action<InputContext> dashQueuedAction;

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
        attackQueuedAction = e => QueueInput(_weaponController.Attack, e);
        dashQueuedAction = e => QueueInput(_playerController.Dash, e);

        SetupPlayerControls();
        SetupUIControls();
    }

    private void OnDestroy()
    {
        DisablePlayerControls();
        DisableUIControls();
    }

    public Vector3 GetMoveDirectionFromCamera()
    {
        // Get forward based on camera
        Vector3 cameraToPlayer = (transform.position - Camera.main.transform.position);
        Vector2 forwardDirection = new Vector2(cameraToPlayer.x, cameraToPlayer.z);
        forwardDirection.Normalize();
        Vector2 rightDirection = new Vector2(forwardDirection.y, -forwardDirection.x);

        // Calculate movement direction based on forward
        Vector2 direction2D = (forwardDirection * movement.y + rightDirection * movement.x).normalized;
        return new Vector3(direction2D.x, 0, direction2D.y);
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

    private void SetupPlayerControls()
    {
        _playerActions.Move.performed += TrackMovement;
        _playerActions.Move.canceled += StopMovement;
        _playerActions.Dash.performed += dashQueuedAction;
        _playerActions.Rotate.performed += _playerController.RotateCamera;
        _playerActions.Attack.performed += attackQueuedAction;
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
        _playerActions.Dash.performed -= dashQueuedAction;
        _playerActions.Rotate.performed -= _playerController.RotateCamera;
        _playerActions.Attack.performed -= attackQueuedAction;
        _playerActions.CycleWeapon.performed -= _weaponController.CycleWeapon;
    }

    private void DisableUIControls()
    {
        _UIActions.Pause.performed -= Pause;
    }
}
