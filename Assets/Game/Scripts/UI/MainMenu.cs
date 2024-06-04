using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : Menu
{
    public string playSceneName;
    public string menuSceneName; //Main menu Scene

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        Label lblPlay = root.Q<Label>("lblPlay");
        Label lblQuit = root.Q<Label>("lblQuit");
        lblPlay.RegisterCallback<ClickEvent>(ev => StartGame());
        lblQuit.RegisterCallback<ClickEvent>(ev => QuitGame());
    }

    private void StartGame()
    {
        SceneLoader.Instance.UnloadScene(menuSceneName);//unload menu
        SceneLoader.Instance.LoadScene(playSceneName);
        UIManager.Instance.SetMainMenu(false);
        UIManager.Instance.SetHUDMenu(true);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
