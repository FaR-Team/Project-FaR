using FaRUtils.Systems.GridSystem;
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

    public Material ghostMaterial;
    public Material noEnergyGhostMaterial;

    public Vector3 finalPosition;
    public Vector3 FinalPosition 
    {
        get 
        {
            Vector3 point = interactor.hit.point;
            point.y += 0.1f;
            return WorldGrid.SnapToGrid(point);
        }
    }

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

    public bool CheckAvailableSpace(Vector3Int coord)
    {            
        return !GridDataManager.Instance.IsCellOccupied(coord);
    }

    private void HandleHoeGhost()
    {
        if (GetItemData() == null || !GetItemData().IsHoe())
        {
            hoeGhost.SetActive(false);
            return;
        }
        
        if (interactor.hit.collider == null)
        {
            hoeGhost.SetActive(false);
            return;
        }
        
        Vector3 hitPoint = interactor.hit.point;
        hitPoint.y += 0.1f;
        finalPosition = WorldGrid.SnapToGrid(hitPoint);
        Vector3Int coord = WorldGrid.WorldToCell(finalPosition);
        
        hoeGhost.SetActive(true);
        hoeGhost.transform.position = finalPosition;
        
        bool isDirtAlreadyHere = GridDataManager.Instance.GetEntityAt<Dirt>(coord) != null;
        bool isCellEmpty = !GridDataManager.Instance.IsCellOccupied(coord);
        
        bool canPlow = !isDirtAlreadyHere && !interactor._LookingAtDirt && isCellEmpty;
        
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

        if (interactor.hit.collider == null)
        {
            seedGhost.SetActive(false);
            return;
        }

        Vector3 hitPoint = interactor.hit.point;
        hitPoint.y += 0.1f;
        finalPosition = WorldGrid.SnapToGrid(hitPoint);
        Vector3Int coord = WorldGrid.WorldToCell(finalPosition);
        
        bool shouldShowGhost = false;
        bool canPlant = false;
        bool hasCropOnDirt = false;

        if (GetItemData().IsCropSeed())
        {
            shouldShowGhost = interactor._LookingAtDirt;
            var currentCheckedDirt = GridDataManager.Instance.GetEntityAt<Dirt>(coord);
            
            if (shouldShowGhost && currentCheckedDirt != null)
            {
                hasCropOnDirt = currentCheckedDirt.currentCrop != null;
            }
            canPlant = shouldShowGhost && hotbarDisplay.CanUseItem() && !hasCropOnDirt;
        }
        else if (GetItemData().IsTreeSeed()) 
        {
            shouldShowGhost = true;
            bool isOccupied = GridDataManager.Instance.IsCellOccupied(coord);
            
            if (interactor._LookingAtDirt)
            {
                var dirt = GridDataManager.Instance.GetEntityAt<Dirt>(coord);
                canPlant = (dirt != null && dirt.IsEmpty);
            }
            else
            {
                canPlant = !isOccupied;
            }
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

    public Dirt CheckDirt(Vector3 worldPos, float radius = 0.5f)
    {
        Vector3 pos = worldPos;
        pos.y += 0.1f;
        Vector3Int coord = WorldGrid.WorldToCell(pos);
        return GridDataManager.Instance.GetEntityAt<Dirt>(coord);
    }

    public bool CheckCrop(Vector3 worldPos, float radius = 0.5f)
    {
        Vector3 pos = worldPos;
        pos.y += 0.1f;
        Vector3Int coord = WorldGrid.WorldToCell(pos);
        return !GridDataManager.Instance.IsCellOccupied(coord);
    }

    public bool PlantDirt()
    {
        if (interactor.hit.collider == null) return false;

        Vector3 hitPoint = interactor.hit.point;
        hitPoint.y += 0.1f;
        Vector3Int coord = WorldGrid.WorldToCell(hitPoint);
        if (GridDataManager.Instance.GetEntityAt<Dirt>(coord) == null)
        {
            PlaceDirtNear(hitPoint);
            return true;
        }
        return false;
    }

    private void PlaceDirtNear(Vector3 nearPoint)
    {
        finalPosition = WorldGrid.SnapToGrid(nearPoint);
        DirtSpawnerPooling.SpawnObject(finalPosition, Quaternion.identity);
    }

    public bool PlantTreeNear(GameObject TreePrefab)
    {
        if (interactor.hit.collider != null)
        {
            Vector3Int coord = WorldGrid.WorldToCell(finalPosition);
            if (!GridDataManager.Instance.IsCellOccupied(coord))
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
        if (remainingEnergy > 0) // TODO: Compare with energy cost of tool
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
