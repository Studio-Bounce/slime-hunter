using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageDealer : MonoBehaviour
{
    public LayerMask hitLayers;
    public int damage;
    private bool active = true;

    private void Start()
    {
        Debug.Log("Weapon Start");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter");
        if ((hitLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Debug.Log("Hit Enemy");
            ITakeDamage damageReceiver = other.gameObject.GetComponent<ITakeDamage>();
            damageReceiver?.TakeDamage(damage);
        }
    }

}
