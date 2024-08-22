using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ButtonSprite : MonoBehaviour
{
    public string actionString;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite buttonSprite = InputManager.Instance.StringActionToSprite(actionString);
        if (buttonSprite != null)
        {
            spriteRenderer.sprite = buttonSprite;
        }
    }
}
