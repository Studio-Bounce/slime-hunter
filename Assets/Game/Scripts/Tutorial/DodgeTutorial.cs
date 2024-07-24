using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeTutorial : MonoBehaviour
{
    public CinemachineVirtualCamera cameraA;
    public float delayToCamA = 3.0f;
    public float delayPostDodge = 4.0f;
    public CinemachineVirtualCamera cameraB;
    public Rigidbody rockRigidBody;
    public Transform dodgeDestination;

    SlimeSteeringAgent sliAgent;
    RabbitEnemy rabbitEnemy;

    CinemachineVirtualCamera currentCamera;
    bool triggered = false;
    bool isRockClose = false;

    private void Start()
    {
        triggered = false;
        isRockClose = false;
        rabbitEnemy = GetComponent<RabbitEnemy>();
        sliAgent = GetComponent<SlimeSteeringAgent>();
        sliAgent.PauseAgent();
    }

    public void RockIsClose()
    {
        isRockClose = true;
    }

    public void TriggerTutorial()
    {
        if (triggered)
        {
            return;
        }
        triggered = true;

        currentCamera = CameraManager.ActiveCineCamera;
        StartCoroutine(InitiateTutorial());
    }

    IEnumerator InitiateTutorial()
    {
        InputManager.Instance.TogglePlayerControls(false);
        CameraManager.Instance.ChangeVirtualCamera(cameraA);
        yield return new WaitForSeconds(delayToCamA);
        CameraManager.Instance.ChangeVirtualCamera(cameraB);

        // Fall the boulder
        rockRigidBody.useGravity = true;

        while (!isRockClose)
        {
            yield return null;
        }

        // Dodge
        rabbitEnemy.ApplyDodgeExternal(dodgeDestination.position - transform.position);
        yield return new WaitForSeconds(delayPostDodge);

        CameraManager.Instance.ChangeVirtualCamera(currentCamera);
        InputManager.Instance.TogglePlayerControls(true);
        sliAgent.UnpauseAgent();

        yield return new WaitForSeconds(2);
        rockRigidBody.freezeRotation = true;
        rockRigidBody.constraints = RigidbodyConstraints.FreezePosition;
        rockRigidBody.gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
