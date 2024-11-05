using UnityEngine;
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

        if (GetItemData() != null &&
            GetItemData().IsHoe() &&
            !interactor._LookingAtDirt)
        {
            RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out RaycastHit hit, _maxPlowDistance - 3, layerMask);

            if (!CheckCrop(grid.GetNearestPointOnGrid(hit.point), 0.1f))
            {
                MakeGridGhostUnavaliable();
            }
            else
            {
                MakeGridGhostAvaliable();
            }

            if (hit.collider != null)
            {
                finalPosition = grid.GetNearestPointOnGrid(hit.point);
                hoeGhost.SetActive(true);
                hoeGhost.transform.position = finalPosition; //Pone el fantasma de la tierra en el lugar que debe estar.
            }
            else
            {
                hoeGhost.SetActive(false);
            }
        }
        else
        {
            hoeGhost.SetActive(false);
        }
        SeedGhost();
    }

    public void SeedGhost()
    {

        /*      
        * Si no tiene semilla en la mano O 
        * Si la semilla es cultivo y no estas mirando tierra O
        * Si la semilla es arbol y estas mirando a una tierra. 
        * Entonces desactiva la seed y no ejecuta el código. 
        */
        if (GetItemData() == null ||
            GetItemData().IsHoe() ||
            GetItemData().IsTreeSeed() && interactor._LookingAtDirt ||
            GetItemData().IsCropSeed() && !interactor._LookingAtDirt)
        {
            seedGhost.SetActive(false);
            return;
        }


        RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out RaycastHit hit, _maxPlowDistance, layerMask);

        if (hit.collider == null) return; //Si el raycast no pega con NADA, entonces no ejecuta el código.

        finalPosition = grid.GetNearestPointOnGrid(hit.point);

        if (!hotbarDisplay.CanUseItem() && GetItemData().IsCropSeed())
        {
            seedGhost.SetActive(false);
            return;
        }


        if (!CheckCrop(finalPosition, 1) && GetItemData().IsTreeSeed())
        {
            seedGhost.SetActive(false);
            return;
        }

        ActivateSeedGhost();

        //seedGhost se activa en la escena, se lo pone en la posicion adecuada y se le cambia la textura por la adecuada.
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
        RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out RaycastHit hit, _maxPlowDistance, layerMask);
#if UNITY_EDITOR
        Debug.DrawRay(RayCameraScreenPoint().origin, RayCameraScreenPoint().direction * _maxPlowDistance, Color.green, 0.01f);
#endif

        if (hit.collider == null) return;

        PlaceDirtNear(hit.point);

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
            Instantiate(TreePrefab, finalPosition, Quaternion.identity, hit.transform.parent.gameObject.transform);
            return true;
        }
        else { return false; }
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
    }

    private void MakeGridGhostAvaliable()
    {
        hoeGhost.GetComponentInChildren<MeshRenderer>().material = ghostMaterial;
    }

}
