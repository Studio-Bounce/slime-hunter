using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBomb : DamageDealer
{
    // During explosion build-up, these values are varied in the shader from start to end
    // as per the explosion time.
    [Serializable]
    struct ShaderParam
    {
        public string name;
        public MeshRenderer renderer;
        public float startValue;
        public float endValue;
    };

    [Header("Weapon")]
    [SerializeField] WeaponSO slimeBomb;

    [Header("Ground Crack / Bomb build-up")]
    [SerializeField] Transform groundCrack;
    [SerializeField] float explosionTime = 1.0f;
    [SerializeField] float damageRadius = 5.0f;
    [SerializeField] ShaderParam[] shaderParams;

    SphereCollider sphereCollider;
    MeshRenderer groundCrackMesh;
    bool didExplode = false;

    [SerializeField] BomberEnemy bomberEnemy;
    [SerializeField] UnityEvent onExplosion;

    protected override void Start()
    {
        base.Start();

        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = 0.1f;  // start off small
        Active = false;
        didExplode = false;
        groundCrack.gameObject.SetActive(false);
    }

    public void Explode()
    {
        if (didExplode)
            return;

        // Disable damage taker of this bomb or else it will detect the damage dealer of this bomb
        bomberEnemy.enabled = false;

        didExplode = true;
        Active = true;

        StartCoroutine(ExplosionSequence());
    }

    IEnumerator ExplosionSequence()
    {
        groundCrack.gameObject.SetActive(true);
        float t = 0.0f;
        float startTime = Time.time;
        while (t < 1.0f)
        {
            t = (Time.time - startTime) / explosionTime;
            sphereCollider.radius = t * damageRadius;

            foreach (ShaderParam _param in shaderParams)
            {
                float value = _param.startValue + (_param.endValue - _param.startValue) * t;
                _param.renderer.material.SetFloat(_param.name, value);
            }

            yield return null;
        }
        CameraManager.Instance.ShakeCamera(2.0f, 0.6f);
        // Self destruct
        onExplosion.Invoke();
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawCircle(transform.position, Color.black, damageRadius);
    }

    private void OnValidate()
    {
        if (slimeBomb != null)
        {
            damage = slimeBomb.damage;
            hitLayers = slimeBomb.hitLayers;
        }
    }
}
