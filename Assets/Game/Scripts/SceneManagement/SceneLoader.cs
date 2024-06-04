using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneLoader : Singleton<SceneLoader>
{
    [Tooltip("Artificial Load Delay")]
    public float loadDelay = 1.0f;

    public void LoadScene(string sceneName, bool showLoadingScreen = true, bool cacheScene = true)
    {
        StartCoroutine(loadScene(sceneName, showLoadingScreen));
    }

    IEnumerator loadScene(string sceneName, bool showLoadingScreen)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded == false)
        {

            if (showLoadingScreen)
            {
                UIManager.Instance.SetLoadMenu(true);
            }

            Application.backgroundLoadingPriority = ThreadPriority.Low;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            operation.completed += (AsyncOperation op) => OnSceneLoaded(op, sceneName);

            // Placeholder for loading bars
            while (!operation.isDone) {
                // float progress = Mathf.Clamp01(operation.progress/ .9f);
                Debug.Log($"Loading Scene [{sceneName}]: {operation.progress}%");
                yield return null; 
            }

            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            if (showLoadingScreen)
            {
                yield return new WaitForSeconds(loadDelay);
                UIManager.Instance.SetLoadMenu(false);
            }
        }
    }

    private void OnSceneLoaded(AsyncOperation asyncOperation, string sceneName) {
        //Scene newScene = SceneManager.GetSceneByName(sceneName);
    }

    public void UnloadScene(string sceneName)
    {
        StartCoroutine(unloadScene(sceneName));
    }

    IEnumerator unloadScene(string sceneName)
    {
        AsyncOperation operation = null;

        try
        {
            operation = SceneManager.UnloadSceneAsync(sceneName);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        if (operation != null)
        {
            while (operation.isDone == false) { yield return null; }
        }

        operation = Resources.UnloadUnusedAssets();
        while (operation.isDone == false) { yield return null; }
    }
}
