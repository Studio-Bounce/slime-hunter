using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasicSlime_FSM : FSM
{
    public readonly int ChasePlayerStateName = Animator.StringToHash("ChasePlayer");
    public readonly int WanderAroundStateName = Animator.StringToHash("WanderAround");
    public readonly int BackToBaseStateName = Animator.StringToHash("BackToBase");
    public readonly int AttackPlayerStateName = Animator.StringToHash("AttackPlayer");
    public readonly int CooldownStateName = Animator.StringToHash("Cooldown");
    public readonly int DeadStateName = Animator.StringToHash("Dead");
    public readonly int StunnedStateName = Animator.StringToHash("Stunned");

    [HideInInspector] public BasicSlime_AttackPlayer.SlimeAttackState currentAttackState = BasicSlime_AttackPlayer.SlimeAttackState.NONE;

    public SeekSteeringBehaviour seekSteeringBehaviour;
    public WanderSteeringBehaviour wanderSteeringBehaviour;

    [HideInInspector] public SteeringAgent slimeAgent;
    [HideInInspector] public CharacterController characterController;
    protected Transform playerTransform;

    public SkinnedMeshRenderer slimeOuterMesh;
    [HideInInspector] public EnemyWeapon weapon;
    [HideInInspector] public Enemy slimeEnemy;

    [Header("Slime Materials")]
    public Material defaultMat;
    public Material chaseMat;
    public Material attackMat;
    public float attackGlowIntensity = 2.0f;

    [Header("Slime Attributes")]
    [Tooltip("Attributes which change the slime's combat behaviour.")]
    public float cooldownTime = 3.0f;
    public float seekDistance = 20.0f;
    public float chaseSpeed = 5.0f;
    public float attackSpeed = 10.0f;
    public float attackRadius = 2.0f;
    // Wander Settings
    public float wanderSpeed = 1.0f;
    public float backToBaseSpeed = 10.0f;
    public float backToBaseLockedDuration = 3.0f;
    [Tooltip("Time taken in reaching full emission before attack")]
    public float attackEmissionTime = 1.0f;
    [Tooltip("How far can the slime hit while on rest?")]
    public float attackProximity = 0.5f;
    public Animator slimeAnimator;

    protected override void Awake()
    {
        base.Awake();

        slimeAgent = GetComponent<SteeringAgent>();
        characterController = GetComponent<CharacterController>();
        weapon = GetComponent<EnemyWeapon>();
        slimeEnemy = GetComponent<Enemy>();
    }

    protected virtual void Start()
    {
        // Player might take some time to get spawned
        // Keep trying to find the player till the player is found
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer()
    {
        while (playerTransform == null)
        {
            playerTransform = GameManager.Instance.PlayerRef?.transform;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public Vector3 GetPlayerForward()
    {
        if (playerTransform == null)
            return Vector3.forward;

        return playerTransform.forward;
    }

    public virtual Vector3 GetPlayerPosition()
    {
        if (playerTransform == null)
            return Vector3.zero;

        return playerTransform.position;
    }

    public void Emit(int steps)
    {
        StartCoroutine(IncreaseEmissionIntensity(steps));
    }

    IEnumerator IncreaseEmissionIntensity(int steps)
    {
        float intensity = 0.0f;
        float intensityDelta = attackGlowIntensity / steps;
        float deltaTime = attackEmissionTime / steps;
        while (steps > 0 && attackMat != null)
        {
            --steps;
            intensity += intensityDelta;

            Color finalEmissionColor = attackMat.color * intensity;
            if (slimeOuterMesh) slimeOuterMesh.materials[0].SetColor("_EmissionColor", finalEmissionColor);

            yield return new WaitForSeconds(deltaTime);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        DebugExtension.DrawCircle(transform.position, new Color(0, 1, 1, 0.9f), seekDistance);
        DebugExtension.DrawCircle(transform.position, new Color(1, 0, 0, 0.9f), attackRadius);
    }

    public BasicSlime_AttackPlayer.SlimeAttackState GetAttackState()
    {
        return currentAttackState;
    }
}
