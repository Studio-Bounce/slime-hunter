using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistentObject : MonoBehaviour, IPersistent
{
    // Serialized to persist through Unity's serialization
    [SerializeField] string uuid;

    protected virtual void Awake()
    {
        if (string.IsNullOrEmpty(uuid))
        {
            uuid = Guid.NewGuid().ToString();
            Debug.LogWarning($"UUID not found! Fix it. This causes invalid save files to be created. Using UUID: {uuid}");
        }
        PersistenceManager.Instance.RegisterPersistent(this);
    }

    protected virtual void OnDestroy()
    {
        PersistenceManager.Instance.UnregisterPersistent(this);
    }

    public string GetUUID()
    {
        return uuid;
    }

    public abstract byte[] GetSaveData();
    public abstract void LoadSaveData(byte[] data);
}
