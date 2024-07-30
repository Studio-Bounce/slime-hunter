using System.Collections;
using System.Collections.Generic;
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
            if (oneShot) gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (inputAction != null) inputAction.performed -= OnActionPerformed;
    }
}
