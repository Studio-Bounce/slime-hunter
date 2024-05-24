using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Enemy : DamageTaker, ITakeDamage
{
    [SerializeField] Material[] slimeMaterials;
    
    float totalHealth = 0.0f;

    protected override void Start()
    {
        base.Start();
        totalHealth = health;
    }

    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
        //Debug.Log("Slime got damaged to " + health);
        //foreach (Material mat in slimeMaterials)
        //{
        //}
    }
}
