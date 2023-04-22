using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Samples;
using UnityEngine.Localization.Settings;
using FaRUtils.Systems.DateTime;

public class ClockManager : MonoBehaviour
{
    public RectTransform ClockFace;
    public TextMeshProUGUI Date, Time, Season, Week, Year, Gold;
    public TimeManager timeManager;
    public LanguageTMPDropdown languageTMPDropdown;

    public GameObject OptionsMenu;
    public GameObject SeasonImageObj;
    private PlayerInventoryHolder _playerInventoryHolder;

    public Sprite seasonSprite1, seasonSprite2, seasonSprite3, seasonSprite4, seasonSprite5, seasonSprite6, seasonSprite7, seasonSprite8;

    [SerializeField] private LightingPreset Preset;

    private float startingRotation;

    public Light sunlight;
    public float nightIntensity;
    public float dayIntensity;

    public AnimationCurve dayNightCurve;

    private void Awake() 
    {
        startingRotation = ClockFace.localEulerAngles.z;
        GameObject player = GameObject.FindWithTag("Player");

        _playerInventoryHolder = player.GetComponent<PlayerInventoryHolder>();
    }

    private void Update() {
        Gold.text = $"{_playerInventoryHolder.PrimaryInventorySystem.Gold.ToString()}G";
    }
    
    private void OnEnable()
    {
        TimeManager.OnDateTimeChanged += UpdateDateTime;
    }

    public void UpdateDateTime(DateTime dateTime)
    {
        //var collection = LocalizationSettings.GetStringTableCollection("Reloj");
        if (languageTMPDropdown.identifier == "en")
        {
            Date.text = dateTime.DateToStringEn();
        }
        else if (languageTMPDropdown.identifier == "es")
        {
            Date.text = dateTime.DateToStringEs();
        }
        else if (languageTMPDropdown.identifier == "sw")
        {
            Date.text = dateTime.DateToStringSw();
        }
        Time.text = CheckTimeFormat(dateTime);
        //Season.text = dateTime.Seasons.ToString();

        if (languageTMPDropdown.identifier == "en")
        {
            Week.text = $"Wk: {dateTime.CurrentWeek}";
            Year.text = $"Year: {dateTime.Year}";
        }
        else if (languageTMPDropdown.identifier == "es")
        {
            Week.text = $"Sm: {dateTime.CurrentWeek}";
            Year.text = $"Año: {dateTime.Year}";
        }
        else if (languageTMPDropdown.identifier == "sw")
        {
            Week.text = $"Idks: {dateTime.CurrentWeek}";
            Year.text = $"Idks: {dateTime.Year}";
        }

        if (dateTime.Seasons.ToString() == "Early_Spring")
        {
            if (languageTMPDropdown.identifier == "en")
            {
                Season.text = "Early Spring";
            }
            else if (languageTMPDropdown.identifier == "es")
            {
                Season.text = "Primavera Precoz";
            }
            else if (languageTMPDropdown.identifier == "sw")
            {
                Season.text = "I don't know Swahilli";
            }
            SeasonImageObj.GetComponent<Image>().sprite = seasonSprite1;
        }
        if (dateTime.Seasons.ToString() == "Late_Spring")
        {
            if (languageTMPDropdown.identifier == "en")
            {
                Season.text = "Late Spring";
            }
            else if (languageTMPDropdown.identifier == "es")
            {
                Season.text = "Primavera Tardía";
            }
            else if (languageTMPDropdown.identifier == "sw")
            {
                Season.text = "I don't know Swahilli";
            }
            SeasonImageObj.GetComponent<Image>().sprite = seasonSprite2;
        }
        if (dateTime.Seasons.ToString() == "Early_Summer")
        {
            if (languageTMPDropdown.identifier == "en")
            {
                Season.text = "Early Summer";
            }
            else if (languageTMPDropdown.identifier == "es")
            {
                Season.text = "Verano Precoz";
            }
            else if (languageTMPDropdown.identifier == "sw")
            {
                Season.text = "I don't know Swahilli";
            }
            SeasonImageObj.GetComponent<Image>().sprite = seasonSprite3;
        }
        if (dateTime.Seasons.ToString() == "Late_Summer")
        {
            if (languageTMPDropdown.identifier == "en")
            {
                Season.text = "Late Summer";
            }
            else if (languageTMPDropdown.identifier == "es")
            {
                Season.text = "Verano Tardío";
            }
            else if (languageTMPDropdown.identifier == "sw")
            {
                Season.text = "I don't know Swahilli";
            }
            SeasonImageObj.GetComponent<Image>().sprite = seasonSprite4;
        }
        if (dateTime.Seasons.ToString() == "Early_Fall")
        {
            if (languageTMPDropdown.identifier == "en")
            {
                Season.text = "Early Fall";
            }
            else if (languageTMPDropdown.identifier == "es")
            {
                Season.text = "Otoño Precoz";
            }
            else if (languageTMPDropdown.identifier == "sw")
            {
                Season.text = "I don't know Swahilli";
            }
            SeasonImageObj.GetComponent<Image>().sprite = seasonSprite5;
        }
        if (dateTime.Seasons.ToString() == "Late_Fall")
        {
            if (languageTMPDropdown.identifier == "en")
            {
                Season.text = "Late Fall";
            }
            else if (languageTMPDropdown.identifier == "es")
            {
                Season.text = "Otoño Tardío";
            }
            else if (languageTMPDropdown.identifier == "sw")
            {
                Season.text = "I don't know Swahilli";
            }
            SeasonImageObj.GetComponent<Image>().sprite = seasonSprite6;
        }
        if (dateTime.Seasons.ToString() == "Early_Winter") 
        {
            if (languageTMPDropdown.identifier == "en")
            {
                Season.text = "Early Winter";
            }
            else if (languageTMPDropdown.identifier == "es")
            {
                Season.text = "Invierno Precoz";
            }
            else if (languageTMPDropdown.identifier == "sw")
            {
                Season.text = "I don't know Swahilli";
            }
            SeasonImageObj.GetComponent<Image>().sprite = seasonSprite7;
        }
        if (dateTime.Seasons.ToString() == "Late_Winter")
        {
            if (languageTMPDropdown.identifier == "en")
            {
                Season.text = "Late Winter";
            }
            else if (languageTMPDropdown.identifier == "es")
            {
                Season.text = "Invierno Tardío";
            }
            else if (languageTMPDropdown.identifier == "sw")
            {
                Season.text = "I don't know Swahilli";
            }
            SeasonImageObj.GetComponent<Image>().sprite = seasonSprite8;
        }


        //weatherSprite.sprite = weatherSprites[(int)WeatherManager.currentWeather];

        float t = (float)dateTime.Hour / 24f;

        float newRotation = Mathf.Lerp(0, 360, t);
        ClockFace.localEulerAngles = new Vector3(0, 0, startingRotation + newRotation);

        //float dayNightT = dayNightCurve.Evaluate(t);

        //sunlight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, dayNightT);        
    }

    public string CheckTimeFormat(DateTime dateTime)
    {
        if (OptionsMenu.GetComponent<OptionsMenu>().doceh == true)
        {
          return dateTime.TimeToString12();
        }
        else
        {
           return dateTime.TimeToString24();
        }
    }
}