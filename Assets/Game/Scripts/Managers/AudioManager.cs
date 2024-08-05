using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public EventReference menuEvent;
    public EventReference explorationEvent;
    public EventReference villageEvent;

    public float CombatIntensity { get; set; } = 0.0f;
    public int maxEnemyIntensity = 3;
    private int enemiesAlerted = 0;
    private PARAMETER_ID combatIntensityParamID;

    private EventInstance menuInstance;
    private EventInstance explorationInstance;
    private EventInstance villageInstance;

    private Dictionary<string, EventInstance> soundEffectInstances = new Dictionary<string, EventInstance>();
    private Dictionary<string, EventInstance> uiSoundEffectInstances = new Dictionary<string, EventInstance>();

    private float combatIntensity;
    private string currentAreaType;

    void Start()
    {
        menuInstance = RuntimeManager.CreateInstance(menuEvent);
        explorationInstance = RuntimeManager.CreateInstance(explorationEvent);
        villageInstance = RuntimeManager.CreateInstance(villageEvent);

        EventDescription eventDescription;
        explorationInstance.getDescription(out eventDescription);
        PARAMETER_DESCRIPTION parameterDescription;
        eventDescription.getParameterDescriptionByName("CombatIntensity", out parameterDescription);
        combatIntensityParamID = parameterDescription.id;

        GameManager.Instance.OnGameStateChange += HandleBGMusic;
    }

    private void Update()
    {
        float val;
        explorationInstance.getParameterByID(combatIntensityParamID, out val);
        Debug.Log($"Desired {enemiesAlerted / maxEnemyIntensity} : Actual {val}");
    }

    private void HandleBGMusic(GameState state)
    {
        switch (state)
        {
            case GameState.MAIN_MENU:
                menuInstance.start();
                break;
            case GameState.GAMEPLAY:
                menuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                explorationInstance.start();
                //villageInstance.start();
                break;
            case GameState.LOADING:
                break;
            case GameState.GAME_OVER:
                break;
            default:
                break;
        }
    }

    public void OnEnemyAlerted()
    {
        enemiesAlerted++;
        // FMOD will clamp intensity
        explorationInstance.setParameterByID(combatIntensityParamID, (float)enemiesAlerted / maxEnemyIntensity);
    }

    public void OnEnemyUnalerted()
    {
        enemiesAlerted--;
        // FMOD will clamp intensity
        explorationInstance.setParameterByID(combatIntensityParamID, (float)enemiesAlerted / maxEnemyIntensity);

    }
}
