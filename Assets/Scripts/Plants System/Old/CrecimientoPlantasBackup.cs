using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class CrecimientoPlantasBackup : MonoBehaviour //CrecimientoFruta
{
    public GameObject Fase1, Fase2, Fase3, Fase4, Fase5, Fase6, Fase7, Fase8, Fase9, Fase10;

    public GameObject Reloj;
    public int Dia;
    public bool yacrecio;

    [SerializeField] private CropSaveData cropSaveData;
    private string id;

    public int Int1, Int2, Int3, Int4, Int5, Int6, Int7, Int8, Int9;

    void Awake()
    {
        SaveLoad.OnLoadGame += LoadGame;
        cropSaveData = new CropSaveData(Dia, transform.position);
    }
    
    void Start()
    {
        Fase1.SetActive(true);
        Reloj = GameObject.FindGameObjectWithTag("Reloj");
        Dia = 0;
        yacrecio = false;

        id = GetComponent<UniqueID>().ID;

        if (SaveGameManager.data.cropDictionary.ContainsKey(id)) return;
        else
        {
            SaveGameManager.data.cropDictionary.Add(id, cropSaveData);
        }  
        
    }

    void Update()
    {
        if(ClockManager.TimeText() == "05:00 AM" && yacrecio == false)
        {
            Dia += 1;
            yacrecio = true;
            CheckDayGrow();
        }

        if (ClockManager.TimeText() == "06:00 AM" && yacrecio == true)
        {
            yacrecio = false;
        }
    }

    public void CheckDayGrow()
    {
        if(Dia == Int1 && yacrecio == true)
        {
            Fase1.SetActive(false);
            Fase2.SetActive(true);
        }

        if (Fase3 == null)
        {
            return;
        }
        
        if(Dia == Int2 && yacrecio == true)
        {
            Destroy(Fase1);
            Fase2.SetActive(false);
            Fase3.SetActive(true);
        }
        
        if (Fase4 == null)
        {
            return;
        }

        if(Dia == Int3 && yacrecio == true)
        {
            Destroy(Fase2);
            Fase3.SetActive(false);
            Fase4.SetActive(true);
        }

        if (Fase5 == null)
        {
            return;
        }
        
        if(Dia == Int4 && yacrecio == true)
        {
            Destroy(Fase3);
            Fase4.SetActive(false);
            Fase5.SetActive(false);
        }

        if (Fase6 == null)
        {
            return;
        }

        if(Dia == Int5 && yacrecio == true)
        {
            Destroy(Fase4);
            Fase5.SetActive(false);
            Fase6.SetActive(false);
        }

        if (Fase7 == null)
        {
            return;
        }

        if(Dia == Int6 && yacrecio == true)
        {
            Destroy(Fase5);
            Fase6.SetActive(false);
            Fase7.SetActive(false);
        }

        if (Fase8 == null)
        {
            return;
        }

        if(Dia == Int7 && yacrecio == true)
        {
            Destroy(Fase6);
            Fase7.SetActive(false);
            Fase8.SetActive(false);
        }

        if (Fase9 == null)
        {
            return;
        }

        if(Dia == Int8 && yacrecio == true)
        {
            Destroy(Fase7);
            Fase8.SetActive(false);
            Fase9.SetActive(false);
        }

        if (Fase10 == null)
        {
            return;
        }

        if(Dia == Int9 && yacrecio == true)
        {
            Destroy(Fase8);
            Fase9.SetActive(false);
            Fase10.SetActive(false);
        }
    }

    private void LoadGame(SaveData data)
    {
        if (data.cropDictionary.ContainsKey(id))
        {
            //Destroy(this.gameObject);
        }
    }

    private void OnDestroy() 
    {
        if (SaveGameManager.data.cropDictionary.ContainsKey(id))
        {
            SaveGameManager.data.cropDictionary.Remove(id);
            SaveLoad.OnLoadGame -= LoadGame;
        }
    }
}
