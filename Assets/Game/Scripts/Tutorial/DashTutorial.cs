using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTutorial : MonoBehaviour
{
    SlimeSteeringAgent sliAgent;
    DashSlime_FSM slimeFSM;

    public GameObject birdGO;
    public CinemachineVirtualCamera targetCamera;

    private CinemachineVirtualCamera currentCamera;
    bool triggered = false;
    bool processing = false;

    private void Start()
    {
        slimeFSM = GetComponent<DashSlime_FSM>();
        sliAgent = GetComponent<SlimeSteeringAgent>();
        sliAgent.PauseAgent();
    }

    public void HeadToBirds()
    {
        if (triggered)
            return;

        triggered = true;
        processing = true;
        currentCamera = CameraManager.ActiveCineCamera;
        StartCoroutine(SwitchCamera());

        sliAgent.UnpauseAgent();
        slimeFSM.SetPlayerTransform(birdGO.transform);
    }

    IEnumerator SwitchCamera()
    {
        InputManager.Instance.TogglePlayerControls(false);
        CameraManager.Instance.ChangeVirtualCamera(targetCamera);
        while (processing)
        {
            yield return null;
        }
        CameraManager.Instance.ChangeVirtualCamera(currentCamera);
        InputManager.Instance.TogglePlayerControls(true);
    }

    public void BirdsStartMoving()
    {
        slimeFSM.SetPlayerTransform(GameManager.Instance.PlayerRef.transform);
    }

    public void BirdsAreAway()
    {
        if (!processing)
            return;

        processing = false;
    }
}
