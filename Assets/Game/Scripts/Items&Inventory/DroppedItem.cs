using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

[RequireComponent(typeof(SpriteRenderer), typeof(Billboard))]
public class DroppedItem : MonoBehaviour
{
    public Vector2 size;
    public float launchAngle = 0;
    public float angleRange = 360;

    // Refs
    private SpriteRenderer spriteRenderer;
    private Player player;

    // Getters Setters
    public ItemSO ItemRef { get; set; }
    public bool CanBePickedUp { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.PlayerRef;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = ItemRef.icon;
        ResizeSprite(spriteRenderer, size);
        StartCoroutine(AnimateDrop());
    }

    void ResizeSprite(SpriteRenderer spriteRenderer, Vector2 desiredSize)
    {
        // Calculate the scale needed to achieve the desired size
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        Vector2 scale = new Vector2(desiredSize.x / spriteSize.x, desiredSize.y / spriteSize.y);

        // Apply the scale to the local scale of the GameObject
        transform.localScale = new Vector3(scale.x, scale.y, 1f);
        Vector3 pos = transform.position;
        pos.y += desiredSize.y/2;
        transform.position = pos;
    }

    IEnumerator AnimateDrop()
    {
        // Animation Vars
        float animHeight = 1;
        float animDist = 3;
        float animDuration = 1.5f;
        animDist = Random.Range(1, animDist);

        // Random Direction
        float randomAngle = launchAngle + Random.Range(-angleRange / 2f, angleRange / 2f);
        Vector3 randomDir = transform.forward;
        Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
        randomDir = rotation * randomDir;
        randomDir.Normalize();

        Vector3 start = transform.position;
        Vector3 end = transform.position + animDist * randomDir;
        float startEndHeight = transform.position.y;

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
            newPos.y = startEndHeight + Mathf.Sin(eased * Mathf.PI) * animHeight;

            transform.position = newPos;
            timer += Time.deltaTime;
            yield return null;
        }
        CanBePickedUp = true;
    }

    private void Update()
    {
        if (CanBePickedUp)
        {
            Vector3 target = player.transform.position;
            target.y += 1;
            float dist = Vector3.Distance(transform.position, target);
            GameManager gm = GameManager.Instance;
            if (dist > 1 && dist < gm.pickupRange)
            {
                Vector3 direction = target - transform.position;
                direction.Normalize();
                float distSpeedUp = gm.pickupRange - dist;
                float speedMultiplier = gm.PlayerSpeedMultiplier * gm.pickupSpeed * distSpeedUp;
                transform.position += speedMultiplier * Time.deltaTime * direction;
            } else if (dist < 1)
            {
                PickUpItem();
            }
        }
    }

    private void PickUpItem()
    {
        InventoryManager.Instance.AddItem(ItemRef);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
