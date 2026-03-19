using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FaRUtils.FPSController;
using FaRUtils.Systems.DateTime;
using System.Collections.Generic;
using UnityEngine.Audio;

[Serializable]
public class OptionsData
{
    public float fov = 75f;
    public float sensitivity = 0.05f;
    public int targetFPS = 300;
    public bool timeFormat12Hour = true;
    public bool isFullscreen = true;
    public int windowMode = 0; // 0: Fullscreen, 1: FullscreenWindow, 2: Windowed
    public int resolutionIndex = 0;

    public float masterVolume = 1.0f;
    public float musicVolume = 0.8f;
    public float sfxVolume = 1.0f;
    public float uiVolume = 1.0f;
    public bool muteWhenInBackground = true;
}

public class OptionsMenu : MonoBehaviour
{
    public static OptionsMenu Instance;
    public GameObject RelojUI;
    public FaRUtils.Systems.DateTime.DateTime dateTime;
    public FPSLimit FPSLimit;
    public TextMeshProUGUI FovVal;
    public TextMeshProUGUI SensVal;
    public TextMeshProUGUI FPSText;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown windowModeDropdown;
    public bool doce;
    public bool isOptionsMenuOpen;

    public Slider fovSlider;
    public Slider sensSlider;

    [Header("Sensitivity Settings")]
    public float minSensitivity = 0.01f;
    public float maxSensitivity = 0.2f;

    private Resolution[] resolutions;
    private OptionsData optionsData;

