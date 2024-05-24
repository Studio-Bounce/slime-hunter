using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrailMeshCollider : DamageDealer
{
    [Header("Hitboxing")]
    public float arcAngle = 90;
    public float arcRadius = 1;
    public int meshResolution = 3;
    public bool liveReload = false;

    private MeshCollider _collider;
    private int _vertexCount;
    private bool _isAttack = false;

    protected override void Start()
    {
        base.Start();
        _vertexCount = meshResolution + 2;
        _collider = GetComponent<MeshCollider>();
        _SetupArcMesh();
        _collider.convex = true;
        _collider.isTrigger = true;
    }

    protected override void Update()
    {
        base.Update();
        if (liveReload) _SetupArcMesh();
    }

    public void SetupWeaponSettings(WeaponSO weaponSO)
    {
        damage = weaponSO.damage;
        hitLayers = weaponSO.hitLayers;
        arcRadius = weaponSO.range;
    }

    public void Attack()
    {
        UpdateArcMesh();
        if (!_isAttack) StartCoroutine(ActiveAttack(0.5f));
    }

    IEnumerator ActiveAttack(float duration)
    {
        active = true;
        yield return new WaitForSeconds(duration);
        active = false;
    }

    private void UpdateArcMesh()
    {
        Mesh mesh = _collider.sharedMesh;
        Vector3[] vertices = mesh.vertices;

        float angleStep = arcAngle / (meshResolution);
        float startAngle = -arcAngle / 2.0f;
        for (int i = 1; i < _vertexCount; ++i)
        {
            float currentAngle = startAngle + (angleStep * (i - 1));
            Vector3 vertexPos = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * arcRadius;
            vertices[i] = vertexPos;
        }
        mesh.vertices = vertices;
        _collider.sharedMesh = mesh;
    }

    private void _SetupArcMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[_vertexCount];

        // Create Vertices
        vertices[0] = Vector3.zero;

        float angleStep = arcAngle / (meshResolution);
        float startAngle = -arcAngle / 2.0f;
        for (int i = 1; i < _vertexCount; ++i)
        {
            float currentAngle = startAngle + (angleStep * (i-1));
            Vector3 vertexPos = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * arcRadius;
            vertices[i] = vertexPos;
        }
        mesh.vertices = vertices;

        // Create Triangles
        int[] triangles = new int[meshResolution*3];
        int triangleVert = 1;
        for (int i = 0; i < triangles.Length-1; i+=3)
        {
            triangles[i] = 0;
            triangles[i + 1] = triangleVert;
            triangleVert++;
            triangles[i + 2] = triangleVert;
        }
        mesh.triangles = triangles;

        _collider.sharedMesh = mesh;
    }

    private void OnDrawGizmos() {
        if (_collider == null) return;

        Gizmos.color = active ? Color.red : Color.green;
        // Get the mesh vertices
        Vector3[] vertices = _collider.sharedMesh.vertices;
        // Get the mesh triangles
        int[] triangles = _collider.sharedMesh.triangles;

        // Draw each triangle
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v0);
        }
    }
}
