using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : Menu
{
    public string playSceneName;
    public string menuSceneName;//Main menu Scene


    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        Button btnUnpause = root.Q<Button>("btnUnpause");
        Button btnBackMainMenu = root.Q<Button>("btnBackMainMenu");

        btnUnpause.clicked += Unpause;
        btnBackMainMenu.clicked += ReturnMainMenu;
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
        SceneLoader.Instance.UnloadScene(playSceneName);
        UIManager.Instance.SetMainMenu(true);
        SceneLoader.Instance.LoadScene(menuSceneName);//load menu scene
    }
}
