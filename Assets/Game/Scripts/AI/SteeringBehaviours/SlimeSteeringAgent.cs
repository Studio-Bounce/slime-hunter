using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSteeringAgent : SteeringAgent
{
    [Header("Slime Attributes")]
    public Transform slimeModel;

    private readonly int IsMoving = Animator.StringToHash("isMoving");

    protected override void Start()
    {
        base.Start();

        alwaysUseMaxSpeed = true;
    }

    protected override void Update()
    {
        base.Update();

        forceStopMovement = (slimeModel.localPosition.y <= 0);
        animator.SetBool(IsMoving, velocity.magnitude != 0);
    }
}
