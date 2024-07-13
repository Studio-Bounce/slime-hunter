using UnityEngine.AddressableAssets;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System;

public enum ItemType
{
    Weapon,
    Spell,
    Material
}

[System.Serializable]
public class ItemSO : ScriptableObject
{
    [Header("REQUIRED: ADDRESSABLE ADDRESS")]
    public string address;

    [Header("Item Properties")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public ItemType itemType;
    public int value;
    public int weight;
}

[System.Serializable]
public class Item
{
    public ItemSO itemRef;
    public int quantity = 1;

    public IEnumerator ReadAsync(BinaryReader br)
    {
        string address = br.ReadString();
        var handle = Addressables.LoadAssetAsync<ItemSO>(address);
        yield return handle;
        itemRef = handle.Result;
        quantity = br.ReadInt32();
    }

    public void Write(BinaryWriter bw)
    {
        bw.Write(itemRef.address);
        bw.Write(quantity);
    }
}