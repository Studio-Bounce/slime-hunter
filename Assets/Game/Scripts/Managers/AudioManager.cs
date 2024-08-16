using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioConfig config;

    private Bus masterBus;
    private int enemiesAlerted = 0;
    private PARAMETER_ID combatIntensityParamID;
    private PARAMETER_ID villagePhaseParamID;
    private bool forceAlert = false;

    // SFX
    public EventInstance SpecialAttackInstance { get; set; }

    // Music
    public EventInstance MenuInstance { get; set; }
    public EventInstance ExplorationInstance { get; set; }
    public EventInstance VillageInstance { get; set; }

    private Dictionary<string, EventInstance> soundEffectInstances = new Dictionary<string, EventInstance>();
    private Dictionary<string, EventInstance> uiSoundEffectInstances = new Dictionary<string, EventInstance>();

  
    public float CombatIntensity { get { return (config != null) ? (float)enemiesAlerted / config.maxEnemyIntensity : 0f; } }
    public static AudioConfig Config => Instance.config;

    void Start()
    {
        // Retrieve the master bus
        masterBus = RuntimeManager.GetBus("bus:/"); // "bus:/" is the path to the master bus

        // Music
        MenuInstance = RuntimeManager.CreateInstance(config.menuEvent);
        ExplorationInstance = RuntimeManager.CreateInstance(config.explorationEvent);
        VillageInstance = RuntimeManager.CreateInstance(config.villageEvent);

        // SFX
        SpecialAttackInstance = RuntimeManager.CreateInstance(config.specialAttack);
        InventoryManager.Instance.OnItemAdded += e => RuntimeManager.PlayOneShot(Config.itemPickup);
        InputManager.Instance.exitEvent += () => RuntimeManager.PlayOneShot(Config.errorEvent);

        // Param IDs
        EventDescription eventDescription;
        ExplorationInstance.getDescription(out eventDescription);
        PARAMETER_DESCRIPTION parameterDescription;
        eventDescription.getParameterDescriptionByName("CombatIntensity", out parameterDescription);
        combatIntensityParamID = parameterDescription.id;

        VillageInstance.getDescription(out eventDescription);
        eventDescription.getParameterDescriptionByName("VillagePhase", out parameterDescription);
        villagePhaseParamID = parameterDescription.id;

        GameManager.Instance.OnGameStateChange += HandleBGMusic;
    }

    private void HandleBGMusic(GameState state)
    {
        switch (state)
        {
            case GameState.MAIN_MENU:
                masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                MenuInstance.start();
                break;
            case GameState.GAMEPLAY:
                MenuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                PLAYBACK_STATE pbState;
                ExplorationInstance.getPlaybackState(out pbState);
                if (pbState != PLAYBACK_STATE.PLAYING)
                    ExplorationInstance.start();
                break;
            case GameState.PAUSED:
                break;
            case GameState.LOADING:
                break;
            case GameState.GAME_OVER:
                masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                VillageInstance.setParameterByID(villagePhaseParamID, 1);
                VillageInstance.start();
                break;
            default:
                break;
        }
    }

    public void ExploreToVillage()
    {
        VillageInstance.setParameterByID(villagePhaseParamID, 3);
        PLAYBACK_STATE pbState;
        VillageInstance.getPlaybackState(out pbState);
        if (pbState != PLAYBACK_STATE.PLAYING)
            VillageInstance.start();
        ExplorationInstance.setParameterByName("Fade", 1);
        VillageInstance.setParameterByName("Fade", 0);
    }

    public void VillageToExplore()
    {
        ExplorationInstance.setParameterByName("Fade", 0);
        VillageInstance.setParameterByName("Fade", 1);
    }

    public void SetVolume(float value)
    {
        // Ensure volume is clamped between 0.0 and 1.0
        value = Mathf.Clamp01(value);

        // Set the volume on the master bus
        masterBus.setVolume(value);
    }

    public void ForceAlert(float value)
    {
        forceAlert = true;
        ExplorationInstance.setParameterByID(combatIntensityParamID, value);
    }

    public void ReleaseAlert()
    {
        forceAlert = false;
        ExplorationInstance.setParameterByID(combatIntensityParamID, CombatIntensity);
    }

    public void OnEnemyAlerted()
    {
        enemiesAlerted++;
        // FMOD will clamp intensity
        if (forceAlert) return;
        ExplorationInstance.setParameterByID(combatIntensityParamID, CombatIntensity);
    }

    public void OnEnemyUnalerted()
    {
        enemiesAlerted = Mathf.Max(enemiesAlerted-1, 0);
        // FMOD will clamp intensity
        if (forceAlert) return;
        ExplorationInstance.setParameterByID(combatIntensityParamID, CombatIntensity);
    }
}
