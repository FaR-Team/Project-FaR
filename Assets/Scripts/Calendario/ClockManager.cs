using System;
using FaRUtils.Systems.DateTime;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DateTime = FaRUtils.Systems.DateTime.DateTime;

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
        SubscribeGoldEvent();
        RefreshGoldDisplay();
    }

    private void ChangeSceneText(Scene arg0, LoadSceneMode arg1)
    {
        place = arg0.buildIndex;
        Refresh();
    }

    private void OnEnable()
    {
        TimeManager.OnDateTimeChanged += UpdateDateTime;
        SubscribeGoldEvent();
        RefreshGoldDisplay();
    }

    private void OnDisable()
    {
        TimeManager.OnDateTimeChanged -= UpdateDateTime;
        UnsubscribeGoldEvent();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ChangeSceneText;
        UnsubscribeGoldEvent();
    }

    private void SubscribeGoldEvent()
    {
        if (PlayerInventoryHolder.instance != null && PlayerInventoryHolder.instance.PrimaryInventorySystem != null)
        {
            PlayerInventoryHolder.instance.PrimaryInventorySystem.OnGoldAmountChanged -= UpdateGoldText;
            PlayerInventoryHolder.instance.PrimaryInventorySystem.OnGoldAmountChanged += UpdateGoldText;
        }
    }

    private void UnsubscribeGoldEvent()
    {
        if (PlayerInventoryHolder.instance != null && PlayerInventoryHolder.instance.PrimaryInventorySystem != null)
        {
            PlayerInventoryHolder.instance.PrimaryInventorySystem.OnGoldAmountChanged -= UpdateGoldText;
        }
    }

    public void RefreshGoldDisplay()
    {
        if (PlayerInventoryHolder.instance != null && PlayerInventoryHolder.instance.PrimaryInventorySystem != null)
        {
            UpdateGoldText(PlayerInventoryHolder.instance.PrimaryInventorySystem.Gold);
        }
    }

    private void UpdateGoldText(int gold)
    {
        if (Gold != null)
        {
            Gold.text = $"{gold}";
        }
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