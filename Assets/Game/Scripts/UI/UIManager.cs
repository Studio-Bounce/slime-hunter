using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : Singleton<UIManager>
{
    public Menu loadMenu;
    public Menu mainMenu;
    public Menu pauseMenu;
    public Menu HUDMenu;//riccio's test

    public void SetPauseMenu(bool active)
    {
        pauseMenu.SetVisible(active);
    }

    public void SetLoadMenu(bool active)
    {
        loadMenu.SetVisible(active);
    }

    public void SetMainMenu(bool active)
    {
        mainMenu.SetVisible(active);
    }

    public void SetHUDMenu(bool active)
    {
        HUDMenu.SetVisible(active);
    }//riccio's test

    public void ShowUI(Menu menuOBJ)
    {
        menuOBJ.Show();
    }

    public void HideUI(Menu menuOBJ)
    {
        menuOBJ.Hide();
    }
}
