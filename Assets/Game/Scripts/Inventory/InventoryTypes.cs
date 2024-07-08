using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Item
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

    public void Write(BinaryWriter bw)
    {
        bw.Write(itemName);
        bw.Write(AssetDatabase.GetAssetPath(icon));
        bw.Write(AssetDatabase.GetAssetPath(itemRef));
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

                // Read the sprite path
                string path = reader.ReadString();
                if (!string.IsNullOrEmpty(path))
                {
                    item.icon = Resources.Load<Sprite>(path);
                }

                // Read the itemRef path
                path = reader.ReadString();
                if (!string.IsNullOrEmpty(path))
                {
                    item.itemRef = Resources.Load<ScriptableObject>(path);
                }

                item.itemName = reader.ReadString();
                item.itemType = (Item.ItemType)reader.ReadInt32();
                item.quantity = reader.ReadInt32();
                item.value = reader.ReadInt32();
                item.weight = reader.ReadInt32();
                items.Add(item);
            }
        }
    }
}
