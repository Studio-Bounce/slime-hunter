using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OpenHinge : MonoBehaviour
{
    [Header("Hinge")]
    public Transform hinge;
    public Vector3 openAngles = Vector3.zero;
    public bool openOnStart = false;

    [Header("Animation")]
    public float duration = 1.0f;
    public float delay = 0.0f;
    public AnimationCurve curve;

    [Header("Event")]
    public UnityEvent openStartEvent;
    public UnityEvent openEvent;

    public bool IsOpen { get; private set; } = false;
    private Vector3 start;
    private Vector3 end;

    private void Start()
    {
        Debug.Assert(hinge != null);
        start = hinge.rotation.eulerAngles;
        end = start + openAngles;
        if (openOnStart) Open();
    }

    public void Open()
    {
        if (IsOpen) return;
        IsOpen = true;
        openStartEvent.Invoke();
        StartCoroutine(BeginOpen());
    }

    IEnumerator BeginOpen()
    {
        yield return new WaitForSeconds(delay);
        float timer = 0;
        while (timer < duration)
        {
            float normalTime = timer / duration;
            float eased = curve.Evaluate(normalTime);
            Vector3 rotation = Vector3.LerpUnclamped(start, end, eased);
            hinge.rotation = Quaternion.Euler(rotation);
            timer += Time.deltaTime;
            yield return null;
        }
        openEvent.Invoke();
    }

    private void OnDrawGizmos()
    {
        if (hinge == null) return;

        // Set the Gizmos matrix to match the GameObject's transform
        Gizmos.matrix = hinge.localToWorldMatrix;

        // Draw the forward direction
        Gizmos.color = Color.blue;
        Vector3 forwardDirection = Quaternion.Euler(openAngles) * Vector3.forward;
        Gizmos.DrawLine(Vector3.zero, forwardDirection);
        Gizmos.DrawSphere(forwardDirection, 0.1f);

        // Draw the right direction
        Gizmos.color = Color.red;
        Vector3 rightDirection = Quaternion.Euler(openAngles) * Vector3.right;
        Gizmos.DrawLine(Vector3.zero, rightDirection);
        Gizmos.DrawSphere(rightDirection, 0.1f);

        // Draw the up direction
        Gizmos.color = Color.green;
        Vector3 upDirection = Quaternion.Euler(openAngles) * Vector3.up;
        Gizmos.DrawLine(Vector3.zero, upDirection);
        Gizmos.DrawSphere(upDirection, 0.1f);
    }
}
