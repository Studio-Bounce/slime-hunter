using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerCallback : MonoBehaviour
{
    [Header("OnTriggerEnter")]
    [Tooltip("Detected layers in OnTriggerEnter")]
    public LayerMask enterHitLayers;
    public UnityEvent onTriggerEnterEvent;
    [Tooltip("Should it be triggered only once?")]
    public bool runEnterOnce = true;
    bool enteredOnce = false;

    [Header("OnTriggerExit")]
    [Tooltip("Detected layers in OnTriggerExit")]
    public LayerMask exitHitLayers;
    public UnityEvent onTriggerExitEvent;
    [Tooltip("Should it be triggered only once?")]
    public bool runExitOnce = true;
    bool exitedOnce = false;

    private void Start()
    {
        enteredOnce = false;
        exitedOnce = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((!runEnterOnce || !enteredOnce) &&
            (enterHitLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            enteredOnce = true;
            onTriggerEnterEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((!runExitOnce || !exitedOnce) &&
            (enterHitLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            exitedOnce = true;
            onTriggerExitEvent.Invoke();
        }
    }
}
