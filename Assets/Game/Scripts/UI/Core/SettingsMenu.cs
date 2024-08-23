using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SettingsMenu : Menu
{
    private DropdownField resolutionDropdown;
    private DropdownField displayModeDropdown;
    private DropdownField qualityDropdown;
    private Slider volumeSlider;
    private Button backBtn;

    List<string> resolutions = new List<string>
    {
        "3840 x 2160",
        "3440 x 1440",
        "2560 x 1440",
        "1920 x 1080",
        "1600 x 900",
        "1366 x 768",
        "1280 x 720"
    };

    List<string> displayModes = new List<string>
    {
        "Fullscreen",
        "Windowed",
        "Borderless Window"
    };

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        resolutionDropdown = root.Q<DropdownField>("ResolutionDropdown");
        displayModeDropdown = root.Q<DropdownField>("DisplayModeDropdown");
        qualityDropdown = root.Q<DropdownField>("QualityDropdown");
        volumeSlider = root.Q<Slider>("VolumeSlider");
        backBtn = root.Q<Button>("BackBtn");

        // Set the choices for dropdowns
        resolutionDropdown.choices = resolutions;
        displayModeDropdown.choices = displayModes;
        qualityDropdown.choices = new List<string>(QualitySettings.names);

        // Load saved settings or set default values
        LoadSettings();

        // Register event handlers
        resolutionDropdown.RegisterValueChangedCallback(evt =>
        {
            string selectedResolution = evt.newValue;
            ApplyResolution(selectedResolution);
            PlayerPrefs.SetString("Resolution", selectedResolution);
        });

        displayModeDropdown.RegisterValueChangedCallback(evt =>
        {
            string selectedMode = evt.newValue;
            ApplyDisplayMode(selectedMode);
            PlayerPrefs.SetString("DisplayMode", selectedMode);
        });

        qualityDropdown.RegisterValueChangedCallback(evt =>
        {
            string selectedQuality = evt.newValue;
            ApplyQualitySetting(selectedQuality);
            PlayerPrefs.SetString("Quality", selectedQuality);
        });

        volumeSlider.RegisterValueChangedCallback(evt =>
        {
            float volume = evt.newValue;
            ApplyVolumeSetting(volume);
            PlayerPrefs.SetFloat("Volume", volume);
        });

        backBtn.RegisterCallback<ClickEvent>(ev => Back());
    }

    private void LoadSettings()
    {
        // Load resolution
        string savedResolution = PlayerPrefs.GetString("Resolution", $"{Screen.currentResolution.width} x {Screen.currentResolution.height}");
        if (!resolutions.Contains(savedResolution)) resolutions.Add(savedResolution);
        resolutionDropdown.value = savedResolution;

        // Load display mode
        string savedDisplayMode = PlayerPrefs.GetString("DisplayMode", GetDefaultDisplayMode());
        displayModeDropdown.value = savedDisplayMode;

        // Load quality setting
        string savedQuality = PlayerPrefs.GetString("Quality", QualitySettings.names[QualitySettings.GetQualityLevel()]);
        qualityDropdown.value = savedQuality;

        // Load volume setting
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1.0f);
        volumeSlider.value = savedVolume;

        ApplyResolution(savedResolution);
        ApplyDisplayMode(savedDisplayMode);
        ApplyQualitySetting(savedQuality);
        ApplyVolumeSetting(savedVolume);
    }

    private string GetDefaultDisplayMode()
    {
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.FullScreenWindow:
                return "Fullscreen";
            case FullScreenMode.Windowed:
                return "Windowed";
            case FullScreenMode.MaximizedWindow:
                return "Borderless Window";
            default:
                return "Windowed";
        }
    }

    private void ApplyResolution(string resolution)
    {
        string[] parts = resolution.Split('x');
        if (parts.Length == 2)
        {
            int width = int.Parse(parts[0].Trim());
            int height = int.Parse(parts[1].Trim());
            Screen.SetResolution(width, height, Screen.fullScreenMode);
        }
    }

    private void ApplyDisplayMode(string mode)
    {
        switch (mode)
        {
            case "Fullscreen":
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case "Windowed":
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case "Borderless Window":
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
        }
    }

    private void ApplyQualitySetting(string quality)
    {
        int qualityIndex = QualitySettings.names.ToList().IndexOf(quality);
        if (qualityIndex >= 0)
        {
            QualitySettings.SetQualityLevel(qualityIndex, true);
        }
    }

    private void ApplyVolumeSetting(float volume)
    {
        AudioManager.Instance.SetVolume(volume);
    }

    private void Back()
    {
        Hide();
        if (GameManager.Instance.GameState == GameState.MAIN_MENU)
        {
            UIManager.Instance.mainMenu.Show();
        } else
        {
            UIManager.Instance.pauseMenu.Show();
        }
    }

    public override void Show()
    {
        base.Show();
        InputManager.Instance.exitEvent += Back;
    }

    public override void Hide()
    {
        base.Hide();
        InputManager.Instance.exitEvent -= Back;
    }
}
