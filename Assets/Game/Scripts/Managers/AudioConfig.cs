using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Audio/Audio Settings")]
public class AudioConfig : ScriptableObject
{
    [Header("Music")]
    public EventReference menuEvent;
    public EventReference explorationEvent;
    public EventReference villageEvent;

    [Header("Combat Transitions")]
    public int maxEnemyIntensity = 3;

    [Header("UI Sound Effects")]
    public EventReference buttonPressEvent;
    public EventReference hoverEvent;
    public EventReference sliderChangeEvent;
    public EventReference windowOpenEvent;
    public EventReference windowCloseEvent;
    public EventReference startGameEvent;
    public EventReference exitGameEvent;
    public EventReference errorEvent;
}