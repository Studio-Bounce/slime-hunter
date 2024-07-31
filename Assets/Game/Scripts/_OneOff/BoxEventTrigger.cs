using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class BoxEventTrigger : MonoBehaviour
{
    public UnityEvent eventTrigger;
    public LayerMask layerMask;
    private BoxCollider _collider;

    void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerMask) != 0)
        {
            eventTrigger.Invoke();
        }
    }
}
