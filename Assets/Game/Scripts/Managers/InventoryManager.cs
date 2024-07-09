using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager : PersistentSingleton<InventoryManager>
{
    [SerializeField] private UIDocument uiDocument;
    public List<Item> items = new List<Item>();
    public int maxWeight = 30;

    // UI
    readonly string inventoryContainer = "InventoryContent";
    readonly string inventorySlot = "InventorySlot"; 
    readonly string quantityLabel = "QuantityLabel";

    public void UpdateInventoryUI()
    {
        VisualElement root = uiDocument.rootVisualElement;
        VisualElement inventoryEl = root.Q<VisualElement>(inventoryContainer);
        List<VisualElement> slotElements = inventoryEl.Query<VisualElement>(name: inventorySlot).ToList();

        for (int i = 0; i < slotElements.Count; i++)
        {
            VisualElement slot = slotElements[i];
            Label quantityEl = slot.Q<Label>(quantityLabel);
            if (i < items.Count) {
                Item item = items[i];
                slot.style.backgroundImage = item.itemRef?.icon.texture;
                quantityEl.text = item.quantity.ToString();
            } 
            else
            {
                slot.style.backgroundImage = null;
                quantityEl.text = string.Empty;
            }

        }
    }

    public int GetTotalWeight()
    {
        int totalValue = 0;
        foreach (Item item in items)
        {
            totalValue += item.itemRef.weight * item.quantity;
        }
        return totalValue;
    }

    public bool AddItem(Item item)
    {
        if (GetTotalWeight() > maxWeight) // TODO: Inefficient
        {
            items.Add(item);
            return true;
        }
        return false;
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }

    public override byte[] GetSaveData()
    {
        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(maxWeight);

            // Write items
            writer.Write(items.Count);
            foreach (var item in items) item.Write(writer);

            return stream.ToArray();
        }
    }

    public override void LoadSaveData(byte[] data)
    {
        StartCoroutine(_LoadSaveDataAsync(data));
    }

    private IEnumerator _LoadSaveDataAsync(byte[] data)
    {
        using (var stream = new MemoryStream(data)) 
        using (var reader = new BinaryReader(stream))
        {
            maxWeight = reader.ReadInt32();

            // Read and add items
            int itemCount = reader.ReadInt32();
            for (int i = 0; i < itemCount; i++)
            {
                Item item = new Item();
                yield return StartCoroutine(item.ReadAsync(reader));
                items.Add(item);
            }
        }
    }
}
