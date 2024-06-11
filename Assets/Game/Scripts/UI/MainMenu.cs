using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : Menu
{
    public string playSceneName;
    public string menuSceneName;

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        Label lblPlay = root.Q<Label>("lblPlay");
        Label lblContinue = root.Q<Label>("lblContinue");
        Label lblQuit = root.Q<Label>("lblQuit");
        lblPlay.RegisterCallback<ClickEvent>(ev => StartGame());
        lblContinue.RegisterCallback<ClickEvent>(ev => ContinueGame());
        lblQuit.RegisterCallback<ClickEvent>(ev => QuitGame());
    }

    private void StartGame()
    {
        SceneLoader.Instance.UnloadScene(menuSceneName);
        SceneLoader.Instance.LoadScene(playSceneName);
        UIManager.Instance.SetMainMenu(false);
        UIManager.Instance.SetHUDMenu(true);
        GameManager.Instance.GameState = GameStates.GAMEPLAY;
    }

    private void ContinueGame()
    {
        SceneLoader.Instance.UnloadScene(menuSceneName);
        SceneLoader.Instance.LoadScene(playSceneName, callback: LoadData);
        UIManager.Instance.SetMainMenu(false);
        UIManager.Instance.SetHUDMenu(true);
        GameManager.Instance.GameState = GameStates.GAMEPLAY;
    }

    void LoadData(AsyncOperation _, string sceneName)
    {
        PersistenceManager.Instance.LoadGame();
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
