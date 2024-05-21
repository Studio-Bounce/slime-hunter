using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Editor : MonoBehaviour
{

    UIDocument document = null;
    Button basicSlimeSpawnerBtn = null;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    private void Start()
    {
        if (document == null)
            return;

        VisualElement root = document.rootVisualElement;
        basicSlimeSpawnerBtn = root.Q<Button>("btnBasicSlime");
        basicSlimeSpawnerBtn.clicked += SpawnBasicSlime;
    }

    private void SpawnBasicSlime()
    {
        Debug.Log("Spawn Slime");
    }

    private void OnDestroy()
    {
        if (basicSlimeSpawnerBtn != null)
            basicSlimeSpawnerBtn.clicked -= SpawnBasicSlime;
    }
}
