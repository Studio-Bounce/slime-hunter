using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSteeringAgent : SteeringAgent
{
    [Header("Slime Attributes")]
    public Transform slimeModel;

    private readonly int IsMoving = Animator.StringToHash("isMoving");

    [Header("For Tutorial")]
    public bool pauseOverride = false;
    bool pauseBehaviour = true;

    protected override void Start()
    {
        base.Start();

        alwaysUseMaxSpeed = true;
    }

    protected override void Update()
    {
        // No point in updating AI without them being visible
        if (!pauseOverride && pauseBehaviour)
            return;

        base.Update();

        if (slimeModel) forceStopMovement = (slimeModel.localPosition.y <= 0);
        animator.SetBool(IsMoving, velocity.magnitude != 0);
    }

    // Used by VisibilityHandler script which must be added to the game object having mesh renderer
    public void PauseAgent()
    {
        pauseBehaviour = true;
    }

    public void UnpauseAgent()
    {
        pauseBehaviour = false;
    }
}
