using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Equipped
{
    public Item[] weapons = new Item[2];
    public Item[] spells = new Item[2];

    public IEnumerator ReadAsync(BinaryReader br)
    {
        int length = br.ReadInt32();
        for (int i = 0; i < length; i++)
        {
            Item weapon = new Item();
            weapon.ReadAsync(br);
            yield return weapon;
            weapons[i] = weapon;
        }
        length = br.ReadInt32();
        for (int i = 0; i < length; i++)
        {
            Item spell = new Item();
            spell.ReadAsync(br);
            yield return spell;
            spells[i] = spell;
        }
    }

    public void Write(BinaryWriter bw)
    {
        bw.Write(weapons.Length);
        foreach (var weapon in weapons) weapon.Write(bw);

        bw.Write(spells.Length);
        foreach (var spell in spells) spell.Write(bw);
    }
}

public class InventoryManager : PersistentSingleton<InventoryManager>
{
    // Saving
    private bool hasChanged = false;

    // Inventory
    public List<Item> items = new List<Item>();
    public int maxWeight = 30;
    int selectedIndex = 0;

    // Equipped
    public Item[] equippedWeapons = new Item[2];
    public Item[] equippedSpells = new Item[2];

    // UI
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
        equip1Btn.RegisterCallback<ClickEvent>(e => EquipItem(true));
        equip2Btn.RegisterCallback<ClickEvent>(e => EquipItem(false));
        dropBtn.RegisterCallback<ClickEvent>(e => DropItem());
    }

    private void ClearInfoPanel()
    {
        infoContainer.AddToClassList(hideContentClass);
    }

    private void _UpdateEquippedToControllers()
    {
        WeaponController weaponController = GameManager.Instance.PlayerRef?.GetComponent<WeaponController>();
        SpellController spellController = GameManager.Instance.PlayerRef?.GetComponent<SpellController>();

        if (weaponController == null || spellController == null)
        {
            Debug.Log("Can't find player controllers");
            return;
        }
        weaponController.availableWeapons[0] = equippedWeapons[0]?.itemRef as WeaponSO;
        weaponController.availableWeapons[1] = equippedWeapons[1]?.itemRef as WeaponSO;
        spellController.availableSpells[0] = equippedSpells[0]?.itemRef as SpellSO;
        spellController.availableSpells[1] = equippedSpells[1]?.itemRef as SpellSO;

        weaponController.InstantiateWeapon(weaponController.CurrentWeapon);
        spellController.LoadSpellIcons();
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
        weapon1Icon.style.backgroundImage = equippedWeapons[0]?.itemRef?.icon.texture;
        weapon2Icon.style.backgroundImage = equippedWeapons[1]?.itemRef?.icon.texture;
        spell1Icon.style.backgroundImage = equippedSpells[0]?.itemRef?.icon.texture;
        spell2Icon.style.backgroundImage = equippedSpells[1]?.itemRef?.icon.texture;
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
    public void EquipItem(bool isSlotOne)
    {
        Item item = items[selectedIndex];
        int equippedIndex = isSlotOne ? 0 : 1;
        EquipState equipState = isSlotOne ? EquipState.One : EquipState.Two;
        Item[] equippedSlots;

        // Assign variables depending on equipped type
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

        // Unequip if slot taken
        if (equippedSlots[equippedIndex] != null)
        {
            equippedSlots[equippedIndex].equipState = EquipState.None;
        }

        // Handle equipping same item in multiple slots
        switch (item.equipState)
        {
            case EquipState.One:
                equippedSlots[0] = null;
                break;
            case EquipState.Two:
                equippedSlots[1] = null;
                break;
        }

        // Equip Item
        equippedSlots[equippedIndex] = item;
        item.equipState = equipState;

        // Update UI
        _UpdateEquippedToControllers();
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

            // Load saved inventory items
            int itemCount = reader.ReadInt32();
            for (int i = 0; i < itemCount; i++)
            {
                Item item = new Item();
                yield return StartCoroutine(item.ReadAsync(reader));
                items.Add(item);
                _EquipLoadedItem(item);
            }

            // Load equipped items
        }
        UpdateInventoryUI();
        _UpdateEquippedToControllers();
    }

    private void _EquipLoadedItem(Item item)
    {
        // Get slot index or exit if not equipped
        int index;
        switch (item.equipState)
        {
            case EquipState.One:
                index = 0;
                break;
            case EquipState.Two:
                index = 1;
                break;
            default:
                return;
        }

        // Add item to equipped
        switch (item.itemRef.itemType)
        {
            case ItemType.Weapon:
                equippedWeapons[index] = item;
                break;
            case ItemType.Spell:
                equippedSpells[index] = item;
                break;
        }
    }

    #endregion
}
