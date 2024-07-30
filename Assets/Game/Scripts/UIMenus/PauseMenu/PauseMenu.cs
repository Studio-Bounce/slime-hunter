using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : Menu
{
    // Inventory Stats
    VisualElement characterContainer;
    Label healthValue;
    Label levelValue;
    Label spValue;

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        Button btnUnpause = root.Q<Button>("btnUnpause");
        Button btnBackMainMenu = root.Q<Button>("btnBackMainMenu");
        Button btnQuit = root.Q<Button>("btnQuit");

        btnUnpause.clicked += Unpause;
        btnBackMainMenu.clicked += ReturnMainMenu;
        btnQuit.clicked += () => Application.Quit();

        characterContainer = root.Q<VisualElement>("CharacterContainer");
        healthValue = root.Q<Label>("HealthValue");
        LinkInventoryStatsUIToPlayer();
    }

    private void LinkInventoryStatsUIToPlayer()
    {
        GameManager.Instance.OnPlayerHealthChange += (int value) => healthValue.text = value.ToString();
    }

    public void Pause()
    {
        if (GameManager.Instance.GameState != GameState.PAUSED)
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
