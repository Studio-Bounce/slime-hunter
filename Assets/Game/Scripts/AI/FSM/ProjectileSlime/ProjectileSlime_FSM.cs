using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSlime_FSM : BasicSlime_FSM
{
    public readonly int FleeStateName = Animator.StringToHash("Flee");

    [Header("Projectile Slime Attributes")]
    public FleeSteeringBehaviour fleeSteeringBehaviour;
    public GameObject projectile;
    public float projectileSpeed = 10.0f;
    public float playerProximityDistance = 5.0f;
    public float fleeSpeed = 5.0f;

    [HideInInspector] public bool isAttacking = false;

    protected override void Start()
    {
        base.Start();

        fleeSteeringBehaviour = GetComponentInChildren<FleeSteeringBehaviour>();
    }

    public void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(ShootProjectiles());
        }
    }

    IEnumerator ShootProjectiles()
    {
        // Shoot projectiles
        while (isAttacking)
        {
            yield return new WaitForSeconds(cooldownTime);

            // Turn towards the player
            transform.LookAt(GetPlayerPosition());

            GameObject projectileGO = Instantiate(projectile, transform.position + transform.forward, Quaternion.identity);
            ProjectileSlimeBullet bullet = projectileGO.GetComponent<ProjectileSlimeBullet>();
            bullet.speed = projectileSpeed;
            bullet.direction = (playerTransform.position - transform.position).normalized;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnValidate()
    {
        // Nullify the fields which are not used for projectile slime
        seekSteeringBehaviour = null;
        chaseSpeed = 0;
        attackSpeed = 0;
        attackRadius = 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        DebugExtension.DrawCircle(transform.position, Color.black, playerProximityDistance);
    }
}
