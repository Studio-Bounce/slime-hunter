using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFall : MonoBehaviour
{
    public Transform target;
    public float slowDownTime = 5.0f;

    Rigidbody body;
    bool actionPerformed = false;
    bool imageShown = false;
    bool tutorialInProgress = false;
    bool moveThePlayer = false;

    bool spacePressed = false;

    [SerializeField] float cameraShakeIntensity = 0.3f;

    private void Start()
    {
        actionPerformed = false;
        imageShown = false;
        body = GetComponent<Rigidbody>();

        InputManager.Instance.OnDashAction = e => RegisterDash();
    }

    void RegisterDash()
    {
        spacePressed = true;
    }

    // Used in Dash tutorial
    public void ObjectFallTutorial()
    {
        if (actionPerformed)
        {
            return;
        }
        actionPerformed = true;

        tutorialInProgress = true;
        moveThePlayer = true;

        body.useGravity = true;
        StartCoroutine(HeadPlayerTowards(target.position));
        StartCoroutine(WaitForSpace());
    }

    IEnumerator HeadPlayerTowards(Vector3 destination)
    {
        // Freeze player movement, attack, spells
        InputManager.Instance.TogglePlayerMovement(false);
        InputManager.Instance.TogglePlayerMeleeControls(false);
        InputManager.Instance.TogglePlayerSpellControls(false);

        // Force move player
        Vector2 dest2D = new Vector2(destination.x, destination.z);
        Transform playerT = GameManager.Instance.PlayerRef.transform;
        InputManager.Instance.MovementOverride(Vector2.zero);
        while (moveThePlayer)
        {
            Vector2 delta = dest2D - new Vector2(playerT.position.x, playerT.position.z);
            if (delta.sqrMagnitude < 0.1f)
                break;

            InputManager.Instance.MovementOverride(delta.normalized);

            yield return null;
        }
        InputManager.Instance.MovementOverride(Vector2.zero);
        GameManager.Instance.PlayerRef.transform.LookAt(target.position + target.forward);

        // Waiting for player to press space
        while (tutorialInProgress)
        {
            yield return null;
        }

        // Unfreeze player movement, attack, spells
        InputManager.Instance.TogglePlayerMovement(true);
        InputManager.Instance.TogglePlayerMeleeControls(true);
        InputManager.Instance.TogglePlayerSpellControls(true);
    }

    IEnumerator WaitForSpace()
    {
        Time.timeScale = 0.6f;
        spacePressed = false;

        while (tutorialInProgress)
        {
            // Show image in 0.5 seconds
            if (!imageShown && Time.timeScale < 0.3f)
            {
                imageShown = true;
                CameraManager.Instance.ShakeCamera(cameraShakeIntensity);
            }
            // Scale down to 0.1 in slowDownTime seconds
            if (Time.timeScale > 0.1f)
            {
                Time.timeScale -= 0.17f*Time.unscaledDeltaTime;
            }
            else if (Time.timeScale > 0 && moveThePlayer)
            {
                // Stop the player
                moveThePlayer = false;
                // Stop the fall
                body.useGravity = false;
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            if (imageShown && spacePressed)
            {
                body.useGravity = true;
                moveThePlayer = false;
                tutorialInProgress = false;
                break;
            }
            yield return null;
        }
        CameraManager.Instance.StopCameraShake();
        Time.timeScale = 1.0f;
    }
}
