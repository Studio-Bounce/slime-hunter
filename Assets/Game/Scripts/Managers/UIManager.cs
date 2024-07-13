using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] public Menu loadMenu;
    [SerializeField] public Menu mainMenu;
    [SerializeField] public Menu pauseMenu;
    [SerializeField] public Menu HUDMenu;
    [SerializeField] public Menu autoSave;

    // ----------------- Pause -----------------
    public void SetPauseMenu(bool active)
    {
        if (active)
        {
            (pauseMenu as PauseMenu).Pause();
        } else
        {
            (pauseMenu as PauseMenu).Unpause();
        }
    }

    // ----------------- Load -----------------
    public void SetLoadMenu(bool active)
    {
        loadMenu.SetVisible(active);
    }

    // ----------------- Main Menu -----------------
    public void SetMainMenu(bool active)
    {
        mainMenu.SetVisible(active);
    }

    // ----------------- HUD -----------------
    public void SetHUDMenu(bool active)
    {
        HUDMenu.SetVisible(active);
    }

    // ----------------- Auto-save -----------------
    public void ShowAutoSave()
    {
        autoSave.Show();
    }
    public void HideAutoSave()
    {
        autoSave.Hide();
    }

    // ----------------- Generic -----------------
    public void ShowUI(Menu menuOBJ)
    {
        menuOBJ.Show();
    }

    public void HideUI(Menu menuOBJ)
    {
        menuOBJ.Hide();
    }

    public void ClearVisualElement(VisualElement veToClear)
    {
        // Safe-deletion
        List<VisualElement> veItems = new();
        foreach (VisualElement veItem in veToClear.Children())
        {
            veItems.Add(veItem);
        }
        foreach (VisualElement veItem in veItems)
        {
            veToClear.Remove(veItem);
        }
    }
}
