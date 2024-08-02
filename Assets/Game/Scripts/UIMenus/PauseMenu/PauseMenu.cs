using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : Menu
{
    VisualElement pauseRootWrapperVE;
    float cachedAlpha;
    bool isMapTabSelected = false;

    // Map
    Camera mapCamera;

    // Inventory Stats
    VisualElement characterContainer;
    Label healthValue;
    Label levelValue;
    Label spValue;

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        pauseRootWrapperVE = root.Q<VisualElement>("RootWrapper");
        cachedAlpha = pauseRootWrapperVE.resolvedStyle.backgroundColor.a;

        Button btnUnpause = root.Q<Button>("btnUnpause");
        Button btnBackMainMenu = root.Q<Button>("btnBackMainMenu");
        Button btnSettings = root.Q<Button>("btnSettings");
        Button btnQuit = root.Q<Button>("btnQuit");

        btnUnpause.clicked += Unpause;
        btnBackMainMenu.clicked += ReturnMainMenu;
        btnSettings.clicked += Settings;
        btnQuit.clicked += () => Application.Quit();

        characterContainer = root.Q<VisualElement>("CharacterContainer");
        healthValue = root.Q<Label>("HealthValue");
        LinkInventoryStatsUIToPlayer();

        VisualElement mapTabVE = root.Q<VisualElement>("MapTab");
        mapTabVE.RegisterCallback<ClickEvent>(evt => {
            MapMenuSelected();
        });
        VisualElement invenTabVE = root.Q<VisualElement>("InventoryTab");
        VisualElement questTabVE = root.Q<VisualElement>("QuestTab");
        VisualElement profileTabVE = root.Q<VisualElement>("ProfileTab");
        VisualElement menuTabVE = root.Q<VisualElement>("MenuTab");
        invenTabVE.RegisterCallback<ClickEvent>(evt => { NonMapMenuSelected(); });
        questTabVE.RegisterCallback<ClickEvent>(evt => { NonMapMenuSelected(); });
        profileTabVE.RegisterCallback<ClickEvent>(evt => { NonMapMenuSelected(); });
        menuTabVE.RegisterCallback<ClickEvent>(evt => { NonMapMenuSelected(); });
    }

    private void Settings()
    {
        Hide();
        UIManager.Instance.settingsMenu.Show();
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
        InputManager.Instance.TogglePlayerControls(false);
        // Hide HUD to prevent it from appearing on Map
        UIManager.Instance.SetHUDMenu(false);

        if (isMapTabSelected) // if it was last selected in pause menu (not a clean solution)
        {
            MapMenuSelected();
        }
    }

    public void Unpause()
    {
        SetVisible(false);
        GameManager.Instance.TimeNormal();
        InputManager.Instance.TogglePlayerControls(true);
        // Show HUD
        UIManager.Instance.SetHUDMenu(true);
        ShowHideMapCamera(false);  // just to ensure map camera is not shown
    }

    void MapMenuSelected()
    {
        isMapTabSelected = true;
        ToggleBackgroundAlpha(false);
        // Reset player transform
        GameManager.Instance.PlayerRef.transform.rotation = Quaternion.identity;
        ShowHideMapCamera(true);
    }

    void NonMapMenuSelected()
    {
        isMapTabSelected = false;
        ToggleBackgroundAlpha(true);
        ShowHideMapCamera(false);
    }

    void ToggleBackgroundAlpha(bool showAlpha)
    {
        if (pauseRootWrapperVE == null)
        {
            return;
        }

        float _alpha = (showAlpha) ? cachedAlpha : 0f;
        pauseRootWrapperVE.style.backgroundColor = new Color(0f, 0f, 0f, _alpha);
    }

    bool FindCamera()
    {
        if (mapCamera == null)
        {
            GameObject mapGO = GameObject.FindGameObjectWithTag(GameConstants.MapCameraTag);
            if (mapGO != null)
            {
                mapCamera = mapGO.GetComponent<Camera>();
            }
        }
        return (mapCamera != null);
    }

    void ShowHideMapCamera(bool show)
    {
        // Try looking for map camera, if you don't have it
        if (!FindCamera())
            return;

        if (show)
        {
            mapCamera.depth = 1;
        }
        else
        {
            mapCamera.depth = -1;
        }
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
