using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    public struct SaleItem
    {
        public ItemSO item;
        public List<ItemCost> costs;
    }

    public List<SaleItem> itemsForSale;

    [Header("UI References")]
    public VisualTreeAsset listTreeAsset;
    private ScrollView shopScrollView;
    private Label slimeGelCount;

    private void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        shopScrollView = root.Q<ScrollView>("ShopItemScrollView");
        slimeGelCount = root.Q<Label>("SlimeGelCount");
    }

    public override void Show()
    {
        base.Show();
        slimeGelCount.text = InventoryManager.Instance.TotalSlimeGel.ToString();
        PopulateList();
        InputManager.Instance.exitEvent += Hide;
    }

    public override void Hide()
    {
        base.Hide();
        InputManager.Instance.exitEvent -= Hide;
    }

    public override void ToggleVisible()
    {
        base.ToggleVisible();
        PopulateList();
    }

    private void PopulateList()
    {
        shopScrollView.Clear();

        foreach (var saleItem in itemsForSale)
        {
            // Create list visual element
            VisualElement listElement = listTreeAsset.CloneTree();
            VisualElement itemImage = listElement.Q<VisualElement>(name: "ItemIcon");
            Label itemName = listElement.Q<Label>(name: "ItemName");
            Label itemDescription = listElement.Q<Label>(name: "ItemDescription");
            Label itemType = listElement.Q<Label>(name: "ItemType");
            VisualElement itemRequirements = listElement.Q<VisualElement>(name: "ItemRequirements");
            Button purchaseBtn = listElement.Q<Button>("PurchaseBtn");

            itemImage.style.backgroundImage = saleItem.item.icon.texture;
            itemName.text = saleItem.item.name;
            itemDescription.text = saleItem.item.description;
            itemType.text = saleItem.item.itemType.ToString();

            purchaseBtn.clicked += () =>
            {
                if (TryPurchase(saleItem))
                {
                    listElement.RemoveFromHierarchy();
                }
            };

            // Create elements for item costs
            foreach (var itemCost in saleItem.costs)
            {
                Label costLabel = new Label();
                costLabel.text = $"{itemCost.item.itemName}: {itemCost.quantity}";
                itemRequirements.Add(costLabel);
            }

            shopScrollView.Add(listElement);
        }
    }

    private bool TryPurchase(SaleItem saleItem)
    {
        // Exit if player can't afford
        foreach (ItemCost cost in saleItem.costs)
        {
            if (InventoryManager.Instance.GetItemQuantity(cost.item) < cost.quantity)
            {
                return false;
            }
        }

        foreach (ItemCost cost in saleItem.costs)
        {
            InventoryManager.Instance.RemoveItem(cost.item, cost.quantity);
        }
        InventoryManager.Instance.AddItem(saleItem.item);
        itemsForSale.Remove(saleItem);

        return true;
    }
}
