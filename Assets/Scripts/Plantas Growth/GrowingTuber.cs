using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UniqueID))]
public class GrowingTuber : GrowingBase { 

    private string id;
    public Dirt tierra;
    public GameObject interactablePrefab;
    [HideInInspector] public SkinnedMeshRenderer skinnedMeshRenderer;

    void Awake()
    {
        tierra = transform.parent.GetComponent<Dirt>();
    }

    public override void Start()
    {
        base.Start();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        id = GetComponent<UniqueID>().ID;

    }
    public override void OnHourChanged(int hour)
    {
        if (!tierra._isWet || hour != 4) return;
        DiasPlantado++; //ESTO DEBERIA SER UN EVENTO. DE DIAS PLANTADO Y CHECKDAYGROW SE SUSCRIBE A ESTE.
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
   
    public override void SetInteractable(int i)
    {
        Instantiate(interactablePrefab, transform.position, Quaternion.identity, transform);
        skinnedMeshRenderer.sharedMesh = null;
        skinnedMeshRenderer.material = null;
    }


    

   
}
