using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(DamageTaker))]
public class DropOnDeath : MonoBehaviour
{
    public List<ItemSO> itemsToDrop;
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
            GameObject go = new GameObject(item.name);
            DroppedItem dropped = go.AddComponent<DroppedItem>();
            dropped.ItemRef = item;
            go.transform.position = transform.position;
            Instantiate(go, gameObject.scene);
            
        }
    }
}

[RequireComponent (typeof(SpriteRenderer), typeof(Billboard))]
public class DroppedItem : MonoBehaviour
{
    public ItemSO ItemRef { get; set; }

    private SpriteRenderer spriteRenderer;

    public bool CanBePickedUp { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerID = 6;
        spriteRenderer.sortingOrder = 5;
        spriteRenderer.sprite = ItemRef.icon;
        StartCoroutine(AnimateDrop());
    }

    IEnumerator AnimateDrop()
    {
        // Animation Vars
        float animHeight = 2;
        float animDist = 3;
        float animDuration = 2;

        animHeight = Random.Range(1, animHeight);
        animDist = Random.Range(1, animDist);
        animDuration = Random.Range(1, animDuration);

        // Random Direction
        Vector3 direction = Vector3.zero;
        direction.x = Mathf.Sin(Random.value * 2 * Mathf.PI);
        direction.z = Mathf.Cos(Random.value * 2 * Mathf.PI);
        direction.Normalize();

        Vector3 start = transform.position;
        Vector3 end = transform.position + animDist * direction;

        float timer = 0;
        float eased = 0;
        while (timer < animDuration)
        {
            float normalTime = timer / animDuration;
            // Horizontal
            eased = Easing.EaseOut(normalTime);
            Vector3 newPos = Vector3.Lerp(start, end, eased);
            // Vertical
            eased = Easing.EaseOutBounce(normalTime);
            newPos.y = Mathf.Sin(eased*Mathf.PI)*animHeight;

            transform.position = newPos;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
}
