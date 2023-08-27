using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingTuber : GrowingBase
{
    [SerializeField] private CropSaveData cropSaveData;
    private string id;
    public Dirt tierra;
    public GameObject interactablePrefab;
    [HideInInspector] public SkinnedMeshRenderer skinnedMeshRenderer;

    void Awake()
    {
        SaveLoad.OnLoadGame += LoadGame;
        cropSaveData = new CropSaveData(DiasPlantado, transform.position, id);
        tierra = transform.parent.GetComponent<Dirt>();
    }

    public override void Start()
    {
        base.Start();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
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
            if(IsLastPhase(i))
            {
                SetInteractable(i);
                
                break;
            }

            int valueToGet = System.Array.IndexOf(DayForChangeOfPhase, i);
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = meshes[valueToGet];
            }
            skinnedMeshRenderer.sharedMesh = meshes[valueToGet];
            skinnedMeshRenderer.material = materials[valueToGet];
            return;
        }
    }
    /* public bool IsLastPhase(int numero)
    {
        if (DayForChangeOfPhase.Length == 0)
        {
            return false;
        }

        int ultimoElemento = DayForChangeOfPhase[DayForChangeOfPhase.Length - 1];
        return numero == ultimoElemento;
    }
    */ 
    public override void SetInteractable(int i)
    {
        Instantiate(interactablePrefab, transform.position, Quaternion.identity, transform);
        skinnedMeshRenderer.sharedMesh = null;
        skinnedMeshRenderer.material = null;
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
