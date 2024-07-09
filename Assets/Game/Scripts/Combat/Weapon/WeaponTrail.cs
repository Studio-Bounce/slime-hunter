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

    private WeaponSO currentWeaponSO;

    // Trail Shader Effects
    private readonly string flipShaderParam = "_Flip";
    private readonly string rotateSpeedShaderParam = "_RotateSpeed";
    private readonly string startAngleShaderParam = "_StartAngle";
    private readonly string rangeShaderParam = "_Range";
    private readonly string rotationGammaShaderParam = "_RotationGamma";
    private readonly string brightPeakShaderParam = "_Bright_Peak";
    private readonly string darkPeakShaderParam = "_Dark_Peak";

    public Renderer trailRenderer;
    private Material trailMaterial;

    protected override void Start()
    {
        base.Start();
        _vertexCount = meshResolution + 2;
        _collider = GetComponent<MeshCollider>();
        _collider.convex = true;
        _collider.isTrigger = true;

        trailMaterial = trailRenderer.material;

        _SetupArcMesh();
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
        if (currentWeaponSO.material != null)
        {
            trailRenderer.material = currentWeaponSO.material;
            trailMaterial = currentWeaponSO.material;
        }
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
        float attackRange = (currentWeaponSO.range) * move.rangeMultiplier;

        // Trail Effects Direction
        trailMaterial.SetFloat(flipShaderParam, move.direction.x < 0 ? 0 : 1);
        trailRenderer.transform.localScale = new Vector3(attackRange, 1, attackRange);
        float verticalRotation = 0;
        if (move.direction.y > 0)
        {
            verticalRotation = -90;
        }
        else if (move.direction.y < 0)
        {
            verticalRotation = 90;
        }

        // Trail Material
        trailMaterial.SetFloat(rotateSpeedShaderParam, move.rotateSpeed);
        trailMaterial.SetFloat(startAngleShaderParam, move.angleStart);
        trailMaterial.SetFloat(rangeShaderParam, move.angleRange);
        trailMaterial.SetFloat(rotationGammaShaderParam, move.rotationGamma);
        trailMaterial.SetFloat(brightPeakShaderParam, move.voranoiPeak);
        trailMaterial.SetFloat(darkPeakShaderParam, move.voranoiPeak);

        UpdateArcMesh();
        applyCameraShake = isFinalAttack;
        StopAllCoroutines();
        StartCoroutine(ActiveAttack(move.animationDuration));
        trailRenderer.transform.rotation = Quaternion.Euler(trailRenderer.transform.rotation.eulerAngles.x, trailRenderer.transform.rotation.eulerAngles.y, verticalRotation);
    }

    IEnumerator ActiveAttack(float duration)
    {
        active = true;
        float _timer = 0.0f;
        float _normalTime = 0.0f;
        while (_timer < duration && active)
        {
            _timer += Time.deltaTime;
            _normalTime = _timer / duration;
            trailMaterial.SetFloat("_Factor", _normalTime);
            yield return null;
        }
        active = false;
        trailMaterial.SetFloat("_Factor", 0);
    }

    public void Deactivate()
    {
        trailMaterial.SetFloat("_Factor", 0);
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
