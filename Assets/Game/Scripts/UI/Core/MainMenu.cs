using UnityEngine;
using UnityEngine.UIElements;
using System;
using FMODUnity;

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
        Button btnPlay = root.Q<Button>("btnPlay");
        //Button btnContinue = root.Q<Button>("btnContinue");
        Button btnSettings = root.Q<Button>("btnSettings");
        Button btnQuit = root.Q<Button>("btnQuit");
        btnPlay.RegisterCallback<ClickEvent>(ev => InitiateGame(SetStartState));
        //btnContinue.RegisterCallback<ClickEvent>(ev => InitiateGame(LoadData));
        btnSettings.RegisterCallback<ClickEvent>(ev =>
        {
            Hide();
            UIManager.Instance.settingsMenu.Show();
        });
        btnQuit.RegisterCallback<ClickEvent>(ev => QuitGame());
    }

    public void InitiateGame(Action<AsyncOperation, string> sceneLoadCallback = null)
    {
        RuntimeManager.PlayOneShot(AudioManager.Config.startGameEvent);
        GameManager.Instance.GameState = GameState.LOADING;
        // Ensure that core scene's camera is enabled
        CameraManager.Instance.SetMainCamera(coreCamera);
        SceneLoader.Instance.UnloadScene(GameManager.Instance.MenuSceneName);
        SceneLoader.Instance.LoadScene(GameManager.Instance.GameSceneName, callback: sceneLoadCallback);
        UIManager.Instance.SetMainMenu(false);
        UIManager.Instance.SetHUDMenu(true);
        UIManager.Instance.ResetEndScreen();
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
        RuntimeManager.PlayOneShot(AudioManager.Config.exitGameEvent);
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
