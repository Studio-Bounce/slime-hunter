using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : Menu
{
    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        Button btnUnpause = root.Q<Button>("btnUnpause");
        Button btnBackMainMenu = root.Q<Button>("btnBackMainMenu");
        Button btnQuit = root.Q<Button>("btnQuit");


        btnUnpause.clicked += Unpause;
        btnBackMainMenu.clicked += ReturnMainMenu;
        btnQuit.clicked += () => Application.Quit();
    }

    private void Unpause()
    {
        UIManager.Instance.SetPauseMenu(false);

        Time.timeScale = 1;
    }

    private void ReturnMainMenu()
    {
        Time.timeScale = 1;
        UIManager.Instance.SetPauseMenu(false);
        SceneLoader.Instance.UnloadScene(GameManager.Instance.GameSceneName,
            (AsyncOperation _, string _) => CanvasManager.Instance.ClearCanvas());
        UIManager.Instance.SetMainMenu(true);
        SceneLoader.Instance.LoadScene(GameManager.Instance.MenuSceneName);
        UIManager.Instance.SetHUDMenu(false);
    }
}
