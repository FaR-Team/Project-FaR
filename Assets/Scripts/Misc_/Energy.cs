using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Energy : MonoBehaviour
{
    [SerializeField] TMP_Text TextoEnergia;
    [SerializeField] Slider Barra;

    private static int MaxEnergy = 100;
    public static int RemainingEnergy;
    private DateTime TiempoEnergiaProx;
    private DateTime TiempoEnergiaAnt;

    public static float timer = 5;
    public static bool _yaAnimo = false;
    public static bool _ContadorActivo = false;

    public float timeForSeconds;

    static WaitForSeconds delay;

    public static Animation _animationComp;

    public static Energy instance;
    public static Slider _Barra => instance.Barra;

    public static event Action<int> OnEnergyUpdated;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        _animationComp = GetComponent<Animation>();
        delay = new WaitForSeconds(timeForSeconds);
        RemainingEnergy = 30;
        UpdateEnergy();
        /*
        if(!PlayerPrefs.HasKey("EnergiaActual"))
        {
            PlayerPrefs.SetInt("EnergiaActual", 30);
            Load();
            StartCoroutine(RestoreEnergy());
        }else{
            Load();
            StartCoroutine(RestoreEnergy());
        }*/
    }

    private void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;

        if (timer <= 0) _ContadorActivo = false;

        if (_ContadorActivo == false && _yaAnimo == false)
        {
            StartCoroutine(Waiter());
            _yaAnimo = true;
        }
    }

    public static void UseEnergy(int UsedEnergyAmount)
    {
        RemainingEnergy -= UsedEnergyAmount;
        if (RemainingEnergy > -1)
        {
            UpdateEnergy();
        }
        else
        {
            Debug.Log("No tenés energía");
        }
    }

    private IEnumerator RestoreEnergy()
    {

        if (RemainingEnergy < MaxEnergy)
        {
            DateTime currentDateTime = DateTime.Now;
            DateTime nextDateTime = TiempoEnergiaProx;
            bool isEnergyAdding = false;
            while (currentDateTime > nextDateTime)
            {
                if (RemainingEnergy < MaxEnergy)
                {
                    isEnergyAdding = true;
                    RemainingEnergy++;
                    UpdateEnergy();
                    DateTime timeToAdd = TiempoEnergiaAnt > nextDateTime ? TiempoEnergiaAnt : nextDateTime;
                }
                else
                {
                    break;
                }
            }

            if (isEnergyAdding == true)
            {
                TiempoEnergiaAnt = DateTime.Now;
                TiempoEnergiaProx = nextDateTime;
            }

            UpdateEnergy();
            Save();
            yield return null;
        }

    }

    public static void UpdateEnergy()
    {
        //TextoEnergia.text = EnergiaActual.ToString() + "/" + EnergiaMax.ToString();
        OnEnergyUpdated.Invoke(RemainingEnergy);
        _Barra.maxValue = MaxEnergy;
        _Barra.value = RemainingEnergy;
    }

    public bool TryUseAndAnimateEnergy(int energyUsed, float newTimer)
    {   
        if(RemainingEnergy >= energyUsed) // Si hay energía suficiente
        {
            UseEnergy(energyUsed);

            if (_ContadorActivo == false)
            {
                _animationComp.Play("Entrar uwuw");
                _ContadorActivo = true;
                _yaAnimo = false;
            }

            timer = newTimer; // Aunque ya se esté animando, se actualiza el timer para que se siga mostrando

            return true;
        }
        else
        {
            StartCoroutine(Walter());

            return false;
        }
        
    }

    private DateTime StringToDate(string datetime)
    {
        if (String.IsNullOrEmpty(datetime))
        {
            return DateTime.Now;
        }
        else
        {
            return DateTime.Parse(datetime);
        }
    }

    IEnumerator Waiter()
    {
        yield return delay;
        GetComponent<Animation>().Play("Salir uwuw");
    }

    public static IEnumerator Walter()
    {
        yield return delay;
        _animationComp.Play("NoHayEnergia");
        yield return delay;
        _animationComp.Play("Salir uwuw");
    }
    public static IEnumerator Walicho()
    {
        yield return delay;
        _animationComp.Play("NoHayEnergia");
    }

    private void Load()
    {
        RemainingEnergy = PlayerPrefs.GetInt("EnergiaActual");
        TiempoEnergiaProx = StringToDate(PlayerPrefs.GetString("TiempoEnergiaProx"));
        TiempoEnergiaAnt = StringToDate(PlayerPrefs.GetString("TiempoEnergiaAnt"));
    }

    private void Save()
    {
        PlayerPrefs.SetInt("EnergiaActual", RemainingEnergy);
        PlayerPrefs.SetString("TiempoEnergiaProx", TiempoEnergiaProx.ToString());
        PlayerPrefs.SetString("TiempoEnergiaAnt", TiempoEnergiaAnt.ToString());
    }
}
