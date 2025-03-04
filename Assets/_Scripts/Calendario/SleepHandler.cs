using FaRUtils.FPSController;
using FaRUtils.Systems.DateTime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class SleepHandler : MonoBehaviour
{
    [SerializeField] private int bedSceneIndex;
    public bool _isSleeping = false;
    public Animation Fade;
    public FaRCharacterController player;
    public LightingManager lightingManager; // TODO: Sacar de acá, solo lo usan los comandos

    public float SleepingTickRate { get; } = 0.05f;

    public bool _yourLetterArrived = false; // Satia de mierda que signifca esto

    public event System.Action OnPlayerSleep;
    public event System.Action OnPlayerWakeUp;

    private bool _skipTonightSleep = false;

    public bool tpToBed;
    public static SleepHandler Instance { get; private set; }

    //TODO: Ver como conviene hacer asi no llenamos todo de singletons(? Shhhhhhhh
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

    private void OnEnable()
    {
        DateTime.OnHourChanged.AddListener(ProcessHourChange);
    }

    void Start()
    {
        player = FaRCharacterController.instance;
    }

    void ProcessHourChange(int currentHour)
    {
        if (currentHour == 2 && !_skipTonightSleep && !_isSleeping) StartCoroutine(FinishDay());
        else if (currentHour == 6) StartDay();
    }

    public void StartDay()
    {
        if (_yourLetterArrived == false)
        {
            TimeManager.TimeBetweenTicks = 10f;
        }

        SaveLoadHandlerSystem.Invoke(false);
        OnPlayerWakeUp?.Invoke();
        _skipTonightSleep = false; // Reset if skipped tonight's sleep
        Fade.Play("NegroOut");
        player.enabled = true;
        StartCoroutine(Wait());
    }

    public IEnumerator FinishDay()
    {
        OnPlayerSleep?.Invoke();
        if (_yourLetterArrived == false)
        {
            TimeManager.TimeBetweenTicks = SleepingTickRate;
        }
        Fade.gameObject.SetActive(true);
        Fade.Play("NegroIn");

        Energy.RemainingEnergy = 100;
        Energy.UpdateEnergy();

        player.enabled = false;
        _isSleeping = true;

        if (tpToBed) //&& _yourLetterArrived == false
        {
            if (SceneManager.GetActiveScene().buildIndex != bedSceneIndex)
            {
                LoadingManager.LoadNewScene(bedSceneIndex);
            }
        }
        else yield return null;
    }

    public bool TrySleep()
    {
        if (_isSleeping)
        {
            return false;
        }
        if (TimeManager.DateTime.Hour >= 6 && TimeManager.DateTime.Hour < 17)
        {
            this.LogOnScreen("No podés dormir antes de las 5PM, avancemos un poco el tiempo...");
            FaRCommands.ImSleepy();
            return false;
        }

        StartCoroutine(FinishDay());
        return true;
    }

    public void SkipTonightSleep() // Para poder llamar de fuera en caso de que por alguna razón se quiera saltear el dormirse esta noche
    {
        _skipTonightSleep = true;
    }


    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.7f);
        Fade.gameObject.SetActive(false);
        _isSleeping = false;
    }


    private void OnDisable()
    {
        DateTime.OnHourChanged.RemoveListener(ProcessHourChange);
    }
}
