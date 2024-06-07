using System.Collections;
using System.Collections.Generic;
using FaRUtils.Systems.DateTime;
using FaRUtils.FPSController;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SleepHandler : MonoBehaviour
{
    
    public bool _isSleeping = false;
    public Animation Fade;
    public FaRCharacterController player;
    public LightingManager lightingManager; // TODO: Sacar de acá, solo lo usan los comandos

    public bool _yourLetterArrived = false; // Satia de mierda que signifca esto
    
    public UnityEvent<bool> SaveDataEvent;
    public event System.Action OnPlayerSleep;
    public event System.Action OnPlayerWakeUp;

    private bool _skipTonightSleep = false;
    public static SleepHandler Instance { get; private set; }
    
    //TODO: Ver como conviene hacer asi no llenamos todo de singletons(?
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
        else if(currentHour == 6) StartDay();
    }
    
    public void StartDay()
    {
        if(_yourLetterArrived == false)
        {
            TimeManager.TimeBetweenTicks = 10f;
        }

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
            Debug.Log("Ya estás durmiendo");
            return false;
        }
        if(TimeManager.DateTime.Hour >= 6 && TimeManager.DateTime.Hour < 17)
        {
            Debug.Log("Es muy temprano para dormir");
            return false;
        }

        //TODO: TP player to side of bed
        
        OnPlayerSleep?.Invoke();
        if(_yourLetterArrived == false)
        {
            TimeManager.TimeBetweenTicks = 0.05f;
        }
        Fade.gameObject.SetActive(true);
        Fade.Play("NegroIn");
        SaveDataEvent.Invoke(false);

        Energy.RemainingEnergy = 100;
        Energy.UpdateEnergy();

        player.enabled = false;
        _isSleeping = true;
        
        
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
