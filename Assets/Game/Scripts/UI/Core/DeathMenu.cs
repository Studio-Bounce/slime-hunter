using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeathMenu : Menu
{
    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        Button tryAgainBtn = root.Q<Button>("TryAgain");
        Button mainMenuBtn = root.Q<Button>("MainMenu");

        // Register callbacks
        tryAgainBtn.RegisterCallback<ClickEvent>(e =>
        {
            Hide();
            PersistenceManager.Instance.LoadGame();
            GameManager.Instance.GameState = GameState.GAMEPLAY;
            GameManager.Instance.PlayerHealth = GameManager.Instance.PlayerMaxHealth;
        });

        mainMenuBtn.RegisterCallback<ClickEvent>(e =>
        {
            Hide();
            GameManager.Instance.PlayerHealth = GameManager.Instance.PlayerMaxHealth;
            SceneLoader.Instance.UnloadScene(GameManager.Instance.GameSceneName);
            UIManager.Instance.mainMenu.Show();
            GameManager.Instance.GameState = GameState.MAIN_MENU;
            SceneLoader.Instance.LoadScene(GameManager.Instance.MenuSceneName);
            UIManager.Instance.SetHUDMenu(false);
        });
    }

    public override void Show()
    {
        base.Show();
        GameManager.Instance.TimeFreeze();
        CameraManager.Instance.SmoothSetVignette(0.5f, 1.0f);
        CameraManager.Instance.SmoothSetSaturation(-100, 0.3f);
        CameraManager.Instance.SmoothSetChromatic(1.0f, 0.3f);
    }

    public override void Hide()
    {
        base.Hide();
        GameManager.Instance.TimeNormal();
        CameraManager.Instance.SmoothSetVignette(0, 0.3f);
        CameraManager.Instance.SmoothSetSaturation(8, 0.3f);
        CameraManager.Instance.SmoothSetChromatic(0, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
