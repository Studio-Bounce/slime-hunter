using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistenceTest : PersistentObject
{
    public int playerData = 10;
    public Vector3 playerPosition;
    public bool saveData = false;

    private void Start()
    {
        PersistenceManager.Instance.LoadGame();
    }

    private void Update()
    {
        if (saveData)
        {
            PersistenceManager.Instance.SaveGame();
            saveData = false;
        }
    }

    public override byte[] GetSaveData()
    {
        using (var stream = new System.IO.MemoryStream())
        {
            using (var writer = new System.IO.BinaryWriter(stream))
            {
                writer.Write(playerData);
                writer.Write(playerPosition.x);
                writer.Write(playerPosition.y);
                writer.Write(playerPosition.z);
            }
            return stream.ToArray();
        }
    }

    public override void LoadSaveData(byte[] data)
    {
        using (var stream = new System.IO.MemoryStream(data))
        {
            using (var reader = new System.IO.BinaryReader(stream))
            {
                playerData = reader.ReadInt32();
                playerPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
        }
    }
}
