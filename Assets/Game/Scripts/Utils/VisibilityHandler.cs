using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisibilityHandler : MonoBehaviour
{
    public UnityEvent OnVisible;
    public UnityEvent OnInvisible;

    SkinnedMeshRenderer[] skinnedMeshRenderers;
    bool isVisible;

    private void Start()
    {
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        isVisible = true;
    }

    private void Update()
    {
        if (!CameraManager.ActiveCamera || !CameraManager.ActiveCamera.enabled)
            return;

        bool visibility = IsVisible(CameraManager.ActiveCamera);
        if (visibility && !isVisible)
        {
            isVisible = true;
            OnVisible?.Invoke();
        }
        else if (!visibility && isVisible)
        {
            isVisible = false;
            OnInvisible?.Invoke();
        }
    }

    bool IsVisible(Camera camera)
    {
        bool isVisible = false;
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            if (GeometryUtility.TestPlanesAABB(planes, skinnedMeshRenderer.bounds))
            {
                isVisible = true;
                break;
            }
        }
        return isVisible;
    }
}
