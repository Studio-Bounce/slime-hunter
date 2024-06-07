using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DamageDealer needs a trigger collider and rigidbody to be able to call OnTriggerEnter.
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DamageDealer : MonoBehaviour
{
    protected LayerMask hitLayers;
    protected Damage damage;

    // Active damage dealer deals damage. Inactive does not.
    protected bool active = false;
    // attackDetected can be used by child classes to do something on attack
    protected bool attackDetected = false;

    protected virtual void Start()
    {
        // Ensure the rigid body is kinematic
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    protected virtual void Update()
    {
        if (!active)
            attackDetected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if ((hitLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            damage.direction = (other.transform.position - transform.position).normalized;
            ITakeDamage damageReceiver = other.gameObject.GetComponent<ITakeDamage>();
            damageReceiver?.TakeDamage(damage);
            attackDetected = true;
        }
    }
}
