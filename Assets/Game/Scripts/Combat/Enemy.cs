using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour, ITakeDamage
{
    public int health = 100;

    public void TakeDamage(int value)
    {
        health -= value;
        if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
