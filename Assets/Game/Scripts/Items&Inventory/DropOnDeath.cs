using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(DamageTaker))]
public class DropOnDeath : ItemSpawner
{
    private DamageTaker damageTaker;

    void Start()
    {
        damageTaker = GetComponent<DamageTaker>();
        damageTaker.onDeathEvent.AddListener(OnDeath);
    }

    private void OnDeath()
    {
        SpawnItems();
    }
}