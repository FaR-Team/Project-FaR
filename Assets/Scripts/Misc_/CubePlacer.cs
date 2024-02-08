using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlacer : MonoBehaviour
{
    private Grid grid;
    public GameObject Dirt;
    public GameObject dirtCheck;
    public GameObject Frutilla;
    public GameObject plano;
    //public GameObject checkeo;

    [SerializeField, Tooltip("La distancia máxima donde se puede agarrar un objeto")]
    private float   _maxGrabDistance = 50f;

    [Header("LayerMask"), Tooltip("La capa en la cual se puede plantar")]
    [SerializeField]
    private LayerMask _Layer;
    [SerializeField]
    private LayerMask layerDirt;



    void Awake()
    {
        grid = FindObjectOfType<Grid>();
    }

    public void PlantDirt()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Grid grid = FindObjectOfType<Grid>();
        RaycastHit hit;

#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * _maxGrabDistance, Color.green, 0.01f);
#endif

        if (Physics.Raycast(ray, out hit, _maxGrabDistance, _Layer))  //Todo: Figure out this shit (&& hit.transform.position == plano.transform.position) or (&& hit.transform.tag == "Plantable")
        {
            if (CheckForDirt() == false)
            {
                PlaceDirtNear(hit.point);
            } 
        }
    }

    public bool CheckForDirt()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * _maxGrabDistance, Color.blue, 0.01f);
#endif

        if (Physics.Raycast(ray, out hit, _maxGrabDistance, layerDirt))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckForCrop(GameObject DirtPrefab)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Grid grid = FindObjectOfType<Grid>();
        RaycastHit hit;

#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * _maxGrabDistance, Color.green, 0.01f);
#endif

        if (Physics.Raycast(ray, out hit, _maxGrabDistance))  //Todo: Figure out this shit (&& hit.transform.position == plano.transform.position) or (&& hit.transform.tag == "Plantable")
        {
            if (hit.transform.tag == "Dirt")
            {
                PlantNear(hit, DirtPrefab);
                foreach(Transform t in hit.transform.parent.transform)
                {
                    t.gameObject.tag = "IsPlanted";
                }
                hit.transform.parent.transform.tag = "IsPlanted";
            }
        }
    }

    void AddTagRecursively(Transform trans, string tag)
    {
     trans.gameObject.tag = tag;
     if(trans.childCount > 0)
         foreach(Transform t in trans)
             AddTagRecursively(t, tag);
    }

    public void PlantNear(RaycastHit hit, GameObject DirtPrefab)
    {
        GameObject.Instantiate(DirtPrefab, hit.transform.parent.gameObject.transform.position, Quaternion.identity, hit.transform.parent.gameObject.transform);
    }

  /* public bool CheckForDirt(Vector3 nearPoint)
   {
        if (checkeo == null)
        {
            var finalPosition = grid.GetNearestPointOnGrid(nearPoint);

            checkeo = Instantiate(dirtCheck, transform.position, Quaternion.identity).transform.gameObject;
            checkeo.transform.position = finalPosition;

            if (checkeo.GetComponent<OnOverDirt>().isDirt = true)
            {
                return true;
                Destroy(checkeo);
            }
            else if (checkeo.GetComponent<OnOverDirt>().isDirt = false)
            {
                return false;
                Destroy(checkeo);
            }
        }

        return false;
        Destroy(checkeo);
   }*/

   public void PlaceDirtNear(Vector3 nearPoint)
   {      
        var finalPosition = grid.GetNearestPointOnGrid(nearPoint);

        GameObject.Instantiate(Dirt, transform.position, Quaternion.identity).transform.position = finalPosition;

        //Destroy(checkeo);
   }

    private void PlaceFrutNear(Vector3 nearPoint)
   {
        var finalPosition = grid.GetNearestPointOnGrid(nearPoint);
        GameObject.Instantiate(Frutilla, transform.position, Quaternion.identity).transform.position = finalPosition;
   }

   IEnumerator DestroyDirt(RaycastHit hit)
   {
        GameObject dirt = hit.transform.parent.gameObject;
        dirt.GetComponent<Animation>().Play("DirtGo");
        yield return new WaitForSeconds(1);
        Destroy(hit.transform.parent.gameObject);
   }
}
