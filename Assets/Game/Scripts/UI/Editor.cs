using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Editor : MonoBehaviour
{
    UIDocument document = null;
    Button basicSlimeSpawnerBtn = null;

    [SerializeField] GameObject basicSlimePrefab;
    [SerializeField] GameObject basicSlimePreviewPrefab;

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
        LevelEditorManager.Instance.StandbyToSpawn(basicSlimePrefab, basicSlimePreviewPrefab);
    }

    private void OnDestroy()
    {
        if (basicSlimeSpawnerBtn != null)
            basicSlimeSpawnerBtn.clicked -= SpawnBasicSlime;
    }
}
