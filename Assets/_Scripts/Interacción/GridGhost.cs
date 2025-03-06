using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class GridGhost : MonoBehaviour
{
    public static GridGhost instance { get; private set; }
    [SerializeField] private Interactor interactor;
    public HotbarDisplay hotbarDisplay;
    public GameObject hoeGhost, seedGhost;
    public RayAndSphereManager rayAndSphereManager;
    public Grid grid;


    public Material ghostMaterial;
    public Material noEnergyGhostMaterial;

    public Vector3 finalPosition;

    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private LayerMask layerDirt;
    [SerializeField]
    private LayerMask layerCrop;

    [SerializeField, Tooltip("La distancia máxima donde se puede arar")]
    private float _maxPlowDistance;

    public static int SeedRotationValue = 0;

    private void Awake()
    {
        if (instance != this || instance == null)
        {
            instance = this;
        }

        hotbarDisplay = FindObjectOfType<HotbarDisplay>();
        if(hotbarDisplay != null)
        {
            hotbarDisplay.SetGridGhost(this);
        }

        interactor = FindObjectOfType<Interactor>();
        rayAndSphereManager = FindObjectOfType<RayAndSphereManager>();
    }

    private void OnEnable()
    {
        Energy.OnEnergyUpdated += HandleRemainingEnergy;
    }

    private void OnDisable()
    {
        Energy.OnEnergyUpdated -= HandleRemainingEnergy;
    }

    void Start()
    {
        SeedRotationValue = RandomPos();
    }


    private static int RandomPos()
    {
        return Random.Range(0, 4);
    }
    private InventoryItemData GetItemData()
    {
        return hotbarDisplay.slots[HotbarDisplayBase._currentIndex].AssignedInventorySlot.ItemData;
    }

    void FixedUpdate()
    {
        if (PauseMenu.GameIsPaused) return;

        HandleHoeGhost();
        HandleSeedGhost();
    }

    public bool CheckAvailableSpace(Vector3 center, Vector3 requiredSpace)
    {            
        int maxColliders = 5;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapBoxNonAlloc(center, requiredSpace * 0.5f, hitColliders, Quaternion.identity, layerCrop);
        
        
        return numColliders == 0;
    }

    private void HandleHoeGhost()
    {
        if (GetItemData() == null || !GetItemData().IsHoe())
        {
            hoeGhost.SetActive(false);
            return;
        }
        
        RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out RaycastHit hit, _maxPlowDistance - 3, layerMask);
        
        if (hit.collider == null)
        {
            hoeGhost.SetActive(false);
            return;
        }
        
        finalPosition = grid.GetNearestPointOnGrid(hit.point);
        hoeGhost.SetActive(true);
        hoeGhost.transform.position = finalPosition;
        
        bool isDirtAlreadyHere = CheckDirt(finalPosition, 0.5f) != null;
        bool noCropPresent = CheckCrop(finalPosition, 0.5f);
        bool canPlow = !isDirtAlreadyHere && !interactor._LookingAtDirt && noCropPresent;
        
        if (canPlow)
        {
            MakeGridGhostAvaliable();
        }
        else
        {
            MakeGridGhostUnavaliable();
        }
    }

    private void HandleSeedGhost()
    {
        if (GetItemData() == null || 
            GetItemData().IsHoe() || 
            (!GetItemData().IsCropSeed() && !GetItemData().IsTreeSeed()))
        {
            seedGhost.SetActive(false);
            return;
        }

        RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out RaycastHit hit, _maxPlowDistance, layerMask);
        if (hit.collider == null)
        {
            seedGhost.SetActive(false);
            return;
        }

        finalPosition = grid.GetNearestPointOnGrid(hit.point);
        
        bool shouldShowGhost = false;
        bool canPlant = false;
        var hasCrop = false;

        Vector3 requiredSpace = GetItemData().RequiredGridSpace;
        
        if (GetItemData().IsCropSeed())
        {
            shouldShowGhost = interactor._LookingAtDirt;
            var currentCheckedDirt = CheckDirt(finalPosition, 0.5f);
            if (shouldShowGhost && currentCheckedDirt != null)
            {
                hasCrop = currentCheckedDirt.currentCrop != null;
            }
            canPlant = shouldShowGhost && hotbarDisplay.CanUseItem() && !hasCrop;
        }
        else if (GetItemData().IsTreeSeed()) 
        {
            shouldShowGhost = true;
            
            // Can only plant on non-dirt areas with enough space
            canPlant = !interactor._LookingAtDirt && CheckAvailableSpace(finalPosition, requiredSpace);
        }
        
        seedGhost.SetActive(shouldShowGhost);
        
        if (shouldShowGhost)
        {
            ActivateSeedGhost();
            
            if (canPlant)
            {
                MakeGridGhostAvaliable();
            }
            else
            {
                MakeGridGhostUnavaliable();
            }
        }
    }

    private static Ray RayCameraScreenPoint()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
    private void ActivateSeedGhost()
    {
        seedGhost.transform.rotation = Rotation();
        seedGhost.SetActive(true);
        seedGhost.transform.position = finalPosition;
        seedGhost.GetComponentInChildren<MeshFilter>().mesh = GetItemData().ghostMesh;
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

        if (numColliders > 0)
        {
            var dirt = hitColliders[0].GetComponentInParent<Dirt>();
            if (dirt != null)
            {
                return dirt;
            }
        }
        return null;
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
        RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out RaycastHit hit, _maxPlowDistance, layerMask);
    #if UNITY_EDITOR
        Debug.DrawRay(RayCameraScreenPoint().origin, RayCameraScreenPoint().direction * _maxPlowDistance, Color.green, 0.01f);
    #endif

        if (hit.collider == null) return;

        if (CheckDirt(grid.GetNearestPointOnGrid(hit.point), 0.75f) == null)
        {
            PlaceDirtNear(hit.point);
        }
    }

    private void PlaceDirtNear(Vector3 nearPoint)
    {
        finalPosition = grid.GetNearestPointOnGrid(nearPoint);
        DirtSpawnerPooling.SpawnObject(finalPosition, Quaternion.identity);
    }

    public bool PlantTreeNear(GameObject TreePrefab)
    {
        RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out RaycastHit hit, _maxPlowDistance, layerMask);

        if (hit.collider != null)
        {
            Vector3 requiredSpace = GetItemData().RequiredGridSpace;
            if (CheckAvailableSpace(finalPosition, requiredSpace))
            {
                TreeSpawnerPooling.SpawnObject(finalPosition, Rotation());
                UpdateRandomSeed();
                return true;
            }
        }
        return false;
    }

    public void HandleRemainingEnergy(int remainingEnergy)
    {
        if (remainingEnergy > 0) // CAMBIAR A QUE COMPARE CON LA ENERGIA QUE GASTA LA AZADA
        {
            MakeGridGhostAvaliable();
        }
        else
        {
            MakeGridGhostUnavaliable();
        }
    }

    private void MakeGridGhostUnavaliable()
    {
        hoeGhost.GetComponentInChildren<MeshRenderer>().material = noEnergyGhostMaterial;
        seedGhost.GetComponentInChildren<MeshRenderer>().material = noEnergyGhostMaterial;
    }

    private void MakeGridGhostAvaliable()
    {
        hoeGhost.GetComponentInChildren<MeshRenderer>().material = ghostMaterial;
        seedGhost.GetComponentInChildren<MeshRenderer>().material = ghostMaterial;
    }

}
