using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// This is a duplicate of Notification to use without canavs
public class NotificationTMP : MonoBehaviour
{
    [Header("Notification")]
    public TextMeshPro tmp;
    public List<NotificationTransitions> transitions;
    public bool playOnStart = true;
    public bool noAlphaOnStart = false;
    public float transitionDuration = 1.0f;
    protected Vector2 startPosition;

    // For temporary in out transition
    private bool inOutActive = false;
    private float inOutTimer = 0.0f;
    private float inOutDuration = 1.0f;

    protected virtual void Start()
    {
        if (tmp == null) tmp = GetComponent<TextMeshPro>();
        Debug.Assert(tmp != null, "Sprite renderer is required");

        startPosition = tmp.transform.localPosition;
        Color clearColor = tmp.color;
        clearColor.a = 0;
        if (noAlphaOnStart) tmp.color = clearColor;
        if (playOnStart) Play();
    }

    protected virtual void Update()
    {
        if (inOutActive)
        {
            inOutTimer += Time.deltaTime;
            if (inOutTimer > transitionDuration+inOutDuration) {
                Play(true);
                inOutActive = false;
            }
        }
    }

    public void PlayInOut(float _duration)
    {
        inOutDuration = _duration;
        inOutTimer = 0.0f;
        if (!inOutActive)
        {
            inOutActive = true;
            Play();
        }
    }

    public void Play(bool reverse = false)
    {
        StopAllCoroutines();
        StartCoroutine(PlayNotification(reverse));
    }

    public void Stop()
    {
        StopAllCoroutines();
    }

    IEnumerator PlayNotification(bool reverse)
    {
        float timer = reverse ? transitionDuration : 0;

        while (reverse ? timer > 0 : timer < transitionDuration)
        {
            timer += reverse ? -Time.deltaTime : Time.deltaTime;
            float normalTime = timer / transitionDuration;
            foreach (var transition in transitions)
            {
                switch (transition)
                {
                    case NotificationTransitions.FADE:
                        Fade(normalTime);
                        break;
                    case NotificationTransitions.FLY_UP:
                        FlyUp(normalTime);
                        break;
                    case NotificationTransitions.SHAKE:
                        Shake(normalTime);
                        break;
                }
            }
            yield return null;
        }  
    }

    private void Fade(float time)
    {
        float t = Easing.EaseInOutCubic(time);
        Color newColor = tmp.color;
        newColor.a = t;
        tmp.color = newColor;
    }

    private void FlyUp(float time)
    {
        Vector2 offset = startPosition;
        offset.y -= 1;
        tmp.transform.localPosition = Vector3.LerpUnclamped(offset, startPosition, Easing.EaseOut(time));
    }

    private void Shake(float time)
    {
        float speed = 5 * Mathf.PI; // Adjust the speed of the shake
        float magnitude = 5f; // Adjust the magnitude of the shake

        float x = Mathf.Sin(time * speed) * magnitude;
        Vector2 offset = startPosition;
        offset.x += x;
        tmp.transform.position = Vector3.Lerp(offset, startPosition, Easing.EaseOut(time));
    }
}
