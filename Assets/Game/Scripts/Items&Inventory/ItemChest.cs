using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(OpenHinge))]
public class ItemChest : MonoBehaviour
{
    public LayerMask activationLayers;

    private OpenHinge openHinge;

    void Start()
    {
        openHinge = GetComponent<OpenHinge>();
    }

    public void Open()
    {
        openHinge.Open();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((activationLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Open();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((activationLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Open();
        }
    }
}
