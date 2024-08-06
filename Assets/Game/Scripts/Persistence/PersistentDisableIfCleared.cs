using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistentDisableIfCleared : PersistentObject
{
    private bool cleared;

    public void MarkCleared()
    {
        cleared = true;
    }

    public void UnmarkCleared()
    {
        cleared = false;
    }

    public override byte[] GetSaveData()
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(cleared);
            }
            return stream.ToArray();
        }
    }

    public override void LoadSaveData(byte[] data)
    {
        using (var stream = new MemoryStream(data))
        {
            using (var reader = new BinaryReader(stream))
            {
                cleared = reader.ReadBoolean();
            }
        }

        if (cleared)
        {
            gameObject.SetActive(false);
        }
    }
}
