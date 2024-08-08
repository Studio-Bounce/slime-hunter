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
    public KeyCode transitionKey = KeyCode.None; // Default key is 'T'

    [Serializable]
    public struct TransitionCamera
    {
        public CinemachineVirtualCamera cam;

        [Header("Transition")]
        public float delay;
        public float duration;
        public AnimationCurve curve;
        public UnityEvent onStart;
        public UnityEvent onComplete;
    }

    public List<TransitionCamera> transitions;
    private CinemachineVirtualCamera startingCamera;

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
        startingCamera = CameraManager.ActiveCineCamera;
        StartCoroutine(StartTransitions());
    }

    private IEnumerator StartTransitions()
    {
        foreach (var trans in transitions)
        {
            trans.onStart.Invoke();
            CameraManager.CamBlend.BlendCurve = trans.curve;
            CameraManager.CamBlend.TimeInBlend = trans.duration;
            CameraManager.Instance.ChangeVirtualCamera(trans.cam);
            yield return new WaitForSecondsRealtime(trans.delay);
            trans.onComplete.Invoke();
        }
        InputManager.Instance.TogglePlayerControls(true);
    }
}
