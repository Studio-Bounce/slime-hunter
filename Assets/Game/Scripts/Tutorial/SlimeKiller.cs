using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeKiller : MonoBehaviour
{
    public int numToKill = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == GameConstants.EnemyLayer)
        {
            --numToKill;
            Destroy(other.gameObject);
        }
    }

    private void Update()
    {
        if (numToKill <= 0)
        {
            Destroy(gameObject);
        }
    }
}
