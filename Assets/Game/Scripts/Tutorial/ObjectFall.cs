using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFall : MonoBehaviour
{
    public GameObject spacebarImage;
    bool actionPerformed = false;
    bool imageShown = false;

    [SerializeField] float cameraShakeIntensity = 0.3f;

    private void Start()
    {
        spacebarImage.SetActive(false);
        actionPerformed = false;
        imageShown = false;
    }

    public void SlowDownTime()
    {
        if (actionPerformed)
        {
            return;
        }
        actionPerformed = true;
        StartCoroutine(WaitForSpace());
        CameraManager.Instance.ShakeCamera(cameraShakeIntensity);
    }

    IEnumerator WaitForSpace()
    {
        Time.timeScale = 0.2f;
        while (true)
        {
            // Show image in 0.5 seconds
            if (!imageShown && Time.timeScale < 0.18f)
            {
                imageShown = true;
                spacebarImage.SetActive(true);
            }

            // Scale down to 0.1 in 1 second
            if (Time.timeScale > 0.1f)
            {
                Time.timeScale -= 0.1f * Time.deltaTime;
            }
            // Scale further down to 0.05 in 1 second
            else if (Time.timeScale > 0.05f)
            {
                Time.timeScale -= 0.05f * Time.deltaTime;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResumeTime();
                break;
            }
            yield return null;
        }
    }

    void ResumeTime()
    {
        CameraManager.Instance.StopCameraShake();
        Time.timeScale = 1.0f;
    }
}
