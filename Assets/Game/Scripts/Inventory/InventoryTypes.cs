using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
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
            foreach (var item in items)
            {
                item.Write(writer);
            }
            return stream.ToArray();
        }
    }

    public override void LoadSaveData(byte[] data)
    {
        throw new System.NotImplementedException();
    }
}
