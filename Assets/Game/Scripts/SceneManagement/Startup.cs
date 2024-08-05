using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Startup : MonoBehaviour
{
    [Header("Startup Scene References")]
    public Menu InitialUI;
    public string StartupSceneName;
    public float InitialBootDelay = 1.0f;

    void Start()
    {
        StartCoroutine(BootSequence());
        return;
    }

    IEnumerator BootSequence()
    {
        GameManager.Instance.GameState = GameState.LOADING;
        yield return new WaitForSecondsRealtime(InitialBootDelay);
        if (InitialUI != null)
        {
            GameManager.Instance.GameState = GameState.MAIN_MENU;
            UIManager.Instance.ShowUI(InitialUI);
            SceneLoader.Instance.LoadScene(StartupSceneName);
        }
    }
}
