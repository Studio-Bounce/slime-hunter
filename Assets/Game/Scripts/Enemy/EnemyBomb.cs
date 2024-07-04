using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBomb : DamageDealer
{
    [SerializeField] WeaponSO slimeBomb;
    [SerializeField] Transform ring;
    [SerializeField] Renderer ringRenderer;

    public float explosionTime = 1.0f;
    [HideInInspector] public float damageRadius = 0.0f;

    SphereCollider sphereCollider;
    bool didExplode = false;

    public UnityEvent onExplosion;

    protected override void Start()
    {
        base.Start();

        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = 0.1f;  // start off small
        Active = false;
        didExplode = false;
        ring.gameObject.SetActive(false);
    }

    public void Explode()
    {
        if (didExplode)
            return;
        didExplode = true;
        Active = true;
        ring.gameObject.SetActive(true);
        StartCoroutine(ExplosionSequence());
    }

    IEnumerator ExplosionSequence()
    {
        float t = 0.0f;
        float startTime = Time.time;
        // Increase collider sphere
        while (t < 1.0f)
        {
            t = (Time.time - startTime) / explosionTime;
            t = Easing.Linear(t);

            float val = t * damageRadius;
            sphereCollider.radius = val;

            float scale = (val / 5.0f);  // ring : unity unit ratio = 5
            ring.localScale = new Vector3(scale, scale, scale);
            Material mat = ringRenderer.material;
            mat.SetFloat("_ThicknessScale", 1 / scale);
            mat.SetFloat("_FeatheringScale", 1 / scale);

            yield return null;
        }

        // Self destruct
        onExplosion.Invoke();
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
