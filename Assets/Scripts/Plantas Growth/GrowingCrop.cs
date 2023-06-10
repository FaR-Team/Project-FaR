using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingCrop : GrowingBase
{
    [SerializeField] private CropSaveData cropSaveData;
    private string id;

    void Awake()
    {
        SaveLoad.OnLoadGame += LoadGame;
        cropSaveData = new CropSaveData(Dia, transform.position);
    }

    public override void Start()
    {
        base.Start();

        id = GetComponent<UniqueID>().ID;

        if (SaveGameManager.data.cropDictionary.ContainsKey(id)) return;
        else
        {
            SaveGameManager.data.cropDictionary.Add(id, cropSaveData);
        }

    }

   public virtual void Update()
    {
        if (ClockManager.TimeText() == "05:00 AM" && yacrecio == false)
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
