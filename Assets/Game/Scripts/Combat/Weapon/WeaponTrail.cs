using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponTrail : DamageDealer
{
    [Header("Hitboxing")]
    public float arcAngle = 90;
    public float arcRadius = 1;
    public int meshResolution = 3;
    public bool liveReload = false;

    private MeshCollider _collider;
    private int _vertexCount;
    private bool _isAttack = false;

    public uint framesToPause = 0;

    private readonly string flipVFXParameter = "Flip";

    // Visual
    public VisualEffect weaponVFX;

    protected override void Start()
    {
        base.Start();
        Debug.Assert(weaponVFX != null, "Requires a VisualEffect");
        _vertexCount = meshResolution + 2;
        _collider = GetComponent<MeshCollider>();
        _collider.convex = true;
        _collider.isTrigger = true;

        _SetupArcMesh();
        _SetupVFX();
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

        weaponVFX.transform.localScale = new Vector3(weaponSO.range/3, 1, weaponSO.range/3); // WIP: Hardcoded /3 as base VFX is roughly 3 units large
    }

    public void Attack(AttackMove move)
    {
        UpdateArcMesh();

        // For Testing
        _framesToPause = framesToPause;
        weaponVFX.SetBool(flipVFXParameter, move.direction.x < 0);
        Debug.Log(weaponVFX.GetBool(flipVFXParameter));

        if (!_isAttack) StartCoroutine(ActiveAttack(move.duration));
    }

    IEnumerator ActiveAttack(float duration)
    {
        active = true;
        weaponVFX.Play();
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

    private void _SetupVFX()
    {
        weaponVFX = Instantiate(weaponVFX.gameObject).GetComponent<VisualEffect>();
        weaponVFX.transform.SetParent(transform);
        weaponVFX.transform.localPosition = Vector3.zero;
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
