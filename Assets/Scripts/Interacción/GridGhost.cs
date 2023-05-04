using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGhost : MonoBehaviour
{
    [SerializeField] private Interactor interactor;
    public HotbarDisplay hotbarDisplay;
    public GameObject hoeGhost, seedGhost, treeSeedGhost;
    public GameObject Dirt;

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
    }
    InventoryItemData GetItemData()
    {
        return hotbarDisplay.slots[hotbarDisplay._currentIndex].AssignedInventorySlot.ItemData;
    }
    void FixedUpdate()
    {
        if (PauseMenu.GameIsPaused == false)
        {
            if (GetItemData() != null && GetItemData().IsHoe == true && interactor._LookingAtDirt == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
#if UNITY_EDITOR
                Debug.DrawRay(ray.origin, ray.direction * _maxGrabDistance, Color.green, 0.01f);
#endif

                if (Physics.Raycast(ray, out hit, _maxGrabDistance, layerMask))
                {
                    finalPosition = grid.GetNearestPointOnGrid(hit.point);
                    hoeGhost.SetActive(true);
                    hoeGhost.transform.position = finalPosition;
                }
            }
            else
            {
                hoeGhost.SetActive(false);
            }
            SeedGhost();
        }

      
    }

    public void SeedGhost()
    {
        if (GetItemData() != null && GetItemData().Seed == true && interactor._LookingAtDirt == true)
        {
            if (GetItemData().DirtPrefabGhost != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                #if UNITY_EDITOR
                    Debug.DrawRay(ray.origin, ray.direction * _maxGrabDistance, Color.green, 0.01f);
                #endif

                GameObject DirtPrefabGhost = GetItemData().DirtPrefabGhost.gameObject;
                    
                if (Physics.Raycast(ray, out hit, _maxGrabDistance, layerMask))
                {
                    finalPosition = grid.GetNearestPointOnGrid(hit.point);
                    if (seedGhost == null)
                    {   
                        seedGhost = GameObject.Instantiate(DirtPrefabGhost, finalPosition, Quaternion.identity);
                        seedGhost.SetActive(true);
                    }
                    seedGhost.transform.position = finalPosition;

                    if (hotbarDisplay._playerControls.Player.MouseWheel.ReadValue<float>() > 0.1f && PauseMenu.GameIsPaused == false)
                    {   
                        if (seedGhost != null)
                        {
                            seedGhost.SetActive(true);
                            Destroy(seedGhost);
                        }

                        if (seedGhost == null)
                        {   
                            seedGhost = GameObject.Instantiate(DirtPrefabGhost, finalPosition, Quaternion.identity);
                            seedGhost.SetActive(true);
                        }
                    }
                    if (hotbarDisplay._playerControls.Player.MouseWheel.ReadValue<float>() < -0.1f &&  PauseMenu.GameIsPaused == false)
                    {   
                        if (seedGhost != null)
                        {
                            seedGhost.SetActive(true);
                            Destroy(seedGhost);
                        }

                        if (seedGhost == null)
                        {   
                            seedGhost = GameObject.Instantiate(DirtPrefabGhost, finalPosition, Quaternion.identity);
                            seedGhost.SetActive(true);
                        }
                    }
                }
            }
        }
        else
        {
            if (seedGhost != null)
            {
                seedGhost.SetActive(false);
                Destroy(seedGhost);
            }
        }

        if (GetItemData() != null && GetItemData().TreeSeed == true && interactor._LookingAtDirt == false)
        {
            if (GetItemData().DirtPrefabGhost != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                #if UNITY_EDITOR
                    Debug.DrawRay(ray.origin, ray.direction * _maxGrabDistance, Color.green, 0.01f);
                #endif

                GameObject DirtPrefabGhost = GetItemData().DirtPrefabGhost.gameObject;
                    
                if (Physics.Raycast(ray, out hit, _maxGrabDistance, layerMask))
                {
                    finalPosition = grid.GetNearestPointOnGrid(hit.point);
                    if (treeSeedGhost == null)
                    {   
                        treeSeedGhost = GameObject.Instantiate(DirtPrefabGhost, finalPosition, Quaternion.identity);
                        treeSeedGhost.SetActive(true);
                    }
                    treeSeedGhost.transform.position = finalPosition;

                    if (hotbarDisplay._playerControls.Player.MouseWheel.ReadValue<float>() > 0.1f && PauseMenu.GameIsPaused == false)
                    {   
                        if (treeSeedGhost != null)
                        {
                            treeSeedGhost.SetActive(true);
                            Destroy(treeSeedGhost);
                        }

                        if (treeSeedGhost == null)
                        {   
                            treeSeedGhost = GameObject.Instantiate(DirtPrefabGhost, finalPosition, Quaternion.identity);
                            treeSeedGhost.SetActive(true);
                        }
                    }
                    if (hotbarDisplay._playerControls.Player.MouseWheel.ReadValue<float>() < -0.1f && PauseMenu.GameIsPaused == false)
                    {   
                        if (treeSeedGhost != null)
                        {
                            treeSeedGhost.SetActive(true);
                            Destroy(treeSeedGhost);
                        }

                        if (treeSeedGhost == null)
                        {   
                            treeSeedGhost = GameObject.Instantiate(DirtPrefabGhost, finalPosition, Quaternion.identity);
                            treeSeedGhost.SetActive(true);
                        }
                    }
                }
            }
        }
        else
        {
            if (treeSeedGhost != null)
            {
                treeSeedGhost.SetActive(true);
                Destroy(treeSeedGhost);
            }
        }

        if (hotbarDisplay._playerControls.Player.MouseWheel.ReadValue<float>() > 0.1f && PauseMenu.GameIsPaused == false)
        {
            if (seedGhost != null)
            {
                seedGhost.SetActive(true);
                Destroy(seedGhost);
            }

            if (treeSeedGhost != null)
            {
            treeSeedGhost.SetActive(true);
                Destroy(treeSeedGhost);
            }
        }
    }

    public bool CheckDirt(Vector3 center, float radius)
    {
        int maxColliders = 5;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(center, radius, hitColliders, layerDirt);
        if (numColliders > 1)
        {
            return false;
        }
        else
        {
            return true;
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        #if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * _maxGrabDistance, Color.green, 0.01f);
        #endif

        if (Physics.Raycast(ray, out hit, _maxGrabDistance, layerMask))  //Todo: Figure out this shit (&& hit.transform.position == plano.transform.position) or (&& hit.transform.tag == "Plantable")
        {
            PlaceDirtNear(hit.point);
        }
    }

    public bool PlantNear(GameObject DirtPrefab)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        GameObject instantiated;

        if (Physics.Raycast(ray, out hit, _maxGrabDistance, layerDirt))
        {
            instantiated = GameObject.Instantiate(DirtPrefab, finalPosition, Quaternion.identity, hit.transform.parent.gameObject.transform);

            return (instantiated != null);
            
        }
        else
        {
            return false;
        }
    }

    public bool PlantTreeNear(GameObject DirtPrefab)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        GameObject instantiated;

        if (Physics.Raycast(ray, out hit, _maxGrabDistance))
        {
            instantiated = GameObject.Instantiate(DirtPrefab, finalPosition, Quaternion.identity, hit.transform.parent.gameObject.transform);
            if (instantiated != null) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void PlaceDirtNear(Vector3 nearPoint)
    {      
        finalPosition = grid.GetNearestPointOnGrid(nearPoint);

        GameObject.Instantiate(Dirt, transform.position, Quaternion.identity).transform.position = finalPosition;

        //Destroy(checkeo);
    }
}