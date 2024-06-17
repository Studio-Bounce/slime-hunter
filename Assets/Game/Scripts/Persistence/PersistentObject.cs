using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistentObject : MonoBehaviour, IPersistent
{
    // Serialized to persist through Unity's serialization
    [SerializeField] string uuid;

    // Cache the current instance of PersistentManager instead of getting it every time
    // This ensures that on destruction of the scene, PersistentManager's instance would
    // not get created again
    PersistenceManager pmInstance = null;

    protected virtual void Awake()
    {
        if (string.IsNullOrEmpty(uuid))
        {
            uuid = Guid.NewGuid().ToString();
            Debug.LogWarning($"UUID not found! Fix it. This causes invalid save files to be created. Using UUID: {uuid}");
        }
        pmInstance = PersistenceManager.Instance;
        pmInstance.RegisterPersistent(this);
    }

    protected virtual void OnDestroy()
    {
        pmInstance.UnregisterPersistent(this);
    }

    public string GetUUID()
    {
        return uuid;
    }

    public abstract byte[] GetSaveData();
    public abstract void LoadSaveData(byte[] data);
}
