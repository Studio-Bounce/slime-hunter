using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputSpriteSettings", menuName = "InputSpriteSettings")]
public class InputSpriteSettings : ScriptableObject
{
    [Serializable]
    public struct ActionSpriteMap
    {
        public string actionName;
        public Sprite keyboardSprite;
        public Sprite gamepadSprite;
    }

    public List<ActionSpriteMap> actionSpriteMap = new List<ActionSpriteMap>();
}
