using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputSpriteSettings;

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

    public List<ActionSpriteMap> actionSpriteMaps = new List<ActionSpriteMap>();

    // Dictionaries to store the mappings
    [NonSerialized] public Dictionary<string, Sprite> keyboardSpriteMap;
    [NonSerialized] public Dictionary<string, Sprite> gamepadSpriteMap;

    public void InitializeDictionaries()
    {
        keyboardSpriteMap = new Dictionary<string, Sprite>();
        gamepadSpriteMap = new Dictionary<string, Sprite>();

        foreach (var map in actionSpriteMaps)
        {
            if (!string.IsNullOrEmpty(map.actionName))
            {
                if (map.keyboardSprite != null)
                {
                    keyboardSpriteMap[map.actionName] = map.keyboardSprite;
                }
                if (map.gamepadSprite != null)
                {
                    gamepadSpriteMap[map.actionName] = map.gamepadSprite;
                }
            }
        }
    }
}
