using UnityEngine;

public class TimeScaler : MonoBehaviour
{
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
