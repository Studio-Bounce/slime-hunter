using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EndScreen : Menu
{
    VisualElement root;
    VisualElement rollingFrame;

    public float rollSpeed = 300f;
    public float rollTime = 10.0f;
    bool rollCredits = false;
    bool returnedToMainMenu = false;
    float timeElapsed = 0;

    private void Start()
    {
        root = uiDocument.rootVisualElement;
        rollingFrame = root.Q<VisualElement>("Frame1");
        rollCredits = false;
    }

    private void Update()
    {
        if (rollCredits)
        {
            Vector3 position = rollingFrame.transform.position;
            position.y -= (rollSpeed * Time.deltaTime);
            rollingFrame.transform.position = position;

            timeElapsed += Time.deltaTime;
            if (timeElapsed > rollTime)
            {
                // Return to main menu
                ReturnToMainMenu();
            }
        }
    }

    public void NewGame()
    {
        rollCredits = false;
        returnedToMainMenu = false;

        Vector3 position = rollingFrame.transform.position;
        position.y = 0;
        rollingFrame.transform.position = position;
    }

    public void RollCredits()
    {
        if (rollingFrame == null)
            return;

        Show();
        rollCredits = true;
        timeElapsed = 0;
    }

    void ReturnToMainMenu()
    {
        if (returnedToMainMenu)
            return;

        returnedToMainMenu = true;

        SceneLoader.Instance.UnloadScene(GameManager.Instance.GameSceneName,
            (AsyncOperation _, string _) => CanvasManager.Instance.ClearCanvas());
        UIManager.Instance.SetMainMenu(true);
        GameManager.Instance.GameState = GameState.MAIN_MENU;
        SceneLoader.Instance.LoadScene(GameManager.Instance.MenuSceneName);
        Hide();
    }
}
