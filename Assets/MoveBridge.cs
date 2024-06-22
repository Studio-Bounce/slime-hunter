using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBridge : MonoBehaviour
{
    [SerializeField] Transform finalTransform;
    [SerializeField] float moveDuration = 2.0f;

    Transform initialTransform;

    // Start is called before the first frame update
    void Start()
    {
        initialTransform = transform;
    }

    public void MoveTheBridge()
    {
        StartCoroutine(LerpTransform());
    }

    IEnumerator LerpTransform()
    {
        float timeElapsed = 0f;

        Vector3 initialPosition = initialTransform.position;
        Quaternion initialRotation = initialTransform.rotation;
        Vector3 initialScale = initialTransform.localScale;

        while (timeElapsed < moveDuration)
        {
            timeElapsed += Time.deltaTime;

            // Calculate the lerp parameter based on the current timeElapsed
            float t = Mathf.Clamp01(timeElapsed / moveDuration);

            // Lerp position, rotation, and scale
            transform.position = Vector3.Lerp(initialPosition, finalTransform.position, t);
            transform.rotation = Quaternion.Lerp(initialRotation, finalTransform.rotation, t);
            transform.localScale = Vector3.Lerp(initialScale, finalTransform.localScale, t);

            yield return null;
        }

        transform.position = finalTransform.position;
        transform.rotation = finalTransform.rotation;
        transform.localScale = finalTransform.localScale;
    }
}
