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
    public float vel = 0.011f;
    public TimeManager timeManager;


    private void OnEnable()
    {
        SleepHandler.Instance.OnPlayerWakeUp += CopyHour;
    }

    private void Start() {
        CopyHour();
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
        //Cambiar el ambiente y la niebla
        RenderSettings.ambientLight = Preset.ColorAmbiente.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.ColorNiebla.Evaluate(timePercent);


        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.ColorDireccional.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

    }

    private void OnDisable()
    {
        SleepHandler.Instance.OnPlayerWakeUp -= CopyHour;
    }


    //Enconctrar una luz direccional en la escena uwu
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        //Buscar la pestaña sol
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
