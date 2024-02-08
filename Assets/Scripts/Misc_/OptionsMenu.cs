using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FaRUtils.Systems.DateTime;

public class OptionsMenu : MonoBehaviour
{
    public static OptionsMenu Instance;

    public GameObject RelojUI;
    public DateTime dateTime;
    public FPSLimit FPSLimit;
    public TextMeshProUGUI FovVal;
    public TextMeshProUGUI FPSText;
    public bool doceh;
    public bool isOptionsMenuOpen;

    public Slider fovSlider;
 
    private void Awake() {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    private void Start() 
    {
        fovSlider.value = Camera.main.fieldOfView;

        if (FPSLimit.target == 0)
        {
            FPSLimit.target = 60;
            PlayerPrefs.SetInt("TargetaFPS", 60);
        }

        FPSLimit.target  =  PlayerPrefs.GetInt("TargetaFPS");
        FPSText.text = "FPS: " + FPSLimit.target;
    }
    public void Update()
    {
        FovVal.text = fovSlider.value.ToString();
        Camera.main.fieldOfView = fovSlider.value;
    }

    public void clock()
    {
        if (doceh == true) 
        {
            doceh = false;
            RelojUI.GetComponent<ClockManager>().Time.text = dateTime.TimeToString12();
        }
        else 
        {
            doceh = true;
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
    }
}
