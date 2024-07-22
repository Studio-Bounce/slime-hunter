using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class RadialEventTrigger : MonoBehaviour
{
    public UnityEvent onEnter;
    public UnityEvent onExit;
    public LayerMask layerMask;
    public SphereCollider _collider;

    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerMask) != 0)
        {
            onEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerMask) != 0)
        {
            onExit.Invoke();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_collider == null) return;
        Handles.color = Color.cyan;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, _collider.radius, 2);
    }
#endif
}
