using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "ButtonSprite", menuName = "ButtonSprite")]
public class ButtonSpriteSO : ScriptableObject
{
    public InputAction action;
    public Sprite keyboardSprite;
    public Sprite gamepadSprite;
}
