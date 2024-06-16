using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCheckpoint : PersistentObject
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

    // ------------------- Save / Load -------------------
    public override void LoadSaveData(byte[] data)
    {
        using (var stream = new MemoryStream(data))
        {
            using (var reader = new BinaryReader(stream))
            {
                isCheckpointCleared = reader.ReadBoolean();
            }
        }
    }

    public override byte[] GetSaveData()
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(isCheckpointCleared);
            }
            return stream.ToArray();
        }
    }
}
