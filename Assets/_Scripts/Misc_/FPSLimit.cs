using UnityEngine;
using TMPro;

public class FPSLimit : MonoBehaviour
{
    public int target = 60;
    public TextMeshProUGUI FPSText;
    
    void OnEnable()
    {
        target = PlayerPrefs.GetInt("TargetFPS", 60);
        UpdateFPSDisplay();
    }

    void OnDisable()
    {
        PlayerPrefs.SetInt("TargetFPS", target);
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
        UpdateFPSDisplay();
    }

    public void SetFrameRateTarget(int newTarget)
    {
        target = newTarget;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
        PlayerPrefs.SetInt("TargetFPS", target);
        UpdateFPSDisplay();
    }

    private void UpdateFPSDisplay()
    {
        if(FPSText != null)
            FPSText.text = "FPS: " + target;
    }
}
