using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Startup : MonoBehaviour
{
    [Header("Startup Scene References")]
    public Menu InitialUI;
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
            UIManager.Instance.SetLoadMenu(false);
            UIManager.Instance.ShowUI(InitialUI);
        }
    }
}
