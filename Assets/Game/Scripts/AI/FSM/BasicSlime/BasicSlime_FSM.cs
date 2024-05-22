using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlime_FSM : FSM
{
    public readonly int ChasePlayerStateName = Animator.StringToHash("ChasePlayer");
    public readonly int WanderAroundStateName = Animator.StringToHash("WanderAround");
    public readonly int AttackPlayerStateName = Animator.StringToHash("AttackPlayer");

    public SeekSteeringBehaviour seekSteeringBehaviour;
    public WanderSteeringBehaviour wanderSteeringBehaviour;

    public SteeringAgent slimeAgent;
    public Transform playerTransform;

    public float seekDistance = 20.0f;
    public float wanderSpeed = 1.0f;
    public float chaseSpeed = 5.0f;
    public float attackSpeed = 10.0f;
    public float attackRadius = 2.0f;

    private void Start()
    {
        slimeAgent = GetComponent<SteeringAgent>();
        playerTransform = GameObject.FindWithTag("Player")?.transform;
        UnityEngine.Assertions.Assert.IsNotNull(playerTransform, "GameObject with tag 'Player' not found!");
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawCircle(transform.position, Color.cyan, seekDistance);
        DebugExtension.DrawCircle(transform.position, Color.red, attackRadius);
    }
}
