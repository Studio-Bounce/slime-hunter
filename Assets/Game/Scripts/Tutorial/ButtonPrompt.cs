using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
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

    public UnityEvent onButtonPressed;
    private InputAction inputAction;

    [Header("Player Reposition")]
    public bool reposition = false;
    public Vector3 positionTarget = Vector3.zero;
    public bool lookAt = false;
    public Vector3 lookAtTarget = Vector3.zero;

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

    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        if (enabled)
        {
            onButtonPressed.Invoke();
            if (reposition) StartCoroutine(LerpPlayerPosition());
            if (lookAt) StartCoroutine(LerpPlayerLookAt());
            if (oneShot) gameObject.SetActive(false);
        }
    }

    private IEnumerator LerpPlayerPosition()
    {
        float elapsed = 0.0f;
        float duration = 1.0f;
        Vector3 targetPosition = transform.position + transform.TransformDirection(positionTarget);
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float eased = Easing.EaseInOut(t);

            Transform playerTransform = GameManager.Instance.PlayerRef.transform;
            playerTransform.position = Vector3.Lerp(playerTransform.position, targetPosition, eased);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator LerpPlayerLookAt()
    {
        float elapsed = 0.0f;
        float duration = 1.0f;

        Transform playerTransform = GameManager.Instance.PlayerRef.transform;
        Vector3 initialDirection = playerTransform.forward;

        Vector3 startPos = reposition ? positionTarget : playerTransform.position;
        Vector3 endPos = transform.position + lookAtTarget;
        Vector3 targetDirection = (startPos - endPos).normalized;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float eased = Easing.EaseInOut(t);

            Vector3 currentDirection = Vector3.Slerp(initialDirection, targetDirection, eased);
            transform.rotation = Quaternion.LookRotation(currentDirection);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.LookRotation(targetDirection);
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
            Vector3 targetPosition = transform.position + transform.TransformDirection(positionTarget);
            Handles.DrawSolidDisc(targetPosition, Vector3.up, 0.2f);
        }

        if (lookAt)
        {
            Gizmos.color = Color.blue;
            Vector3 targetPosition = transform.position + transform.TransformDirection(lookAtTarget);
            Gizmos.DrawSphere(targetPosition, 0.2f);
        }
    }
#endif
}
