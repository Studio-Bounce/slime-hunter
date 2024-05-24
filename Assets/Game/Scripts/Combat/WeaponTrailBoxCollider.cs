using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrailBoxCollider : MonoBehaviour
{
    [Header("Hitboxing")]
    private List<BoxCollider> _colliders = new List<BoxCollider>();
    public float arcAngle;
    public float arcRadius;
    public float arcHeight;

    public int colliderResolution = 5;
    public float colliderWidthOffset = 0;

    public bool liveReload = false;

    void Start()
    {
        InstantiateColliders();
        SetupBoxArc(arcAngle, arcRadius, arcHeight);
    }

    private void Update()
    {
        if (liveReload) SetupBoxArc(arcAngle, arcRadius, arcHeight);
    }

    private void InstantiateColliders()
    {
        _colliders.Clear();
        for (int i = 0; i < colliderResolution; i++)
        {
            GameObject arcPart = new GameObject($"ArcPart-{i}");
            arcPart.transform.SetParent(transform);

            BoxCollider collider =  arcPart.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            _colliders.Add(collider);
            arcPart.AddComponent<Weapon>();
        }
    }

    private void SetWeapon(WeaponSO weaponSO)
    {
        foreach (BoxCollider collider in _colliders)
        {
            Weapon weapon = collider.gameObject.GetComponent<Weapon>();
            weapon.Setup(weaponSO);
        }
    }

    private void SetupBoxArc(float angle, float radius, float height)
    {
        float angleStep = angle / (colliderResolution - 1);
        float startAngle = -angle / 2.0f;

        float boxWidth = Mathf.Tan(Mathf.Deg2Rad*angle / colliderResolution)*radius + colliderWidthOffset;
        for (int i = 0; i < _colliders.Count; ++i)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector3 position = transform.position + Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * radius/2;
            _colliders[i].transform.position = position;
            _colliders[i].size = new Vector3(boxWidth, height, radius);
            _colliders[i].transform.rotation = Quaternion.Euler(0, currentAngle, 0);
        }
    }

    private void OnDrawGizmos()
    {
        if (_colliders == null) return;

        foreach (BoxCollider boxCollider in _colliders)
        {
            if (boxCollider == null) continue;

            Gizmos.color = new Color(1, 0, 1);
            Transform colliderTransform = boxCollider.transform;

            // Calculate the center and size in world space
            Vector3 center = colliderTransform.TransformPoint(boxCollider.center);
            Vector3 size = Vector3.Scale(boxCollider.size, colliderTransform.lossyScale);

            // Draw the BoxCollider
            Gizmos.matrix = Matrix4x4.TRS(center, colliderTransform.rotation, size);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
