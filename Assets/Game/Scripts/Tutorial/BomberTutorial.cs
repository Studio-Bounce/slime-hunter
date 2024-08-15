using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberTutorial : MonoBehaviour
{
    public CinemachineVirtualCamera targetCamera;

    [SerializeField] GameObject exploBomber;
    [SerializeField] List<GameObject> followingBombers = new List<GameObject>();
    [SerializeField] float waitBetweenBombers = 2f;
    [SerializeField] float waitAfterAllBombers = 5f;

    CinemachineVirtualCamera currentCamera;
    bool triggered = false;
    bool processing = false;

    public void StartTutorial()
    {
        if (triggered) return;
        triggered = true;
        processing = true;

        StartCoroutine(InitiateBombTutorial());
    }

    IEnumerator InitiateBombTutorial()
    {
        currentCamera = CameraManager.ActiveCineCamera;
        InputManager.Instance.TogglePlayerControls(false);
        CameraManager.Instance.ChangeVirtualCamera(targetCamera);

        // Wait till bomber dies
        exploBomber.SetActive(true);
        while (exploBomber != null)
        {
            yield return null;
        }
        foreach (GameObject go in followingBombers)
        {
            go.SetActive(true);
            yield return new WaitForSeconds(waitBetweenBombers);
        }
        yield return new WaitForSeconds(waitAfterAllBombers);

        CameraManager.Instance.ChangeVirtualCamera(currentCamera);
        InputManager.Instance.TogglePlayerControls(true);
    }

    public void TutorialComplete()
    {
        processing = false;
    }
}
