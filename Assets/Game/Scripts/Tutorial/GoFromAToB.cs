using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoFromAToB : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] float timeTaken = 3.0f;

    public UnityEvent startMoving;
    public UnityEvent callbackOnB;
    bool moving = false;

    private void Start()
    {
        moving = false;
    }

    public void MoveAToB()
    {
        if (moving)
            return;

        moving = true;
        startMoving.Invoke();
        StartCoroutine(InitiateMovement());
    }

    IEnumerator InitiateMovement()
    {
        float timeElapsed = 0;

        while (timeElapsed < timeTaken)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / timeTaken);
            float progress = Easing.EaseInCubic(t);

            transform.Translate(progress * (pointB.position - pointA.position));

            yield return null;
        }

        callbackOnB.Invoke();
    }
}
