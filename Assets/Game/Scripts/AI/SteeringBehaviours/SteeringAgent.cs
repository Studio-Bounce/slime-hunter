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

    public float mass = 1.0f;
    public float maxSpeed = 1.0f;
    public float maxForce = 10.0f;

    public Vector3 velocity = Vector3.zero;

    private List<SteeringBehaviourBase> steeringBehaviours = new List<SteeringBehaviourBase>();

    public float angularDampeningTime = 5.0f;
    public float deadZone = 10.0f;

    public bool reachedGoal = false;

    // Animation
    public bool useRootMotion = true;
    public bool useGravity = true;

    private Animator animator;
    private CharacterController characterController;
    private Rigidbody agentRigidbody;

    private void Start()
    {
        animator = GetComponent<Animator>();
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

    private void Update()
    {
        Vector3 steeringForce = CalculateSteeringForce();
        //steeringForce.y = 0.0f;

        if (reachedGoal)
        {
            velocity = Vector3.zero;
            if (animator != null)
                animator.SetFloat("Speed", 0.0f);
        }
        else
        {
            Vector3 acceleration = steeringForce / mass;
            velocity = velocity + (acceleration * Time.deltaTime);
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            //transform.position += velocity * Time.deltaTime;

            float speed = velocity.magnitude;
            if (animator != null)
                animator.SetFloat("Speed", speed);

            // Movement
            if (!useRootMotion)
            {
                if (agentRigidbody != null)
                {
                    agentRigidbody.velocity = velocity;
                }
                else if (characterController != null)
                {
                    characterController.Move(velocity * Time.deltaTime);
                    if (useGravity)
                        characterController.Move(Physics.gravity * Time.deltaTime);
                }
                else
                {
                    transform.position += (velocity * Time.deltaTime);
                }
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
