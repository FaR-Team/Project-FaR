using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridGhost : MonoBehaviour
{
    public static GridGhost instance { get; private set; }
    [SerializeField] private Interactor interactor;
    public HotbarDisplay hotbarDisplay;
    public GameObject hoeGhost, seedGhost;
    public GameObject Dirt;
    public RayAndSphereManager rayAndSphereManager;
    public Grid grid;


    public Vector3 finalPosition;

    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private LayerMask layerDirt;
    [SerializeField]
    private LayerMask layerCrop;
    
    [SerializeField, Tooltip("La distancia máxima donde se puede arar")]
    private float   _maxGrabDistance;



    void Start()
    {
        grid = FindObjectOfType<Grid>();
        if (instance != this || instance == null)
        {
            instance = this;
        }
        SeedRotationValue = RandomPos();
    }


    public static int SeedRotationValue = 0;
    private static int RandomPos()
    {
        return Random.Range(-180, 180);
    }
    private InventoryItemData GetItemData()
    {
        return hotbarDisplay.slots[hotbarDisplay._currentIndex].AssignedInventorySlot.ItemData;
    }
    void FixedUpdate()
    {
        if (PauseMenu.GameIsPaused) return;

        if (GetItemData() != null && 
            GetItemData().IsHoe == true && 
            interactor._LookingAtDirt == false)
        { 
            RaycastHit hit;
            RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out hit, _maxGrabDistance - 3, layerMask);

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
        * Entonces desactiva la seed y no ejecuta el código. */
        if (GetItemData() == null || GetItemData().Seed && !interactor._LookingAtDirt || GetItemData().TreeSeed && interactor._LookingAtDirt || GetItemData().IsHoe)
        {
            seedGhost.SetActive(false);
            return;
        }
        
        RaycastHit hit;
        RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out hit, _maxGrabDistance, layerMask);

        if (hit.collider == null) return; //Si el raycast no pega con NADA, entonces no ejecuta el código.
        
        finalPosition = grid.GetNearestPointOnGrid(hit.point);
        
        if (!hotbarDisplay.CanUseItem())
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
        Quaternion rotation = Quaternion.Euler(0, SeedRotationValue, 0);
        seedGhost.transform.rotation = rotation;
        seedGhost.SetActive(true);
        seedGhost.transform.position = finalPosition;
        seedGhost.GetComponentInChildren<MeshFilter>().mesh = GetItemData().ghostMesh;
    }

    public static void UpdateRandomSeed()
    {
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

        RaycastHit hit;
        RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out hit, _maxGrabDistance, layerMask);
#if UNITY_EDITOR
            Debug.DrawRay(RayCameraScreenPoint().origin, RayCameraScreenPoint().direction * _maxGrabDistance, Color.green, 0.01f);
#endif

        if (hit.collider == null) return;

        PlaceDirtNear(hit.point);
        
    }

    public bool PlantNear(GameObject DirtPrefab)
    {
        RaycastHit hit;
        RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out hit, _maxGrabDistance, layerDirt);

        if(hit.collider != null)
        {
            GameObject.Instantiate(DirtPrefab, finalPosition, Quaternion.identity, hit.transform.parent.gameObject.transform);
            return true;
        }
        else {  return false;    }
        
    }

    public bool PlantTreeNear(GameObject DirtPrefab)
    {
        RaycastHit hit;
        RayAndSphereManager.DoRaycast(RayCameraScreenPoint(), out hit, _maxGrabDistance, layerDirt);

        if(hit.collider != null)
        {
            GameObject.Instantiate(DirtPrefab, finalPosition, Quaternion.identity, hit.transform.parent.gameObject.transform);
            return true;
        }
        else {  return false;   }
    }

    public void PlaceDirtNear(Vector3 nearPoint)
    {      
        finalPosition = grid.GetNearestPointOnGrid(nearPoint);

        GameObject.Instantiate(Dirt, finalPosition, Quaternion.identity);
    }
}