using UnityEngine;
using UnityEngine.UIElements;
using System;
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
    private Label backBtn;

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
        VisualElement root = uiDocument.rootVisualElement;
        resolutionDropdown = root.Q<DropdownField>("ResolutionDropdown");
        displayModeDropdown = root.Q<DropdownField>("DisplayModeDropdown");
        qualityDropdown = root.Q<DropdownField>("QualityDropdown");
        volumeSlider = root.Q<Slider>("VolumeSlider");
        backBtn = root.Q<Label>("Back");

        // Set the choices for dropdowns
        resolutionDropdown.choices = resolutions;
        displayModeDropdown.choices = displayModes;
        qualityDropdown.choices = new List<string>(QualitySettings.names);

        // Set default values based on current settings
        SetDefaultDropdownValues();

        // Register event handlers
        resolutionDropdown.RegisterValueChangedCallback(evt =>
        {
            string selectedResolution = evt.newValue;
            ApplyResolution(selectedResolution);
        });

        displayModeDropdown.RegisterValueChangedCallback(evt =>
        {
            string selectedMode = evt.newValue;
            ApplyDisplayMode(selectedMode);
        });

        qualityDropdown.RegisterValueChangedCallback(evt =>
        {
            string selectedQuality = evt.newValue;
            ApplyQualitySetting(selectedQuality);
        });

        backBtn.RegisterCallback<ClickEvent>(ev => Back());
    }

    private void SetDefaultDropdownValues()
    {
        // Set resolution dropdown to current resolution
        string currentResolution = $"{Screen.currentResolution.width} x {Screen.currentResolution.height}";
        if (!resolutions.Contains(currentResolution)) resolutions.Add(currentResolution);
        resolutionDropdown.value = currentResolution;


        // Set display mode dropdown to current display mode
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.FullScreenWindow:
                displayModeDropdown.value = "Fullscreen";
                break;
            case FullScreenMode.Windowed:
                displayModeDropdown.value = "Windowed";
                break;
            case FullScreenMode.MaximizedWindow:
                displayModeDropdown.value = "Borderless Window";
                break;
        }

        // Set quality dropdown to current quality setting
        qualityDropdown.value = QualitySettings.names[QualitySettings.GetQualityLevel()];
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

    private void Back()
    {
        Hide();
        UIManager.Instance.mainMenu.Show();
    }
}
