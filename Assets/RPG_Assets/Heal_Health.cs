using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal_Health : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerHealth = 100;
        }
    }
}
