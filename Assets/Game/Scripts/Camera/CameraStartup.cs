using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraStartup : MonoBehaviour
{
    private Camera _camera;
    public CinemachineVirtualCamera virtualCamera;

    public bool targetCameraOnStart;

    void Awake()
    {
        _camera = GetComponent<Camera>();

        if (targetCameraOnStart)
        {
            CameraManager.Instance.SetMainCamera(_camera, virtualCamera);
        }
    }
}
