using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileSlime_FSM : BasicSlime_FSM
{
    public readonly int FleeStateName = Animator.StringToHash("Flee");

    [Header("Projectile Slime Attributes")]
    public FleeSteeringBehaviour fleeSteeringBehaviour;
    public GameObject projectile;
    public float projectileSpeed = 10.0f;
    [Tooltip("Slime flees if player gets in proximity")]
    public float playerProximityDistance = 5.0f;
    public float fleeSpeed = 5.0f;
    [Tooltip("Time taken in rotating towards the player after cooldown")]
    [SerializeField] float rotationTime = 0.5f;

    [HideInInspector] public bool isAttacking = false;

    // Animation parameters
    public readonly int ShootTrigger = Animator.StringToHash("shoot");
    [Tooltip("Delay after triggering attack animation")]
    [SerializeField] float spitterTimeSync = 1.0f;

    public UnityEvent onAttackEvent;

    Coroutine attackCoroutine;

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
            attackCoroutine = StartCoroutine(ShootProjectiles());
        }
    }

    public void StopAttacking()
    {
        if (isAttacking)
        {
            isAttacking = false;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    IEnumerator ShootProjectiles()
    {
        // Shoot projectiles
        while (isAttacking)
        {
            yield return new WaitForSeconds(cooldownTime);

            // Turn towards player
            float elapsedTime = 0f;
            Quaternion startRot = transform.rotation;
            Quaternion endRot = Quaternion.LookRotation(GetPlayerPosition() - transform.position);
            while (elapsedTime < rotationTime)
            {
                float t = elapsedTime / rotationTime;
                transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            transform.rotation = endRot;

            slimeAnimator.SetTrigger(ShootTrigger);
            yield return new WaitForSeconds(spitterTimeSync);  // Animation time
            onAttackEvent.Invoke();
            Vector3 position = transform.position + transform.forward;
            position.y += characterController.height / 2.0f;
            GameObject projectileGO = Instantiate(projectile, position, Quaternion.identity);
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
        attackGlowIntensity = 0;
        chaseSpeed = 0;
        attackSpeed = 0;
        attackRadius = 0;
        attackEmissionTime = 0;
        attackProximity = 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        DebugExtension.DrawCircle(transform.position, Color.black, playerProximityDistance);
    }
}
