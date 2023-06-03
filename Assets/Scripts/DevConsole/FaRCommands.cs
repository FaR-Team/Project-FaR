using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.Systems.DateTime;
using FaRUtils.Systems.ItemSystem;
using FaRUtils.FPSController;
using UnityStandardAssets.CrossPlatformInput;
using IngameDebugConsole;
using System;

public class FaRCommands : MonoBehaviour
{
	public float Sensitivity {
		get { return sensitivity; }
		set { sensitivity = value; }
	}
	[Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
	[Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

	Vector2 rotation = Vector2.zero;
	const string xAxis = "Mouse X";
	const string yAxis = "Mouse Y";

	public int actualDay;
	private bool skipdays;
	private int daysToSkip;
	public GameObject player;
    public GameObject TimeManager;
	public Rigidbody rb;
    public Cama _cama;
    public Database _database;
	public bool _noclip;
	private Camera m_Camera;

	void Start()
	{
		m_Camera = Camera.main;
		rb = player.GetComponent<Rigidbody>();
		DebugLogConsole.AddCommand<int, int>( "give", "Te da un item con el número de ID que especifiques", GiveItem );
		DebugLogConsole.AddCommand( "rosebud", "te da 1000 de oro", Rosebud );
		DebugLogConsole.AddCommand<int>( "add_gold", "te da la cantidad de oro que escribas", AddGold);
		DebugLogConsole.AddCommand( "hurrypotter", "Avanza muy rápido el tiempo", HurryPotter);
		DebugLogConsole.AddCommand( "relaxpotter", "Vuelve el tiempo a la normalidad", RelaxPotter);
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
        PlayerStats.Instance.AreaHarvestLevel = x;
    }

	public void Noclip()
    {
        if(!_noclip)
        {
            rb.useGravity = false;
            player.GetComponent<FaRCharacterController>().enabled = false;
			player.GetComponent<CharacterController>().enabled = false;
            _noclip = true;
        }
        else
        {
            rb.useGravity = true;
			player.GetComponent<FaRCharacterController>().enabled = true;
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
		TimeManager.GetComponent<TimeManager>().TimeBetweenTicks = 0.01f;
        _cama._yourLetterArrived = true;
	}

	void RelaxPotter()
	{
		TimeManager.GetComponent<TimeManager>().TimeBetweenTicks = 10f;
        _cama._yourLetterArrived = false;
        _cama.lightingManager.CopyHour();
	}

	void SkipCarrotGrowth()
	{
		actualDay = TimeManager.GetComponent<TimeManager>().DateTime.Date;
		daysToSkip = 3;
		skipdays = true;
		TimeManager.GetComponent<TimeManager>().TimeBetweenTicks = 0.01f;
	}

	void SkipAppleGrowth()
	{
		actualDay = TimeManager.GetComponent<TimeManager>().DateTime.Date;
		daysToSkip = 4;
		skipdays = true;
		TimeManager.GetComponent<TimeManager>().TimeBetweenTicks = 0.01f;
	}

	void SkipStrawberryGrowth()
	{
		actualDay = TimeManager.GetComponent<TimeManager>().DateTime.Date;
		daysToSkip = 7;
		skipdays = true;
		TimeManager.GetComponent<TimeManager>().TimeBetweenTicks = 0.01f;
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

			rotation.x += Input.GetAxis(xAxis) * sensitivity;
			rotation.y += Input.GetAxis(yAxis) * sensitivity;
			rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
			var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
			var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

			player.transform.localRotation = xQuat * yQuat;
		}

		if (skipdays)
		{
			if ((actualDay + daysToSkip) == TimeManager.GetComponent<TimeManager>().DateTime.Date)
			{
				TimeManager.GetComponent<TimeManager>().TimeBetweenTicks = 10f;
				skipdays = false;
			}
		}
	}
}