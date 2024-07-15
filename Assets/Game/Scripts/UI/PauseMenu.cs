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

    public void Pause()
    {
        GameManager.Instance.GameState = GameState.PAUSED;
        SetVisible(true);
        GameManager.Instance.TimeFreeze();
        InventoryManager.Instance.UpdateInventoryUI();
    }

    public void Unpause()
    {
        SetVisible(false);
        GameManager.Instance.TimeNormal();
    }

    private void ReturnMainMenu()
    {
        Unpause();

        SceneLoader.Instance.UnloadScene(GameManager.Instance.GameSceneName,
            (AsyncOperation _, string _) => CanvasManager.Instance.ClearCanvas());
        UIManager.Instance.SetMainMenu(true);
        SceneLoader.Instance.LoadScene(GameManager.Instance.MenuSceneName);
        UIManager.Instance.SetHUDMenu(false);
    }
}
