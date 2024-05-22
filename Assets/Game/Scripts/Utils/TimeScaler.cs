using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    [Range(0.0f, 5.0f)]
    public float timeScale = 1f;

    void Update()
    {
        // Apply the time scale
        Time.timeScale = timeScale;
    }

    void OnDisable()
    {
        // Reset time scale to normal speed when this script is disabled or destroyed
        Time.timeScale = 1f;
    }
}
