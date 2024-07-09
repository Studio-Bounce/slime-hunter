using Ink.Parsed;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ScriptableObject itemRef;
    public ItemType itemType;
    public int quantity;
    public int value;
    public int weight;

    public enum ItemType
    {
        Weapon,
        Armor,
        Spell,
        Material,
        Other
    }

    public void Read(BinaryReader br)
    {
        // Read the sprite path
        string path = br.ReadString();
        if (!string.IsNullOrEmpty(path))
            icon = Resources.Load<Sprite>(path);
        // Read the itemRef path
        path = br.ReadString();
        if (!string.IsNullOrEmpty(path))
            itemRef = Resources.Load<ScriptableObject>(path);

        itemName = br.ReadString();
        itemType = (Item.ItemType)br.ReadInt32();
        quantity = br.ReadInt32();
        value = br.ReadInt32();
        weight = br.ReadInt32();
    }

    public void Write(BinaryWriter bw)
    {
        bw.Write(AssetDatabase.GetAssetPath(icon));
        bw.Write(AssetDatabase.GetAssetPath(itemRef));
        bw.Write(itemName);
        bw.Write((int)itemType);
        bw.Write((int)quantity);
        bw.Write((int)value);
        bw.Write((int)weight);
    }
}

[System.Serializable]
public class Inventory : PersistentObject
{
    public List<Item> items = new List<Item>();
    public int maxWeight = 30;

    public override byte[] GetSaveData()
    {
        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(items.Count);
            foreach (var item in items)
            {
                item.Write(writer);
            }
            return stream.ToArray();
        }
    }

    public override void LoadSaveData(byte[] data)
    {
        using (var stream = new MemoryStream(data))
        using (var reader = new BinaryReader(stream))
        {
            int itemCount = reader.ReadInt32();
            for (int i = 0; i < itemCount; i++)
            {
                Item item = new Item();
                item.Read(reader);
                items.Add(item);
            }
        }
    }
}
