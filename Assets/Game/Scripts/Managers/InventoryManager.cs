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

    Item selectedItem;

    // UI
    VisualElement root;
    List<VisualElement> slotElements;

    readonly string inventorySlot = "InventorySlot"; 
    readonly string quantityLabel = "QuantityLabel";

    // Inventory Container
    VisualElement inventoryContainer;

    // Info Container
    VisualElement infoContainer;
    Label infoName;
    Label infoDescription;
    VisualElement infoIcon;
    Button equipBtn;
    Button dropBtn;

    private void Start()
    {
        // Inventory Container
        root = uiDocument.rootVisualElement;
        inventoryContainer = root.Q<VisualElement>("InventoryContainer");
        slotElements = inventoryContainer.Query<VisualElement>(name: inventorySlot).ToList();
        // Info Container
        infoContainer = root.Q<VisualElement>("ItemInfoContainer");
        infoName = infoContainer.Q<Label>("ItemName");
        infoDescription = infoContainer.Q<Label>("ItemDescription");
        infoIcon = infoContainer.Q<VisualElement>("ItemIcon");
        equipBtn = infoContainer.Q<Button>("EquipBtn");
        dropBtn = infoContainer.Q<Button>("DropBtn");

        // Add item slot callbacks
        for (int i = 0; i < slotElements.Count; i++)
        {
            int index = i;
            var slot = slotElements[index];
            slot.RegisterCallback<ClickEvent>(e => UpdateSelectedItemInfo(index));
        }

        // Add equip/drop callbacks
        equipBtn.RegisterCallback<ClickEvent>(e => EquipItem(selectedItem));
        dropBtn.RegisterCallback<ClickEvent>(e => DropItem(selectedItem));
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

        Item item = items[index];
        infoIcon.style.backgroundImage = item.itemRef.icon.texture;
        infoName.text = item.itemRef.itemName;
        infoDescription.text = item.itemRef.description;

        // Equip and Stats
        switch (item.itemRef.itemType)
        {
            case ItemType.Weapon:
                equipBtn.SetEnabled(true);
                dropBtn.SetEnabled(false);
                break;
            case ItemType.Spell:
                equipBtn.SetEnabled(true);
                dropBtn.SetEnabled(false);
                break;
            case ItemType.Material:
                equipBtn.SetEnabled(false);
                dropBtn.SetEnabled(true);
                break;
            default:
                break;
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

    public void EquipItem(Item item)
    {
        switch (item.itemRef.itemType)
        {
            case ItemType.Weapon:

                break;
            case ItemType.Spell:
                break;
            case ItemType.Material:
                break;
            default:
                break;
        }
    }

    public void DropItem(Item item)
    {
        item.quantity -= 1;

        // Remove if 0

        switch (item.itemRef.itemType)
        {
            case ItemType.Weapon:
                break;
            case ItemType.Spell:
                break;
            case ItemType.Material:
                break;
            default:
                break;
        }
    }

    #region Saving/Loading

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

    #endregion
}
