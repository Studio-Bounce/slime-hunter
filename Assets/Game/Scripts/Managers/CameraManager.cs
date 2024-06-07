using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public Camera _activeCamera;
    public CinemachineVirtualCamera _activeCineCamera;

    public Camera ActiveCamera { get { return _activeCamera; } }
    public CinemachineVirtualCamera ActiveCineCamera { get { return _activeCineCamera; } }

    public void SwitchToCamera(Camera cam)
    {
        if (_activeCamera)
        {
            _activeCamera.enabled = false;
        }

        _activeCamera = cam;
        _activeCamera.enabled = true;
        _activeCineCamera = cam.GetComponent<CinemachineBrain>()?.ActiveVirtualCamera as CinemachineVirtualCamera;
    }

    public void ShakeCamera(float intensity, float time)
    {
        if (_activeCineCamera == null) return;
        StartCoroutine(StartShake(intensity, time));
    }

    IEnumerator StartShake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin camNoise = _activeCineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (camNoise == null)
        {
            Debug.Log("No nosie on virtual camera to shake");
        }
        else
        {
            camNoise.m_AmplitudeGain = intensity;
            yield return new WaitForSeconds(time);
            camNoise.m_AmplitudeGain = 0;
        }
    }
}
