using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingCrop : GrowingBase
{
    [SerializeField] private CropSaveData cropSaveData;
    private string id;
    public Dirt tierra;

    void Awake()
    {
        SaveLoad.OnLoadGame += LoadGame;
        cropSaveData = new CropSaveData(Dia, transform.position);
        tierra = transform.parent.GetComponent<Dirt>();
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

    public override void OnHourChanged(int hour)
    {
        if (!tierra._isWet || hour != 5) return;
        
        Dia++;
        CheckDayGrow();
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
