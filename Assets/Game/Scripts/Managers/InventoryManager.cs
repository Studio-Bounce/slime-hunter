using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager : PersistentSingleton<InventoryManager>
{
    [SerializeField] private UIDocument uiDocument;
    public List<Item> items = new List<Item>();
    public int maxWeight = 30;

    // UI
    VisualElement root;
    List<VisualElement> slotElements;

    readonly string inventoryContainer = "InventoryContent";
    readonly string inventorySlot = "InventorySlot"; 
    readonly string quantityLabel = "QuantityLabel";

    readonly string itemInfoContainer = "ItemInfoContainer";
    readonly string itemIcon = "ItemIcon";
    readonly string itemName = "ItemName";
    readonly string itemDescription = "ItemDescription";

    private void Start()
    {
        root = uiDocument.rootVisualElement;
        VisualElement container = root.Q<VisualElement>(inventoryContainer);
        slotElements = container.Query<VisualElement>(name: inventorySlot).ToList();

        for (int i = 0; i < slotElements.Count; i++)
        {
            int index = i;
            var slot = slotElements[index];
            slot.RegisterCallback<ClickEvent>(e => UpdateSelectedItemInfo(index));
        }
    }

    public void UpdateInventoryUI()
    {
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

    public void UpdateSelectedItemInfo(int index)
    {
        if (index >= items.Count)
        {
            Debug.Log($"No item at the selected index {index} >= {items.Count}");
            return;
        }

        VisualElement container = root.Q<VisualElement>(itemInfoContainer);
        VisualElement icon = container.Q<VisualElement>(itemIcon);
        Label name = container.Q<Label>(itemName);
        Label description = container.Q<Label>(itemDescription);

        Item item = items[index];
        icon.style.backgroundImage = item.itemRef.icon.texture;
        name.text = item.itemRef.itemName;
        description.text = item.itemRef.description;
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
