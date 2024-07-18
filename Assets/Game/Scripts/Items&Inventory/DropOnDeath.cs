using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(DamageTaker))]
public class DropOnDeath : MonoBehaviour
{
    public List<ItemSO> itemsToDrop;
    public GameObject droppedItemPrefab;

    private DamageTaker damageTaker;

    void Start()
    {
        damageTaker = GetComponent<DamageTaker>();
        damageTaker.onDeathEvent.AddListener(OnDeath);
    }

    private void OnDeath()
    {
        foreach (var item in itemsToDrop)
        {
            GameObject go = Instantiate(droppedItemPrefab, gameObject.scene) as GameObject;
            DroppedItem droppedItem = go.GetComponent<DroppedItem>();
            droppedItem.transform.position = transform.position;
            droppedItem.ItemRef = item;
        }
    }
}