using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] Transform playerStartT;
    [SerializeField] Transform playerEndT;
    [SerializeField] Transform myloOnBoat;
    [SerializeField] PlayerSpawner playerSpawner;

    [SerializeField] Transform boat;
    [SerializeField] Transform boatStartT;
    [SerializeField] Transform boatEndT;
    [SerializeField] float timeTaken = 5.0f;

    public CinemachineVirtualCamera boatCamera;

    CinemachineVirtualCamera currentCamera;
    bool boatMoved = false;

    public void InitiateBoatSequence()
    {
        if (boatMoved) return;
        boatMoved = true;

        // Disable controls & give a good camera angle
        InputManager.Instance.TogglePlayerControls(false);
        currentCamera = CameraManager.ActiveCineCamera;
        CameraManager.Instance.ChangeVirtualCamera(boatCamera);

        // Place player on the boat
        GameManager.Instance.PlayerRef.HideVisuals();
        myloOnBoat.gameObject.SetActive(true);

        // Move the boat
        StartCoroutine(InitiateMovement());
    }

    void EndBoatSequence()
    {
        if (!boatMoved) return;

        // Remove player from boat
        GameManager.Instance.PlayerRef.ShowVisuals();
        myloOnBoat.gameObject.SetActive(false);
        playerSpawner.MovePlayerTEMP(playerEndT.position);

        // Revert controls & camera
        CameraManager.Instance.ChangeVirtualCamera(currentCamera);
        InputManager.Instance.TogglePlayerControls(true);
    }

    IEnumerator InitiateMovement()
    {
        float timeElapsed = 0;

        boat.transform.localRotation = Quaternion.LookRotation(boatStartT.position - boatEndT.position);
        boat.transform.localPosition = boatStartT.position;

        while (timeElapsed < timeTaken)
        {
            timeElapsed += Time.deltaTime;

            boat.transform.localPosition = Vector3.Lerp(boatStartT.position, boatEndT.position, timeElapsed / timeTaken);

            yield return null;
        }

        boat.transform.localPosition = boatEndT.position;
        EndBoatSequence();
    }

    private void OnDrawGizmos()
    {
        if (boatStartT != null && boatEndT != null)
        {
            DebugExtension.DrawArrow(boatStartT.position, boatEndT.position - boatStartT.position);
        }
    }
}
