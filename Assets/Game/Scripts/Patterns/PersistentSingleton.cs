using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentSingleton<T> : PersistentObject where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name, typeof(T));
                    instance = go.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    public static bool IsValidSingleton()
    {
        return (instance != null);
    }

    public static void DestroySingleton()
    {
        Destroy(instance.gameObject);
        instance = null;
    }

    public override void LoadSaveData(byte[] data)
    {
        throw new System.NotImplementedException();
    }
    public override byte[] GetSaveData()
    {
        throw new System.NotImplementedException();
    }
}