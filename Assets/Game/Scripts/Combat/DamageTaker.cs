using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DamageTaker : MonoBehaviour, ITakeDamage
{
    public int health = 100;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Death()
    {
       //Destroy(gameObject);
    }

    public void TakeDamage(Damage damage)
    {
        health -= damage.value;
        StartCoroutine(ApplyKnockback(damage));

        if (health <= 0)
        {
            Death();
        }
    }

    private IEnumerator ApplyKnockback(Damage damage)
    {
        Vector3 knockbackVec = damage.direction * damage.knockback;
        float verticalKnockback = Mathf.Log(knockbackVec.magnitude + 1); // Manual height calculation
        Vector3 isometricKnockback = new Vector3(knockbackVec.x, verticalKnockback, knockbackVec.z);
        rb.AddForce(isometricKnockback, ForceMode.Impulse);
        yield return null;
    }
}
