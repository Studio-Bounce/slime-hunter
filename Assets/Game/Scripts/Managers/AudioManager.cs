using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioConfig config;

    private int enemiesAlerted = 0;
    private PARAMETER_ID combatIntensityParamID;
    private bool forceAlert = false;

    // Footsteps
    private EventInstance walkDirt;
    private EventInstance walkGrass;
    private EventInstance walkGravel;

    // Music
    private EventInstance menuInstance;
    private EventInstance explorationInstance;
    private EventInstance villageInstance;

    private Dictionary<string, EventInstance> soundEffectInstances = new Dictionary<string, EventInstance>();
    private Dictionary<string, EventInstance> uiSoundEffectInstances = new Dictionary<string, EventInstance>();

    public float CombatIntensity { get { return (float)enemiesAlerted / config.maxEnemyIntensity; } }
    public static AudioConfig Config => Instance.config;

    void Start()
    {
        menuInstance = RuntimeManager.CreateInstance(config.menuEvent);
        explorationInstance = RuntimeManager.CreateInstance(config.explorationEvent);
        villageInstance = RuntimeManager.CreateInstance(config.villageEvent);

        EventDescription eventDescription;
        explorationInstance.getDescription(out eventDescription);
        PARAMETER_DESCRIPTION parameterDescription;
        eventDescription.getParameterDescriptionByName("CombatIntensity", out parameterDescription);
        combatIntensityParamID = parameterDescription.id;

        GameManager.Instance.OnGameStateChange += HandleBGMusic;
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

    public void SetVolume(float value)
    {
        // Ensure volume is clamped between 0.0 and 1.0
        value = Mathf.Clamp01(value);

        // Retrieve the master bus
        Bus masterBus = RuntimeManager.GetBus("bus:/"); // "bus:/" is the path to the master bus

        // Set the volume on the master bus
        masterBus.setVolume(value);
    }

    public void ForceAlert(float value)
    {
        forceAlert = true;
        explorationInstance.setParameterByID(combatIntensityParamID, value);
    }

    public void ReleaseAlert()
    {
        forceAlert = false;
        explorationInstance.setParameterByID(combatIntensityParamID, CombatIntensity);
    }

    public void OnEnemyAlerted()
    {
        enemiesAlerted++;
        // FMOD will clamp intensity
        if (forceAlert) return;
        explorationInstance.setParameterByID(combatIntensityParamID, CombatIntensity);
    }

    public void OnEnemyUnalerted()
    {
        enemiesAlerted = Mathf.Max(enemiesAlerted-1, 0);
        // FMOD will clamp intensity
        if (forceAlert) return;
        explorationInstance.setParameterByID(combatIntensityParamID, CombatIntensity);
    }
}
