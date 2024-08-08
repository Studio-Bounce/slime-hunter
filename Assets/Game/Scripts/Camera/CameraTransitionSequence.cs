using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraTransitionSequence : MonoBehaviour
{
    public bool disableControlsInTransition = true;
    [Header("Trigger Key")]
    public KeyCode transitionKey = KeyCode.None;

    [Serializable]
    public struct TransitionCamera
    {
        public CinemachineVirtualCamera cam;

        [Header("Transition")]
        public float delay;
        public float duration;
        public CinemachineBlendDefinition.Style curve;
        public UnityEvent onComplete;
    }

    public List<TransitionCamera> transitions;
    private CinemachineVirtualCamera _startingCamera;

    private void Update()
    {
        // Check if the specified key is pressed
        if (Input.GetKeyDown(transitionKey))
        {
            BeginTransitions();
        }
    }

    public void BeginTransitions()
    {
        if (disableControlsInTransition)
        {
            InputManager.Instance.TogglePlayerControls(false);
        }
        _startingCamera = CameraManager.ActiveCineCamera;
        StartCoroutine(StartTransitions());
    }

    private IEnumerator StartTransitions()
    {
        foreach (var trans in transitions)
        {
            CameraManager.Instance.AddBlend(CameraManager.ActiveCineCamera, trans.cam, trans.curve, trans.duration);
            yield return new WaitForSecondsRealtime(trans.delay);
            CameraManager.Instance.ChangeVirtualCamera(trans.cam);
            yield return new WaitForSecondsRealtime(trans.duration);
            trans.onComplete.Invoke();
        }
        InputManager.Instance.TogglePlayerControls(true);
    }
}
