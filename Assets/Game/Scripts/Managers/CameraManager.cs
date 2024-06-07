using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public Camera _activeCamera;
    public CinemachineVirtualCamera _activeVCamera;

    public Camera ActiveCamera { get { return _activeCamera; } }
    public CinemachineVirtualCamera ActiveCineCamera { get { return _activeVCamera; } }

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
}
