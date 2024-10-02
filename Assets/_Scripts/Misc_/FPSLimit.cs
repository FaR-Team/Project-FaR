using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSLimit : MonoBehaviour
{
    public int target = 60;
    public TextMeshProUGUI FPSText;
    
    void OnEnable()
    {
        target  =  PlayerPrefs.GetInt("TargetaFPS");
        FPSText.text = "FPS: " + target;
    }

    void OnDisable()
    {
        PlayerPrefs.SetInt("TargetaFPS", target);
    }

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
    }
      
    void Update()
    {
        if(Application.targetFrameRate != target)
            Application.targetFrameRate = target;
    }
}
