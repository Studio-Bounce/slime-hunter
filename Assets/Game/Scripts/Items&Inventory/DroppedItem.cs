using System.Collections;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer), typeof(Billboard))]
public class DroppedItem : MonoBehaviour
{
    [Header("Appearance")]
    public Vector2 size;

    [Header("Pickup Settings")]
    public bool forcePickup = false;
    public float launchDist = 3.0f;
    public float launchAngle = 0;
    public float angleRange = 360;
    public float dropLifetime = 5.0f;

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

    private IEnumerator RemoveAfterSeconds(float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            float alpha = Mathf.Clamp01(1 - (timer / duration));
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    private IEnumerator AnimateDrop()
    {
        // Animation Vars
        float animHeight = 1;
        float animDist = Random.Range(1, launchDist);
        float animDuration = 1.5f;

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
        StartCoroutine(RemoveAfterSeconds(dropLifetime));
    }

    private void Update()
    {
        if (InventoryManager.Instance.IsFull) return;

        

        if (CanBePickedUp)
        {
            if (forcePickup) PickUpItem(true);

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

    private void PickUpItem(bool force = false)
    {
        InventoryManager.Instance.AddItem(ItemRef, force);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
