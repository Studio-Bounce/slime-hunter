using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public static Camera _activeCamera;
    public static CinemachineVirtualCamera _activeVCamera;

    public static Camera ActiveCamera { get { return _activeCamera; } }
    public static CinemachineVirtualCamera ActiveCineCamera { get { return _activeVCamera; } }

    public void SetCameraFollow(Transform _transform)
    {
        if (ActiveCineCamera != null)
        {
            ActiveCineCamera.Follow = _transform;
        } else
        {
            Debug.Log("No active CineCamera");
        }
    }

    public void SwitchToCamera(Camera cam, CinemachineVirtualCamera vCam = null)
    {
        if (_activeCamera)
        {
            _activeCamera.enabled = false;
        }

        _activeCamera = cam;
        _activeCamera.enabled = true;

        _activeVCamera = vCam;
        if (_activeVCamera == null)
        {
            _activeVCamera = cam.GetComponent<CinemachineBrain>()?.ActiveVirtualCamera as CinemachineVirtualCamera;
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        if (_activeVCamera == null) return;
        StartCoroutine(StartShake(intensity, time));
    }

    IEnumerator StartShake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin camNoise = _activeVCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (camNoise == null)
        {
            Debug.Log("No noise channel on virtual camera to shake");
        }
        else
        {
            camNoise.m_AmplitudeGain = intensity;
            yield return new WaitForSeconds(time);
            camNoise.m_AmplitudeGain = 0;
        }
    }

    public Vector3 DirectionToCameraForward(Vector3 targetPosition, Vector2 targetDirection)
    {
        // Get forward based on camera
        Vector3 cameraToPlayer = (targetPosition - _activeCamera.transform.position);
        Vector2 forwardDirection = new Vector2(cameraToPlayer.x, cameraToPlayer.z);
        forwardDirection.Normalize();
        Vector2 rightDirection = new Vector2(forwardDirection.y, -forwardDirection.x);

        // Calculate movement direction based on forward
        Vector2 direction2D = (forwardDirection * targetDirection.y + rightDirection * targetDirection.x).normalized;
        return new Vector3(direction2D.x, 0, direction2D.y);
    }
}
