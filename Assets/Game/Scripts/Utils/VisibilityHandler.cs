using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class VisibilityHandler : MonoBehaviour
{
    public UnityEvent OnVisible;
    public UnityEvent OnInvisible;

    SkinnedMeshRenderer[] skinnedMeshRenderers;
    bool isVisible, firstTime;
    float timeElapsed = 0.0f;
    const float TIME_DELAY = 1.0f;

    private void Start()
    {
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        isVisible = true;
        firstTime = true;
        timeElapsed = 0.0f;
    }

    private void Update()
    {
        if (!CameraManager.ActiveCamera || !CameraManager.ActiveCamera.enabled)
            return;
        timeElapsed += Time.deltaTime;
        if (timeElapsed < TIME_DELAY)
            return;

        bool visibility = IsVisible(CameraManager.ActiveCamera);
        if (visibility && (!isVisible || firstTime))
        {
            isVisible = true;
            OnVisible?.Invoke();
        }
        else if (!visibility && (isVisible || firstTime))
        {
            isVisible = false;
            OnInvisible?.Invoke();
        }
        firstTime = false;
        timeElapsed = 0.0f;
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
