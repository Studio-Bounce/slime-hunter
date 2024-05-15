using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageTaker : MonoBehaviour, ITakeDamage
{
    public int health = 100;

    public void Death()
    {
       //Destroy(gameObject);
    }

    public void TakeDamage(Damage damage)
    {
        health -= damage.value;
        transform.position = transform.position + damage.direction*damage.knockback;

        if (health <= 0)
        {
            Death();
        }
    }
}
