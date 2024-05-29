using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    public enum SummingMethod
    {
        WeightedAverage,
        Prioritized,
    };
    public SummingMethod summingMethod = SummingMethod.WeightedAverage;

    [Header("Agent Attributes")]
    public float mass = 1.0f;
    public float maxSpeed = 1.0f;
    public float maxForce = 10.0f;

    public Vector3 velocity = Vector3.zero;

    protected List<SteeringBehaviourBase> steeringBehaviours = new List<SteeringBehaviourBase>();

    public float angularDampeningTime = 5.0f;
    public float deadZone = 10.0f;

    [HideInInspector] public bool reachedGoal = false;

    [Header("Animation")]
    public Animator animator;
    public bool useRootMotion = true;
    public bool useGravity = true;
    protected float upVelocity = 0;

    protected CharacterController characterController;
    protected Rigidbody agentRigidbody;

    protected virtual void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (animator == null)
        {
            useRootMotion = false;
        }
        characterController = GetComponent<CharacterController>();
        agentRigidbody = GetComponent<Rigidbody>();

        steeringBehaviours.AddRange(GetComponentsInChildren<SteeringBehaviourBase>());
        foreach (SteeringBehaviourBase behaviour in steeringBehaviours)
        {
            behaviour.steeringAgent = this;
        }
    }

    protected virtual void Update()
    {
        Vector3 steeringForce = CalculateSteeringForce();

        // Y movement
        if (!useRootMotion && characterController != null && characterController.enabled)
        {
            if (useGravity)
            {
                upVelocity += Physics.gravity.y * Time.deltaTime;
            }

            if (characterController.isGrounded & upVelocity < 0)
            {
                upVelocity = 0.0f;
            }

            characterController.Move(new Vector3(0, upVelocity * Time.deltaTime, 0));
        }
        // XZ movement
        if (reachedGoal)
        {
            velocity = Vector3.zero;
        }
        else
        {
            Vector3 acceleration = steeringForce / mass;
            velocity += (acceleration * Time.deltaTime);
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            // Movement
            if (!useRootMotion)
            {
                MoveWithVelocity(velocity);
            }

            // Rotation
            if (velocity.magnitude > 0.0f)
            {
                Vector3 vector3 = new Vector3(velocity.x, 0.0f, velocity.z);
                float angle = Vector3.Angle(transform.forward, vector3);
                if (Mathf.Abs(angle) <= deadZone)
                {
                    transform.LookAt(transform.position + vector3);
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                                                          Quaternion.LookRotation(vector3),
                                                          Time.deltaTime * angularDampeningTime);
                }
            }
        }
    }

    private void MoveWithVelocity(Vector3 velocity)
    {
        if (agentRigidbody != null && !agentRigidbody.isKinematic)
        {
            agentRigidbody.velocity = velocity;
        }
        else if (characterController != null && characterController.enabled)
        {
            characterController.Move(velocity * Time.deltaTime);
        }
        else
        {
            transform.position += (velocity * Time.deltaTime);
        }
    }

    private Vector3 CalculateSteeringForce()
    {
        Vector3 totalForce = Vector3.zero;

        foreach (SteeringBehaviourBase behaviour in steeringBehaviours)
        {
            if (behaviour.enabled)
            {
                switch (summingMethod)
                {
                    case SummingMethod.WeightedAverage:
                        totalForce = totalForce + (behaviour.CalculateForce() * behaviour.weight);
                        totalForce = Vector3.ClampMagnitude(totalForce, maxForce);
                        break;

                    case SummingMethod.Prioritized:
                        Vector3 steeringForce = (behaviour.CalculateForce() * behaviour.weight);
                        if (!AccumulateForce(ref totalForce, steeringForce))
                        {
                            return totalForce;
                        }
                        break;
                }

            }
        }

        return totalForce;
    }

    private bool AccumulateForce(ref Vector3 runningTotalForce, Vector3 forceToAdd)
    {
        float magnitudeSoFar = runningTotalForce.magnitude;
        float magnitudeRemaining = maxForce - magnitudeSoFar;

        if (magnitudeRemaining <= 0.0f)
        {
            return false;
        }

        float magnitudeToAdd = forceToAdd.magnitude;
        if (magnitudeToAdd < magnitudeRemaining)
        {
            runningTotalForce = runningTotalForce + forceToAdd;
        }
        else
        {
            runningTotalForce = runningTotalForce + (forceToAdd * magnitudeRemaining);
            return false;
        }

        return true;
    }

    private void OnAnimatorMove()
    {
        if (Time.deltaTime != 0.0f && useRootMotion)
        {
            Vector3 animationVelocity = animator.deltaPosition / Time.deltaTime;
            if (characterController != null)
                characterController.Move((transform.forward * animationVelocity.magnitude) * Time.deltaTime);
            else
                transform.position += (transform.forward * animationVelocity.magnitude) * Time.deltaTime;

            if (useGravity)
                characterController.Move(Physics.gravity * Time.deltaTime);
        }
    }
}
