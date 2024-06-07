using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraStartup : MonoBehaviour
{
    private Camera _camera;

    public bool targetCameraOnStart;

    void Start()
    {
        _camera = GetComponent<Camera>();

        if (targetCameraOnStart)
        {
            CameraManager.Instance.SwitchToCamera(_camera);
        }
    }
}
