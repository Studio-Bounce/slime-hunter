using UnityEngine;
using UnityEngine.UIElements;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : Menu
{
    public Camera coreCamera;

    protected override void Awake()
    {
        base.Awake();
        if (coreCamera == null)
        {
            coreCamera = Camera.main;
        }
    }

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        Label lblPlay = root.Q<Label>("lblPlay");
        Label lblContinue = root.Q<Label>("lblContinue");
        Label lblSettings = root.Q<Label>("lblSettings");
        Label lblQuit = root.Q<Label>("lblQuit");
        lblPlay.RegisterCallback<ClickEvent>(ev => InitiateGame(SetStartState));
        lblContinue.RegisterCallback<ClickEvent>(ev => InitiateGame(LoadData));
        lblSettings.RegisterCallback<ClickEvent>(ev =>
        {
            Hide();
            UIManager.Instance.settingsMenu.Show();
        });
        lblQuit.RegisterCallback<ClickEvent>(ev => QuitGame());
    }

    void InitiateGame(Action<AsyncOperation, string> sceneLoadCallback = null)
    {
        GameManager.Instance.GameState = GameState.LOADING;
        // Ensure that core scene's camera is enabled
        CameraManager.Instance.SetMainCamera(coreCamera);
        SceneLoader.Instance.UnloadScene(GameManager.Instance.MenuSceneName);
        SceneLoader.Instance.LoadScene(GameManager.Instance.GameSceneName, callback: sceneLoadCallback);
        UIManager.Instance.SetMainMenu(false);
        UIManager.Instance.SetHUDMenu(true);
    }

    void SetStartState(AsyncOperation _, string _s)
    {
        GameManager.Instance.GameState = GameState.GAMEPLAY;
    }

    void LoadData(AsyncOperation _, string _s)
    {
        PersistenceManager.Instance.LoadGame();
        SetStartState(_, _s);
    }

    private void QuitGame()
    {
        GameManager.Instance.GameState = GameState.GAME_OVER;
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
