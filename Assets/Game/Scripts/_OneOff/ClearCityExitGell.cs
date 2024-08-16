using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClearCityExitGell : MonoBehaviour
{
    public float timeTaken = 5.0f;
    public float moveDist = 5.0f;
    public UnityEvent OnMoveComplete;

    bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.gameObject.layer == GameConstants.PlayerLayer)
        {
            triggered = true;
            StartCoroutine(MoveDown());
        }
    }

    IEnumerator MoveDown()
    {
        float timeElapsed = 0f;
        float vel = moveDist / timeTaken;
        while (timeElapsed < timeTaken)
        {
            timeElapsed += Time.deltaTime;
            transform.Translate(vel * Time.deltaTime * Vector3.forward);

            yield return null;
        }
        OnMoveComplete.Invoke();
        Destroy(gameObject);
    }
}
