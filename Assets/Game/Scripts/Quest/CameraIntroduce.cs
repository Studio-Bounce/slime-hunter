using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraIntroduce : MonoBehaviour
{
    public float duration = 5;
    public CinemachineVirtualCamera targetCamera;

    private CinemachineVirtualCamera currentCamera;

    public void Transition()
    {
        currentCamera = CameraManager.ActiveCineCamera;
        StartCoroutine(StartTransition());
    }

    private IEnumerator StartTransition()
    {
        StartCoroutine(UIManager.Instance.gladeVillageIntroMenu.FadeIn(2.0f));
        InputManager.Instance.TogglePlayerControls(false);
        CameraManager.Instance.ChangeVirtualCamera(targetCamera);
        yield return new WaitForSeconds(duration);
        CameraManager.Instance.ChangeVirtualCamera(currentCamera);
        InputManager.Instance.TogglePlayerControls(true);
        StartCoroutine(UIManager.Instance.gladeVillageIntroMenu.FadeOut(2.0f));

    }
}
