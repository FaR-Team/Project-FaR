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

    [SerializeField] private bool tpToBed;
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
        if (currentHour == 2 && !_skipTonightSleep && !_isSleeping) FinishDay();
        else if (currentHour == 6) StartCoroutine(StartDay());
    }

    public IEnumerator StartDay()
    {
        if (_yourLetterArrived == false)
        {
            TimeManager.TimeBetweenTicks = 10f;
        }

        if (tpToBed && _yourLetterArrived == false)
        {
            if (SceneManager.GetActiveScene().buildIndex != bedSceneIndex)
            {
                yield return LoadSceneAsync(bedSceneIndex);
            }
        }
        else yield return null;

        SaveLoadHandlerSystem.Invoke(false);
        OnPlayerWakeUp?.Invoke();
        _skipTonightSleep = false; // Reset if skipped tonight's sleep
        Fade.Play("NegroOut");
        player.enabled = true;
        StartCoroutine(Wait());
    }

    public bool FinishDay()
    {
        if (_isSleeping)
        {
            return false;
        }
        if (TimeManager.DateTime.Hour >= 6 && TimeManager.DateTime.Hour < 17)
        {
        #if DEBUG
            this.LogOnScreen("Es muy temprano para dormir {Dev: Usá el comando imsleepy}");
        #endif
            return false;
        }

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


        return true;

    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            yield return null;
            if (progress == 1)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
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
