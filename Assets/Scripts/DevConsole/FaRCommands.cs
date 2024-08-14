using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.Systems.DateTime;
using FaRUtils.Systems.ItemSystem;
using FaRUtils.FPSController;
using UnityStandardAssets.CrossPlatformInput;
using IngameDebugConsole;
using UnityEngine.Serialization;

public class FaRCommands : MonoBehaviour
{
	public float Sensitivity {
		get { return sensitivity; }
		set { sensitivity = value; }
	}
	[SerializeField] float sensitivity = 1.0f;

	public int actualDay;
	private bool skipdays;
	private bool areYouSleepy;
	private int daysToSkip;
	public GameObject player;
	public Rigidbody rb;
	public Camera cam;
    [FormerlySerializedAs("_cama")] public SleepHandler sleepHandler;
    public Database _database;
	public bool _noclip;
	private Camera m_Camera;

	public DirtSpawnerPooling dirtSpawner;

	void OnEnable()
	{
		DateTime.OnHourChanged.AddListener(OnHourChanged);
	}

	void OnDisable()
	{
		DateTime.OnHourChanged.AddListener(OnHourChanged);
	}

	void Start()
	{
		m_Camera = Camera.main;
		rb = player.GetComponent<Rigidbody>();
		DebugLogConsole.AddCommand<int, int>( "give", "Te da un item con el número de ID que especifiques", GiveItem );
		DebugLogConsole.AddCommand( "rosebud", "te da 1000 de oro", Rosebud );
		DebugLogConsole.AddCommand<int>( "add_gold", "te da la cantidad de oro que escribas", AddGold);
		DebugLogConsole.AddCommand( "hurrypotter", "Avanza muy rápido el tiempo", HurryPotter);
		DebugLogConsole.AddCommand( "hurrypotterslower", "Avanza rápido el tiempo (pero no tan rápido)", HurryPotterSlower);
		DebugLogConsole.AddCommand( "relaxpotter", "Vuelve el tiempo a la normalidad", RelaxPotter);
		DebugLogConsole.AddCommand( "imsleepy", "Avanza el tiempo hasta que puedas dormir", ImSleepy);
		DebugLogConsole.AddCommand("noclip", "Noclip, no es tan difícil de entender", Noclip);
		DebugLogConsole.AddCommand("skipcarrotgrowth", "Se salta el crecimiento de la zanahoria, avanzando los días necesarios", SkipCarrotGrowth);
		DebugLogConsole.AddCommand("skipapplegrowth", "Se salta el crecimiento de la manzana, avanzando los días necesarios", SkipAppleGrowth);
		DebugLogConsole.AddCommand("skipstrawberrygrowth", "Se salta el crecimiento de la frutilla, avanzando los días necesarios", SkipStrawberryGrowth);
		DebugLogConsole.AddCommand("skiptomatogrowth", "Se salta el crecimiento del tomate, avanzando los días necesarios", SkipTomatoGrowth);
		DebugLogConsole.AddCommand<int>("setharvestlevel", "Aumenta el nivel de AreaHarvest", SetAreaHarvestLevel);
		DebugLogConsole.AddCommand("givePants", "le da pantalones", GivePants);
		DebugLogConsole.AddCommand("giveShirt", "le da pantalones", GiveShirt);
		DebugLogConsole.AddCommand("save", "le da pantalones", TestSave);
		//DebugLogConsole.AddCommand("load", "le da pantalones", TestLoad);
	}

    private void SetAreaHarvestLevel(int x)
    {
        PlayerStats.Instance.areaHarvestLevel = x;
    }

	private void TestSave()
    {
		SleepHandler.Instance.SaveDataEvent.Invoke(false);
    }

	public void Noclip()
    {
        if(!_noclip)
        {
            rb.useGravity = false;
			player.GetComponent<CharacterController>().enabled = false;
            _noclip = true;
        }
        else
        {
            rb.useGravity = true;
			player.GetComponent<CharacterController>().enabled = true;
            _noclip = false; 
        }
    }

	void GiveItem(int x, int y)
	{
		ItemPickUp.GiveItem(_database.GetItem(x), y);
	}

	void Rosebud()
	{
		player.GetComponent<PlayerInventoryHolder>().PrimaryInventorySystem.GainGold(1000);
	}

