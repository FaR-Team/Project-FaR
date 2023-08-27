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
        cropSaveData = new CropSaveData(DiasPlantado, transform.position, id);
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
        if (!tierra._isWet || hour != 4) return;

        DiasPlantado++;
        CheckDayGrow();
    }
    public override void CheckDayGrow()
    {
        foreach (int i in DayForChangeOfPhase)
        {
            if (DiasPlantado != i) continue;

            SetInteractable(i);

            int valueToGet = System.Array.IndexOf(DayForChangeOfPhase, i);
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = meshes[valueToGet];
            }
            meshFilter.mesh = meshes[valueToGet];
            meshRenderer.material = materials[valueToGet];
            return;
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
