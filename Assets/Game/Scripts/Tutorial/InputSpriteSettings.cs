using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputSpriteSettings;

[Serializable]
public struct ActionSpriteMap
{
    public string actionName;
    public Sprite keyboardSprite;
    public Sprite gamepadSprite;
}

[CreateAssetMenu(fileName = "InputSpriteSettings", menuName = "InputSpriteSettings")]
public class InputSpriteSettings : ScriptableObject
{
    public Sprite defaultSprite;
    public List<ActionSpriteMap> actionSpriteMaps = new List<ActionSpriteMap>();


    // Dictionaries to store the mappings
    [NonSerialized] public Dictionary<string, Sprite> keyboardSpriteMap = new Dictionary<string, Sprite>();
    [NonSerialized] public Dictionary<string, Sprite> gamepadSpriteMap = new Dictionary<string, Sprite>();

    public ActionSpriteMap FindMapByName(string actionName)
    {
        foreach (ActionSpriteMap map in actionSpriteMaps)
        {
            if (map.actionName == actionName)
            {
                return map;
            }
        }

        return default(ActionSpriteMap);
    }

    public void InitializeDictionaries()
    {
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
