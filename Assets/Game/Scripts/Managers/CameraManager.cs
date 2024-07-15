using Cinemachine;
using Ink.Runtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraManager : Singleton<CameraManager>
{
    // Cameras
    private static Camera _activeCamera;
    private static CinemachineVirtualCamera _activeVCamera;
    public static Camera ActiveCamera { get { return _activeCamera; } }
    public static CinemachineVirtualCamera ActiveCineCamera { get { return _activeVCamera; } }

    private static Volume _globalVolume;

    // Look for a global volume in the scene if isn't already set
    public static Volume GlobalVolume
    {
        get
        {
            if (_globalVolume != null) return _globalVolume;

            // Find all Volume components in the scene
            Volume[] volumes = FindObjectsOfType<Volume>();

            // Look for a volume with the "isGlobal" flag set to true
            foreach (Volume volume in volumes)
            {
                if (volume.isGlobal)
                {
                    return volume;
                }
            }

            // No global volume found, return null
            return null;
        }
    }

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

    #region PostProcess

    // Ideally target would be another VolumeComponent but for simplicity, we'll only set the intensity therefore passing in just a target float

    public IEnumerator SmoothSetVignette(float target, float duration)
    {
        Vignette _vignette;
        if (GlobalVolume.profile.TryGet(out _vignette))
        {
            float startIntensity = _vignette.intensity.value;

            float elapsedTime = 0f;
            float t = 0;
            while (elapsedTime < duration)
            {
                t = elapsedTime / duration;
                _vignette.intensity.value = Mathf.Lerp(startIntensity, target, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
    }

    public IEnumerator SmoothSetChromatic(float target, float duration)
    {
        ChromaticAberration _chroma;
        if (GlobalVolume.profile.TryGet(out _chroma))
        {
            float startIntensity = _chroma.intensity.value;

            float elapsedTime = 0f;
            float t = 0;
            while (elapsedTime < duration)
            {
                t = elapsedTime / duration;
                _chroma.intensity.value = Mathf.Lerp(startIntensity, target, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    #endregion
}
