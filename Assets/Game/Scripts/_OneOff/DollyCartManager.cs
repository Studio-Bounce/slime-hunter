using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CinemachineDollyCart))]
public class DollyCartManager : MonoBehaviour
{
    [Header("Trigger Key")]
    public KeyCode startKey = KeyCode.None;

    public float maxPosition;
    public UnityEvent onStart;
    public UnityEvent onComplete;

    public float minSpeed = 1.0f;
    public float maxSpeed = 1.0f;
    public AnimationCurve curve;

    private CinemachineDollyCart dolly;
    private bool started = false;

    private void Start()
    {
        dolly = GetComponent<CinemachineDollyCart>();
        dolly.m_Speed = 0;
        dolly.m_Position = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(startKey)) StartDolly();
    }

    private void StartDolly()
    {
        onStart.Invoke();
        dolly.m_Speed = minSpeed;
        dolly.m_Position = 0;
        started = true;
    }

    private void FixedUpdate()
    {
        if (!started) return;

        float progress = dolly.m_Position / maxPosition;
        dolly.m_Speed = minSpeed + curve.Evaluate(progress)*(maxSpeed - minSpeed);

        if (dolly.m_Position >= maxPosition)
        {
            onComplete.Invoke();
            started = false;
        }
    }
}
