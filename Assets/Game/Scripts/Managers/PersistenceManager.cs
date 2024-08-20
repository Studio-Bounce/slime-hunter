using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistenceManager : Singleton<PersistenceManager>
{
    [SerializeField] bool enableAutosave = false;
    [Tooltip("Time between each auto-save")]
    [SerializeField] float autoSaveInterval = 5.0f;

    List<PersistentObject> persistentGameObjects = new List<PersistentObject>();
    const string SAVE_FOLDER = "slime_hunter_save";

    private void Awake()
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, SAVE_FOLDER)))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, SAVE_FOLDER));
        }
    }

    private void Start()
    {
        if (enableAutosave)
        {
            // Auto-save every x seconds
            StartCoroutine(AutoSaveSequence());
        }
    }

    IEnumerator AutoSaveSequence()
    {
        while (true)
        {
            // If loading async the AutoSaveSequence might start too quickly
            if (GameManager.Instance.GameState != GameState.GAMEPLAY) yield return null;

            yield return new WaitForSecondsRealtime(autoSaveInterval - 1.0f);
            if (GameManager.Instance.GameState == GameState.GAMEPLAY)
            {
                UIManager.Instance.ShowAutoSave();
                SaveGame();
                yield return new WaitForSecondsRealtime(1.0f); // Ideally this should be the time taken in saving
                UIManager.Instance.HideAutoSave();
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator ShowSavePopup()
    {
        UIManager.Instance.ShowAutoSave();
        yield return new WaitForSecondsRealtime(2.0f);
        UIManager.Instance.HideAutoSave();
    }

    // ---------------------- Save / Load ----------------------
    public void SaveGame()
    {
        StartCoroutine(ShowSavePopup());
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
        if (persistentGameObjects.Contains(persistentObject))
        {
            persistentGameObjects.Remove(persistentObject);
        }
    }
}
