using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class FSM : MonoBehaviour
{
    public RuntimeAnimatorController FSMController;

    public bool lockState = false;

    public Animator fsmAnimator { get; private set; }

    protected virtual void Awake()
    {
        GameObject FSMGO = new GameObject("FSM", typeof(Animator));
        FSMGO.transform.parent = transform;

        fsmAnimator = FSMGO.GetComponent<Animator>();
        fsmAnimator.runtimeAnimatorController = FSMController;

        // Hide animator (optional)
        fsmAnimator.hideFlags = HideFlags.HideInInspector;

        InternalFSMBaseState[] behaviours = fsmAnimator.GetBehaviours<InternalFSMBaseState>();
        foreach(var behaviour in behaviours)
        {
            behaviour.Init(gameObject, this);
        }
    }

    // Deny any state changes temporarily
    public void LockStateForSeconds(float duration)
    {
        if (!lockState) StartCoroutine(TempStopStateChanges(duration));
    }

    private IEnumerator TempStopStateChanges(float duration)
    {
        lockState = true;
        yield return new WaitForSeconds(duration);
        lockState = false;
    }

    public bool ChangeState(string _stateName)
    {
        if (lockState) return false;
        return ChangeState(Animator.StringToHash(_stateName));
    }

    public bool ChangeState(int _stateName)
    {
        if (lockState) return false;
        bool hasState = true;
#if UNITY_EDITOR
        //hasState = fsmAnimator.HasState(-1, _stateName);
        //Debug.Assert(hasState == true, $"{gameObject.name} missing state behaviour {_stateName}");
#endif
        //fsmAnimator.CrossFade(_stateName, 0.0f, -1);
        fsmAnimator.CrossFade(_stateName, 0.0f);
        return hasState;
    }
}
