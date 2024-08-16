using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBridge : MonoBehaviour
{
    [SerializeField] Transform finalTransform;
    [SerializeField] Transform rotateWheel;
    [SerializeField] float rotationSpeed = 5.0f;
    [SerializeField] float moveDuration = 2.0f;
    [SerializeField] Transform[] objToDestroy;

    Transform initialTransform;
    bool isItDown = false;

    // Start is called before the first frame update
    void Start()
    {
        initialTransform = transform;
        isItDown = false;
    }

    public void MoveTheBridgeWithDelay(float delay)
    {
        isItDown = false;
        StartCoroutine(MoveBridgeDelay(delay));
    }

    IEnumerator MoveBridgeDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        MoveTheBridge();
    }

    public void MoveTheBridge()
    {
        isItDown = false;
        StartCoroutine(LerpTransform());
        foreach (Transform _transform in objToDestroy)
        {
            Destroy(_transform.gameObject, 1.0f);
        }
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

            if (rotateWheel != null)
            {
                rotateWheel.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }

            yield return null;
        }

        transform.position = finalTransform.position;
        transform.rotation = finalTransform.rotation;
        transform.localScale = finalTransform.localScale;
        isItDown = true;
    }

    public bool IsBridgeDown()
    {
        return isItDown;
    }
}
