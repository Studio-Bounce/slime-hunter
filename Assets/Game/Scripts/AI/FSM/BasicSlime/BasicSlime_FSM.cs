using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlime_FSM : FSM
{
    public readonly int ChasePlayerStateName = Animator.StringToHash("ChasePlayer");
    public readonly int WanderAroundStateName = Animator.StringToHash("WanderAround");
    public readonly int AttackPlayerStateName = Animator.StringToHash("AttackPlayer");
    public readonly int CooldownStateName = Animator.StringToHash("Cooldown");
    public readonly int DeadStateName = Animator.StringToHash("Dead");

    public BasicSlime_AttackPlayer.AttackState currentAttackState = BasicSlime_AttackPlayer.AttackState.NONE;

    public SeekSteeringBehaviour seekSteeringBehaviour;
    public WanderSteeringBehaviour wanderSteeringBehaviour;

    [HideInInspector] public SteeringAgent slimeAgent;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Transform playerTransform;

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
    public float wanderSpeed = 1.0f;
    public float chaseSpeed = 5.0f;
    public float attackSpeed = 10.0f;
    public float attackRadius = 2.0f;
    [Tooltip("Time taken in reaching full emission before attack")]
    public float attackEmissionTime = 1.0f;
    [Tooltip("How far can the slime hit while on rest?")]
    public float attackProximity = 0.5f;
    public Animator slimeAnimator;

    protected virtual void Start()
    {
        slimeAgent = GetComponent<SteeringAgent>();
        characterController = GetComponent<CharacterController>();
        weapon = GetComponent<EnemyWeapon>();
        slimeEnemy = GetComponent<Enemy>();
        playerTransform = GameObject.FindWithTag("Player")?.transform;
        UnityEngine.Assertions.Assert.IsNotNull(playerTransform, "GameObject with tag 'Player' not found!");
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawCircle(transform.position, Color.cyan, seekDistance);
        DebugExtension.DrawCircle(transform.position, Color.red, attackRadius);
    }

    public BasicSlime_AttackPlayer.AttackState GetAttackState()
    {
        return currentAttackState;
    }
}
