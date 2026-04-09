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
    public static int RemainingEnergy = 100;
    private DateTime TiempoEnergiaProx;
    private DateTime TiempoEnergiaAnt;

    private float timer = 0;
    private bool _isBarVisible = false;

    public float timeForSeconds = 2f;

    private Coroutine _animationRoutine;
    private Animation _animationComp;

    public static Energy instance;
    public Slider _Barra => Barra;

    public static event Action<int> OnEnergyUpdated;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    void Start()
    {
        _animationComp = GetComponent<Animation>();
        UpdateEnergy();
        
        if (_animationComp != null)
        {
            if (_animationComp["Entrar uwuw"] != null) _animationComp["Entrar uwuw"].speed = 3f;
            if (_animationComp["Salir uwuw"] != null) _animationComp["Salir uwuw"].speed = 3f;
            if (_animationComp["NoHayEnergia"] != null) _animationComp["NoHayEnergia"].speed = 2f;
        }
        
        if (_animationComp != null && _animationComp["Salir uwuw"] != null)
        {
            _animationComp.Play("Salir uwuw");
        }
        else if (Barra != null)
        {
            Barra.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                HideBar();
            }
        }
    }

    public static void UseEnergy(int UsedEnergyAmount)
    {
        RemainingEnergy -= UsedEnergyAmount;
        RemainingEnergy = Mathf.Clamp(RemainingEnergy, 0, MaxEnergy);
        UpdateEnergy();
    }

    public static void UpdateEnergy()
    {
        if (instance == null) return;
        
        OnEnergyUpdated?.Invoke(RemainingEnergy);
        if (instance.Barra != null)
        {
            instance.Barra.maxValue = MaxEnergy;
            instance.Barra.value = RemainingEnergy;
        }
    }

    public bool TryUseAndAnimateEnergy(int energyUsed, float newTimer)
    {   
        if(RemainingEnergy >= energyUsed)
        {
            UseEnergy(energyUsed);
            ShowBar(newTimer);
            return true;
        }
        else
        {
            ShowNoEnergyFeedback();
            return false;
        }
    }

    public void ShowBarOnly(float duration)
    {
        ShowBar(duration);
    }

    private void ShowBar(float duration)
    {
        timer = duration;
        if (!_isBarVisible)
        {
            if (_animationRoutine != null) StopCoroutine(_animationRoutine);
            _animationRoutine = StartCoroutine(AnimateBar("Entrar uwuw", true));
        }
    }

    private void HideBar()
    {
        if (_animationRoutine != null) StopCoroutine(_animationRoutine);
        _animationRoutine = StartCoroutine(AnimateBar("Salir uwuw", false));
    }

    private bool _isNoEnergyAnimating = false;

    private void ShowNoEnergyFeedback()
    {
        if (_isNoEnergyAnimating) return;
        if (_animationRoutine != null) StopCoroutine(_animationRoutine);
        _animationRoutine = StartCoroutine(NoEnergyRoutine());
    }

    private IEnumerator AnimateBar(string animName, bool visible)
    {
        _isNoEnergyAnimating = false;
        _isBarVisible = visible;
        if (_animationComp != null && _animationComp[animName] != null)
        {
            _animationComp.Play(animName);
            yield return new WaitForSeconds(_animationComp[animName].length);
        }
        else
        {
            if (visible) _Barra.gameObject.SetActive(true);
            else _Barra.gameObject.SetActive(false);
            yield return null;
        }
        _animationRoutine = null;
    }

    private IEnumerator NoEnergyRoutine()
    {
        _isNoEnergyAnimating = true;

        if (!_isBarVisible)
        {
            if (_animationComp != null && _animationComp["Entrar uwuw"] != null)
            {
                _animationComp.Play("Entrar uwuw");
                yield return new WaitForSeconds(_animationComp["Entrar uwuw"].length);
            }
            _isBarVisible = true;
        }

        if (_animationComp != null && _animationComp["NoHayEnergia"] != null)
        {
            _animationComp.Play("NoHayEnergia");
            yield return new WaitForSeconds(_animationComp["NoHayEnergia"].length);
        }

        if (_animationComp != null && _animationComp["Salir uwuw"] != null)
        {
            _animationComp.Play("Salir uwuw");
            yield return new WaitForSeconds(_animationComp["Salir uwuw"].length);
        }

        _isBarVisible = false;
        _isNoEnergyAnimating = false;
        _animationRoutine = null;
    }

    private DateTime StringToDate(string datetime)
    {
        if (String.IsNullOrEmpty(datetime))
        {
            return DateTime.Now;
        }
        return DateTime.Parse(datetime);
    }

    private void Load()
    {
        RemainingEnergy = PlayerPrefs.GetInt("EnergiaActual", 100);
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
