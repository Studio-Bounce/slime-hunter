using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistenceManager : Singleton<PersistenceManager>
{
    List<PersistentObject> persistentGameObjects;
    const string SAVE_FOLDER = "slime_hunter_save";

    private void Awake()
    {
        persistentGameObjects = new List<PersistentObject>();
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, SAVE_FOLDER)))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, SAVE_FOLDER));
        }
    }

    // ---------------------- Save / Load ----------------------
    public void SaveGame()
    {
        foreach (PersistentObject obj in persistentGameObjects)
        {
            byte[] data = obj.GetSaveData();
            string path = Path.Combine(Application.persistentDataPath, SAVE_FOLDER, $"{obj.GetUUID()}.dat");
            File.WriteAllBytes(path, data);
        }
    }

    public void LoadGame()
    {
        foreach (PersistentObject obj in persistentGameObjects)
        {
            string path = Path.Combine(Application.persistentDataPath, SAVE_FOLDER, $"{obj.GetUUID()}.dat");
            if (File.Exists(path))
            {
                byte[] data = File.ReadAllBytes(path);
                obj.LoadSaveData(data);
            }
        }
    }

    // ---------------------- Track / UnTrack ----------------------
    public void RegisterPersistent(PersistentObject persistentObject)
    {
        if (!persistentGameObjects.Contains(persistentObject))
        {
            persistentGameObjects.Add(persistentObject);
        }
    }

    public void UnregisterPersistent(PersistentObject persistentObject)
    {
        if (!persistentGameObjects.Contains(persistentObject))
        {
            persistentGameObjects.Remove(persistentObject);
        }
    }
}
