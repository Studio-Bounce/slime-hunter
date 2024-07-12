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

public enum EquipState
{
    None,
    One,
    Two
}

[System.Serializable]
public class Item
{
    public ItemSO itemRef;
    public int quantity = 1;

    //[HideInInspector]
    public EquipState equipState = EquipState.None;

    public IEnumerator ReadAsync(BinaryReader br)
    {
        string path = br.ReadString();
        var handle = Addressables.LoadAssetAsync<ItemSO>(path);
        yield return handle;
        itemRef = handle.Result;
        quantity = br.ReadInt32();
        equipState = (EquipState)br.ReadInt32();
    }

    public void Write(BinaryWriter bw)
    {
        string path = AssetDatabase.GetAssetPath(itemRef);
        bw.Write(path);
        bw.Write(quantity);
        bw.Write((int)equipState);
    }
}