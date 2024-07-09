using UnityEngine.AddressableAssets;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;

public enum ItemType
{
    Weapon,
    Spell,
    Material
}

[System.Serializable]
public class ItemSO : ScriptableObject
{
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
    public int quantity;
    public IEnumerator ReadAsync(BinaryReader br)
    {
        string path = br.ReadString();
        Debug.Log($"Read: {path}");
        var handle = Addressables.LoadAssetAsync<ItemSO>(path);
        yield return handle;
        itemRef = handle.Result;
        quantity = br.ReadInt32();
    }

    public void Write(BinaryWriter bw)
    {
        string path = AssetDatabase.GetAssetPath(itemRef);
        Debug.Log($"Write: {path}");
        bw.Write(path);
        bw.Write((int)quantity);
    }
}