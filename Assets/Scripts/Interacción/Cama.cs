using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using FaRUtils.FPSController;
using FaRUtils.Systems.DateTime;

public class Cama : MonoBehaviour, IInteractable
{
    public static Cama Instance;

    [SerializeField] private GameObject _prompt;
    public SellSystem _sellSystem;
    public GameObject Negrura;
    public GameObject Hotbar;
    public GameObject player;
    public LightingManager lightingManager;
    public bool yasonlas6 = false;
    public bool _isSleeping = false;
    public bool _yourLetterArrived = false;

    //I DON'T KNOW SWAHILI, BUT I THINK THIS IS THE CORRECT WAY TO DO THIS (Last part was written by github copilot.)
    public GameObject InteractionPrompt => _prompt;

    
    private void Awake() {
        _prompt = GameObject.FindGameObjectWithTag("BedInteraction");

        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    private void Start() 
    {
        DateTime.OnHourChanged.AddListener(OnHourChanged);
    }

    private void OnHourChanged(int hour)
    {
        if (hour != 6) return;

        if(_yourLetterArrived == false)
        {
            TimeManager.TimeBetweenTicks = 10f;
        }
        lightingManager.CopyHour();
        Negrura.GetComponent<Animation>().Play("NegroOut");
        yasonlas6 = true;
        player = GameObject.FindWithTag("Player");
        player.GetComponent<FaRCharacterController>().enabled = true;
        StartCoroutine(Wait());
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        if (_isSleeping)
        {
            Debug.Log("Ya estás durmiendo");
            interactSuccessful = false;
            return;
        }
        else if(TimeManager.DateTime.Hour >= 6 && TimeManager.DateTime.Hour < 17)
        {
            Debug.Log("Es muy temprano para dormir");
            interactSuccessful = false;
            return;
        }
        

        if(_yourLetterArrived == false)
        {
            TimeManager.TimeBetweenTicks = 0.05f;
        }
        player = GameObject.FindWithTag("Player");
        Negrura.SetActive(true);
        Negrura.GetComponent<Animation>().Play("NegroIn");
        //THIS SHITTY CODE TOOK ME A WHOLE WEEK, I CRIED 5 NIGHTS, AND KILLED 7 DOGS BECAUSE OF MY STRESS, btw, if you're reading this, tell me witch errors you find.
        Energy.RemainingEnergy = 100;
        Energy.UpdateEnergy();
        player.GetComponent<FaRCharacterController>().enabled = false;
        yasonlas6 = false;
        _isSleeping = true;

        _sellSystem.Sell();


        Debug.Log("Interactuando con Cama");
        interactSuccessful = true;
    }

    public void InteractOut()
    {
        Debug.Log(null);
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.7f);
        Negrura.SetActive(false);
        _isSleeping = false;
    }

    public void EndInteraction()
    {
        Debug.Log("Terminando interacción con Cama");
    }
}
