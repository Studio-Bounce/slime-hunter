using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public class InventoryManager : PersistentSingleton<InventoryManager>
{
    // Saving
    private bool hasChanged = false;

    // Inventory
    public List<Item> items = new List<Item>();
    public int maxWeight = 30;
    int selectedIndex = 0;

    // Equipped
    private WeaponSO[] equippedWeapons = new WeaponSO[2];
    private SpellSO[] equippedSpells = new SpellSO[2];

    // UI Refs
    [SerializeField] private UIDocument uiDocument;
    VisualElement root;
    List<VisualElement> slotElements;

    // IDs
    private const string inventorySlot = "InventorySlot";
    private const string quantityLabel = "QuantityLabel";

    // Classes
    private const string slotSelectedClass = "slot-selected";
    private const string hideContentClass = "hide-content";

    // Inventory Container
    VisualElement inventoryContainer;

    // Info Container
    VisualElement infoContainer;
    Label infoName;
    Label infoDescription;
    VisualElement infoIcon;
    Label infoType;
    Button equip1Btn;
    Button equip2Btn;
    Button dropBtn;

    // Character Container
    VisualElement characterContainer;
    VisualElement weapon1Icon;
    VisualElement weapon2Icon;
    VisualElement spell1Icon;
    VisualElement spell2Icon;

    // Events
    public event Action OnInventoryChanged = delegate { };
    public event Action<WeaponSO[]> OnEquippedWeaponsChanged = delegate { };
    public event Action<SpellSO[]> OnEquippedSpellsChanged = delegate { };

    private void Start()
    {
        InitializeUIElements();
        InitializeEventHandlers();
        ClearInfoPanel();
    }

    private void InitializeUIElements()
    {
        root = uiDocument.rootVisualElement;
        inventoryContainer = root.Q<VisualElement>("InventoryContainer");
        slotElements = inventoryContainer.Query<VisualElement>(name: inventorySlot).ToList();
        infoContainer = root.Q<VisualElement>("ItemInfoContainer");
        infoName = infoContainer.Q<Label>("ItemName");
        infoDescription = infoContainer.Q<Label>("ItemDescription");
        infoIcon = infoContainer.Q<VisualElement>("ItemIcon");
        infoType = infoContainer.Q<Label>("ItemType");
        equip1Btn = infoContainer.Q<Button>("Equip1Btn");
        equip2Btn = infoContainer.Q<Button>("Equip2Btn");
        dropBtn = infoContainer.Q<Button>("DropBtn");
        characterContainer = root.Q<VisualElement>("CharacterContainer");
        weapon1Icon = characterContainer.Q<VisualElement>("Weapon1Icon");
        weapon2Icon = characterContainer.Q<VisualElement>("Weapon2Icon");
        spell1Icon = characterContainer.Q<VisualElement>("Spell1Icon");
        spell2Icon = characterContainer.Q<VisualElement>("Spell2Icon");
    }

    private void InitializeEventHandlers()
    {
        for (int i = 0; i < slotElements.Count; i++)
        {
            int index = i;
            var slot = slotElements[index];
            slot.RegisterCallback<ClickEvent>(e => _UpdateSelectedItemInfo(index));
        }
        equip1Btn.RegisterCallback<ClickEvent>(e => EquipItemToSlot(true));
        equip2Btn.RegisterCallback<ClickEvent>(e => EquipItemToSlot(false));
        dropBtn.RegisterCallback<ClickEvent>(e => DropItem());
    }

    private void ClearInfoPanel()
    {
        infoContainer.AddToClassList(hideContentClass);
    }

    public void UpdateInventoryUI()
    {
        _UpdateSlotsUI();
        _UpdateEquippedUI();
    }

    private void _UpdateSlotsUI()
    {
        for (int i = 0; i < slotElements.Count; i++)
        {
            VisualElement slot = slotElements[i];
            Label quantityEl = slot.Q<Label>(quantityLabel);
            if (i < items.Count)
            {
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

    private void _UpdateEquippedUI()
    {
        weapon1Icon.style.backgroundImage = equippedWeapons[0]?.icon.texture;
        weapon2Icon.style.backgroundImage = equippedWeapons[1]?.icon.texture;
        spell1Icon.style.backgroundImage = equippedSpells[0]?.icon.texture;
        spell2Icon.style.backgroundImage = equippedSpells[1]?.icon.texture;
    }

    private void _UpdateSelectedItemInfo(int index)
    {
        if (index >= items.Count)
        {
            Debug.Log($"No item at the selected index {index} >= {items.Count}");
            return;
        }

        // Show Content
        infoContainer.RemoveFromClassList(hideContentClass);

        // Add selected style
        slotElements[selectedIndex].RemoveFromClassList(slotSelectedClass);
        slotElements[index].AddToClassList(slotSelectedClass);
        selectedIndex = index;

        Item item = items[index];
        infoIcon.style.backgroundImage = item.itemRef.icon.texture;
        infoName.text = item.itemRef.itemName;
        infoDescription.text = item.itemRef.description;
        infoType.text = item.itemRef.itemType.ToString();

        // Show cooresponding buttons based on item type
        switch (item.itemRef.itemType)
        {
            case ItemType.Weapon:
                equip1Btn.style.display = DisplayStyle.Flex;
                equip2Btn.style.display = DisplayStyle.Flex;
                dropBtn.style.display = DisplayStyle.None;
                break;
            case ItemType.Spell:
                equip1Btn.style.display = DisplayStyle.Flex;
                equip2Btn.style.display = DisplayStyle.Flex;
                dropBtn.style.display = DisplayStyle.None;
                break;
            case ItemType.Material:
                equip1Btn.style.display = DisplayStyle.None;
                equip2Btn.style.display = DisplayStyle.None;
                dropBtn.style.display = DisplayStyle.Flex;
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
        UpdateInventoryUI();
    }

    // Equip item into indicated slot
    public void EquipItemToSlot(bool isSlotOne)
    {
        Item item = items[selectedIndex];
        int equippedIndex = isSlotOne ? 0 : 1;
        ItemSO[] equippedSlots;

        // Choose slot pair based on item type
        switch (item.itemRef.itemType)
        {
            case ItemType.Weapon:
                equippedSlots = equippedWeapons;
                break;
            case ItemType.Spell:
                equippedSlots = equippedSpells;
                break;
            default: return;
        }

        // Handle equipping same item in multiple slots
        int otherIndex = (equippedIndex == 0) ? 1 : 0;
        if (item.itemRef == equippedSlots[otherIndex])
        {
            equippedSlots[otherIndex] = null;
        }

        // Equip Item
        equippedSlots[equippedIndex] = item.itemRef;

        // Update UI
        switch (item.itemRef.itemType)
        {
            case ItemType.Weapon:
                OnEquippedWeaponsChanged(equippedWeapons);
                break;
            case ItemType.Spell:
                OnEquippedSpellsChanged(equippedSpells);
                break;
        }
        _UpdateEquippedUI();
    }

    public void DropItem()
    {
        if (selectedIndex >= items.Count)
        {
            Debug.Log("No item to drop at this index");
            return;
        }

        Item item = items[selectedIndex];
        item.quantity -= 1;

        // Update slot UI
        VisualElement slot = slotElements[selectedIndex];
        Label quantityEl = slot.Q<Label>(quantityLabel);
        quantityEl.text = item.quantity.ToString();

        if (item.quantity < 1)
        {
            items.RemoveAt(selectedIndex);
            UpdateInventoryUI();
        }
    }

    #region Saving/Loading

    public override byte[] GetSaveData()
    {
        hasChanged = false;
        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(maxWeight);
            // Write inventory items
            writer.Write(items.Count);
            foreach (var item in items) item.Write(writer);
            // Write equipped items
            _WriteEquipped(writer);

            return stream.ToArray();
        }
    }

    private void _WriteEquipped(BinaryWriter bw)
    {
        for (int i = 0; i < 2; i++)
        {
            // Weapons
            bw.Write(equippedWeapons[i] != null);
            if (equippedWeapons[i] != null)
            {
                bw.Write(equippedWeapons[i].address);
            }
            // Spells
            bw.Write(equippedSpells[i] != null);
            if (equippedSpells[i] != null)
            {
                bw.Write(equippedSpells[i].address);
            }
        }
    }

    public override void LoadSaveData(byte[] data)
    {
        items.Clear();
        StartCoroutine(_LoadSaveDataAsync(data));
    }

    // Async necessary to load addressables
    private IEnumerator _LoadSaveDataAsync(byte[] data)
    {
        using (var stream = new MemoryStream(data)) 
        using (var reader = new BinaryReader(stream))
        {
            maxWeight = reader.ReadInt32();

            // Load inventory items
            int itemCount = reader.ReadInt32();
            for (int i = 0; i < itemCount; i++)
            {
                Item item = new Item();
                yield return StartCoroutine(item.ReadAsync(reader));
                items.Add(item);
            }

            // Load equipped items
            yield return StartCoroutine(_LoadEquippedAsync(reader));
        }

        OnInventoryChanged.Invoke();
        OnEquippedWeaponsChanged(equippedWeapons);
        OnEquippedSpellsChanged(equippedSpells);

        UpdateInventoryUI();
    }

    private IEnumerator _LoadEquippedAsync(BinaryReader br)
    {
        for (int i = 0; i < 2; i++)
        {
            // Weapons
            if (br.ReadBoolean())
            {
                string address = br.ReadString();
                var handle = Addressables.LoadAssetAsync<WeaponSO>(address);
                yield return handle;
                equippedWeapons[i] = handle.Result;
            }
            // Spells
            if (br.ReadBoolean())
            {
                string address = br.ReadString();
                var handle = Addressables.LoadAssetAsync<SpellSO>(address);
                yield return handle;
                equippedSpells[i] = handle.Result;
            }
        }
    }

    #endregion
}