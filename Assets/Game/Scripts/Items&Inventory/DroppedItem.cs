using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(SpriteRenderer), typeof(Billboard))]
public class DroppedItem : MonoBehaviour
{
    [Header("Appearance")]
    public Vector2 iconSize;

    [Header("Pickup Settings")]
    public bool forcePickup = false;
    public float dropLifetime = 5.0f;

    [Header("Launch Settings")]
    public Vector3 launchDirection = Vector3.zero;
    public float launchDist = 2.0f;
    public float launchHeight = 1.0f;
    public float launchAngle = 0;
    public float launchDuration = 1.5f;
    public float angleRange = 360;

    // Refs
    private SpriteRenderer spriteRenderer;
    private Player player;
    private GameManager gm;

    // Getters Setters
    public ItemSO ItemRef { get; set; }
    public bool CanBePickedUp { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        player = gm.PlayerRef;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = ItemRef.icon;
        ResizeSprite(spriteRenderer, iconSize);
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
        // Random Direction
        float randomAngle = launchAngle + Random.Range(-angleRange / 2f, angleRange / 2f);
        Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
        Vector3 randomDir = rotation * launchDirection;
        randomDir.Normalize();

        Vector3 start = transform.position;
        Vector3 end = transform.position + launchDist * randomDir;
        float startEndHeight = transform.position.y;
        float timer = 0;
        float eased = 0;
        while (timer < launchDuration)
        {
            float normalTime = timer / launchDuration;
            // Horizontal
            eased = Easing.EaseOut(normalTime);
            Vector3 newPos = Vector3.Lerp(start, end, eased);
            // Vertical
            eased = Easing.EaseOutBounce(normalTime);
            newPos.y = startEndHeight + Mathf.Sin(eased * Mathf.PI) * launchHeight;

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
        if (!CanBePickedUp) return;

        if (forcePickup) ForceMagnetPickup(); else MagnetPickup();
    }

    private void ForceMagnetPickup()
    {
        Vector3 target = player.transform.position;
        target.y += 1;
        Vector3 direction = target - transform.position;
        direction.Normalize();
        transform.position += gm.pickupSpeed * 2 * Time.unscaledDeltaTime * direction;

        float dist = Vector3.Distance(transform.position, target);
        if (dist < 1)
        {
            PickUpItem(true);
        }
    }

    private void MagnetPickup()
    {
        Vector3 target = player.transform.position;
        target.y += 1;
        float dist = Vector3.Distance(transform.position, target);

        if (dist > 1 && dist < gm.pickupRange)
        {
            Vector3 direction = target - transform.position;
            direction.Normalize();
            // Speed up magnet when closer
            float distSpeedUp = gm.pickupRange - dist;
            float speedMultiplier = gm.pickupSpeed * distSpeedUp;
            transform.position += speedMultiplier * Time.unscaledDeltaTime * direction;
        }
        else if (dist < 1)
        {
            PickUpItem();
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

        Vector3 startPoint = transform.position;
        Vector3 endPoint = startPoint + transform.forward;

        // Draw the main line of the arrow
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPoint, endPoint);
    }
}
