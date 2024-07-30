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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(spriteRenderer != null, "Requires a target sprite renderer");

        Sprite buttonSprite = InputManager.Instance.ActionToSprite(actionString);
        if (buttonSprite != null )
        {
            spriteRenderer.sprite = buttonSprite;
        }

        InitializeInputAction();
    }

    private void InitializeInputAction()
    {
        InputAction inputAction = InputManager.Instance.StringToAction(actionString);

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
}
