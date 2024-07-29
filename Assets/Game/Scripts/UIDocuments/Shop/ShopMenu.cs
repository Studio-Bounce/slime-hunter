using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopMenu : Menu
{
    [Serializable]
    public struct ItemCost
    {
        public ItemSO item;
        public int quantity;
    }

    [Serializable]
    public struct ItemForSale
    {
        public ItemSO item;
        public List<ItemCost> costs;
    }

    public List<ItemForSale> inventory;

    [Header("UI References")]
    public VisualTreeAsset listTreeAsset;
    private VisualElement shopContainer;

    private void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        shopContainer = root.Q<VisualElement>("ShopItemContainer");
        Button purchaseBtn = shopContainer.Q<Button>("PurchaseBtn");
    }

    public override void Show()
    {
        base.Show();
        PopulateList();
    }

    private void PopulateList()
    {
        foreach (var saleItem in inventory)
        {
            // Create list visual element
            VisualElement listElement = listTreeAsset.CloneTree();
            VisualElement itemImage = listElement.Q<VisualElement>(name: "ItemIcon");
            Label itemName = listElement.Q<Label>(name: "ItemName");
            Label itemDescription = listElement.Q<Label>(name: "itemDescription");
            Label itemType = listElement.Q<Label>(name: "ItemType");
            VisualElement itemRequirements = listElement.Q<VisualElement>(name: "ItemRequirements");

            itemImage.style.backgroundImage = saleItem.item.icon.texture;
            itemName.text = saleItem.item.name;
            itemDescription.text = saleItem.item.description;
            itemType.text = saleItem.item.itemType.ToString();

            // Create elements for item costs
            foreach (var itemCost in saleItem.costs)
            {
                Label costLabel = new Label();
                costLabel.text = $"{itemCost.item.itemName}: {itemCost.quantity}";
                itemRequirements.Add(costLabel);
            }
        }
    }
}
