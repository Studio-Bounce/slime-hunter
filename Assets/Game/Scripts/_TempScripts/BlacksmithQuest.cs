using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithQuest : MonoBehaviour
{
    public GameObject questItem;
    public CinemachineVirtualCamera targetCamera;
    public MoveBridge moveBridge;

    private CinemachineVirtualCamera currentCamera;
    bool triggered = false;

    public void CompleteQuest()
    {
        if (triggered)
            return;

        triggered = true;
        Destroy(questItem);  // pick up the item

        // Move the bridge
        currentCamera = CameraManager.ActiveCineCamera;
        StartCoroutine(MoveTheBridge());
    }

    IEnumerator MoveTheBridge()
    {
        InputManager.Instance.TogglePlayerControls(false);
        CameraManager.Instance.ChangeVirtualCamera(targetCamera);

        moveBridge.MoveTheBridge();
        while (!moveBridge.IsBridgeDown())
        {
            yield return null;
        }

        CameraManager.Instance.ChangeVirtualCamera(currentCamera);
        InputManager.Instance.TogglePlayerControls(true);
    }
}
