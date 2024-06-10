using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Editor : Menu
{
    UIDocument document = null;
    Button basicSlimeSpawnerBtn = null;
    Button rabbitSlimeSpawnerBtn = null;

    [SerializeField] GameObject basicSlimePrefab;
    [SerializeField] GameObject basicSlimePreviewPrefab;
    [SerializeField] GameObject rabbitSlimePrefab;
    [SerializeField] GameObject rabbitSlimePreviewPrefab;

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
        rabbitSlimeSpawnerBtn = root.Q<Button>("btnRabbitSlime");
        rabbitSlimeSpawnerBtn.clicked += SpawnRabbitSlime;
    }

    private void SpawnBasicSlime()
    {
        LevelEditorManager.Instance.StandbyToSpawn(basicSlimePrefab, basicSlimePreviewPrefab);
    }

    private void SpawnRabbitSlime()
    {
        LevelEditorManager.Instance.StandbyToSpawn(rabbitSlimePrefab, rabbitSlimePreviewPrefab);
    }

    private void OnDestroy()
    {
        if (basicSlimeSpawnerBtn != null)
            basicSlimeSpawnerBtn.clicked -= SpawnBasicSlime;
    }
}
