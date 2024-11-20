using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FaRUtils.FPSController;
using FaRUtils.Systems.DateTime;
using System.Collections.Generic;

[Serializable]
public class OptionsData
{
    public float fov = 75f;
    public float sensitivity = 0.05f;
    public int targetFPS = 60;
    public bool timeFormat12Hour = true;
    public bool isFullscreen = true;
    public int resolutionIndex = 0;
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
    public bool doce;
    public bool isOptionsMenuOpen;

    public Slider fovSlider;
    public Slider sensSlider;

    private Resolution[] resolutions;
    private OptionsData optionsData;
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
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = optionsData.resolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void LoadOptions()
    {
        string jsonData = PlayerPrefs.GetString(OPTIONS_KEY, "");
        optionsData = string.IsNullOrEmpty(jsonData) ? 
            new OptionsData() : 
            JsonUtility.FromJson<OptionsData>(jsonData);
    }

    private void SaveOptions()
    {
        optionsData.fov = fovSlider.value;
        optionsData.sensitivity = sensSlider.value;
        optionsData.targetFPS = FPSLimit.target;
        optionsData.timeFormat12Hour = doce;
        optionsData.isFullscreen = Screen.fullScreen;
        optionsData.resolutionIndex = resolutionDropdown.value;

        string jsonData = JsonUtility.ToJson(optionsData);
        PlayerPrefs.SetString(OPTIONS_KEY, jsonData);
        PlayerPrefs.Save();
    }
    private void ApplyLoadedOptions()
    {
        fovSlider.value = optionsData.fov;
        sensSlider.value = optionsData.sensitivity;
        FPSLimit.target = optionsData.targetFPS;
        doce = optionsData.timeFormat12Hour;
        Screen.fullScreen = optionsData.isFullscreen;
    
        UpdateUIValues();
        RelojUI.GetComponent<ClockManager>().Time.text = doce ? 
            TimeManager.DateTime.TimeToString12() : 
            TimeManager.DateTime.TimeToString24();
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
        SensVal.text = optionsData.sensitivity.ToString("F3");
        FPSText.text = $"FPS: {optionsData.targetFPS}";
    }

    private void Update()
    {
        FaRCharacterController.instance.lookSensitivity = sensSlider.value;
        UpdateUIValues();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        SaveOptions();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        SaveOptions();
    }

    public void FPS()
    {
        switch (FPSLimit.target)
        {
            case 30:
                FPSLimit.target = 60;
                break;
            case 60:
                FPSLimit.target = 120;
                break;
            case 120:
                FPSLimit.target = 144;
                break;
            case 144:
                FPSLimit.target = 300;
                break;
            default:
                FPSLimit.target = 30;
                break;
        }
        FPSText.text = $"FPS: {FPSLimit.target}";
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

    public void ResetSensButton()
    {
        sensSlider.value = 0.05f;
        SaveOptions();
    }

    private void OnDisable()
    {
        SaveOptions();
    }
}