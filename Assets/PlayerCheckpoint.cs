using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCheckpoint : MonoBehaviour
{
    [SerializeField] PlayerSpawner spawner;
    public bool isCheckpointCleared = false;

    private void Start()
    {
        if (spawner == null)
        {
            spawner = FindObjectOfType<PlayerSpawner>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isCheckpointCleared && other.gameObject.layer == GameConstants.PlayerLayer)
        {
            isCheckpointCleared = true;
            spawner.ModifyRecentCheckpoint(this);
        }
    }
}
