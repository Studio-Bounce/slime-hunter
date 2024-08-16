using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BomberSlime_FSM : BasicSlime_FSM
{
    // Animation parameters
    public readonly int AlertTrigger = Animator.StringToHash("alert");
    public readonly int PlayerLostTrigger = Animator.StringToHash("playerLost");
    public readonly int PlayerInRangeTrigger = Animator.StringToHash("playerInRange");

    [HideInInspector] public EnemyBomb enemyBomb;
    bool didExplode = false;

    // Used in tutorial
    [Header("For tutorial")]
    public Transform targetOverride = null;
    public bool alertTriggerOverride = false;

    // Events
    public UnityEvent onDetonationStart;

    protected override void Start()
    {
        base.Start();

        didExplode = false;
        enemyBomb = GetComponentInChildren<EnemyBomb>();
    }

    public void GotHit()
    {
        ChangeState(AttackPlayerStateName);
    }

    public void Explode()
    {
        if (!didExplode)
        {
            didExplode = true;
            Emit(10);
            StartCoroutine(ExplosionSequence());
        }
    }

    IEnumerator ExplosionSequence()
    {
        // wait for glow
        yield return new WaitForSeconds(attackEmissionTime);
        onDetonationStart.Invoke();
        enemyBomb.Explode();
    }

    public override Vector3 GetPlayerPosition()
    {
        if (targetOverride != null)
            return targetOverride.position;

        return base.GetPlayerPosition();
    }

    private void OnValidate()
    {
        // Nullify the fields which are not used for bomber slime
        cooldownTime = 0;
        attackSpeed = 0;
        attackProximity = 0;
        defaultMat = null;
        chaseMat = null;
        attackMat = null;
        attackGlowIntensity = 1.0f;
    }
}
