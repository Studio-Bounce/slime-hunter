using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageTaker : MonoBehaviour, ITakeDamage
{
    public void Death()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int value)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
