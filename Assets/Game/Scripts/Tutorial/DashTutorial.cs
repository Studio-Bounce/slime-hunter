using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTutorial : MonoBehaviour
{
    SlimeSteeringAgent sliAgent;
    DashSlime_FSM slimeFSM;

    public GameObject birdGO;

    bool triggered = false;

    private void Start()
    {
        slimeFSM = GetComponent<DashSlime_FSM>();
        sliAgent = GetComponent<SlimeSteeringAgent>();
        sliAgent.PauseAgent();
    }

    public void HeadToBirds()
    {
        if (triggered)
            return;

        triggered = true;
        sliAgent.UnpauseAgent();
        slimeFSM.SetPlayerTransform(birdGO.transform);
    }

    public void BirdsAreAway()
    {
        slimeFSM.SetPlayerTransform(GameManager.Instance.PlayerRef.transform);
    }
}