	void AddGold(int amount)
	{
		player.GetComponent<PlayerInventoryHolder>().PrimaryInventorySystem.GainGold(amount);
	}

	void HurryPotter()
    {

        TimeManager.TimeBetweenTicks = 0.01f;
        SetTestingAndIsWet(true, true);
        sleepHandler._yourLetterArrived = true;
    }
	
	void HurryPotterSlower()
	{

		TimeManager.TimeBetweenTicks = 0.1f;
		SetTestingAndIsWet(true, true);
		sleepHandler._yourLetterArrived = true;
	}

    private void SetTestingAndIsWet(bool test, bool isWet)
    {
        foreach (GameObject dirt in dirtSpawner.GetActiveDirts())
        {
            dirt.GetComponent<Dirt>().testing = test;
            dirt.GetComponent<Dirt>()._isWet = isWet;
        }
    }

    void RelaxPotter()
    {
	    TimeManager.TimeBetweenTicks = sleepHandler._isSleeping ? sleepHandler.SleepingTickRate : 10f;
        SetTestingAndIsWet(false, false);

        sleepHandler._yourLetterArrived = false;
        sleepHandler.lightingManager.CopyHour();
	}

	void ImSleepy()
	{
		TimeManager.TimeBetweenTicks = 0.01f;
		areYouSleepy = true;
	}

	private void OnHourChanged(int hour)
	{
		if (hour >= 17 && areYouSleepy)
		{
			TimeManager.TimeBetweenTicks = 10f;
        	sleepHandler.lightingManager.CopyHour();
			areYouSleepy = false;
		}
	}

	void SkipCarrotGrowth()
    {
        actualDay = TimeManager.DateTime.Date;
        daysToSkip = 6;
        skipdays = true;
        SetTestingDirt(true);
        TimeManager.TimeBetweenTicks = 0.01f;
    }

    private void SetTestingDirt(bool testing)
    {
        foreach (GameObject dirt in dirtSpawner.GetActiveDirts())
        {
            dirt.GetComponent<Dirt>().testing = testing;
        }
    }

    void SkipAppleGrowth()
	{
		actualDay = TimeManager.DateTime.Date;
		daysToSkip = 4;
		skipdays = true;
        SetTestingDirt(true);

        TimeManager.TimeBetweenTicks = 0.01f;
	}

	void SkipStrawberryGrowth()
	{
		actualDay = TimeManager.DateTime.Date;
		daysToSkip = 3;
		skipdays = true;
        SetTestingDirt(true);

        TimeManager.TimeBetweenTicks = 0.01f;
	}

	void SkipTomatoGrowth()
	{
		//Saltar el crecimiento del tomate, avanzando rápido los días necesarios
	}

	void GivePants()
	{
		PlayerStats.hasPants = true;
	}

	void GiveShirt()
	{
		PlayerStats.hasShirt = true;
	}
    public void Update()
	{
		if (_noclip)
		{
			if(Input.GetKey(KeyCode.W))
			{
				player.transform.Translate(Vector3.forward * Time.deltaTime * 20f);
			}
			if(Input.GetKey(KeyCode.A))
			{
				player.transform.Translate(Vector3.left * Time.deltaTime * 20f);
			}
			if(Input.GetKey(KeyCode.S))
			{
				player.transform.Translate(Vector3.back * Time.deltaTime * 20f);
			}
			if(Input.GetKey(KeyCode.D))
			{
				player.transform.Translate(Vector3.right * Time.deltaTime * 20f);
			}
			if(Input.GetKey(KeyCode.Space))
			{
				player.transform.Translate(Vector3.up * Time.deltaTime * 20f);
			}
			if(Input.GetKey(KeyCode.LeftShift))
			{
				player.transform.Translate(Vector3.down * Time.deltaTime * 20f);
			}

			player.GetComponent<FaRCharacterController>().DoLooking();
		}

		if (skipdays)
		{
			if ((actualDay + daysToSkip) == TimeManager.DateTime.Date)
			{
				TimeManager.TimeBetweenTicks = 10f;
				foreach(GameObject dirt in dirtSpawner.GetActiveDirts())
				{
					dirt.GetComponent<Dirt>().testing = false;
				}
				skipdays = false;
			}
		}
	}
}