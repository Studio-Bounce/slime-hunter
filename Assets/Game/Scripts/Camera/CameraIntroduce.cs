using Cinemachine;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class CameraIntroduce : MonoBehaviour
{
    public float duration = 5;
    public CinemachineVirtualCamera targetCamera;

    CinemachineVirtualCamera currentCamera;

    public void Transition()
    {
        Debug.Log("Transition Hello");
        currentCamera = CameraManager.ActiveCineCamera;
        StartCoroutine(StartTransition());
    }

    private IEnumerator StartTransition()
    {
        InputManager.Instance.TogglePlayerControls(false);
        CameraManager.Instance.ChangeVirtualCamera(targetCamera);
        yield return UIManager.Instance.gladeVillageIntroMenu.FadeIn(2.0f);

        yield return new WaitForSecondsRealtime(duration);

        yield return UIManager.Instance.gladeVillageIntroMenu.FadeOut(2.0f);
        CameraManager.Instance.ChangeVirtualCamera(currentCamera);
        InputManager.Instance.TogglePlayerControls(true);
    }
}
