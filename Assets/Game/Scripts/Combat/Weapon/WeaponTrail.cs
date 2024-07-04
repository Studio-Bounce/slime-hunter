using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    private readonly string flipVFXParameter = "Flip";
    private WeaponSO currentWeaponSO;

    // Visual
    public VisualEffect weaponVFX;

    // New
    private readonly string flipShaderParameter = "_Flip";
    public Renderer trailRenderer;
    private Material trailMaterial;

    protected override void Start()
    {
        base.Start();
        Debug.Assert(weaponVFX != null, "Requires a VisualEffect");
        _vertexCount = meshResolution + 2;
        _collider = GetComponent<MeshCollider>();
        _collider.convex = true;
        _collider.isTrigger = true;

        trailMaterial = trailRenderer.material;

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
        currentWeaponSO = weaponSO;

        damage = weaponSO.damage;
        hitLayers = weaponSO.hitLayers;
        arcRadius = weaponSO.range;
    }

    // If the attack is part of an attack combo sequence, isFinalAttack tells whether
    // it is the final attack in this sequence or not
    public void Attack(AttackMove move, bool isFinalAttack)
    {
        // Update weapon damage on attack
        damage.value = (int)(currentWeaponSO.damage.value * move.damageMultiplier);
        // Update weapon knockback on attack
        damage.knockback = currentWeaponSO.damage.knockback * move.knockbackMultiplier;
        // Update weapon range on attack
        arcRadius = currentWeaponSO.range * move.rangeMultiplier;
        float attackRange = (currentWeaponSO.range / 3) * move.rangeMultiplier; // TODO: Hardcoded /3 as base VFX is roughly 3 units large
        weaponVFX.transform.localScale = new Vector3(attackRange, 1, attackRange);
        // VFX Direction
        weaponVFX.SetBool(flipVFXParameter, move.direction.x < 0);


        // TODO: SHADER
        trailMaterial.SetFloat(flipShaderParameter, move.direction.x < 0 ? 1 : 0); ;


        float verticalRotation = 0;
        if (move.direction.y > 0)
        {
            verticalRotation = -90;
        }
        else if (move.direction.y < 0)
        {
            verticalRotation = 90;
        }

        weaponVFX.transform.rotation = Quaternion.Euler(weaponVFX.transform.rotation.eulerAngles.x, weaponVFX.transform.rotation.eulerAngles.y, verticalRotation);
        UpdateArcMesh();

        applyCameraShake = isFinalAttack;
        StartCoroutine(ActiveAttack(move.duration));
    }

    IEnumerator ActiveAttack(float duration)
    {


        active = true;
        weaponVFX.Play();
        float _timer = 0.0f;
        float _normalTime = 0.0f;
        while (_timer < duration && active)
        {
            _timer += Time.deltaTime;
            _normalTime = _timer / duration;
            trailMaterial.SetFloat("_Factor", _normalTime/3);
            Debug.Log($"{trailMaterial.GetFloat("_Factor")}");
            yield return null;
        }
        active = false;
        trailMaterial.SetFloat("_Factor", 0);
    }

    public void Deactivate()
    {
        trailMaterial.SetFloat("_Factor", 0);
        active = false;
        weaponVFX.Stop();
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
