using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] Transform boatT;
    [SerializeField] Transform destinationT;
    [SerializeField] GoFromAToB boatMover;

    public CinemachineVirtualCamera boatCamera;

    CinemachineVirtualCamera currentCamera;
    bool boatMoved = false;

    public void InitiateBoatSequence()
    {
        if (boatMoved) return;

        boatMoved = true;

        // Disable controls & give a good camera angle
        InputManager.Instance.TogglePlayerControls(false);
        //currentCamera = CameraManager.ActiveCineCamera;
        //CameraManager.Instance.ChangeVirtualCamera(boatCamera);

        // Place player on the boat
        Player playerRef = GameManager.Instance.PlayerRef;
        playerRef.transform.position = boatT.position;
        playerRef.transform.LookAt(boatT.forward);

        // Move the boat
        boatMover.MoveAToB();
    }

    public void EndBoatSequence()
    {
        if (!boatMoved) return;

        // Remove player from boat
        Player playerRef = GameManager.Instance.PlayerRef;
        playerRef.transform.position = destinationT.position;

        // Revert controls & camera
        //CameraManager.Instance.ChangeVirtualCamera(currentCamera);
        InputManager.Instance.TogglePlayerControls(true);
    }
}