    [Header("Sound Options")]
    public AudioMixer audioMixer;
    public GameObject soundOptionsMenuUI;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider uiVolumeSlider;
    public Toggle muteBackgroundToggle;
    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    public TextMeshProUGUI uiVolumeText;
    private const string OPTIONS_KEY = "GameOptions";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        LoadOptions();
    }

    private void Start()
    {
        InitializeResolutions();
        ApplyLoadedOptions();

        fovSlider.onValueChanged.AddListener(OnFOVValueChanged);
    }

    private void InitializeResolutions()
    {
        Resolution[] allResolutions = Screen.resolutions;
        List<Resolution> filteredResolutions = new List<Resolution>();
        HashSet<string> seenResolutions = new HashSet<string>();

        float targetAspect = (float)Screen.width / Screen.height;
        float epsilon = 0.01f;

        for (int i = allResolutions.Length - 1; i >= 0; i--)
        {
            float aspect = (float)allResolutions[i].width / allResolutions[i].height;
            if (Mathf.Abs(aspect - targetAspect) > epsilon) continue;

            string key = $"{allResolutions[i].width}x{allResolutions[i].height}";
            if (!seenResolutions.Contains(key))
            {
                filteredResolutions.Add(allResolutions[i]);
                seenResolutions.Add(key);
            }
        }
        
        if (filteredResolutions.Count == 0)
        {
            for (int i = allResolutions.Length - 1; i >= 0; i--)
            {
                string key = $"{allResolutions[i].width}x{allResolutions[i].height}";
                if (!seenResolutions.Contains(key))
                {
                    filteredResolutions.Add(allResolutions[i]);
                    seenResolutions.Add(key);
                }
            }
        }

        filteredResolutions.Reverse();
        resolutions = filteredResolutions.ToArray();

        resolutionDropdown.ClearOptions();
        var options = new List<string>();
        int currentResolutionIndex = -1;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.width && 
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        
        if (optionsData.resolutionIndex >= 0 && optionsData.resolutionIndex < resolutions.Length)
        {
            resolutionDropdown.value = optionsData.resolutionIndex;
        }
        else if (currentResolutionIndex != -1)
        {
            resolutionDropdown.value = currentResolutionIndex;
        }

        resolutionDropdown.RefreshShownValue();

        if (windowModeDropdown != null)
        {
            windowModeDropdown.ClearOptions();
            windowModeDropdown.AddOptions(new List<string> { "Fullscreen", "Borderless", "Windowed" });
            windowModeDropdown.value = optionsData.windowMode;
            windowModeDropdown.RefreshShownValue();
        }
    }

    private void LoadOptions()
    {
        string jsonData = PlayerPrefs.GetString(OPTIONS_KEY, "");
        optionsData = string.IsNullOrEmpty(jsonData) ? 
            new OptionsData() : 
            JsonUtility.FromJson<OptionsData>(jsonData);
    }

    private void ApplyLoadedOptions()
    {
        fovSlider.value = optionsData.fov;
        sensSlider.value = optionsData.sensitivity;
        FPSLimit.target = optionsData.targetFPS;
        doce = optionsData.timeFormat12Hour;
        
        InitializeResolutions(); 
        ApplyGraphicsSettings();

        masterVolumeSlider.value = optionsData.masterVolume;
        musicVolumeSlider.value = optionsData.musicVolume;
        sfxVolumeSlider.value = optionsData.sfxVolume;
        uiVolumeSlider.value = optionsData.uiVolume;
        muteBackgroundToggle.isOn = optionsData.muteWhenInBackground;
    
        UpdateUIValues();
        RelojUI.GetComponent<ClockManager>().Time.text = doce ? 
            TimeManager.DateTime.TimeToString12() : 
            TimeManager.DateTime.TimeToString24();
    }

    private void SaveOptions()
    {
        optionsData.fov = fovSlider.value;
        optionsData.sensitivity = sensSlider.value;
        optionsData.targetFPS = FPSLimit.target;
        optionsData.timeFormat12Hour = doce;
        optionsData.windowMode = windowModeDropdown != null ? windowModeDropdown.value : 0;
        optionsData.isFullscreen = optionsData.windowMode == 0;
        optionsData.resolutionIndex = resolutionDropdown.value;

        optionsData.masterVolume = masterVolumeSlider.value;
        optionsData.musicVolume = musicVolumeSlider.value;
        optionsData.sfxVolume = sfxVolumeSlider.value;
        optionsData.uiVolume = uiVolumeSlider.value;
        optionsData.muteWhenInBackground = muteBackgroundToggle.isOn;

        string jsonData = JsonUtility.ToJson(optionsData);
        PlayerPrefs.SetString(OPTIONS_KEY, jsonData);
        PlayerPrefs.Save();
        ApplySoundSettings();
    }

    public void Clock()
    {
        doce = !doce;
        RelojUI.GetComponent<ClockManager>().Time.text = doce ? 
            TimeManager.DateTime.TimeToString12() : 
            TimeManager.DateTime.TimeToString24();
        SaveOptions();
    }

    private void UpdateUIValues()
    {
        FovVal.text = fovSlider.value.ToString("F1"); 
        SensVal.text = sensSlider.value.ToString();
        FPSText.text = $"FPS: {optionsData.targetFPS}";
    }

    private void Update()
    {
        // TODO: Sacar todo de update
        float mappedSensitivity = Mathf.Lerp(minSensitivity, maxSensitivity, sensSlider.normalizedValue);
        FaRCharacterController.instance.lookSensitivity = mappedSensitivity;
        UpdateUIValues(); 
    }

    public void ApplyGraphicsSettings()
    {
        if (resolutions == null || resolutionDropdown.value < 0 || resolutionDropdown.value >= resolutions.Length)
        {
            Debug.LogError("[GraphicsSettings] Invalid resolution index selected.");
            return;
        }

        Resolution resolution = resolutions[resolutionDropdown.value];
        FullScreenMode mode = GetFullScreenMode(windowModeDropdown.value);
        
        Debug.Log($"[GraphicsSettings] User Apply Pressed: {resolution.width}x{resolution.height} @ {mode}");
        
        optionsData.resolutionIndex = resolutionDropdown.value;
        optionsData.windowMode = windowModeDropdown.value;
        
        Screen.SetResolution(resolution.width, resolution.height, mode);
        SaveOptions();
    }

    public void SetResolution(int resolutionIndex)
    {
        optionsData.resolutionIndex = resolutionIndex;
        Debug.Log($"[GraphicsSettings] Selected Resolution Index: {resolutionIndex}");
    }

    public void SetWindowMode(int index)
    {
        optionsData.windowMode = index;
        Debug.Log($"[GraphicsSettings] Selected Window Mode Index: {index}");
    }

    private FullScreenMode GetFullScreenMode(int index)
    {
        return index switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            1 => FullScreenMode.FullScreenWindow,
            2 => FullScreenMode.Windowed,
            _ => FullScreenMode.ExclusiveFullScreen
        };
    }

    public void SetFullscreen(bool isFullscreen)
    {
        optionsData.windowMode = isFullscreen ? 0 : 2;
        Screen.fullScreenMode = GetFullScreenMode(optionsData.windowMode);
        SaveOptions();
    }

    public void FPS()
    {
        int newTarget = FPSLimit.target switch
        {
            30 => 60,
            60 => 120,
            120 => 144,
            144 => 300,
            _ => 30
        };
        
        FPSLimit.SetFrameRateTarget(newTarget);
        SaveOptions();
    }

    public void CancelFovButton()
    {
        fovSlider.value = 75f;
        SaveOptions();
    }

    private void OnFOVValueChanged(float value)
    {
        FovVal.text = value.ToString("F1");
        Camera.main.fieldOfView = value;
    }

    public void OnMasterVolumeChanged(float value)
    {
        optionsData.masterVolume = value;
        UpdateUIValues();
        
        ApplySoundSettings();
    }

    public void OnMusicVolumeChanged(float value)
    {
        optionsData.musicVolume = value;
        UpdateUIValues();
        
        ApplySoundSettings();
    }

    public void OnSFXVolumeChanged(float value)
    {
        optionsData.sfxVolume = value;
        UpdateUIValues();
        
        ApplySoundSettings();
    }

    public void OnUIVolumeChanged(float value)
    {
        optionsData.uiVolume = value;
        UpdateUIValues();

        ApplySoundSettings();
    }


    public void OnMuteBackgroundChanged(bool value)
    {
        optionsData.muteWhenInBackground = value;
        ApplySoundSettings();
    }

    private void ApplySoundSettings()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ApplyMixerVolumes();
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", ConvertToDecibel(optionsData.masterVolume));
            audioMixer.SetFloat("MusicVolume", ConvertToDecibel(optionsData.musicVolume));
            audioMixer.SetFloat("SFXVolume", ConvertToDecibel(optionsData.sfxVolume));
            audioMixer.SetFloat("UIVolume", ConvertToDecibel(optionsData.uiVolume));
        }    
    }

    private float ConvertToDecibel(float sliderValue)
    {
        if (sliderValue <= 0.0001f)
            return -80f;
            
        return Mathf.Log10(sliderValue) * 20f;
    }

    public void ResetSoundSettings()
    {
        masterVolumeSlider.value = 1.0f;
        musicVolumeSlider.value = 0.8f;
        sfxVolumeSlider.value = 1.0f;
        uiVolumeSlider.value = 1.0f;
        muteBackgroundToggle.isOn = true;
        ApplySoundSettings();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (optionsData.muteWhenInBackground)
        {
            AudioListener.pause = !hasFocus;
        }
    }

    public void ResetSensButton()
    {
        sensSlider.value = 0.25f;
        SaveOptions();
    }

    private void OnDisable()
    {
        SaveOptions();
    }

    public void ResetAllSettings()
    {
        Debug.Log("[GraphicsSettings] Reverting all settings to defaults");
        optionsData.fov = 75f;
        optionsData.sensitivity = 0.05f;
        optionsData.targetFPS = 300;
        optionsData.timeFormat12Hour = true;
        optionsData.windowMode = 0; // Fullscreen
        optionsData.masterVolume = 1.0f;
        optionsData.musicVolume = 0.8f;
        optionsData.sfxVolume = 1.0f;
        optionsData.uiVolume = 1.0f;
        optionsData.muteWhenInBackground = true;

        if (resolutions != null && resolutions.Length > 0)
        {
            optionsData.resolutionIndex = resolutions.Length - 1;
        }

        fovSlider.value = optionsData.fov;
        sensSlider.value = optionsData.sensitivity;
        FPSLimit.target = optionsData.targetFPS;
        FPSText.text = "FPS: " + optionsData.targetFPS;
        doce = optionsData.timeFormat12Hour;
        windowModeDropdown.value = optionsData.windowMode;
        resolutionDropdown.value = optionsData.resolutionIndex;

        masterVolumeSlider.value = optionsData.masterVolume;
        musicVolumeSlider.value = optionsData.musicVolume;
        sfxVolumeSlider.value = optionsData.sfxVolume;
        uiVolumeSlider.value = optionsData.uiVolume;
        muteBackgroundToggle.isOn = optionsData.muteWhenInBackground;

        ApplyGraphicsSettings();
        SaveOptions();
    }
}