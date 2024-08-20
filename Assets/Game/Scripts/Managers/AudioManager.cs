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

    private bool forceAlert = false;

    // SFX
    public EventInstance SpecialAttackInstance { get; set; }

    // Music
    public EventInstance MenuInstance { get; set; }
    public EventInstance ExplorationInstance { get; set; }
    public EventInstance VillageInstance { get; set; }
    public EventInstance CreditsInstance { get; set; }
    public EventInstance DeathInstance { get; set; }

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
        VillageInstance.setParameterByName("VillagePhase", 3);
        CreditsInstance = RuntimeManager.CreateInstance(config.villageEvent);
        CreditsInstance.setParameterByName("VillagePhase", 2);
        DeathInstance = RuntimeManager.CreateInstance(config.villageEvent);

        // SFX
        SpecialAttackInstance = RuntimeManager.CreateInstance(config.specialAttack);
        InventoryManager.Instance.OnItemAdded += e => RuntimeManager.PlayOneShot(Config.itemPickup);
        InputManager.Instance.exitEvent += () => RuntimeManager.PlayOneShot(Config.errorEvent);
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
                ReleaseAlert();
                MenuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                DeathInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
                DeathInstance.start();
                break;
            default:
                break;
        }
    }

    public void CreditsMusic()
    {
        CreditsInstance.start();
        ExplorationInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        VillageInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ExploreToVillage()
    {
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
        ExplorationInstance.setParameterByName("CombatIntensity", value);
    }

    public void ReleaseAlert()
    {
        forceAlert = false;
        ExplorationInstance.setParameterByName("CombatIntensity", CombatIntensity);
    }

    public void OnEnemyAlerted()
    {
        enemiesAlerted++;
        // FMOD will clamp intensity
        if (forceAlert) return;
        ExplorationInstance.setParameterByName("CombatIntensity", CombatIntensity);
    }

    public void OnEnemyUnalerted()
    {
        enemiesAlerted = Mathf.Max(enemiesAlerted-1, 0);
        // FMOD will clamp intensity
        if (forceAlert) return;
        ExplorationInstance.setParameterByName("CombatIntensity", CombatIntensity);
    }
}
