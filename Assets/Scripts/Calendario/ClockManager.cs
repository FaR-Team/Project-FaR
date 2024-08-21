using FaRUtils.Systems.DateTime;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{
    public RectTransform ClockFace;
    public TextMeshProUGUI Time, Gold;

    public static ClockManager InstanceClock;
    public static TextMeshProUGUI _time => InstanceClock.Time;

    public OptionsMenu optionsMenu;

    [SerializeField] Image SeasonImageObj;

    [SerializeField] Sprite[] SeasonSprites;

    private float startingRotation;

    [HideInInspector] public int place;

    [SerializeField] LocalizeStringEvent[] stringEvents;


    private void Awake()
    {
        InstanceClock = this;
        startingRotation = ClockFace.localEulerAngles.z;


    }

    private void Start()
    {
        place = SceneManager.GetActiveScene().buildIndex;
        SceneManager.sceneLoaded += ChangeSceneText;
    }

    private void ChangeSceneText(Scene arg0, LoadSceneMode arg1)
    {
        place = arg0.buildIndex;
        Refresh();
    }

    private void Update()
    {
        Gold.text = $"{PlayerInventoryHolder.instance.PrimaryInventorySystem.Gold}G";
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
        Refresh();
    }

    private void Refresh()
    {
        foreach (LocalizeStringEvent locEvent in stringEvents)
        {
            locEvent.RefreshString();
        }
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