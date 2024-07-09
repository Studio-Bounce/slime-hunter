using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public Inventory inventory;

    // UI
    readonly string inventoryContainerClass = "grid-inventory-container";

    void Start()
    {
        inventory = new Inventory();
        inventory.maxWeight = 30;
    }

    public void UpdateInventoryUI()
    {
    }

    public int GetTotalWeight()
    {
        int totalValue = 0;
        foreach (Item item in inventory.items)
        {
            totalValue += item.weight * item.quantity;
        }
        return totalValue;
    }

    public bool AddItem(Item item)
    {
        if (GetTotalWeight() > inventory.maxWeight) // TODO: Inefficient
        {
            inventory.items.Add(item);
            return true;
        }
        return false;
    }

    public void RemoveItem(Item item)
    {
        inventory.items.Remove(item);
    }

    public void UseItem(Item item)
    {

    }
}
