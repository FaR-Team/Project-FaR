using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Energía : MonoBehaviour
{
    [SerializeField] TMP_Text TextoEnergia;
    [SerializeField] Slider Barra;
    private int EnergiaMax = 30;
    public int EnergiaActual;
    private DateTime TiempoEnergiaProx;
    private DateTime TiempoEnergiaAnt;

    public float timer = 5;
    public bool _yaAnimo = false;
    public bool _ContadorActivo = false;



    void Start()
    {
        EnergiaActual = 30;
        UpdateEnergy();
        //if(!PlayerPrefs.HasKey("EnergiaActual"))
        //{
        //    PlayerPrefs.SetInt("EnergiaActual", 30);
        //    Load();
        //    StartCoroutine(RestoreEnergy());
        //}else{
        //    Load();
        //    StartCoroutine(RestoreEnergy());
        //}
    }

    private void Update()
    {
        if (timer > 0 ) timer -= Time.deltaTime;

        if (timer <= 0) _ContadorActivo = false;

        if (_ContadorActivo == false && _yaAnimo == false)
        {
            StartCoroutine(waiter());
            _yaAnimo = true;
        }
    }

    public void UseEnergy(int EnergiaUsada)
    {
        EnergiaActual -= EnergiaUsada;
        if(EnergiaActual >- 1)
        {
            UpdateEnergy();
        }else{
            Debug.Log("No tenés energía");
        }
    }

    private IEnumerator RestoreEnergy()
    {
        while (true)
        {
            if (EnergiaActual < EnergiaMax)
            {
                DateTime currentDateTime = DateTime.Now;
                DateTime nextDateTime = TiempoEnergiaProx;
                bool isEnergyAdding = false;
                while(currentDateTime > nextDateTime)
                {
                    if (EnergiaActual < EnergiaMax)
                    {
                        isEnergyAdding = true;
                        EnergiaActual++;
                        UpdateEnergy();
                        DateTime timeToAdd = TiempoEnergiaAnt > nextDateTime ? TiempoEnergiaAnt : nextDateTime;
                    }else{
                        break;
                    }
                }

                if(isEnergyAdding == true) 
                {
                    TiempoEnergiaAnt = DateTime.Now;
                    TiempoEnergiaProx = nextDateTime;
                }

                UpdateEnergy();
                Save();
                yield return null;
            }
        }
    }

    public void UpdateEnergy() 
    {
        //TextoEnergia.text = EnergiaActual.ToString() + "/" + EnergiaMax.ToString();
        Barra.maxValue = EnergiaMax;
        Barra.value = EnergiaActual;
    }

    private DateTime StringToDate(string datetime)
    {
        if(String.IsNullOrEmpty(datetime))
        {
            return DateTime.Now;
        }
        else
        {
            return DateTime.Parse(datetime);
        }
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<Animation>().Play("Salir uwuw");
    }

    public IEnumerator walter()
    {
        yield return new WaitForSeconds(1f);
        this.GetComponent<Animation>().Play("NoHayEnergia");
        yield return new WaitForSeconds(1f);
        this.GetComponent<Animation>().Play("Salir uwuw");
    }
    public IEnumerator Walicho()
    {
        yield return new WaitForSeconds(1f);
        this.GetComponent<Animation>().Play("NoHayEnergia");
    }

    private void Load()
    {
        EnergiaActual = PlayerPrefs.GetInt("EnergiaActual");
        TiempoEnergiaProx = StringToDate(PlayerPrefs.GetString("TiempoEnergiaProx"));
        TiempoEnergiaAnt = StringToDate(PlayerPrefs.GetString("TiempoEnergiaAnt"));
    }

    private void Save() 
    {
        PlayerPrefs.SetInt("EnergiaActual", EnergiaActual);
        PlayerPrefs.SetString("TiempoEnergiaProx", TiempoEnergiaProx.ToString());
        PlayerPrefs.SetString("TiempoEnergiaAnt", TiempoEnergiaAnt.ToString());
    }
}
