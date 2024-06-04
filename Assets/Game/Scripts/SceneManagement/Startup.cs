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
        yield return new WaitForSeconds(InitialBootDelay);
        if (InitialUI != null )
        {
            UIManager.Instance.ShowUI(InitialUI);
            SceneLoader.Instance.LoadScene(StartupSceneName);
        }
    }
}
