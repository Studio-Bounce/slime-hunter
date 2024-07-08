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

    public void LoadScene(string sceneName, bool showLoadingScreen = true, Action<AsyncOperation, string> callback = null)
    {
        StartCoroutine(loadScene(sceneName, showLoadingScreen, callback));
    }

    IEnumerator loadScene(string sceneName, bool showLoadingScreen, Action<AsyncOperation, string> callback)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded == false)
        {

            if (showLoadingScreen)
            {
                UIManager.Instance.SetLoadMenu(true);
            }

            Application.backgroundLoadingPriority = ThreadPriority.Low;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (callback != null)
            {
                operation.completed += (AsyncOperation op) => callback.Invoke(op, sceneName);
            }

            // Placeholder for loading bars
            while (!operation.isDone) {
                // float progress = Mathf.Clamp01(operation.progress/ .9f);
                //Debug.Log($"Loading Scene [{sceneName}]: {operation.progress}%");
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

    public void UnloadScene(string sceneName, Action<AsyncOperation, string> callback = null)
    {
        StartCoroutine(unloadScene(sceneName, callback));
    }

    IEnumerator unloadScene(string sceneName, Action<AsyncOperation, string> callback)
    {
        AsyncOperation operation = null;

        try
        {
            operation = SceneManager.UnloadSceneAsync(sceneName);
            if (callback != null)
            {
                operation.completed += (AsyncOperation op) => callback.Invoke(op, sceneName);
            }
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
