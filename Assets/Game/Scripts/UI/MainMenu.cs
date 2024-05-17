using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : Menu
{
    public string playSceneName;

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        Button btnPlay = root.Q<Button>("btnPlay");
        btnPlay.clicked += StartGame;

    }

    private void StartGame()
    {
        SceneLoader.Instance.LoadScene(playSceneName);
        UIManager.Instance.SetMainMenu(false);
    }
}
