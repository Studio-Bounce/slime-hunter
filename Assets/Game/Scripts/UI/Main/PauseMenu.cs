using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class PauseMenu : TabbedMenu
{
    VisualElement pauseRootWrapperVE;
    float cachedAlpha;
    bool isMapTabSelected = false;

    // Map
    Camera mapCamera;

    // Inventory Stats
    VisualElement characterContainer;
    Label healthValue;
    Label spValue;

    void Start()
    {
        pauseRootWrapperVE = root.Q<VisualElement>("RootWrapper");
        cachedAlpha = pauseRootWrapperVE.resolvedStyle.backgroundColor.a;

        Button btnUnpause = root.Q<Button>("btnUnpause");
        Button btnBackMainMenu = root.Q<Button>("btnBackMainMenu");
        Button btnSettings = root.Q<Button>("btnSettings");
        Button btnQuit = root.Q<Button>("btnQuit");

        btnUnpause.RegisterCallback<ClickEvent>(evt => GameManager.Instance.GameState = GameState.GAMEPLAY);
        btnBackMainMenu.RegisterCallback<ClickEvent>(evt => ReturnMainMenu());
        btnSettings.RegisterCallback<ClickEvent>(evt => Settings());
        btnQuit.RegisterCallback<ClickEvent>(evt => Application.Quit());

        characterContainer = root.Q<VisualElement>("CharacterContainer");
        healthValue = root.Q<Label>("HealthValue");
        spValue = root.Q<Label>("SPValue");
        LinkInventoryStatsUIToPlayer();

        Button mapTabVE = root.Q<Button>("MapTab");
        mapTabVE.RegisterCallback<ClickEvent>(evt => {
            MapMenuSelected();
        });
        Button invenTabVE = root.Q<Button>("InventoryTab");
        Button questTabVE = root.Q<Button>("QuestTab");
        Button menuTabVE = root.Q<Button>("MenuTab");
        invenTabVE.RegisterCallback<ClickEvent>(evt => { NonMapMenuSelected(); });
        questTabVE.RegisterCallback<ClickEvent>(evt => { NonMapMenuSelected(); });
        menuTabVE.RegisterCallback<ClickEvent>(evt => { NonMapMenuSelected(); });

        GameManager.Instance.OnGameStateChange += OnPause;
        InputManager.Instance.exitEvent += ExitPause;
    }

    public override void Show()
    {
        base.Show();
        if (InputManager.IsGamepad) root.Q<Button>().Focus(); // Hacky way to focus pause menu
        InputManager.Instance.exitEvent += ExitPause;
    }

    public override void Hide()
    {
        base.Hide();
        InputManager.Instance.exitEvent -= ExitPause;
    }

    private void Settings()
    {
        Hide();
        UIManager.Instance.settingsMenu.Show();
    }

    private void LinkInventoryStatsUIToPlayer()
    {
        GameManager.Instance.OnPlayerHealthChange += (int value) => healthValue.text = value.ToString();
        GameManager.Instance.OnPlayerSpecialAttackChange += (float value) => spValue.text = value.ToString();

        InventoryManager.Instance.OnEquippedWeaponsChanged += e => _UpdateWeaponStatsUI();
    }

    private void _UpdateWeaponStatsUI()
    {
        for (int i = 0; i < 2; i++) 
        {
            Label weaponName = pauseRootWrapperVE.Q<Label>($"Weapon{i+1}Name");
            Label damageLabel = pauseRootWrapperVE.Q<Label>($"Weapon{i+1}Damage");
            Label rangeLabel = pauseRootWrapperVE.Q<Label>($"Weapon{i+1}Range");

            WeaponSO weaponSO = InventoryManager.Instance.equippedWeapons[i];
            if (weaponSO != null)
            {
                weaponName.text = weaponSO.name;
                damageLabel.text = weaponSO.damage.value.ToString();
                rangeLabel.text = weaponSO.range.ToString();
            } else
            {
                weaponName.text = "Unequipped";
                damageLabel.text = "N/A";
                rangeLabel.text = "N/A";
            }
        }
    }

    public void ExitPause()
    {
        if (GameManager.Instance.GameState == GameState.PAUSED)
        {
            GameManager.Instance.GameState = GameState.GAMEPLAY;
        }
    }

    public void OnPause(GameState state)
    {
        switch (state)
        {
            case GameState.PAUSED:
                Show();
                GameManager.Instance.TimeFreeze();
                CameraManager.Instance.SmoothSetBlur(15.0f, 0.3f);
                InventoryManager.Instance.UpdateInventoryUI();
                UIManager.Instance.SetHUDMenu(false); // Hide HUD to prevent it from appearing on Map
                if (isMapTabSelected) // if it was last selected in pause menu (not a clean solution)
                {
                    MapMenuSelected();
                }
                break;
            case GameState.GAMEPLAY:
                Hide();
                UIManager.Instance.settingsMenu.Hide();
                GameManager.Instance.TimeNormal();
                CameraManager.Instance.SmoothSetBlur(0.0f, 0.3f);
                UIManager.Instance.SetHUDMenu(true);
                if (mapCamera != null) mapCamera.depth = -1;  // Just to ensure map camera is not shown
                break;
            default:
                Hide();
                break;
        }
    }

    public override void SwitchTab(string tabName)
    {
        if (IsVisible && controller.CurrentTab() == tabName)
        {
            NonMapMenuSelected();
            Hide();
            GameManager.Instance.GameState = GameState.GAMEPLAY;
            return;
        }

        GameManager.Instance.GameState = GameState.PAUSED;
        controller.SwitchTab(tabName);
        RuntimeManager.PlayOneShot(AudioManager.Config.buttonPressEvent);
        if (tabName == "MapTab")
        {
            MapMenuSelected();
        } else
        {
            NonMapMenuSelected();
        }
    }

    public void MapMenuSelected()
    {
        isMapTabSelected = true;
        ToggleBackgroundAlpha(false);
        // Reset player transform
        GameManager.Instance.PlayerRef.transform.rotation = Quaternion.identity;
        ShowHideMapCamera(true);
    }

    public void NonMapMenuSelected()
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
            mapCamera.enabled = true;
            CameraManager.Instance.SmoothSetBlur(0.0f, 0.3f);
            mapCamera.depth = 1;
        }
        else
        {
            mapCamera.enabled = false;
            CameraManager.Instance.SmoothSetBlur(15.0f, 0.3f);
            mapCamera.depth = -1;
        }
    }

    private void ReturnMainMenu()
    {
        SceneLoader.Instance.UnloadScene(GameManager.Instance.GameSceneName,
            (AsyncOperation _, string _) => CanvasManager.Instance.ClearCanvas());
        UIManager.Instance.SetMainMenu(true);
        GameManager.Instance.GameState = GameState.MAIN_MENU;
        SceneLoader.Instance.LoadScene(GameManager.Instance.MenuSceneName);
        UIManager.Instance.SetHUDMenu(false);
    }
}
