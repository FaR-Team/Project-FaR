using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FaRUtils.FPSController;
using FaRUtils.Systems.DateTime;

public class OptionsMenu : MonoBehaviour
{
    public static OptionsMenu Instance;
    public GameObject RelojUI;
    public DateTime dateTime;
    public FPSLimit FPSLimit;
    public TextMeshProUGUI FovVal;
    public TextMeshProUGUI SensVal;
    public TextMeshProUGUI FPSText;
    public TMP_Dropdown resolutionDropdown;
    public bool doce;
    public bool isOptionsMenuOpen;

    public Slider fovSlider;
    public Slider sensSlider;

    Resolution[] resolutions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();


        if (PlayerPrefs.GetFloat("FovVal") == 0)
        {
            FovVal.text = fovSlider.value.ToString();
            fovSlider.value = Camera.main.fieldOfView;
            PlayerPrefs.SetFloat("FovVal", Camera.main.fieldOfView);
        }
        else
        {
            FovVal.text = PlayerPrefs.GetFloat("FovVal").ToString();
            fovSlider.value = PlayerPrefs.GetFloat("FovVal");
        }

        if (PlayerPrefs.GetFloat("SensVal") == 0)
        {
            SensVal.text = FaRCharacterController.instance.lookSensitivity.ToString(); ;
            sensSlider.value = FaRCharacterController.instance.lookSensitivity;
            PlayerPrefs.SetFloat("SensVal", FaRCharacterController.instance.lookSensitivity);
        }
        else
        {
            SensVal.text = PlayerPrefs.GetFloat("SensVal").ToString();
            sensSlider.value = PlayerPrefs.GetFloat("SensVal");
        }

        if (FPSLimit.target == 0)
        {
            FPSLimit.target = 60;
            PlayerPrefs.SetInt("TargetaFPS", 60);
        }

        FPSLimit.target = PlayerPrefs.GetInt("TargetaFPS");
        FPSText.text = "FPS: " + FPSLimit.target;
    }
    public void Update()
    {
        FovVal.text = fovSlider.value.ToString();
        Camera.main.fieldOfView = fovSlider.value;
        PlayerPrefs.SetFloat("FovVal", Camera.main.fieldOfView);

        SensVal.text = FaRCharacterController.instance.lookSensitivity.ToString();
        FaRCharacterController.instance.lookSensitivity = sensSlider.value;
        PlayerPrefs.SetFloat("SensVal", FaRCharacterController.instance.lookSensitivity);
    }

    public void clock()
    {
        if (doce)
        {
            doce = false;
            RelojUI.GetComponent<ClockManager>().Time.text = dateTime.TimeToString12();
        }
        else
        {
            doce = true;
            RelojUI.GetComponent<ClockManager>().Time.text = dateTime.TimeToString24();
        }
    }

    public void FPS()
    {
        if (FPSLimit.target == 30)
        {
            FPSLimit.target = 60;
            FPSText.text = "FPS: " + FPSLimit.target;
            PlayerPrefs.SetInt("TargetaFPS", 60);
        }
        else if (FPSLimit.target == 60)
        {
            FPSLimit.target = 120;
            FPSText.text = "FPS: " + FPSLimit.target;
            PlayerPrefs.SetInt("TargetaFPS", 120);
        }
        else if (FPSLimit.target == 120)
        {
            FPSLimit.target = 144;
            FPSText.text = "FPS: " + FPSLimit.target;
            PlayerPrefs.SetInt("TargetaFPS", 144);
        }
        else if (FPSLimit.target == 144)
        {
            FPSLimit.target = 300;
            FPSText.text = "FPS: " + FPSLimit.target;
            PlayerPrefs.SetInt("TargetaFPS", 300);
        }
        else if (FPSLimit.target == 300)
        {
            FPSLimit.target = 30;
            FPSText.text = "FPS: " + FPSLimit.target;
            PlayerPrefs.SetInt("TargetaFPS", 30);
        }
    }

    public void CancelFovButton()
    {
        fovSlider.value = 75;
        PlayerPrefs.SetFloat("FovVal", 75);
    }

    public void ResetSensButton()
    {
        sensSlider.value = 0.05f;
        PlayerPrefs.SetFloat("SensVal", 0.05f);
    }
}
