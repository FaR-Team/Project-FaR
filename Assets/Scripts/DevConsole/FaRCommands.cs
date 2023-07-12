using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.Systems.DateTime;
using FaRUtils.Systems.ItemSystem;
using FaRUtils.FPSController;
using UnityStandardAssets.CrossPlatformInput;
using IngameDebugConsole;

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
    public Cama _cama;
    public Database _database;
	public bool _noclip;
	private Camera m_Camera;

	public DirtSpawnerPooling dirtSpawner;

	void Start()
	{
		m_Camera = Camera.main;
		rb = player.GetComponent<Rigidbody>();
		DateTime.OnHourChanged.AddListener(OnHourChanged);
		DebugLogConsole.AddCommand<int, int>( "give", "Te da un item con el número de ID que especifiques", GiveItem );
		DebugLogConsole.AddCommand( "rosebud", "te da 1000 de oro", Rosebud );
		DebugLogConsole.AddCommand<int>( "add_gold", "te da la cantidad de oro que escribas", AddGold);
		DebugLogConsole.AddCommand( "hurrypotter", "Avanza muy rápido el tiempo", HurryPotter);
		DebugLogConsole.AddCommand( "relaxpotter", "Vuelve el tiempo a la normalidad", RelaxPotter);
		DebugLogConsole.AddCommand( "imsleepy", "Avanza el tiempo hasta que puedas dormir", ImSleepy);
		DebugLogConsole.AddCommand("noclip", "Noclip, no es tan difícl de entender", Noclip);
		DebugLogConsole.AddCommand("skipcarrotgrowth", "Se salta el crecimiento de la zanahoria, avanzando los días necesarios", SkipCarrotGrowth);
		DebugLogConsole.AddCommand("skipapplegrowth", "Se salta el crecimiento de la manzana, avanzando los días necesarios", SkipAppleGrowth);
		DebugLogConsole.AddCommand("skipstrawberrygrowth", "Se salta el crecimiento de la frutilla, avanzando los días necesarios", SkipStrawberryGrowth);
		DebugLogConsole.AddCommand("skiptomatogrowth", "Se salta el crecimiento del tomate, avanzando los días necesarios", SkipTomatoGrowth);
		DebugLogConsole.AddCommand<int>("setharvestlevel", "Aumenta el nivel de AreaHarvest", SetAreaHarvestLevel);
		DebugLogConsole.AddCommand("givePants", "le da pantalones", GivePants);
		DebugLogConsole.AddCommand("giveShirt", "le da pantalones", GiveShirt);
	}

    private void SetAreaHarvestLevel(int x)
    {
        PlayerStats.Instance.areaHarvestLevel = x;
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
		foreach(GameObject dirt in dirtSpawner.GetActiveDirts())
		{
			dirt.GetComponent<Dirt>().testing = true;
		}
        _cama._yourLetterArrived = true;
	}

	void RelaxPotter()
	{
		TimeManager.TimeBetweenTicks = 10f;
		foreach(GameObject dirt in dirtSpawner.GetActiveDirts())
		{
			dirt.GetComponent<Dirt>().testing = false;
		}
        _cama._yourLetterArrived = false;
        _cama.lightingManager.CopyHour();
	}

	void ImSleepy()
	{
		TimeManager.TimeBetweenTicks = 0.01f;
		areYouSleepy = true;
	}

	private void OnHourChanged(int hour)
	{
		if (hour == 17 && areYouSleepy)
		{
			TimeManager.TimeBetweenTicks = 10f;
        	_cama.lightingManager.CopyHour();
			areYouSleepy = false;
		}
	}

	void SkipCarrotGrowth()
	{
		actualDay = TimeManager.DateTime.Date;
		daysToSkip = 6;
		skipdays = true;
		foreach(GameObject dirt in dirtSpawner.GetActiveDirts())
		{
			dirt.GetComponent<Dirt>().testing = true;
		}
		TimeManager.TimeBetweenTicks = 0.01f;
	}

	void SkipAppleGrowth()
	{
		actualDay = TimeManager.DateTime.Date;
		daysToSkip = 4;
		skipdays = true;
		foreach(GameObject dirt in dirtSpawner.GetActiveDirts())
		{
			dirt.GetComponent<Dirt>().testing = true;
		}
		TimeManager.TimeBetweenTicks = 0.01f;
	}

	void SkipStrawberryGrowth()
	{
		actualDay = TimeManager.DateTime.Date;
		daysToSkip = 3;
		skipdays = true;
		foreach(GameObject dirt in dirtSpawner.GetActiveDirts())
		{
			dirt.GetComponent<Dirt>().testing = true;
		}
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