using FaRUtils.Systems.DateTime;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Samples;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{
    public RectTransform ClockFace;
    public TextMeshProUGUI Date, Time, Season, Week, Year, Gold;

    public TimeManager timeManager;
    public LanguageTMPDropdown languageTMPDropdown;


    public static ClockManager InstanceClock;
    public static TextMeshProUGUI _time => InstanceClock.Time;

    public OptionsMenu optionsMenu;
    [SerializeField] Image SeasonImageObj;
    private PlayerInventoryHolder _playerInventoryHolder;

    [SerializeField] Sprite[] SeasonSprites;

    [SerializeField] private LightingPreset Preset;

    private float startingRotation;

    public Light sunlight;
    public float nightIntensity;
    public float dayIntensity;

    public AnimationCurve dayNightCurve;

    public int place;

    private void Awake()
    {
        InstanceClock = this;
        startingRotation = ClockFace.localEulerAngles.z;
        GameObject player = GameObject.FindWithTag("Player");

        _playerInventoryHolder = player.GetComponent<PlayerInventoryHolder>();
    }

    private void Start()
    {
        place = SceneManager.GetActiveScene().buildIndex;
        SceneManager.activeSceneChanged += ChangeSceneText;
    }

    private void ChangeSceneText(Scene current, Scene next)
    {
        place = next.buildIndex;
        // {locationVariable.place:choose(0|1|2):Farm|House|Bocaaaaaa}
        //{locationVariable.place:choose(0|1|2):Granja|Casa|Y DAAALEEE BOCAAAA}
        //{locationVariable.place:choose(0|1|2):swanjigranja|swajicasa|BOCAAAAAA}
    }

    private void Update()
    {
        Gold.text = $"{_playerInventoryHolder.PrimaryInventorySystem.Gold}G";
    }

    private void OnEnable()
    {
        TimeManager.OnDateTimeChanged += UpdateDateTime;
    }
    public static string TimeText()
    {
        return _time.text;
    }

    public void UpdateDateTime(DateTime dateTime)
    {
        Time.text = CheckTimeFormat(dateTime);

        SeasonImageObj.sprite = SeasonSprites[(int)dateTime.Seasons];
        RotateSprite(dateTime);
    }

    private void RotateSprite(DateTime dateTime)
    {
        float t = dateTime.Hour / 24f;
        float newRotation = Mathf.Lerp(0, 360, t);

        ClockFace.localEulerAngles = new Vector3(0, 0, startingRotation + newRotation);
    }

    public string CheckTimeFormat(DateTime dateTime)
    {
        if (optionsMenu.doce)
        {
            return dateTime.TimeToString12();
        }
        else
        {
            return dateTime.TimeToString24();
        }
    }
}