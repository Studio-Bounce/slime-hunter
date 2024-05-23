using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Damage Dealer uses OnTriggerEnter to detect & deal damage.
// OnTriggerEnter needs rigidbody to trigger.
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DamageDealer : MonoBehaviour
{
    public LayerMask hitLayers;
    public Damage damage;

    // Active damage dealer deals damage. Inactive does not.
    public bool active = true;

    private void Start()
    {
        // Ensure the rigid body is kinematic
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if ((hitLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            damage.direction = (other.transform.position - transform.position).normalized;
            ITakeDamage damageReceiver = other.gameObject.GetComponent<ITakeDamage>();
            damageReceiver?.TakeDamage(damage);
        }
    }

}
