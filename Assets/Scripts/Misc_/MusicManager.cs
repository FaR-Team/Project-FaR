using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject Reloj;
    public bool yasonlas6am = false;
    public bool yasonlas12pm = false;
    public bool yasonlas6pm = false;
    public bool YaEligioHoy = false;
    public AudioClip Pos1, Pos2, Pos3, Pos4, Pos5;

    public Slider slider;
    public float volumen;


    private void Update() {
        if(Reloj.GetComponent<ClockManager>().Time.text == "06:00 AM" && yasonlas6am == false || Reloj.GetComponent<ClockManager>().Time.text == "06:00" && yasonlas6am == false)
        {
            if (YaEligioHoy == false)
            {
                BuscarElQuesoDelGobierno();
            }
            yasonlas6am = true;
        }

        if(Reloj.GetComponent<ClockManager>().Time.text == "12:00 PM" && yasonlas12pm == false || Reloj.GetComponent<ClockManager>().Time.text == "12:00" && yasonlas12pm == false && Cama.Instance._isSleeping == false)
        {
            if (YaEligioHoy == false)
            {
                BuscarElQuesoDelGobierno();
            }
            yasonlas12pm = true;
        }

        if(Reloj.GetComponent<ClockManager>().Time.text == "06:00 PM" && yasonlas6pm == false || Reloj.GetComponent<ClockManager>().Time.text == "18:00" && yasonlas6pm == false && Cama.Instance._isSleeping == false)
        {
            if (YaEligioHoy == false)
            {
                BuscarElQuesoDelGobierno();
            }
            yasonlas6pm = true;
        }

        if(Reloj.GetComponent<ClockManager>().Time.text == "05:00 AM" && yasonlas6am == true || Reloj.GetComponent<ClockManager>().Time.text == "05:00" && yasonlas6am == true )
        {
            yasonlas6am = false;
            yasonlas12pm = false;
            yasonlas6pm = false;
            YaEligioHoy = false;
        }
        audioSource.volume = volumen = slider.value;
    }
    public void BuscarElQuesoDelGobierno()
    {
        var rand = Random.Range(1,5);
        switch(rand) {
            case 1: 
                audioSource.clip = Pos1;
                
                audioSource.Play();
                YaEligioHoy = true;
                break;
            case 2: 
                audioSource.clip = null;
                break;
            case 3: 
                audioSource.clip = Pos1;
                audioSource.Play();
                YaEligioHoy = true;
                break;
            case 4: 
                audioSource.clip = null;
                break;
            case 5: 
                audioSource.clip = Pos5;
                audioSource.Play();
                YaEligioHoy = true;
                break;
            default:
                Debug.LogError($"La cagaste bro");
                break;
        }
    }
}
