using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlime_FSM : FSM
{
    public readonly int ChasePlayerStateName = Animator.StringToHash("ChasePlayer");
    public readonly int WanderAroundStateName = Animator.StringToHash("WanderAround");

    public SeekSteeringBehaviour seekSteeringBehaviour;
    public WanderSteeringBehaviour wanderSteeringBehaviour;

    public SteeringAgent slimeAgent;
    public float seekDistance = 20.0f;
    public Transform playerTransform;

    private void Start()
    {
        slimeAgent = GetComponent<SteeringAgent>();
        playerTransform = GameObject.FindWithTag("Player")?.transform;
        UnityEngine.Assertions.Assert.IsNotNull(playerTransform, "GameObject with tag 'Player' not found!");
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawCircle(transform.position, Color.cyan, seekDistance);
    }
}
