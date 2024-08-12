using Cinemachine.Utility;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ButtonPrompt : MonoBehaviour
{
    [Header("Button Action")]
    public SpriteRenderer spriteRenderer;
    public string actionString;
    public bool autoSetSprite = true;
    public bool oneShot = false;
    public float delay = 0;

    public UnityEvent onButtonPressed;
    public UnityEvent onButtonDisabled;
    private InputAction inputAction;

    [Header("Player Animator Trigger")]
    public string triggerString;

    [Header("Player Orientation")]
    public float animationDuration = 0.5f;
    public bool reposition = false;
    public Vector3 positionOffset = Vector3.zero;
    public bool lookAt = false;
    public Vector3 lookAtOffset = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(spriteRenderer != null, "Requires a target sprite renderer");
        UpdateButtonSprite();
        InitializeInputAction();
    }

    private void OnEnable()
    {
        UpdateButtonSprite();
    }

    public void UpdateButtonSprite()
    {
        Sprite buttonSprite = InputManager.Instance.StringActionToSprite(actionString);
        if (buttonSprite != null)
        {
            spriteRenderer.sprite = buttonSprite;
        }
    }

    private void InitializeInputAction()
    {
        inputAction = InputManager.Instance.StringToAction(actionString);

        if (inputAction == null)
        {
            Debug.Log($"InputAction for '{actionString}' not found.");
            return;
        }

        inputAction.performed += OnActionPerformed;
    }

    IEnumerator InvokeButton()
    {
        RuntimeManager.PlayOneShot(AudioManager.Config.buttonPressEvent);
        yield return new WaitForSecondsRealtime(delay);
        onButtonPressed.Invoke();
    }

    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        if (enabled)
        {
            StartCoroutine(InvokeButton());
            if (!string.IsNullOrEmpty(triggerString))
            {
                GameManager.Instance.PlayerTriggerAnimation(triggerString);
            }
            if (reposition) LerpPlayerPosition();
            if (lookAt) SlerpPlayerLookAt();
            if (oneShot)
            {
                if (inputAction != null) inputAction.performed -= OnActionPerformed;
                onButtonDisabled.Invoke();
            };
        }
    }

    private void LerpPlayerPosition()
    {
        Vector3 targetPosition = transform.position + transform.TransformDirection(positionOffset);
        Transform playerTransform = GameManager.Instance.PlayerRef.transform;
        StartCoroutine(GameManager.RunEasedLerp(
            playerTransform.position,
            targetPosition,
            animationDuration,
            Easing.EaseInOutCubic,
            (Vector3 pos) => playerTransform.position = pos
            ));
    }

    private void SlerpPlayerLookAt()
    {
        Transform playerTransform = GameManager.Instance.PlayerRef.transform;
        Vector3 initialDirection = playerTransform.forward;
        Vector3 startPos = reposition ? transform.position + transform.TransformDirection(positionOffset) : playerTransform.position;
        Vector3 endPos = transform.position + transform.TransformDirection(lookAtOffset);
        Vector3 targetDirection = (endPos - startPos).normalized;

        // Flatten the directions to the y-axis
        initialDirection.y = 0;
        targetDirection.y = 0;

        StartCoroutine(GameManager.RunEasedSlerp(
            initialDirection,
            targetDirection,
            animationDuration,
            Easing.EaseInOutCubic,
            (Vector3 rot) => playerTransform.rotation = Quaternion.LookRotation(rot)
            ));
    }



    private void OnDestroy()
    {
        if (inputAction != null) inputAction.performed -= OnActionPerformed;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (reposition)
        {
            Handles.color = Color.red;
            Vector3 targetPosition = transform.position + transform.TransformDirection(positionOffset);
            Handles.DrawSolidDisc(targetPosition, Vector3.up, 0.1f);
            Handles.DrawWireDisc(targetPosition, Vector3.up, 0.2f);
        }

        if (lookAt)
        {
            Gizmos.color = Color.blue;
            Vector3 targetPosition = transform.position + transform.TransformDirection(lookAtOffset);
            Gizmos.DrawSphere(targetPosition, 0.1f);
            Gizmos.DrawWireSphere(targetPosition, 0.2f);
        }
    }
#endif
}
