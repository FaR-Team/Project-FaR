using UnityEngine;
using Random = UnityEngine.Random;
public class GridGhost : MonoBehaviour
{
    public static GridGhost instance { get; private set; }
    public bool PlantDirtBool { get; private set; }
    public bool PlantTreeNearBool { get; private set; }

    [SerializeField] private Interactor interactor;
    public HotbarDisplay hotbarDisplay;
    public GameObject dirtGhost, seedGhost;
    public Grid grid;
    public GameObject DirtPrefab;


    public Vector3 finalPosition;

    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private LayerMask layerDirt;
    [SerializeField]
    private LayerMask layerCrop;

    [SerializeField, Tooltip("La distancia máxima donde se puede arar")]
    private float _maxGrabDistance;


    public static int SeedRotationValue = 0;


    void Start()
    {
        grid = FindObjectOfType<Grid>();
        if (instance != this || instance == null)
        {
            instance = this;
        }
        SeedRotationValue = RandomPos();
    }
    private void FixedUpdate()
    {
        CastingCommander.jobHandle.Complete();
        if (PauseMenu.GameIsPaused) return;

        if (GetItemData() == null)
        {
            dirtGhost.SetActive(false);
            return;
        }
        RaycastHit raycastHit = CastingCommander.rayCastHits[0];
        PutDirtGhost(raycastHit);

        if (raycastHit.collider != null)
        {
            PlantDirtRayHit(raycastHit);
        }
        SeedGhost(raycastHit.point);

    }

    private static int RandomPos()
    {
        return Random.Range(0, 4);
    }
    private InventoryItemData GetItemData()
    {
        return hotbarDisplay.slots[hotbarDisplay._currentIndex].AssignedInventorySlot.ItemData;
    }

    private void PlantDirtRayHit(RaycastHit raycastHit)
    {
        if (!PlantDirtBool) return;

        PlaceDirtNear(raycastHit.point);
        PlantDirtBool = false;
    }

    private void PutDirtGhost(RaycastHit raycastHit)
    {
        if (GetItemData() != null &&
            GetItemData().IsHoe &&
            !CastingCommander.IsLookingAtDirt())
        {
            if (raycastHit.collider == null)
            {
                dirtGhost.SetActive(false);
            }
            else
            {
                finalPosition = grid.GetNearestPointOnGrid(raycastHit.point);
                dirtGhost.SetActive(true);
                dirtGhost.transform.position = finalPosition; //Pone el fantasma de la tierra en el lugar que debe estar.
            }
        }
        else
        {
            dirtGhost.SetActive(false);
        }
    }

    public static Quaternion Rotation()
    {
        return Quaternion.Euler(0, SeedRotationValue * 90, 0);
    }

    public static void UpdateRandomSeed()
    {
        Rotation();
        SeedRotationValue = RandomPos();
    }

    public Dirt CheckDirt(Vector3 center, float radius)
    {
        int maxColliders = 5;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(center, radius, hitColliders, layerDirt);


        if (numColliders >= 1)
        {
            var dirt = hitColliders[0].GetComponentInParent<Dirt>();
            if (dirt != null)
            {
                return dirt;
            }

            return null;
        }
        else
        {
            return null;
        }
    }

    public bool CheckCrop(Vector3 center, float radius)
    {
        int maxColliders = 5;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(center, radius, hitColliders, layerCrop);
        if (numColliders >= 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void PlantDirt()
    {
        PlantDirtBool = true;
    }

    private void PlaceDirtNear(Vector3 nearPoint)
    {
        finalPosition = grid.GetNearestPointOnGrid(nearPoint);
        DirtSpawnerPooling.SpawnObject(finalPosition, Quaternion.identity);
    }

    public bool PlantTreeNear(GameObject TreePrefab)
    {
        RaycastHit raycastHit = CastingCommander.rayCastHits[0];
        if (raycastHit.collider != null)
        {
            GameObject.Instantiate(TreePrefab, finalPosition, Quaternion.identity, raycastHit.transform.parent.gameObject.transform);
            return true;
        }
        else
        {
            return false;
        }
    }
    private void SeedGhost(Vector3 hitPoint)
    {

        /*      
        * Si no tiene semilla en la mano O 
        * Si la semilla es cultivo y no estas mirando tierra O
        * Si la semilla es arbol y estas mirando a una tierra. 
        * Entonces desactiva la seed y no ejecuta el código. 
        */
        if (GetItemData() == null ||
            GetItemData().Seed && !CastingCommander.IsLookingAtDirt() ||
            GetItemData().TreeSeed && CastingCommander.IsLookingAtDirt() ||
            GetItemData().IsHoe)
        {
            seedGhost.SetActive(false);
            return;
        }

        finalPosition = grid.GetNearestPointOnGrid(hitPoint);

        if (!hotbarDisplay.CanUseItem() && GetItemData().Seed || !CheckCrop(finalPosition, 1) && GetItemData().TreeSeed)
        {
            seedGhost.SetActive(false);
            return;
        }

        ActivateSeedGhost();

        //seedGhost se activa en la escena, se lo pone en la posicion adecuada y se le cambia la textura por la adecuada.
    }

    private void ActivateSeedGhost()
    {
        seedGhost.transform.rotation = Rotation();
        seedGhost.SetActive(true);
        seedGhost.transform.position = finalPosition;
        seedGhost.GetComponentInChildren<MeshFilter>().mesh = GetItemData().ghostMesh;
    }

}
