using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(OpenHinge))]
public class ItemChest : MonoBehaviour
{
    public List<ItemSO> itemContents;
    public LayerMask activationLayers;
    public GameObject droppedItemPrefab;

    private OpenHinge openHinge;

    void Start()
    {
        openHinge = GetComponent<OpenHinge>();
    }

    public void Open()
    {
        openHinge.Open();
        foreach (var item in itemContents)
        {
            GameObject go = Instantiate(droppedItemPrefab, gameObject.scene) as GameObject;
            DroppedItem droppedItem = go.GetComponent<DroppedItem>();
            droppedItem.transform.position = transform.position;
            droppedItem.ItemRef = item;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((activationLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Open();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((activationLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Open();
        }
    }

}
