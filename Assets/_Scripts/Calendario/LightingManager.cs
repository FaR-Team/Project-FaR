using System;
using UnityEngine;
using FaRUtils.Systems.DateTime;

//[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [Header("Variables")]
    [SerializeField, Range(0, 24)] public float TimeOfDay;
    public int Day;
    public float vel = 0.01666667f;
    public TimeManager timeManager;
    [SerializeField] private float transitionSpeed = 1f;
    private Color targetAmbientColor;
    private Color targetFogColor;
    private Color targetDirectionalColor;

    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private float minIntensity = 0.1f;
    [SerializeField] private float sunriseTime = 6f;
    [SerializeField] private float sunsetTime = 18f;

    [SerializeField] private Material daySkyboxMaterial;
    [SerializeField] private Material nightSkyboxMaterial;
    private Material tintedSkyboxMaterial;
    private Color startTintColor = Color.black;
    private Color endTintColor = new Color(0.519608f, 0.519608f, 0.519608f);
    private float tintStartTime = 20f; // 8 PM
    private float tintEndTime = 21f;   // 9 PM
    private float sunriseTintStartTime = 4f; // 5 AM
    private float sunriseTintEndTime = 6f;   // 6 AM
    [SerializeField] private float starRotationSpeed = 1f;

    private void OnEnable()
    {
        SleepHandler.Instance.OnPlayerWakeUp += CopyHour;
    }

    private void Start() {
        CopyHour();
        tintedSkyboxMaterial = new Material(Shader.Find("Skybox/6 Sided"));
        RenderSettings.skybox = tintedSkyboxMaterial;
    }
    private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime * vel;
            TimeOfDay %= 24; //Módulo para asegurarse que esté entre 0 y 24
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }

        if (TimeOfDay >= 24)
        {
            Day++;
            TimeOfDay = 0;
        }
    }

    public void CopyHour()
    {
        TimeOfDay = TimeManager.DateTime.Hour;
    }
    private void UpdateLighting(float timePercent)
    {
        targetAmbientColor = Preset.ColorAmbiente.Evaluate(timePercent);
        targetFogColor = Preset.ColorNiebla.Evaluate(timePercent);

        RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, targetAmbientColor, Time.deltaTime * transitionSpeed);
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetFogColor, Time.deltaTime * transitionSpeed);

        if (DirectionalLight != null)
        {
            targetDirectionalColor = Preset.ColorDireccional.Evaluate(timePercent);
            DirectionalLight.color = Color.Lerp(DirectionalLight.color, targetDirectionalColor, Time.deltaTime * transitionSpeed);

            // New intensity calculation
            float intensity;
            float sunriseIntensity = maxIntensity * 1.5f;
            float middayIntensity = maxIntensity * 0.8f;

            if (TimeOfDay >= sunriseTime && TimeOfDay <= sunsetTime)
            {
                float t = Mathf.InverseLerp(sunriseTime, sunsetTime, TimeOfDay);
                if (t < 0.25f) // Sunrise period
                {
                    intensity = Mathf.Lerp(minIntensity, sunriseIntensity, t * 4);
                }
                else if (t < 0.75f) // Daytime period
                {
                    intensity = Mathf.Lerp(sunriseIntensity, middayIntensity, (t - 0.25f) * 2);
                }
                else // Sunset period
                {
                    intensity = Mathf.Lerp(middayIntensity, minIntensity, (t - 0.75f) * 4);
                }
            }
            else
            {
                intensity = minIntensity;
            }

            DirectionalLight.intensity = Mathf.Lerp(DirectionalLight.intensity, intensity, Time.deltaTime * transitionSpeed);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

        // Implement sunrise tint-up logic
        if (TimeOfDay >= sunriseTintStartTime && TimeOfDay < sunriseTintEndTime)
        {
            float tintProgress = Mathf.InverseLerp(sunriseTintStartTime, sunriseTintEndTime, TimeOfDay);
            Color currentTint = Color.Lerp(endTintColor, startTintColor, tintProgress);
            daySkyboxMaterial.SetColor("_Tint", currentTint);
            if (TimeOfDay >= sunriseTintEndTime)
            {
                RenderSettings.skybox = daySkyboxMaterial;
            }
        }
        else if (TimeOfDay >= tintStartTime || TimeOfDay < sunriseTintStartTime) // Night time
        {
            float tintProgress;
            if (TimeOfDay >= tintStartTime && TimeOfDay <= tintEndTime)
            {
                tintProgress = Mathf.InverseLerp(tintStartTime, tintEndTime, TimeOfDay);
                Color currentTint = Color.Lerp(startTintColor, endTintColor, tintProgress);
                nightSkyboxMaterial.SetColor("_Tint", currentTint);
            }

            RenderSettings.skybox = nightSkyboxMaterial;
        }
        else // Day time
        {
            daySkyboxMaterial.SetColor("_Tint", Color.white); // Reset tint
            RenderSettings.skybox = daySkyboxMaterial;
        }
        // Rotate the skybox
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * starRotationSpeed);
        // Force skybox update
        DynamicGI.UpdateEnvironment();
    }
    private void OnDisable()
    {
        SleepHandler.Instance.OnPlayerWakeUp -= CopyHour;
    }


    //Encontrar una luz direccional en la escena
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }

}